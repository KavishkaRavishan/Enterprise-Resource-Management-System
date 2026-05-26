import { useState, useEffect } from 'react';
import api from '../../api/axios';
import { useAuthStore } from '../auth/useAuthStore';
import { CheckSquare, Clock, AlertCircle } from 'lucide-react';

export default function MyTasksPage() {
  const { user } = useAuthStore();
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get(`/tasks/user/${user?.id}`)
      .then(r => setTasks(r.data.data || []))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [user?.id]);

  const handleStatusChange = async (taskId, status) => {
    await api.patch(`/tasks/${taskId}/status`, { status });
    const r = await api.get(`/tasks/user/${user?.id}`);
    setTasks(r.data.data || []);
  };

  if (loading) return <div className="space-y-4 animate-pulse">{[...Array(5)].map((_,i)=><div key={i} className="h-20 bg-surface-200 rounded-xl"/>)}</div>;

  const groups = { ToDo: tasks.filter(t=>t.status==='ToDo'), InProgress: tasks.filter(t=>t.status==='InProgress'), Done: tasks.filter(t=>t.status==='Done') };

  return (
    <div className="space-y-6 animate-fade-in">
      <div>
        <h1 className="text-2xl font-bold text-surface-900">My Tasks</h1>
        <p className="text-surface-500 mt-1">{tasks.length} tasks assigned to you</p>
      </div>
      {tasks.length === 0 ? (
        <div className="text-center py-16 text-surface-400">
          <CheckSquare className="w-12 h-12 mx-auto mb-3 opacity-50"/>
          <p>No tasks assigned to you yet</p>
        </div>
      ) : (
        <div className="space-y-6">
          {Object.entries(groups).map(([status, items]) => items.length > 0 && (
            <div key={status}>
              <h2 className="text-sm font-semibold text-surface-500 uppercase tracking-wider mb-3 flex items-center gap-2">
                {status === 'ToDo' && <Clock className="w-4 h-4 text-amber-500"/>}
                {status === 'InProgress' && <AlertCircle className="w-4 h-4 text-blue-500"/>}
                {status === 'Done' && <CheckSquare className="w-4 h-4 text-emerald-500"/>}
                {status === 'InProgress' ? 'In Progress' : status === 'ToDo' ? 'To Do' : status} ({items.length})
              </h2>
              <div className="space-y-2">
                {items.map(task => (
                  <div key={task.id} className="bg-white border border-surface-200 rounded-xl p-4 hover:shadow-md transition-all flex items-center justify-between">
                    <div className="flex-1 min-w-0">
                      <h3 className="font-medium text-surface-900">{task.title}</h3>
                      <div className="flex items-center gap-3 mt-1 text-xs text-surface-500">
                        <span>{task.projectName}</span>
                        <span className={`px-1.5 py-0.5 rounded font-medium ${task.priority==='High'?'bg-red-50 text-red-600':task.priority==='Medium'?'bg-amber-50 text-amber-600':'bg-emerald-50 text-emerald-600'}`}>{task.priority}</span>
                        {task.dueDate && <span>{new Date(task.dueDate).toLocaleDateString()}</span>}
                      </div>
                    </div>
                    <select value={task.status} onChange={e=>handleStatusChange(task.id,e.target.value)} className="px-3 py-1.5 text-xs border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none bg-white">
                      <option value="ToDo">To Do</option>
                      <option value="InProgress">In Progress</option>
                      <option value="Done">Done</option>
                    </select>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
