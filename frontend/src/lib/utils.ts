export const daysUntil = (dateStr: string): number => {
  const diff = new Date(dateStr).getTime() - new Date().getTime();
  return Math.ceil(diff / (1000 * 60 * 60 * 24));
};

export const monthlyEquivalent = (amount: number, cycle: string): number => {
  switch (cycle) {
    case 'Yearly': return amount / 12;
    case 'Weekly': return (amount * 52) / 12;
    case 'Quarterly': return amount / 3;
    default: return amount;
  }
};

export const CATEGORY_COLORS: Record<string, string> = {
  Streaming: '#E50914',
  Music: '#1DB954',
  Gaming: '#107C10',
  SaaS: '#a78bfa',
  Cloud: '#60a5fa',
  Security: '#1A8CFF',
  Other: '#6b7280',
};

export const CURRENCIES = ['USD', 'EUR', 'GBP', 'DKK', 'SEK', 'NOK', 'JPY', 'CHF', 'CAD', 'AUD'];

export const CATEGORIES = ['Streaming', 'Music', 'Gaming', 'SaaS', 'Cloud', 'Security', 'Other'] as const;

export const BILLING_CYCLES = ['Weekly', 'Monthly', 'Quarterly', 'Yearly'] as const;

export const formatCurrency = (amount: number, currency = 'USD'): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
  }).format(amount);
};
