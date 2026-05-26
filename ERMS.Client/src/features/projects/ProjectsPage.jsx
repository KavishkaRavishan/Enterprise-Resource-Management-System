import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../../api/axios';
import { useAuthStore } from '../auth/useAuthStore';
import { Plus, FolderKanban, Users, Calendar, Search, X } from 'lucide-react';

export default function ProjectsPage() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editProject, setEditProject] = useState(null);
  const [search, setSearch] = useState('');
  const [allUsers, setAllUsers] = useState([]);
  const { user } = useAuthStore();
  const navigate = useNavigate();
  const canCreate = user?.role === 'Admin' || user?.role === 'Manager';

  const fetchProjects = () => {
    api.get('/projects').then(r => setProjects(r.data.data || [])).catch(() => {}).finally(() => setLoading(false));
  };
  useEffect(() => { fetchProjects(); api.get('/users').then(r => setAllUsers(r.data.data || [])); }, []);

  const filtered = projects.filter(p => p.name.toLowerCase().includes(search.toLowerCase()));

  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold text-surface-900">Projects</h1><p className="text-surface-500 mt-1">{projects.length} total projects</p></div>
        {canCreate && <button onClick={() => { setEditProject(null); setShowForm(true); }} className="flex items-center gap-2 px-4 py-2.5 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium text-sm" id="create-project-btn"><Plus className="w-4 h-4" /> New Project</button>}
      </div>

      <div className="relative max-w-md">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-surface-400" />
        <input type="text" placeholder="Search projects..." value={search} onChange={e => setSearch(e.target.value)} className="w-full pl-10 pr-4 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 outline-none text-surface-900 bg-white" id="project-search" />
      </div>

      {loading ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">{[...Array(6)].map((_, i) => <div key={i} className="h-48 bg-surface-200 rounded-xl animate-pulse" />)}</div>
      ) : filtered.length === 0 ? (
        <div className="text-center py-16 text-surface-400"><FolderKanban className="w-12 h-12 mx-auto mb-3 opacity-50" /><p>No projects found</p></div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map((project) => {
            const progress = project.taskCount > 0 ? Math.round((project.completedTaskCount / project.taskCount) * 100) : 0;
            return (
              <div key={project.id} className="bg-white rounded-xl border border-surface-200 p-5 hover:shadow-lg hover:border-primary-200 transition-all duration-300 cursor-pointer group" onClick={() => navigate(`/projects/${project.id}`)}>
                <div className="flex items-start justify-between mb-3">
                  <h3 className="font-semibold text-surface-900 group-hover:text-primary-600 transition-colors">{project.name}</h3>
                  <span className={`text-xs px-2 py-1 rounded-full font-medium ${project.status==='InProgress'?'bg-blue-50 text-blue-600':project.status==='Completed'?'bg-emerald-50 text-emerald-600':'bg-surface-100 text-surface-600'}`}>{project.status}</span>
                </div>
                <p className="text-sm text-surface-500 mb-4 line-clamp-2">{project.description}</p>
                <div className="mb-3">
                  <div className="flex items-center justify-between text-xs text-surface-500 mb-1"><span>Progress</span><span>{progress}%</span></div>
                  <div className="w-full bg-surface-100 rounded-full h-1.5"><div className="bg-primary-500 h-1.5 rounded-full transition-all" style={{ width: `${progress}%` }} /></div>
                </div>
                <div className="flex items-center gap-4 text-xs text-surface-400">
                  <span className="flex items-center gap-1"><Users className="w-3.5 h-3.5" /> {project.memberCount}</span>
                  <span className="flex items-center gap-1"><FolderKanban className="w-3.5 h-3.5" /> {project.taskCount} tasks</span>
                  <span className="flex items-center gap-1"><Calendar className="w-3.5 h-3.5" /> {new Date(project.startDate).toLocaleDateString()}</span>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {showForm && <ProjectFormModal project={editProject} users={allUsers} onClose={() => setShowForm(false)} onSave={() => { setShowForm(false); fetchProjects(); }} />}
    </div>
  );
}

function ProjectFormModal({ project, users, onClose, onSave }) {
  const [form, setForm] = useState({
    name: project?.name || '', description: project?.description || '',
    startDate: project?.startDate?.split('T')[0] || new Date().toISOString().split('T')[0],
    endDate: project?.endDate?.split('T')[0] || '', memberIds: project?.members?.map(m => m.userId) || [],
    status: project?.status || 'NotStarted',
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault(); setSaving(true);
    try {
      const payload = { ...form, startDate: new Date(form.startDate).toISOString(), endDate: form.endDate ? new Date(form.endDate).toISOString() : null };
      if (project) await api.put(`/projects/${project.id}`, payload);
      else await api.post('/projects', payload);
      onSave();
    } catch (err) { alert(err.response?.data?.message || 'Failed'); }
    setSaving(false);
  };

  const toggleMember = (id) => setForm(f => ({ ...f, memberIds: f.memberIds.includes(id) ? f.memberIds.filter(m => m !== id) : [...f.memberIds, id] }));

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto animate-scale-in" onClick={e => e.stopPropagation()}>
        <div className="flex items-center justify-between p-6 border-b border-surface-100">
          <h2 className="text-lg font-semibold text-surface-900">{project ? 'Edit Project' : 'New Project'}</h2>
          <button onClick={onClose} className="p-1 rounded-lg hover:bg-surface-100 text-surface-500"><X className="w-5 h-5" /></button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div><label className="block text-sm font-medium text-surface-700 mb-1">Name</label><input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900" /></div>
          <div><label className="block text-sm font-medium text-surface-700 mb-1">Description</label><textarea value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} rows={3} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900 resize-none" /></div>
          <div className="grid grid-cols-2 gap-4">
            <div><label className="block text-sm font-medium text-surface-700 mb-1">Start Date</label><input type="date" value={form.startDate} onChange={e => setForm({ ...form, startDate: e.target.value })} required className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900" /></div>
            <div><label className="block text-sm font-medium text-surface-700 mb-1">End Date</label><input type="date" value={form.endDate} onChange={e => setForm({ ...form, endDate: e.target.value })} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900" /></div>
          </div>
          {project && <div><label className="block text-sm font-medium text-surface-700 mb-1">Status</label><select value={form.status} onChange={e => setForm({ ...form, status: e.target.value })} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900"><option value="NotStarted">Not Started</option><option value="InProgress">In Progress</option><option value="Completed">Completed</option></select></div>}
          <div>
            <label className="block text-sm font-medium text-surface-700 mb-2">Team Members</label>
            <div className="max-h-40 overflow-y-auto border border-surface-200 rounded-lg p-2 space-y-1">
              {users.filter(u => u.isActive).map(u => (
                <label key={u.id} className="flex items-center gap-2 px-2 py-1.5 rounded hover:bg-surface-50 cursor-pointer">
                  <input type="checkbox" checked={form.memberIds.includes(u.id)} onChange={() => toggleMember(u.id)} className="rounded border-surface-300 text-primary-600 focus:ring-primary-500" />
                  <span className="text-sm text-surface-700">{u.firstName} {u.lastName}</span>
                  <span className="text-xs text-surface-400">({u.role})</span>
                </label>
              ))}
            </div>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={onClose} className="px-4 py-2 text-sm text-surface-600 hover:bg-surface-100 rounded-lg transition-colors">Cancel</button>
            <button type="submit" disabled={saving} className="px-4 py-2 text-sm bg-primary-600 text-white rounded-lg hover:bg-primary-700 disabled:opacity-50 transition-colors font-medium">{saving ? 'Saving...' : project ? 'Update' : 'Create'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}
