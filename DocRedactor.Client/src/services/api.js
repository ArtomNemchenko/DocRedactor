import axios from 'axios';

const API_BASE_URL = 'http://localhost:5180/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  register: async (email, userName, password) => {
    const response = await api.post('/auth/register', { email, userName, password });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  },

  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getCurrentUser: () => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },
};

export const documentService = {
  getAll: async () => {
    const response = await api.get('/documents');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/documents/${id}`);
    return response.data;
  },

  create: async (title, content) => {
    const response = await api.post('/documents', { title, content });
    return response.data;
  },

  update: async (id, content, changeDescription) => {
    const response = await api.put(`/documents/${id}`, { content, changeDescription });
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/documents/${id}`);
  },

  getVersions: async (id) => {
    const response = await api.get(`/documents/${id}/versions`);
    return response.data;
  },

  revertToVersion: async (id, versionNumber, changeDescription) => {
    const response = await api.post(`/documents/${id}/revert`, { versionNumber, changeDescription });
    return response.data;
  },

  rateVersion: async (documentId, versionId, rating) => {
    const response = await api.put(`/documents/${documentId}/versions/${versionId}/rate`, { rating });
    return response.data;
  },
};

export default api;
