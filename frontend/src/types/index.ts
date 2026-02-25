export type BillingCycle = 'Weekly' | 'Monthly' | 'Quarterly' | 'Yearly';
export type SubscriptionCategory = 'Streaming' | 'Music' | 'Gaming' | 'SaaS' | 'Cloud' | 'Security' | 'Other';
export type TeamRole = 'Member' | 'Admin' | 'Owner';

export interface Subscription {
  id: string;
  name: string;
  logoUrl?: string;
  amount: number;
  currencyCode: string;
  billingCycle: BillingCycle;
  nextBillingDate: string;
  category: SubscriptionCategory;
  isActive: boolean;
  notes?: string;
  monthlyEquivalent: number;
  createdAt: string;
}

export interface DashboardSummary {
  totalActiveSubscriptions: number;
  monthlyTotal: number;
  yearlyTotal: number;
  currency: string;
  budgetLimit?: number;
  budgetUsedPercent?: number;
  byCategory: CategoryBreakdown[];
  renewingSoon: Subscription[];
}

export interface CategoryBreakdown {
  category: SubscriptionCategory;
  monthlyAmount: number;
  count: number;
}

export interface SpendTimeline {
  month: string;
  year: number;
  total: number;
  currency: string;
}

export interface UserProfile {
  id: string;
  email: string;
  fullName: string;
  preferredCurrency: string;
  monthlyBudgetLimit?: number;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: UserProfile;
}

export interface Team {
  id: string;
  name: string;
  description?: string;
  ownerId: string;
  memberCount: number;
  createdAt: string;
}

export interface CreateSubscriptionRequest {
  name: string;
  amount: number;
  currencyCode: string;
  billingCycle: BillingCycle;
  nextBillingDate: string;
  category: SubscriptionCategory;
  logoUrl?: string;
  notes?: string;
  teamId?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
