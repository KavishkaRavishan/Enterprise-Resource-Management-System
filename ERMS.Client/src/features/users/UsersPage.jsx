import { useState, useEffect, useRef } from 'react';
import api from '../../api/axios';
import { Plus, Search, Trash2, Edit, X, Shield, Upload, Camera } from 'lucide-react';

export default function UsersPage() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editUser, setEditUser] = useState(null);
  const [search, setSearch] = useState('');

  const fetchUsers = () => {
    api.get('/users')
      .then(r => setUsers(r.data.data || []))
      .catch(() => {})
      .finally(() => setLoading(false));
  };
  useEffect(() => { fetchUsers(); }, []);

  const filtered = users.filter(u =>
    `${u.firstName} ${u.lastName} ${u.email}`.toLowerCase().includes(search.toLowerCase())
  );

  const handleDelete = async (id) => {
    if (!confirm('Delete this user?')) return;
    await api.delete(`/users/${id}`);
    fetchUsers();
  };

  const handleAvatarUpload = async (userId, file) => {
    const formData = new FormData();
    formData.append('file', file);
    await api.post(`/users/${userId}/avatar`, formData, { headers: { 'Content-Type': 'multipart/form-data' } });
    fetchUsers();
  };

  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">User Management</h1>
          <p className="text-surface-500 mt-1">{users.length} total users</p>
        </div>
        <button onClick={() => { setEditUser(null); setShowForm(true); }} className="flex items-center gap-2 px-4 py-2.5 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium text-sm" id="create-user-btn">
          <Plus className="w-4 h-4" /> Add User
        </button>
      </div>

      <div className="relative max-w-md">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-surface-400" />
        <input type="text" placeholder="Search users..." value={search} onChange={e => setSearch(e.target.value)} className="w-full pl-10 pr-4 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900 bg-white" id="user-search" />
      </div>

      {loading ? (
        <div className="space-y-3">{[...Array(5)].map((_,i)=><div key={i} className="h-16 bg-surface-200 rounded-xl animate-pulse"/>)}</div>
      ) : (
        <div className="bg-white border border-surface-200 rounded-xl overflow-hidden">
          <table className="w-full">
            <thead><tr className="bg-surface-50 border-b border-surface-200">
              <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase">User</th>
              <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase">Email</th>
              <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase">Role</th>
              <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase">Status</th>
              <th className="text-right px-5 py-3 text-xs font-semibold text-surface-500 uppercase">Actions</th>
            </tr></thead>
            <tbody>
              {filtered.map((u) => (
                <UserRow key={u.id} user={u} onEdit={()=>{setEditUser(u);setShowForm(true)}} onDelete={()=>handleDelete(u.id)} onAvatarUpload={handleAvatarUpload} />
              ))}
              {filtered.length===0 && <tr><td colSpan={5} className="text-center py-8 text-surface-400">No users found</td></tr>}
            </tbody>
          </table>
        </div>
      )}

      {showForm && <UserFormModal user={editUser} onClose={()=>setShowForm(false)} onSave={()=>{setShowForm(false);fetchUsers()}} />}
    </div>
  );
}

