import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

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

  delete: async (id) => {
    await api.delete(`/documents/${id}`);
  },
};

export const redactionService = {
  getByDocument: async (documentId) => {
    const response = await api.get(`/redactions/document/${documentId}`);
    return response.data;
  },

  create: async (documentId, startPosition, endPosition, reason) => {
    const response = await api.post('/redactions', {
      documentId,
      startPosition,
      endPosition,
      reason,
    });
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/redactions/${id}`);
  },
};

export default api;
