interface BudgetMeterProps {
  spent: number;
  limit: number;
  currency: string;
}

function fmt(n: number, currency: string) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency, maximumFractionDigits: 2 }).format(n);
}

export function BudgetMeter({ spent, limit, currency }: BudgetMeterProps) {
  const rawPct = (spent / limit) * 100;
  const isOver = rawPct > 100;
  const barPct = Math.min(rawPct, 100);
  const color = isOver ? '#f87171' : rawPct > 80 ? '#fbbf24' : '#34d399';

  return (
    <div style={{ marginTop: 6 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
        <span style={{ color: 'rgba(255,255,255,0.5)', fontSize: 12, fontFamily: "'IBM Plex Mono', monospace" }}>BUDGET USAGE</span>
        <span style={{ color, fontSize: 12, fontFamily: "'IBM Plex Mono', monospace", fontWeight: 600 }}>
          {isOver ? `+${(rawPct - 100).toFixed(0)}% OVER` : `${rawPct.toFixed(0)}%`}
        </span>
      </div>
      <div style={{ height: 6, background: 'rgba(255,255,255,0.08)', borderRadius: 99, overflow: 'hidden' }}>
        <div style={{
          height: '100%', width: `${barPct}%`, background: color,
          borderRadius: 99, transition: 'width 1s ease',
          boxShadow: `0 0 12px ${color}66`,
          animation: isOver ? 'pulse 1.5s ease-in-out infinite' : undefined,
        }} />
      </div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 6 }}>
        <span style={{ color: isOver ? '#f87171' : 'rgba(255,255,255,0.3)', fontSize: 11, fontFamily: "'IBM Plex Mono', monospace" }}>
          {fmt(spent, currency)} spent
        </span>
        <span style={{ color: 'rgba(255,255,255,0.3)', fontSize: 11, fontFamily: "'IBM Plex Mono', monospace" }}>
          {isOver
            ? <span style={{ color: '#f87171' }}>{fmt(spent - limit, currency)} over</span>
            : <>{fmt(limit - spent, currency)} left · {fmt(limit, currency)} limit</>
          }
        </span>
      </div>
    </div>
  );
}
