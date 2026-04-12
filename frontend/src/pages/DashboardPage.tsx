import { useQuery } from '@tanstack/react-query';
import {
  LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer,
  PieChart, Pie, Cell,
} from 'recharts';
import { dashboardApi } from '../api/dashboard';
import { StatCard } from '../components/StatCard';
import { CATEGORY_COLORS, daysUntil } from '../lib/utils';

interface DashboardPageProps {
  currency: string;
}

function fmt(amount: number, currency: string) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency, maximumFractionDigits: 2 }).format(amount);
}

function fmtShort(amount: number, currency: string) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency, maximumFractionDigits: 0 }).format(amount);
}

function currencySymbol(currency: string) {
  return (0).toLocaleString('en-US', { style: 'currency', currency, minimumFractionDigits: 0 }).replace(/[\d.,\s]/g, '').trim();
}

export function DashboardPage({ currency }: DashboardPageProps) {
  const { data: summary, isLoading } = useQuery({
    queryKey: ['dashboard', 'summary', currency],
    queryFn: () => dashboardApi.getSummary(currency),
  });

  const { data: timeline } = useQuery({
    queryKey: ['dashboard', 'timeline', currency],
    queryFn: () => dashboardApi.getTimeline(6, currency),
  });

  if (isLoading) {
    return (
      <div style={{ padding: '80px 0', textAlign: 'center', color: 'rgba(255,255,255,0.3)' }}>
        Loading dashboard...
      </div>
    );
  }

  const sym = currencySymbol(currency);

  const categoryData = (summary?.byCategory ?? []).map(c => ({
    name: c.category,
    value: c.monthlyAmount,
    color: CATEGORY_COLORS[c.category] ?? '#6b7280',
  }));

  const timelineData = (timeline ?? []).map(t => ({
    month: t.month,
    amount: t.total,
  }));

  return (
    <div>
      <div style={{ padding: '32px 0 20px' }}>
        <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 28, fontWeight: 700 }}>Dashboard</div>
        <div style={{ color: 'rgba(255,255,255,0.35)', fontSize: 13, marginTop: 6 }}>
          Overview for {new Date().toLocaleString('default', { month: 'long', year: 'numeric' })}
        </div>
      </div>

      <div style={{ display: 'flex', gap: 16, marginBottom: 24 }}>
        <StatCard
          label="Monthly Total"
          value={fmt(summary?.monthlyTotal ?? 0, currency)}
          sub={`${currency} · ${summary?.totalActiveSubscriptions ?? 0} active`}
          accent="linear-gradient(90deg,#a78bfa,#818cf8)"
        />
        <StatCard
          label="Yearly Projection"
          value={fmtShort(summary?.yearlyTotal ?? 0, currency)}
          sub="at current rate"
          accent="linear-gradient(90deg,#60a5fa,#34d399)"
        />
        <StatCard
          label="Budget Used"
          value={summary?.budgetUsedPercent != null ? `${summary.budgetUsedPercent.toFixed(0)}%` : 'No limit'}
          sub={summary?.budgetLimit ? `${fmt(summary.budgetLimit, currency)} limit` : 'Set a budget limit'}
          accent="linear-gradient(90deg,#f472b6,#fb923c)"
        />
        <StatCard
          label="Renewing Soon"
          value={summary?.renewingSoon.length ?? 0}
          sub="within 7 days"
          accent="linear-gradient(90deg,#fbbf24,#f87171)"
        />
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1.4fr 1fr', gap: 20 }}>
        <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.07)', borderRadius: 16, padding: 24 }}>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 16, fontWeight: 600, marginBottom: 20 }}>Monthly Spend</div>
          <ResponsiveContainer width="100%" height={200}>
            <LineChart data={timelineData}>
              <XAxis dataKey="month" stroke="rgba(255,255,255,0.2)" tick={{ fill: 'rgba(255,255,255,0.4)', fontSize: 11 }} axisLine={false} tickLine={false} />
              <YAxis stroke="rgba(255,255,255,0.2)" tick={{ fill: 'rgba(255,255,255,0.4)', fontSize: 11 }} axisLine={false} tickLine={false} tickFormatter={v => `${sym}${v}`} width={45} />
              <Tooltip
                contentStyle={{ background: '#1a1a24', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 10, fontFamily: "'IBM Plex Mono', monospace", fontSize: 12 }}
                formatter={(v: number) => [fmt(v, currency), 'Spent']}
              />
              <Line type="monotone" dataKey="amount" stroke="#a78bfa" strokeWidth={2.5} dot={{ fill: '#a78bfa', r: 4, strokeWidth: 0 }} activeDot={{ r: 6, fill: '#fff' }} />
            </LineChart>
          </ResponsiveContainer>
        </div>

        <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.07)', borderRadius: 16, padding: 24 }}>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 16, fontWeight: 600, marginBottom: 16 }}>By Category</div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
            <ResponsiveContainer width={120} height={120}>
              <PieChart>
                <Pie data={categoryData} dataKey="value" cx="50%" cy="50%" innerRadius={35} outerRadius={55} strokeWidth={0}>
                  {categoryData.map((entry, i) => <Cell key={i} fill={entry.color} />)}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
            <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: 8 }}>
              {categoryData.map(cat => (
                <div key={cat.name} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                    <div style={{ width: 8, height: 8, borderRadius: '50%', background: cat.color }} />
                    <span style={{ fontSize: 11, color: 'rgba(255,255,255,0.5)' }}>{cat.name}</span>
                  </div>
                  <span style={{ fontSize: 11, color: 'rgba(255,255,255,0.7)', fontWeight: 500 }}>{fmt(cat.value, currency)}</span>
                </div>
              ))}
            </div>
          </div>
        </div>

        {(summary?.renewingSoon?.length ?? 0) > 0 && (
          <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.07)', borderRadius: 16, padding: 24, gridColumn: '1 / -1' }}>
            <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 16, fontWeight: 600, marginBottom: 16 }}>Renewing Soon</div>
            <div style={{ display: 'flex', gap: 12 }}>
              {summary?.renewingSoon.map(s => {
                const days = daysUntil(s.nextBillingDate);
                const urgentColor = days <= 2 ? '#f87171' : days <= 5 ? '#fbbf24' : '#34d399';
                const logoLetter = s.name[0]?.toUpperCase() ?? '?';
                const logoColor = CATEGORY_COLORS[s.category] ?? '#6b7280';
                return (
                  <div key={s.id} style={{ flex: 1, background: 'rgba(255,255,255,0.04)', borderRadius: 12, padding: 16, border: `1px solid ${urgentColor}22` }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 10 }}>
                      <div style={{ width: 32, height: 32, borderRadius: 8, background: logoColor, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, fontWeight: 700, color: '#fff' }}>{logoLetter}</div>
                      <div>
                        <div style={{ fontSize: 13, fontWeight: 600, color: '#fff' }}>{s.name}</div>
                        <div style={{ fontSize: 10, color: 'rgba(255,255,255,0.3)' }}>{s.category}</div>
                      </div>
                    </div>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end' }}>
                      <span style={{ color: urgentColor, fontSize: 11, fontWeight: 600 }}>
                        {days === 0 ? 'TODAY' : days === 1 ? 'TOMORROW' : `${days} DAYS`}
                      </span>
                      <span style={{ fontSize: 14, fontWeight: 700, color: '#fff' }}>{fmt(s.amount, s.currencyCode)}</span>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
