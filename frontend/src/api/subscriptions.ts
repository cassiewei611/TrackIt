import { apiClient } from './client';
import type { Subscription, CreateSubscriptionRequest } from '../types';

export const subscriptionsApi = {
  getAll: (params?: { includeInactive?: boolean; category?: string; search?: string; currency?: string }) =>
    apiClient.get<Subscription[]>('/subscriptions', { params }).then(r => r.data),

  create: (data: CreateSubscriptionRequest) =>
    apiClient.post<Subscription>('/subscriptions', data).then(r => r.data),

  delete: (id: string) =>
    apiClient.delete(`/subscriptions/${id}`),
};
