import { useState, useEffect } from 'react';
import api from '../../api/axios';
import { useAuthStore } from '../auth/useAuthStore';
import { Users, FolderKanban, CheckSquare, Clock, TrendingUp, AlertCircle, BarChart3, PieChart as PieIcon, LayoutGrid } from 'lucide-react';
import {
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend
} from 'recharts';

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
      <div className="space-y-6">
        <div className="space-y-2">
          <div className="h-8 bg-surface-100 rounded w-48 animate-pulse" />
          <div className="h-4 bg-surface-50 rounded w-80 animate-pulse" />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {[...Array(4)].map((_, i) => (
            <div key={i} className="h-32 bg-white border border-surface-150 rounded-2xl p-5 space-y-4 shadow-sm animate-pulse">
              <div className="flex justify-between items-center">
                <div className="w-10 h-10 bg-surface-100 rounded-xl" />
                <div className="w-4 h-4 bg-surface-100 rounded" />
              </div>
              <div className="h-8 bg-surface-100 rounded w-16" />
              <div className="h-3 bg-surface-50 rounded w-24" />
            </div>
          ))}
        </div>
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="h-80 bg-white border border-surface-150 rounded-2xl p-6 shadow-sm animate-pulse" />
          <div className="h-80 bg-white border border-surface-150 rounded-2xl p-6 shadow-sm animate-pulse" />
        </div>
      </div>
    );
  }

  const stats = [
    { label: 'Total Users', value: data?.totalUsers ?? 0, icon: Users, color: 'text-primary-600', bg: 'bg-primary-50', trend: 'Users logged' },
    { label: 'Active Projects', value: data?.activeProjects ?? 0, icon: FolderKanban, color: 'text-indigo-600', bg: 'bg-indigo-50', trend: 'Active workspace' },
    { label: 'Pending Tasks', value: data?.pendingTasks ?? 0, icon: Clock, color: 'text-amber-600', bg: 'bg-amber-50', trend: 'To do backlog' },
    { label: 'Completed Tasks', value: data?.completedTasks ?? 0, icon: CheckSquare, color: 'text-emerald-600', bg: 'bg-emerald-50', trend: 'Tasks finished' },
  ];

  // Pie chart task distribution data
  const taskDistributionData = [
    { name: 'To Do', value: data?.pendingTasks ?? 0, color: '#f59e0b' },
    { name: 'In Progress', value: data?.inProgressTasks ?? 0, color: '#3b82f6' },
    { name: 'Completed', value: data?.completedTasks ?? 0, color: '#10b981' }
  ].filter(item => item.value > 0);

  // Fallback if no task data
  const chartTaskData = taskDistributionData.length > 0 
    ? taskDistributionData 
    : [{ name: 'No Tasks', value: 1, color: '#e2e8f0' }];

  // Bar chart logged hours data per project
  const projectHoursData = (data?.projectSummaries ?? []).map(p => ({
    name: p.name.length > 15 ? `${p.name.substring(0, 15)}...` : p.name,
    Hours: p.totalHoursLogged,
    fullName: p.name
  }));

  const CustomTooltip = ({ active, payload }) => {
    if (active && payload && payload.length) {
      return (
        <div className="bg-white border border-surface-200 shadow-xl rounded-xl p-3 text-xs">
          <p className="font-bold text-surface-900 mb-1">{payload[0].payload.fullName || payload[0].name}</p>
          <div className="flex items-center gap-2">
            <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: payload[0].fill || payload[0].color }} />
            <span className="text-surface-600 font-medium">
              {payload[0].name}: <span className="font-bold text-surface-900">{payload[0].value} {payload[0].name === 'Hours' ? 'hrs' : 'tasks'}</span>
            </span>
          </div>
        </div>
      );
    }
    return null;
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">
            Welcome back, {user?.firstName}! 👋
          </h1>
          <p className="text-surface-500 mt-1 text-sm">Here's a premium visual report of your business operations today.</p>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {stats.map((stat, idx) => (
          <div
            key={stat.label}
            className="bg-white rounded-2xl border border-surface-200 p-5 shadow-sm hover:shadow-md transition-all duration-300 group flex flex-col justify-between"
            style={{ animationDelay: `${idx * 80}ms` }}
          >
            <div className="flex items-center justify-between">
              <div className={`p-2.5 rounded-xl ${stat.bg}`}>
                <stat.icon className={`w-5 h-5 ${stat.color}`} />
              </div>
              <TrendingUp className="w-4 h-4 text-surface-300 group-hover:text-primary-500 transition-colors" />
            </div>
            <div className="mt-4">
              <p className="text-3xl font-extrabold text-surface-900 tracking-tight">{stat.value}</p>
              <p className="text-xs font-semibold text-surface-400 uppercase tracking-wider mt-1">{stat.label}</p>
            </div>
            <div className="mt-3 pt-3 border-t border-surface-100 flex justify-between items-center text-[10px] text-surface-400">
              <span>{stat.trend}</span>
              <span className="font-semibold text-surface-600">Active</span>
            </div>
          </div>
        ))}
      </div>

      {/* Visual Analytics Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Project Logged Hours Bar Chart */}
        <div className="bg-white rounded-2xl border border-surface-200 p-6 shadow-sm flex flex-col justify-between">
          <div className="flex items-center justify-between mb-4 border-b border-surface-100 pb-3">
            <div className="flex items-center gap-2">
              <BarChart3 className="w-4 h-4 text-primary-500" />
              <h2 className="text-sm font-bold text-surface-900 uppercase tracking-wider">Project Timesheets</h2>
            </div>
            <span className="text-[10px] font-semibold text-primary-700 bg-primary-50 px-2.5 py-0.5 rounded-full">Total Hours Logged</span>
          </div>

          <div className="h-64 w-full">
            {projectHoursData.length > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={projectHoursData} margin={{ top: 10, right: 10, left: -25, bottom: 0 }}>
                  <defs>
                    <linearGradient id="barGradient" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="0%" stopColor="#4f46e5" />
                      <stop offset="100%" stopColor="#3b82f6" />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                  <XAxis dataKey="name" tickLine={false} axisLine={false} tick={{ fill: '#64748b', fontSize: 10 }} />
                  <YAxis tickLine={false} axisLine={false} tick={{ fill: '#64748b', fontSize: 10 }} />
                  <Tooltip content={<CustomTooltip />} cursor={{ fill: '#f8fafc' }} />
                  <Bar dataKey="Hours" fill="url(#barGradient)" radius={[6, 6, 0, 0]} barSize={28} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <div className="h-full flex flex-col items-center justify-center text-center p-6 text-surface-400">
                <BarChart3 className="w-8 h-8 text-surface-300 mb-2" />
                <p className="text-xs">No hours logged per project yet.</p>
              </div>
            )}
          </div>
        </div>

        {/* Task Distribution Donut Chart */}
        <div className="bg-white rounded-2xl border border-surface-200 p-6 shadow-sm flex flex-col justify-between">
          <div className="flex items-center justify-between mb-4 border-b border-surface-100 pb-3">
            <div className="flex items-center gap-2">
              <PieIcon className="w-4 h-4 text-indigo-500" />
              <h2 className="text-sm font-bold text-surface-900 uppercase tracking-wider">Task Distribution</h2>
            </div>
            <span className="text-[10px] font-semibold text-indigo-700 bg-indigo-50 px-2.5 py-0.5 rounded-full">Status Ratios</span>
          </div>

          <div className="h-64 w-full flex items-center justify-center">
            <div className="relative w-full h-full flex items-center justify-center">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={chartTaskData}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={85}
                    paddingAngle={3}
                    dataKey="value"
                  >
                    {chartTaskData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip content={<CustomTooltip />} />
                </PieChart>
              </ResponsiveContainer>

              {/* Central counter */}
              <div className="absolute flex flex-col items-center justify-center">
                <span className="text-2xl font-black text-surface-900">
                  {data?.totalTasks ?? 0}
                </span>
                <span className="text-[10px] text-surface-400 font-semibold uppercase tracking-wider">
                  Total Tasks
                </span>
              </div>
            </div>
          </div>

          {/* Color Legends */}
          <div className="flex items-center justify-center gap-6 mt-2 pt-3 border-t border-surface-50 text-xs">
            {[
              { label: 'To Do', color: '#f59e0b', count: data?.pendingTasks ?? 0 },
              { label: 'In Progress', color: '#3b82f6', count: data?.inProgressTasks ?? 0 },
              { label: 'Completed', color: '#10b981', count: data?.completedTasks ?? 0 }
            ].map(item => (
              <div key={item.label} className="flex items-center gap-2">
                <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: item.color }} />
                <span className="text-surface-600 font-medium">
                  {item.label} (<span className="font-bold text-surface-950">{item.count}</span>)
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Projects Overview progress lists */}
      <div className="bg-white rounded-2xl border border-surface-200 p-6 shadow-sm">
        <div className="flex items-center gap-2 mb-6 border-b border-surface-100 pb-3">
          <LayoutGrid className="w-4 h-4 text-emerald-500" />
          <h2 className="text-sm font-bold text-surface-900 uppercase tracking-wider">Project Progress Overview</h2>
        </div>

        {data?.projectSummaries?.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {data.projectSummaries.map((project) => (
              <div key={project.id} className="bg-surface-50/50 rounded-xl p-4 border border-surface-150 space-y-3 hover:bg-surface-50 transition-colors">
                <div className="flex items-center justify-between">
                  <span className="text-sm font-bold text-surface-800">{project.name}</span>
                  <span className="text-xs font-bold text-primary-600 bg-primary-50 px-2 py-0.5 rounded">
                    {project.progressPercentage}% Complete
                  </span>
                </div>
                
                {/* Custom Gradient Progress Bar */}
                <div className="w-full bg-surface-200 rounded-full h-2">
                  <div
                    className="bg-gradient-to-r from-primary-500 to-indigo-500 h-2 rounded-full transition-all duration-500"
                    style={{ width: `${Math.max(project.progressPercentage, 2)}%` }}
                  />
                </div>

                <div className="flex items-center justify-between text-xs text-surface-500">
                  <div className="flex items-center gap-3">
                    <span>{project.completedTasks} / {project.totalTasks} Tasks</span>
                    <span>•</span>
                    <span className="font-medium text-surface-700">{project.totalHoursLogged} hrs logged</span>
                  </div>
                  <span className={`px-2.5 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider ${
                    project.status === 'InProgress' ? 'bg-blue-50 text-blue-600 border border-blue-100' :
                    project.status === 'Completed' ? 'bg-emerald-50 text-emerald-600 border border-emerald-100' :
                    'bg-surface-100 text-surface-600'
                  }`}>{project.status}</span>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center py-10 text-surface-400 text-xs">
            No projects added to display progress metrics.
          </div>
        )}
      </div>
    </div>
  );
}
