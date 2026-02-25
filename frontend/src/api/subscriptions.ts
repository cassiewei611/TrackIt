import { apiClient } from './client';
import type { Subscription, CreateSubscriptionRequest } from '../types';

export const subscriptionsApi = {
  getAll: (params?: { includeInactive?: boolean; category?: string; search?: string }) =>
    apiClient.get<Subscription[]>('/subscriptions', { params }).then(r => r.data),

  getById: (id: string) =>
    apiClient.get<Subscription>(`/subscriptions/${id}`).then(r => r.data),

  create: (data: CreateSubscriptionRequest) =>
    apiClient.post<Subscription>('/subscriptions', data).then(r => r.data),

  update: (id: string, data: Partial<CreateSubscriptionRequest>) =>
    apiClient.put<Subscription>(`/subscriptions/${id}`, data).then(r => r.data),

  delete: (id: string) =>
    apiClient.delete(`/subscriptions/${id}`),
};
