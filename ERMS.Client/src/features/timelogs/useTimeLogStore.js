import { create } from 'zustand';
import api from '../../api/axios';

export const useTimeLogStore = create((set, get) => ({
  timeLogs: [],
  loading: false,
  totalHours: 0,

  fetchTimeLogsByTask: async (taskId) => {
    set({ loading: true });
    try {
      const response = await api.get(`/timelogs/task/${taskId}`);
      set({ timeLogs: response.data.data || [] });
    } catch (err) {
      console.error('Failed to fetch task time logs', err);
    } finally {
      set({ loading: false });
    }
  },

  fetchTimeLogsByProject: async (projectId) => {
    set({ loading: true });
    try {
      const response = await api.get(`/timelogs/project/${projectId}`);
      set({ timeLogs: response.data.data || [] });
    } catch (err) {
      console.error('Failed to fetch project time logs', err);
    } finally {
      set({ loading: false });
    }
  },

  fetchTotalHours: async (projectId) => {
    try {
      const response = await api.get(`/timelogs/project/${projectId}/total`);
      set({ totalHours: response.data.data || 0 });
    } catch (err) {
      console.error('Failed to fetch project total hours', err);
    }
  },

  logTime: async (logData) => {
    try {
      const response = await api.post('/timelogs', logData);
      const newLog = response.data.data;
      set((state) => ({
        timeLogs: [newLog, ...state.timeLogs],
      }));
      // Refresh project total hours
      if (get().timeLogs.length > 0) {
        // We could extract projectId from state but we can also just trigger total refetch where appropriate.
      }
      return { success: true, data: newLog };
    } catch (err) {
      console.error('Failed to log hours', err);
      return { success: false, error: err.response?.data?.message || 'Failed to log hours' };
    }
  },

  deleteTimeLog: async (id) => {
    try {
      await api.delete(`/timelogs/${id}`);
      set((state) => ({
        timeLogs: state.timeLogs.filter((log) => log.id !== id),
      }));
      return { success: true };
    } catch (err) {
      console.error('Failed to delete time log', err);
      return { success: false, error: err.response?.data?.message || 'Failed to delete time log' };
    }
  },
}));
