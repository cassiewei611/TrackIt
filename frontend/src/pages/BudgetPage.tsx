import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts';
import { dashboardApi } from '../api/dashboard';
import { BudgetMeter } from '../components/BudgetMeter';
import { CATEGORY_COLORS } from '../lib/utils';

interface BudgetPageProps {
  currency: string;
}

export function BudgetPage({ currency }: BudgetPageProps) {
  const [budgetInput, setBudgetInput] = useState('100');

  const { data: summary } = useQuery({
    queryKey: ['dashboard', 'summary', currency],
    queryFn: () => dashboardApi.getSummary(currency),
  });

  const monthlyTotal = summary?.monthlyTotal ?? 0;
  const budgetLimit = summary?.budgetLimit ?? (parseFloat(budgetInput) || 100);

  const categoryData = (summary?.byCategory ?? []).map(c => ({
    name: c.category,
    value: c.monthlyAmount,
    color: CATEGORY_COLORS[c.category] ?? '#6b7280',
  }));

  return (
    <div style={{ padding: '32px 0' }}>
      <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 24, fontWeight: 700, marginBottom: 8 }}>Budget Planner</div>
      <div style={{ color: 'rgba(255,255,255,0.35)', fontSize: 12, marginBottom: 32 }}>Set limits and track your spending health</div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 20 }}>
        <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.07)', borderRadius: 16, padding: 28 }}>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 16, fontWeight: 600, marginBottom: 24 }}>Monthly Budget</div>
          <BudgetMeter spent={monthlyTotal} limit={budgetLimit} />
          <div style={{ marginTop: 28 }}>
            <div style={{ color: 'rgba(255,255,255,0.4)', fontSize: 11, letterSpacing: '0.08em', textTransform: 'uppercase', marginBottom: 10 }}>Set Budget Limit</div>
            <div style={{ display: 'flex', gap: 10 }}>
              <input
                value={budgetInput}
                onChange={e => setBudgetInput(e.target.value)}
                type="number"
                style={{ flex: 1, background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.12)', borderRadius: 10, padding: '10px 14px', color: '#fff', fontSize: 14, outline: 'none', fontFamily: "'IBM Plex Mono', monospace" }}
              />
              <button
                style={{ padding: '10px 20px', background: 'linear-gradient(135deg,#a78bfa,#818cf8)', border: 'none', borderRadius: 10, color: '#fff', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace", fontSize: 13 }}
              >
                Save
              </button>
            </div>
          </div>
        </div>

        <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.07)', borderRadius: 16, padding: 28 }}>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 16, fontWeight: 600, marginBottom: 20 }}>Spending Breakdown</div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 16, marginBottom: 20 }}>
            <ResponsiveContainer width={100} height={100}>
              <PieChart>
                <Pie data={categoryData} dataKey="value" cx="50%" cy="50%" innerRadius={28} outerRadius={46} strokeWidth={0}>
                  {categoryData.map((entry, i) => <Cell key={i} fill={entry.color} />)}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
            <div style={{ flex: 1 }}>
              {categoryData.map(cat => (
                <div key={cat.name} style={{ marginBottom: 10 }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                    <span style={{ fontSize: 12, color: 'rgba(255,255,255,0.6)', display: 'flex', alignItems: 'center', gap: 6 }}>
                      <span style={{ width: 8, height: 8, borderRadius: '50%', background: cat.color, display: 'inline-block' }} />
                      {cat.name}
                    </span>
                    <span style={{ fontSize: 12, color: 'rgba(255,255,255,0.8)', fontWeight: 600 }}>${cat.value.toFixed(2)}</span>
                  </div>
                  <div style={{ height: 4, background: 'rgba(255,255,255,0.06)', borderRadius: 99 }}>
                    <div style={{ height: '100%', width: monthlyTotal > 0 ? `${(cat.value / monthlyTotal) * 100}%` : '0%', background: cat.color, borderRadius: 99 }} />
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(167,139,250,0.15)', borderRadius: 16, padding: 28, gridColumn: '1 / -1' }}>
          <div style={{ display: 'flex', gap: 16, alignItems: 'flex-start' }}>
            <div style={{ width: 44, height: 44, borderRadius: 12, background: 'rgba(167,139,250,0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20, flexShrink: 0 }}>💡</div>
            <div>
              <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 15, fontWeight: 600, marginBottom: 8 }}>Smart Insights</div>
              <div style={{ color: 'rgba(255,255,255,0.45)', fontSize: 13, lineHeight: 1.7 }}>
                {summary?.totalActiveSubscriptions
                  ? <>You have <strong style={{ color: '#a78bfa' }}>{summary.totalActiveSubscriptions}</strong> active subscriptions totaling <strong style={{ color: '#60a5fa' }}>${monthlyTotal.toFixed(2)}/month</strong>. {summary.budgetUsedPercent && summary.budgetUsedPercent > 80 ? <span style={{ color: '#f87171' }}>You are close to your budget limit!</span> : 'You are within budget.'}</>
                  : 'Add subscriptions to see spending insights.'}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
