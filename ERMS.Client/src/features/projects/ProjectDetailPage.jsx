import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../../api/axios';
import { useAuthStore } from '../auth/useAuthStore';
import { ArrowLeft, Plus, Users, Calendar, Trash2, Edit, X, MessageSquare, GripVertical, Clock, History } from 'lucide-react';
import { useTimeLogStore } from '../timelogs/useTimeLogStore';

export default function ProjectDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuthStore();
  const [project, setProject] = useState(null);
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [editTask, setEditTask] = useState(null);
  const [selectedTask, setSelectedTask] = useState(null);
  const [allUsers, setAllUsers] = useState([]);
  const canManage = user?.role === 'Admin' || user?.role === 'Manager';
  const { totalHours, fetchTotalHours } = useTimeLogStore();

  const fetchData = async () => {
    try {
      const [pRes, tRes] = await Promise.all([
        api.get(`/projects/${id}`),
        api.get(`/tasks/project/${id}`),
      ]);
      setProject(pRes.data.data);
      setTasks(tRes.data.data || []);
      fetchTotalHours(id);
    } catch { navigate('/projects'); }
    setLoading(false);
  };

  useEffect(() => {
    fetchData();
    api.get('/users').then(r => setAllUsers(r.data.data || []));
  }, [id]);

  const columns = [
    { key: 'ToDo', label: 'To Do', color: 'border-amber-400', bg: 'bg-amber-50' },
    { key: 'InProgress', label: 'In Progress', color: 'border-blue-400', bg: 'bg-blue-50' },
    { key: 'Done', label: 'Done', color: 'border-emerald-400', bg: 'bg-emerald-50' },
  ];

  const handleDragStart = (e, taskId) => { e.dataTransfer.setData('taskId', taskId); };
  const handleDrop = async (e, status) => {
    e.preventDefault();
    const taskId = e.dataTransfer.getData('taskId');
    await api.patch(`/tasks/${taskId}/status`, { status });
    fetchData();
  };
  const handleDragOver = (e) => e.preventDefault();

  const handleDeleteTask = async (taskId) => {
    if (!confirm('Delete this task?')) return;
    await api.delete(`/tasks/${taskId}`);
    fetchData();
  };

  if (loading) return <div className="animate-pulse space-y-4"><div className="h-8 bg-surface-200 rounded w-64" /><div className="h-96 bg-surface-200 rounded-xl" /></div>;

  return (
    <div className="space-y-6 animate-fade-in">
      <button onClick={() => navigate('/projects')} className="flex items-center gap-1 text-sm text-surface-500 hover:text-primary-600 transition-colors">
        <ArrowLeft className="w-4 h-4" /> Back to Projects
      </button>
      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">{project?.name}</h1>
          <p className="text-surface-500 mt-1">{project?.description}</p>
          <div className="flex items-center gap-4 mt-3 text-sm text-surface-400">
            <span className="flex items-center gap-1"><Calendar className="w-4 h-4" />{new Date(project?.startDate).toLocaleDateString()}</span>
            <span className="flex items-center gap-1"><Users className="w-4 h-4" />{project?.memberCount} members</span>
            <span className="flex items-center gap-1"><Clock className="w-4 h-4" />{totalHours} hours logged</span>
            <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${project?.status==='InProgress'?'bg-blue-50 text-blue-600':project?.status==='Completed'?'bg-emerald-50 text-emerald-600':'bg-surface-100 text-surface-600'}`}>{project?.status}</span>
          </div>
        </div>
        {canManage && (
          <button onClick={() => { setEditTask(null); setShowTaskForm(true); }} className="flex items-center gap-2 px-4 py-2.5 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium text-sm" id="add-task-btn">
            <Plus className="w-4 h-4" /> Add Task
          </button>
        )}
      </div>

      {/* Kanban Board */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {columns.map((col) => (
          <div key={col.key} onDrop={(e) => handleDrop(e, col.key)} onDragOver={handleDragOver} className={`rounded-xl border-t-4 ${col.color} bg-white border border-surface-200 min-h-[300px]`}>
            <div className={`px-4 py-3 ${col.bg} rounded-t-lg border-b border-surface-100`}>
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-surface-700">{col.label}</h3>
                <span className="text-xs bg-white px-2 py-0.5 rounded-full text-surface-500 font-medium">{tasks.filter(t=>t.status===col.key).length}</span>
              </div>
            </div>
            <div className="p-3 space-y-2">
              {tasks.filter(t=>t.status===col.key).map((task) => (
                <div key={task.id} draggable onDragStart={(e)=>handleDragStart(e,task.id)} className="bg-white border border-surface-200 rounded-lg p-3 cursor-grab hover:shadow-md transition-all group" onClick={()=>setSelectedTask(task)}>
                  <div className="flex items-start justify-between mb-2">
                    <h4 className="text-sm font-medium text-surface-800">{task.title}</h4>
                    <GripVertical className="w-4 h-4 text-surface-300 opacity-0 group-hover:opacity-100 transition-opacity" />
                  </div>
                  <div className="flex items-center gap-2 flex-wrap">
                    <span className={`text-xs px-1.5 py-0.5 rounded font-medium ${task.priority==='High'?'bg-red-50 text-red-600':task.priority==='Medium'?'bg-amber-50 text-amber-600':'bg-emerald-50 text-emerald-600'}`}>{task.priority}</span>
                    {task.assignedToName && <span className="text-xs text-surface-500">{task.assignedToName}</span>}
                    {task.dueDate && <span className="text-xs text-surface-400">{new Date(task.dueDate).toLocaleDateString()}</span>}
                    {task.commentCount > 0 && <span className="flex items-center gap-0.5 text-xs text-surface-400"><MessageSquare className="w-3 h-3"/>{task.commentCount}</span>}
                  </div>
                  {canManage && (
                    <div className="flex gap-1 mt-2 opacity-0 group-hover:opacity-100 transition-opacity">
                      <button onClick={(e)=>{e.stopPropagation();setEditTask(task);setShowTaskForm(true)}} className="p-1 rounded hover:bg-surface-100 text-surface-400"><Edit className="w-3.5 h-3.5"/></button>
                      <button onClick={(e)=>{e.stopPropagation();handleDeleteTask(task.id)}} className="p-1 rounded hover:bg-red-50 text-surface-400 hover:text-red-500"><Trash2 className="w-3.5 h-3.5"/></button>
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>

      {showTaskForm && <TaskFormModal task={editTask} projectId={id} users={allUsers} onClose={()=>setShowTaskForm(false)} onSave={()=>{setShowTaskForm(false);fetchData()}} />}
      {selectedTask && <TaskDetailModal task={selectedTask} onClose={()=>setSelectedTask(null)} onUpdate={fetchData} />}
    </div>
  );
}

function TaskFormModal({ task, projectId, users, onClose, onSave }) {
  const [form, setForm] = useState({
    title: task?.title||'', description: task?.description||'', priority: task?.priority||'Medium',
    dueDate: task?.dueDate?.split('T')[0]||'', assignedToId: task?.assignedToId||'', projectId,
  });
  const [saving, setSaving] = useState(false);
  const handleSubmit = async (e) => {
    e.preventDefault(); setSaving(true);
    try {
      const payload = { ...form, dueDate: form.dueDate?new Date(form.dueDate).toISOString():null, assignedToId: form.assignedToId||null };
      if (task) await api.put(`/tasks/${task.id}`, payload);
      else await api.post('/tasks', payload);
      onSave();
    } catch(err) { alert(err.response?.data?.message||'Failed'); }
    setSaving(false);
  };
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md animate-scale-in" onClick={e=>e.stopPropagation()}>
        <div className="flex items-center justify-between p-6 border-b border-surface-100">
          <h2 className="text-lg font-semibold">{task?'Edit Task':'New Task'}</h2>
          <button onClick={onClose} className="p-1 rounded-lg hover:bg-surface-100"><X className="w-5 h-5"/></button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div><label className="block text-sm font-medium text-surface-700 mb-1">Title</label><input value={form.title} onChange={e=>setForm({...form,title:e.target.value})} required className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"/></div>
          <div><label className="block text-sm font-medium text-surface-700 mb-1">Description</label><textarea value={form.description} onChange={e=>setForm({...form,description:e.target.value})} rows={3} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none resize-none"/></div>
          <div className="grid grid-cols-2 gap-4">
            <div><label className="block text-sm font-medium text-surface-700 mb-1">Priority</label><select value={form.priority} onChange={e=>setForm({...form,priority:e.target.value})} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"><option>Low</option><option>Medium</option><option>High</option></select></div>
            <div><label className="block text-sm font-medium text-surface-700 mb-1">Due Date</label><input type="date" value={form.dueDate} onChange={e=>setForm({...form,dueDate:e.target.value})} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"/></div>
          </div>
          <div><label className="block text-sm font-medium text-surface-700 mb-1">Assign To</label><select value={form.assignedToId} onChange={e=>setForm({...form,assignedToId:e.target.value})} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"><option value="">Unassigned</option>{users.filter(u=>u.isActive).map(u=><option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>)}</select></div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={onClose} className="px-4 py-2 text-sm text-surface-600 hover:bg-surface-100 rounded-lg">Cancel</button>
            <button type="submit" disabled={saving} className="px-4 py-2 text-sm bg-primary-600 text-white rounded-lg hover:bg-primary-700 disabled:opacity-50 font-medium">{saving?'Saving...':task?'Update':'Create'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}

function TaskDetailModal({ task, onClose, onUpdate }) {
  const { user } = useAuthStore();
  const [activeTab, setActiveTab] = useState('comments');
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');
  const [sending, setSending] = useState(false);

  // Time logging states
  const { timeLogs, fetchTimeLogsByTask, logTime, deleteTimeLog } = useTimeLogStore();
  const [hoursSpent, setHoursSpent] = useState('');
  const [logDescription, setLogDescription] = useState('');
  const [dateLogged, setDateLogged] = useState(new Date().toISOString().split('T')[0]);
  const [submittingLog, setSubmittingLog] = useState(false);

  useEffect(() => {
    api.get(`/comments/task/${task.id}`).then(r => setComments(r.data.data || []));
    fetchTimeLogsByTask(task.id);
  }, [task.id, fetchTimeLogsByTask]);

  const addComment = async () => {
    if (!newComment.trim()) return;
    setSending(true);
    await api.post(`/comments/task/${task.id}`, { content: newComment });
    setNewComment('');
    const r = await api.get(`/comments/task/${task.id}`);
    setComments(r.data.data || []);
    onUpdate();
    setSending(false);
  };

  const handleLogTimeSubmit = async (e) => {
    e.preventDefault();
    if (!hoursSpent || hoursSpent <= 0 || !logDescription.trim()) return;
    setSubmittingLog(true);
    const result = await logTime({
      taskId: task.id,
      hoursSpent: Number(hoursSpent),
      description: logDescription,
      dateLogged: new Date(dateLogged).toISOString(),
    });
    if (result.success) {
      setHoursSpent('');
      setLogDescription('');
      fetchTimeLogsByTask(task.id);
      onUpdate();
    } else {
      alert(result.error);
    }
    setSubmittingLog(false);
  };

  const handleDeleteLog = async (logId) => {
    if (!confirm('Are you sure you want to delete this time entry?')) return;
    const result = await deleteTimeLog(logId);
    if (result.success) {
      fetchTimeLogsByTask(task.id);
      onUpdate();
    } else {
      alert(result.error);
    }
  };

  const taskTotalHours = timeLogs.reduce((sum, log) => sum + Number(log.hoursSpent), 0);

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[85vh] flex flex-col animate-scale-in" onClick={e=>e.stopPropagation()}>
        <div className="flex items-center justify-between p-6 border-b border-surface-100">
          <div>
            <h2 className="text-lg font-semibold text-surface-900">{task.title}</h2>
            <p className="text-xs text-surface-400 mt-0.5">Project Task</p>
          </div>
          <button onClick={onClose} className="p-1 rounded-lg hover:bg-surface-100"><X className="w-5 h-5"/></button>
        </div>

        <div className="flex-1 overflow-y-auto p-6 space-y-4">
          <p className="text-sm text-surface-600">{task.description||'No description provided.'}</p>
          
          <div className="flex gap-2 flex-wrap">
            <span className={`text-xs px-2 py-1 rounded-full font-medium ${task.priority==='High'?'bg-red-50 text-red-600':task.priority==='Medium'?'bg-amber-50 text-amber-600':'bg-emerald-50 text-emerald-600'}`}>{task.priority} Priority</span>
            <span className={`text-xs px-2 py-1 rounded-full font-medium ${task.status==='Done'?'bg-emerald-50 text-emerald-600':task.status==='InProgress'?'bg-blue-50 text-blue-600':'bg-amber-50 text-amber-600'}`}>{task.status}</span>
          </div>

          {task.assignedToName && (
            <div className="flex items-center gap-2 text-sm text-surface-500">
              <span>Assigned to:</span>
              <span className="font-medium text-surface-800 bg-surface-100 px-2 py-0.5 rounded">{task.assignedToName}</span>
            </div>
          )}

          {/* tab selection bar */}
          <div className="flex border-b border-surface-200 mt-6 mb-4">
            <button
              onClick={() => setActiveTab('comments')}
              className={`flex-1 pb-2 text-sm font-semibold border-b-2 text-center transition-all ${
                activeTab === 'comments'
                  ? 'border-primary-600 text-primary-600'
                  : 'border-transparent text-surface-400 hover:text-surface-600'
              }`}
            >
              Comments ({comments.length})
            </button>
            <button
              onClick={() => setActiveTab('timelogs')}
              className={`flex-1 pb-2 text-sm font-semibold border-b-2 text-center transition-all flex items-center justify-center gap-1.5 ${
                activeTab === 'timelogs'
                  ? 'border-primary-600 text-primary-600'
                  : 'border-transparent text-surface-400 hover:text-surface-600'
              }`}
            >
              <Clock className="w-4 h-4" /> Time Logs ({timeLogs.length})
            </button>
          </div>

          {/* tab contents */}
          {activeTab === 'comments' ? (
            <div className="space-y-4">
              <div className="space-y-3 max-h-60 overflow-y-auto pr-1">
                {comments.map(c=>(
                  <div key={c.id} className="bg-surface-50 rounded-lg p-3">
                    <div className="flex items-center justify-between mb-1">
                      <span className="text-xs font-semibold text-surface-700">{c.authorName}</span>
                      <span className="text-xs text-surface-400">{new Date(c.created).toLocaleString()}</span>
                    </div>
                    <p className="text-sm text-surface-600 whitespace-pre-wrap">{c.content}</p>
                  </div>
                ))}
                {comments.length===0 && <p className="text-xs text-center text-surface-400 py-4">No comments posted yet.</p>}
              </div>
              
              <div className="flex gap-2">
                <input value={newComment} onChange={e=>setNewComment(e.target.value)} onKeyDown={e=>{if(e.key==='Enter')addComment()}} placeholder="Add a comment..." className="flex-1 px-3 py-2 border border-surface-200 rounded-lg text-sm focus:ring-2 focus:ring-primary-500 outline-none"/>
                <button onClick={addComment} disabled={sending||!newComment.trim()} className="px-4 py-2 bg-primary-600 text-white rounded-lg text-sm hover:bg-primary-700 disabled:opacity-50 font-medium">Send</button>
              </div>
            </div>
          ) : (
            <div className="space-y-6">
              {/* log time entry form */}
              <form onSubmit={handleLogTimeSubmit} className="bg-surface-50 rounded-xl p-4 border border-surface-100 space-y-3">
                <h4 className="text-xs font-bold text-surface-700 uppercase tracking-wider">Log Time Entry</h4>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="block text-[11px] font-medium text-surface-500 mb-1">Hours Spent</label>
                    <input
                      type="number"
                      step="0.1"
                      min="0.1"
                      max="24"
                      required
                      value={hoursSpent}
                      onChange={e=>setHoursSpent(e.target.value)}
                      placeholder="e.g. 2.5"
                      className="w-full px-2 py-1.5 text-sm border border-surface-200 rounded-md focus:ring-1 focus:ring-primary-500 outline-none"
                    />
                  </div>
                  <div>
                    <label className="block text-[11px] font-medium text-surface-500 mb-1">Date Logged</label>
                    <input
                      type="date"
                      required
                      value={dateLogged}
                      onChange={e=>setDateLogged(e.target.value)}
                      className="w-full px-2 py-1.5 text-sm border border-surface-200 rounded-md focus:ring-1 focus:ring-primary-500 outline-none"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-[11px] font-medium text-surface-500 mb-1">Activity Description</label>
                  <input
                    type="text"
                    required
                    value={logDescription}
                    onChange={e=>setLogDescription(e.target.value)}
                    placeholder="Describe what you worked on..."
                    className="w-full px-2 py-1.5 text-sm border border-surface-200 rounded-md focus:ring-1 focus:ring-primary-500 outline-none"
                  />
                </div>
                <div className="flex justify-end pt-1">
                  <button
                    type="submit"
                    disabled={submittingLog}
                    className="px-3 py-1.5 bg-primary-600 hover:bg-primary-700 text-white rounded-md text-xs font-semibold disabled:opacity-50 transition-colors"
                  >
                    {submittingLog ? 'Logging...' : 'Log Hours'}
                  </button>
                </div>
              </form>

              {/* logged history list */}
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <h4 className="text-xs font-bold text-surface-700 uppercase tracking-wider">Logged History</h4>
                  <span className="text-xs font-semibold text-primary-700 bg-primary-50 px-2.5 py-0.5 rounded-full">Total: {taskTotalHours} hrs</span>
                </div>

                <div className="space-y-2 max-h-52 overflow-y-auto pr-1">
                  {timeLogs.map((log) => (
                    <div key={log.id} className="flex items-center justify-between p-3 border border-surface-150 rounded-lg hover:bg-surface-50/50 transition-colors">
                      <div className="flex gap-2.5 items-center min-w-0">
                        <div className="w-7 h-7 rounded-full bg-primary-100 text-primary-800 flex items-center justify-center text-xs font-bold flex-shrink-0">
                          {log.userName?.[0]}
                        </div>
                        <div className="min-w-0">
                          <p className="text-xs font-semibold text-surface-800 truncate">{log.userName}</p>
                          <p className="text-xs text-surface-500 truncate">{log.description}</p>
                          <span className="text-[10px] text-surface-400 block mt-0.5">{new Date(log.dateLogged).toLocaleDateString()}</span>
                        </div>
                      </div>
                      <div className="flex items-center gap-3">
                        <span className="text-xs font-bold text-surface-700 bg-surface-100 px-2 py-0.5 rounded">+{log.hoursSpent}h</span>
                        {(user?.role === 'Admin' || user?.role === 'Manager' || log.userId === user?.id) && (
                          <button
                            onClick={() => handleDeleteLog(log.id)}
                            className="p-1 rounded text-surface-400 hover:text-red-500 hover:bg-red-50 transition-all"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </button>
                        )}
                      </div>
                    </div>
                  ))}
                  {timeLogs.length === 0 && (
                    <p className="text-xs text-center text-surface-400 py-6">No hours logged on this task yet.</p>
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
