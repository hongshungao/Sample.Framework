import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

export const entityApi = {
  getAll: () => api.get('/entity'),
  getById: (id) => api.get(`/entity/${id}`),
  create: (entity) => api.post('/entity', entity),
  update: (id, entity) => api.put(`/entity/${id}`, entity),
  delete: (id) => api.delete(`/entity/${id}`),
  generateCode: (entityId) => api.post(`/entity/generate${entityId ? `?entityId=${entityId}` : ''}`)
};

export const enumApi = {
  getAll: () => api.get('/enum'),
  getById: (id) => api.get(`/enum/${id}`),
  create: (enumDef) => api.post('/enum', enumDef),
  update: (id, enumDef) => api.put(`/enum/${id}`, enumDef),
  delete: (id) => api.delete(`/enum/${id}`)
};

export default {
  entity: entityApi,
  enum: enumApi
};