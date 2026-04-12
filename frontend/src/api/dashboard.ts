import { apiClient } from './client';
import type { DashboardSummary, SpendTimeline } from '../types';

export const dashboardApi = {
  getSummary: (currency = 'USD') =>
    apiClient.get<DashboardSummary>('/dashboard/summary', { params: { currency } }).then(r => r.data),

  getTimeline: (months = 6, currency = 'USD') =>
    apiClient.get<SpendTimeline[]>('/dashboard/timeline', { params: { months, currency } }).then(r => r.data),
};
