import { NavLink, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../features/auth/useAuthStore';
import {
  LayoutDashboard,
  FolderKanban,
  Users,
  CheckSquare,
  LogOut,
  ChevronLeft,
  Menu,
} from 'lucide-react';
import { useState } from 'react';

const navItems = [
  { path: '/', label: 'Dashboard', icon: LayoutDashboard, roles: ['Admin', 'Manager', 'Employee'] },
  { path: '/projects', label: 'Projects', icon: FolderKanban, roles: ['Admin', 'Manager', 'Employee'] },
  { path: '/tasks', label: 'My Tasks', icon: CheckSquare, roles: ['Admin', 'Manager', 'Employee'] },
  { path: '/users', label: 'Users', icon: Users, roles: ['Admin'] },
];

export default function Sidebar() {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();
  const [collapsed, setCollapsed] = useState(false);

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const filteredNav = navItems.filter((item) => item.roles.includes(user?.role));

  return (
    <aside
      className={`fixed left-0 top-0 h-screen bg-white border-r border-surface-200 flex flex-col transition-all duration-300 z-30 ${
        collapsed ? 'w-[72px]' : 'w-64'
      }`}
    >
      {/* Header */}
      <div className="h-16 flex items-center justify-between px-4 border-b border-surface-100">
        {!collapsed && (
          <span className="text-lg font-bold bg-gradient-to-r from-primary-600 to-primary-500 bg-clip-text text-transparent">
            ERMS
          </span>
        )}
        <button
          onClick={() => setCollapsed(!collapsed)}
          className="p-2 rounded-lg hover:bg-surface-100 text-surface-500 transition-colors"
          id="sidebar-toggle"
        >
          {collapsed ? <Menu className="w-5 h-5" /> : <ChevronLeft className="w-5 h-5" />}
        </button>
      </div>

      {/* Navigation */}
      <nav className="flex-1 py-4 px-3 space-y-1 overflow-y-auto">
        {filteredNav.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            end={item.path === '/'}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all duration-200 group ${
                isActive
                  ? 'bg-primary-50 text-primary-700 font-medium'
                  : 'text-surface-600 hover:bg-surface-50 hover:text-surface-900'
              }`
            }
          >
            <item.icon className="w-5 h-5 flex-shrink-0" />
            {!collapsed && <span className="text-sm">{item.label}</span>}
          </NavLink>
        ))}
      </nav>

      {/* User section */}
      <div className="p-3 border-t border-surface-100">
        <div className={`flex items-center gap-3 p-2 rounded-lg ${collapsed ? 'justify-center' : ''}`}>
          <div className="w-8 h-8 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-sm font-semibold flex-shrink-0">
            {user?.firstName?.[0]}{user?.lastName?.[0]}
          </div>
          {!collapsed && (
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-surface-900 truncate">
                {user?.firstName} {user?.lastName}
              </p>
              <p className="text-xs text-surface-500">{user?.role}</p>
            </div>
          )}
        </div>
        <button
          onClick={handleLogout}
          className={`mt-2 w-full flex items-center gap-3 px-3 py-2 rounded-lg text-surface-600 hover:bg-danger/10 hover:text-danger transition-all ${
            collapsed ? 'justify-center' : ''
          }`}
          id="logout-button"
        >
          <LogOut className="w-4 h-4 flex-shrink-0" />
          {!collapsed && <span className="text-sm">Logout</span>}
        </button>
      </div>
    </aside>
  );
}
