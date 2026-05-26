import { create } from 'zustand';
import api from '../../api/axios';

export const useAttachmentStore = create((set, get) => ({
  attachments: [],
  loading: false,

  fetchAttachments: async (taskId) => {
    set({ loading: true });
    try {
      const response = await api.get(`/attachments/task/${taskId}`);
      set({ attachments: response.data.data || [] });
    } catch (err) {
      console.error('Failed to fetch task attachments', err);
    } finally {
      set({ loading: false });
    }
  },

  uploadAttachment: async (taskId, file) => {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await api.post(`/attachments/task/${taskId}`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      const newAttachment = response.data.data;
      set((state) => ({
        attachments: [newAttachment, ...state.attachments],
      }));
      return { success: true, data: newAttachment };
    } catch (err) {
      console.error('Failed to upload file attachment', err);
      return {
        success: false,
        error: err.response?.data?.message || err.response?.data || 'Failed to upload attachment',
      };
    }
  },

  deleteAttachment: async (id) => {
    try {
      await api.delete(`/attachments/${id}`);
      set((state) => ({
        attachments: state.attachments.filter((a) => a.id !== id),
      }));
      return { success: true };
    } catch (err) {
      console.error('Failed to delete attachment', err);
      return {
        success: false,
        error: err.response?.data?.message || 'Failed to delete attachment',
      };
    }
  },
}));
