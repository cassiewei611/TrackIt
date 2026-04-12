import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { subscriptionsApi } from '../api/subscriptions';
import { dashboardApi } from '../api/dashboard';
import { AddSubscriptionModal } from '../components/AddSubscriptionModal';
import { CATEGORY_COLORS, daysUntil } from '../lib/utils';

interface SubscriptionsPageProps {
  currency: string;
}

function fmt(n: number, currency: string) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency, maximumFractionDigits: 2 }).format(n);
}

export function SubscriptionsPage({ currency }: SubscriptionsPageProps) {
  const [showModal, setShowModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);
  const queryClient = useQueryClient();

  const { data: subscriptions = [], isLoading } = useQuery({
    queryKey: ['subscriptions', currency],
    queryFn: () => subscriptionsApi.getAll({ includeInactive: true, currency }),
  });

  // Use dashboard summary for accurate converted monthly total
  const { data: summary } = useQuery({
    queryKey: ['dashboard', 'summary', currency],
    queryFn: () => dashboardApi.getSummary(currency),
  });

  const deleteMutation = useMutation({
    mutationFn: subscriptionsApi.delete,
    onSuccess: () => {
      setConfirmDeleteId(null);
      queryClient.invalidateQueries({ queryKey: ['subscriptions', currency] });
      queryClient.invalidateQueries({ queryKey: ['dashboard', 'summary', currency] });
      queryClient.invalidateQueries({ queryKey: ['dashboard', 'timeline'] });
    },
  });

  const filtered = subscriptions.filter(s =>
    s.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const activeCount = subscriptions.filter(s => s.isActive).length;
  const monthlyTotal = summary?.monthlyTotal ?? 0;

  return (
    <div>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '32px 0 24px' }}>
        <div>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 24, fontWeight: 700 }}>Subscriptions</div>
          <div style={{ color: 'rgba(255,255,255,0.35)', fontSize: 12, marginTop: 4 }}>
            {activeCount} active · {fmt(monthlyTotal, currency)}/mo
          </div>
        </div>
        <div style={{ display: 'flex', gap: 10 }}>
          <input
            value={searchTerm}
            onChange={e => setSearchTerm(e.target.value)}
            placeholder="Search..."
            style={{ background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 10, padding: '10px 16px', color: '#fff', fontSize: 13, outline: 'none', fontFamily: "'IBM Plex Mono', monospace", width: 200 }}
          />
          <button
            onClick={() => setShowModal(true)}
            style={{ padding: '10px 20px', background: 'linear-gradient(135deg, #a78bfa, #818cf8)', border: 'none', borderRadius: 10, color: '#fff', cursor: 'pointer', fontSize: 13, fontFamily: "'IBM Plex Mono', monospace", fontWeight: 600 }}
          >
            + Add New
          </button>
        </div>
      </div>

      {isLoading ? (
        <div style={{ textAlign: 'center', color: 'rgba(255,255,255,0.3)', padding: '40px 0' }}>Loading...</div>
      ) : (
        <div style={{ display: 'grid', gap: 10 }}>
          <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 100px', padding: '8px 20px', color: 'rgba(255,255,255,0.3)', fontSize: 10, letterSpacing: '0.1em', textTransform: 'uppercase' }}>
            <span>Service</span><span>Amount</span><span>Cycle</span><span>Category</span><span>Next Billing</span><span style={{ textAlign: 'right' }}>Actions</span>
          </div>

          {filtered.map((s, i) => {
            const days = daysUntil(s.nextBillingDate);
            const logoColor = CATEGORY_COLORS[s.category] ?? '#6b7280';
            return (
              <div
                key={s.id}
                className="fade-up"
                style={{
                  display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 100px',
                  padding: '16px 20px',
                  background: s.isActive ? 'rgba(255,255,255,0.03)' : 'rgba(255,255,255,0.015)',
                  border: '1px solid rgba(255,255,255,0.06)',
                  borderRadius: 12,
                  alignItems: 'center',
                  animationDelay: `${i * 0.05}s`,
                  opacity: s.isActive ? 1 : 0.5,
                }}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
                  <div style={{ width: 40, height: 40, borderRadius: 10, background: logoColor, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16, fontWeight: 700, color: '#fff', flexShrink: 0 }}>
                    {s.name[0]?.toUpperCase()}
                  </div>
                  <div>
                    <div style={{ fontSize: 14, fontWeight: 600, color: '#fff' }}>{s.name}</div>
                    <div style={{ fontSize: 11, color: 'rgba(255,255,255,0.35)' }}>{s.isActive ? 'Active' : 'Paused'}</div>
                  </div>
                </div>

                <div style={{ fontSize: 15, fontWeight: 600 }}>
                  {fmt(s.amount, s.currencyCode)}
                  {s.splitCount > 1 ? (
                    <div style={{ fontSize: 10, color: '#a78bfa', fontWeight: 400 }}>
                      ÷{s.splitCount} = {s.convertedMonthlyEquivalent != null && s.targetCurrency
                        ? fmt(s.convertedMonthlyEquivalent, s.targetCurrency)
                        : fmt(s.effectiveMonthlyAmount, s.currencyCode)}/mo
                    </div>
                  ) : s.convertedMonthlyEquivalent != null && s.targetCurrency ? (
                    <div style={{ fontSize: 10, color: 'rgba(167,139,250,0.7)', fontWeight: 400 }}>
                      ≈ {fmt(s.convertedMonthlyEquivalent, s.targetCurrency)}/mo
                    </div>
                  ) : s.billingCycle !== 'Monthly' ? (
                    <div style={{ fontSize: 10, color: 'rgba(255,255,255,0.3)', fontWeight: 400 }}>
                      {fmt(s.monthlyEquivalent, s.currencyCode)}/mo
                    </div>
                  ) : null}
                </div>

                <div style={{ fontSize: 12, color: 'rgba(255,255,255,0.5)' }}>{s.billingCycle}</div>

                <div>
                  <span style={{ fontSize: 11, padding: '3px 10px', borderRadius: 99, background: 'rgba(167,139,250,0.1)', color: '#a78bfa', border: '1px solid rgba(167,139,250,0.2)' }}>{s.category}</span>
                </div>

                <div style={{ fontSize: 12 }}>
                  <div style={{ color: days <= 3 ? '#fbbf24' : 'rgba(255,255,255,0.6)' }}>
                    {new Date(s.nextBillingDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                  </div>
                  <div style={{ fontSize: 10, color: days <= 3 ? '#f87171' : 'rgba(255,255,255,0.3)', marginTop: 2 }}>
                    {days <= 0 ? 'Today' : `${days}d away`}
                  </div>
                </div>

                <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 6 }}>
                  {confirmDeleteId === s.id ? (
                    <>
                      <button
                        onClick={() => deleteMutation.mutate(s.id)}
                        disabled={deleteMutation.isPending}
                        style={{ fontSize: 11, padding: '4px 10px', background: 'rgba(248,113,113,0.15)', border: '1px solid rgba(248,113,113,0.4)', borderRadius: 6, color: '#f87171', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace" }}
                      >
                        Confirm
                      </button>
                      <button
                        onClick={() => setConfirmDeleteId(null)}
                        style={{ fontSize: 11, padding: '4px 10px', background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 6, color: 'rgba(255,255,255,0.4)', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace" }}
                      >
                        Cancel
                      </button>
                    </>
                  ) : (
                    <button
                      onClick={() => setConfirmDeleteId(s.id)}
                      style={{ fontSize: 11, padding: '4px 10px', background: 'transparent', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 6, color: 'rgba(255,255,255,0.3)', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace" }}
                    >
                      Delete
                    </button>
                  )}
                </div>
              </div>
            );
          })}

          {filtered.length === 0 && (
            <div style={{ textAlign: 'center', color: 'rgba(255,255,255,0.3)', padding: '40px 0', fontSize: 14 }}>
              {searchTerm ? 'No subscriptions match your search.' : 'No subscriptions yet. Add your first one!'}
            </div>
          )}
        </div>
      )}

      {showModal && <AddSubscriptionModal onClose={() => setShowModal(false)} />}
    </div>
  );
}
