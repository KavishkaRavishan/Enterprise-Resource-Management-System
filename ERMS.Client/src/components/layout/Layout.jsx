import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import NotificationDropdown from '../../features/notifications/NotificationDropdown';

export default function Layout() {
  return (
    <div className="min-h-screen bg-surface-50">
      <Sidebar />
      <main className="ml-64 min-h-screen transition-all duration-300">
        {/* Top Header */}
        <header className="h-16 border-b border-surface-200 bg-white flex items-center justify-end px-6 sticky top-0 z-20">
          <NotificationDropdown />
        </header>
        <div className="p-6 max-w-7xl mx-auto">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
