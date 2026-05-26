import { useState, useEffect } from 'react';
import api from '../../api/axios';
import { useAuthStore } from '../auth/useAuthStore';
import { Users, FolderKanban, CheckSquare, Clock, TrendingUp, AlertCircle } from 'lucide-react';

export default function DashboardPage() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const { user } = useAuthStore();

  useEffect(() => {
    api.get('/dashboard')
      .then((res) => setData(res.data.data))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="space-y-6 animate-pulse">
        <div className="h-8 bg-surface-200 rounded w-48" />
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {[...Array(4)].map((_, i) => (
            <div key={i} className="h-32 bg-surface-200 rounded-xl" />
          ))}
        </div>
      </div>
    );
  }

  const stats = [
    { label: 'Total Users', value: data?.totalUsers ?? 0, icon: Users, color: 'from-blue-500 to-blue-600', bg: 'bg-blue-50', text: 'text-blue-600' },
    { label: 'Active Projects', value: data?.activeProjects ?? 0, icon: FolderKanban, color: 'from-purple-500 to-purple-600', bg: 'bg-purple-50', text: 'text-purple-600' },
    { label: 'Pending Tasks', value: data?.pendingTasks ?? 0, icon: Clock, color: 'from-amber-500 to-amber-600', bg: 'bg-amber-50', text: 'text-amber-600' },
    { label: 'Completed Tasks', value: data?.completedTasks ?? 0, icon: CheckSquare, color: 'from-emerald-500 to-emerald-600', bg: 'bg-emerald-50', text: 'text-emerald-600' },
  ];

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-surface-900">
          Welcome back, {user?.firstName}! 👋
        </h1>
        <p className="text-surface-500 mt-1">Here's what's happening with your projects today.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {stats.map((stat, idx) => (
          <div
            key={stat.label}
            className="bg-white rounded-xl border border-surface-200 p-5 hover:shadow-lg transition-all duration-300 group"
            style={{ animationDelay: `${idx * 80}ms` }}
          >
            <div className="flex items-center justify-between mb-3">
              <div className={`p-2.5 rounded-lg ${stat.bg}`}>
                <stat.icon className={`w-5 h-5 ${stat.text}`} />
              </div>
              <TrendingUp className="w-4 h-4 text-surface-300 group-hover:text-success transition-colors" />
            </div>
            <p className="text-3xl font-bold text-surface-900">{stat.value}</p>
            <p className="text-sm text-surface-500 mt-1">{stat.label}</p>
          </div>
        ))}
      </div>

      {/* Projects Overview & Task Distribution */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Projects */}
        <div className="bg-white rounded-xl border border-surface-200 p-6">
          <h2 className="text-lg font-semibold text-surface-900 mb-4">Project Progress</h2>
          {data?.projectSummaries?.length > 0 ? (
            <div className="space-y-4">
              {data.projectSummaries.map((project) => (
                <div key={project.id} className="space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium text-surface-700">{project.name}</span>
                    <span className="text-xs text-surface-500">{project.progressPercentage}%</span>
                  </div>
                  <div className="w-full bg-surface-100 rounded-full h-2">
                    <div
                      className="bg-gradient-to-r from-primary-500 to-primary-600 h-2 rounded-full transition-all duration-500"
                      style={{ width: `${Math.max(project.progressPercentage, 2)}%` }}
                    />
                  </div>
                  <div className="flex items-center gap-4 text-xs text-surface-500">
                    <span>{project.completedTasks}/{project.totalTasks} tasks</span>
                    <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${
                      project.status === 'InProgress' ? 'bg-blue-50 text-blue-600' :
                      project.status === 'Completed' ? 'bg-emerald-50 text-emerald-600' :
                      'bg-surface-100 text-surface-600'
                    }`}>{project.status}</span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-surface-400 text-sm">No projects yet.</p>
          )}
        </div>

        {/* Task Distribution */}
        <div className="bg-white rounded-xl border border-surface-200 p-6">
          <h2 className="text-lg font-semibold text-surface-900 mb-4">Task Distribution</h2>
          <div className="space-y-3">
            {[
              { label: 'To Do', value: data?.pendingTasks ?? 0, color: 'bg-amber-500' },
              { label: 'In Progress', value: data?.inProgressTasks ?? 0, color: 'bg-blue-500' },
              { label: 'Completed', value: data?.completedTasks ?? 0, color: 'bg-emerald-500' },
            ].map((item) => {
              const total = (data?.totalTasks || 1);
              const pct = Math.round((item.value / total) * 100);
              return (
                <div key={item.label} className="flex items-center gap-3">
                  <div className={`w-3 h-3 rounded-full ${item.color}`} />
                  <span className="text-sm text-surface-700 w-28">{item.label}</span>
                  <div className="flex-1 bg-surface-100 rounded-full h-2">
                    <div
                      className={`${item.color} h-2 rounded-full transition-all duration-500`}
                      style={{ width: `${Math.max(pct, 2)}%` }}
                    />
                  </div>
                  <span className="text-sm font-medium text-surface-900 w-12 text-right">{item.value}</span>
                </div>
              );
            })}
          </div>

          <div className="mt-6 pt-4 border-t border-surface-100">
            <div className="flex items-center justify-between text-sm">
              <span className="text-surface-500">Total Tasks</span>
              <span className="font-semibold text-surface-900">{data?.totalTasks ?? 0}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