function UserRow({ user, onEdit, onDelete, onAvatarUpload }) {
  const fileRef = useRef(null);
  return (
    <tr className="border-b border-surface-100 hover:bg-surface-50 transition-colors">
      <td className="px-5 py-3">
        <div className="flex items-center gap-3">
          <div className="relative group">
            {user.avatarPath ? (
              <img src={user.avatarPath} alt="" className="w-9 h-9 rounded-full object-cover"/>
            ) : (
              <div className="w-9 h-9 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-sm font-semibold">{user.firstName[0]}{user.lastName[0]}</div>
            )}
            <button onClick={()=>fileRef.current?.click()} className="absolute inset-0 bg-black/40 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
              <Camera className="w-3.5 h-3.5 text-white"/>
            </button>
            <input ref={fileRef} type="file" accept="image/*" className="hidden" onChange={e=>{if(e.target.files[0])onAvatarUpload(user.id,e.target.files[0])}}/>
          </div>
          <span className="font-medium text-surface-900 text-sm">{user.firstName} {user.lastName}</span>
        </div>
      </td>
      <td className="px-5 py-3 text-sm text-surface-600">{user.email}</td>
      <td className="px-5 py-3"><span className={`text-xs px-2 py-1 rounded-full font-medium ${user.role==='Admin'?'bg-purple-50 text-purple-600':user.role==='Manager'?'bg-blue-50 text-blue-600':'bg-surface-100 text-surface-600'}`}><Shield className="w-3 h-3 inline mr-1"/>{user.role}</span></td>
      <td className="px-5 py-3"><span className={`text-xs px-2 py-1 rounded-full font-medium ${user.isActive?'bg-emerald-50 text-emerald-600':'bg-red-50 text-red-600'}`}>{user.isActive?'Active':'Inactive'}</span></td>
      <td className="px-5 py-3 text-right">
        <button onClick={onEdit} className="p-1.5 rounded-lg hover:bg-surface-100 text-surface-400 hover:text-primary-600"><Edit className="w-4 h-4"/></button>
        <button onClick={onDelete} className="p-1.5 rounded-lg hover:bg-red-50 text-surface-400 hover:text-red-500 ml-1"><Trash2 className="w-4 h-4"/></button>
      </td>
    </tr>
  );
}

function UserFormModal({ user, onClose, onSave }) {
  const [form, setForm] = useState({
    firstName: user?.firstName||'', lastName: user?.lastName||'', email: user?.email||'',
    password: '', role: user?.role||'Employee', isActive: user?.isActive??true,
  });
  const [saving, setSaving] = useState(false);
  const handleSubmit = async (e) => {
    e.preventDefault(); setSaving(true);
    try {
      if (user) {
        await api.put(`/users/${user.id}`, { firstName:form.firstName, lastName:form.lastName, role:form.role, isActive:form.isActive });
      } else {
        await api.post('/users', form);
      }
      onSave();
    } catch(err) { alert(err.response?.data?.message||'Failed'); }
    setSaving(false);
  };
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md animate-scale-in" onClick={e=>e.stopPropagation()}>
        <div className="flex items-center justify-between p-6 border-b border-surface-100">
          <h2 className="text-lg font-semibold">{user?'Edit User':'New User'}</h2>
          <button onClick={onClose} className="p-1 rounded-lg hover:bg-surface-100"><X className="w-5 h-5"/></button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div><label className="block text-sm font-medium text-surface-700 mb-1">First Name</label><input value={form.firstName} onChange={e=>setForm({...form,firstName:e.target.value})} required className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"/></div>
            <div><label className="block text-sm font-medium text-surface-700 mb-1">Last Name</label><input value={form.lastName} onChange={e=>setForm({...form,lastName:e.target.value})} required className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"/></div>
          </div>
          {!user && <div><label className="block text-sm font-medium text-surface-700 mb-1">Email</label><input type="email" value={form.email} onChange={e=>setForm({...form,email:e.target.value})} required className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"/></div>}
          {!user && <div><label className="block text-sm font-medium text-surface-700 mb-1">Password</label><input type="password" value={form.password} onChange={e=>setForm({...form,password:e.target.value})} required minLength={6} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"/></div>}
          <div><label className="block text-sm font-medium text-surface-700 mb-1">Role</label><select value={form.role} onChange={e=>setForm({...form,role:e.target.value})} className="w-full px-3 py-2.5 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none"><option>Admin</option><option>Manager</option><option>Employee</option></select></div>
          {user && <label className="flex items-center gap-2 cursor-pointer"><input type="checkbox" checked={form.isActive} onChange={e=>setForm({...form,isActive:e.target.checked})} className="rounded border-surface-300 text-primary-600"/><span className="text-sm text-surface-700">Active</span></label>}
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={onClose} className="px-4 py-2 text-sm text-surface-600 hover:bg-surface-100 rounded-lg">Cancel</button>
            <button type="submit" disabled={saving} className="px-4 py-2 text-sm bg-primary-600 text-white rounded-lg hover:bg-primary-700 disabled:opacity-50 font-medium">{saving?'Saving...':user?'Update':'Create'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}
