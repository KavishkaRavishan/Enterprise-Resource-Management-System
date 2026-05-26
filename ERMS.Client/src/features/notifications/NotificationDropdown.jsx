import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useNotificationStore } from './useNotificationStore';
import { Bell, Check, Folder, CheckSquare, Sparkles } from 'lucide-react';

export default function NotificationDropdown() {
  const { notifications, fetchNotifications, markAsRead, markAllAsRead } = useNotificationStore();
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef(null);
  const navigate = useNavigate();

  const unreadCount = notifications.filter(n => !n.isRead).length;

  useEffect(() => {
    fetchNotifications();
    // Poll notifications every 20 seconds
    const interval = setInterval(fetchNotifications, 20000);
    return () => clearInterval(interval);
  }, [fetchNotifications]);

  useEffect(() => {
    function handleClickOutside(event) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleNotificationClick = async (n) => {
    if (!n.isRead) {
      await markAsRead(n.id);
    }
    setIsOpen(false);

    if (n.referenceId) {
      if (n.type === 'ProjectAdded') {
        navigate(`/projects/${n.referenceId}`);
      } else if (n.type === 'TaskAssigned' || n.type === 'TaskStatusChanged') {
        // Find which project this task is in is done on details, or we can just redirect to /tasks
        navigate('/tasks');
      }
    }
  };

  const getIcon = (type) => {
    switch (type) {
      case 'ProjectAdded':
        return <Folder className="w-4 h-4 text-indigo-600" />;
      case 'TaskAssigned':
        return <CheckSquare className="w-4 h-4 text-emerald-600" />;
      default:
        return <Sparkles className="w-4 h-4 text-amber-600" />;
    }
  };

  return (
    <div className="relative" ref={dropdownRef}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="relative p-2 rounded-full hover:bg-surface-100 text-surface-600 transition-all focus:outline-none"
        id="notification-bell-btn"
      >
        <Bell className="w-5 h-5" />
        {unreadCount > 0 && (
          <span className="absolute top-1 right-1 flex h-4 w-4">
            <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary-400 opacity-75"></span>
            <span className="relative inline-flex rounded-full h-4 w-4 bg-primary-600 text-[10px] text-white font-bold items-center justify-center">
              {unreadCount}
            </span>
          </span>
        )}
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 w-80 bg-white rounded-xl border border-surface-200 shadow-xl overflow-hidden z-50 animate-scale-in">
          <div className="flex items-center justify-between px-4 py-3 border-b border-surface-100 bg-surface-50">
            <span className="font-semibold text-sm text-surface-900">Notifications</span>
            {unreadCount > 0 && (
              <button
                onClick={markAllAsRead}
                className="flex items-center gap-1 text-xs text-primary-600 hover:text-primary-700 font-medium"
              >
                <Check className="w-3.5 h-3.5" /> Mark all read
              </button>
            )}
          </div>

          <div className="max-h-72 overflow-y-auto divide-y divide-surface-100">
            {notifications.length === 0 ? (
              <div className="py-8 text-center text-xs text-surface-400">
                No notifications yet
              </div>
            ) : (
              notifications.map((n) => (
                <div
                  key={n.id}
                  onClick={() => handleNotificationClick(n)}
                  className={`flex gap-3 px-4 py-3 hover:bg-surface-50 transition-colors cursor-pointer text-left ${
                    !n.isRead ? 'bg-primary-50/30' : ''
                  }`}
                >
                  <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${
                    n.type === 'ProjectAdded' ? 'bg-indigo-50' : 'bg-emerald-50'
                  }`}>
                    {getIcon(n.type)}
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className={`text-xs text-surface-900 leading-normal ${!n.isRead ? 'font-semibold' : ''}`}>
                      {n.message}
                    </p>
                    <span className="text-[10px] text-surface-400 mt-1 block">
                      {new Date(n.created).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </span>
                  </div>
                  {!n.isRead && (
                    <div className="w-2 h-2 rounded-full bg-primary-600 self-center flex-shrink-0" />
                  )}
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}
