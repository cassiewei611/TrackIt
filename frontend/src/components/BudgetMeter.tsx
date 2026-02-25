interface BudgetMeterProps {
  spent: number;
  limit: number;
}

export function BudgetMeter({ spent, limit }: BudgetMeterProps) {
  const pct = Math.min((spent / limit) * 100, 100);
  const color = pct > 85 ? '#f87171' : pct > 65 ? '#fbbf24' : '#34d399';
  return (
    <div style={{ marginTop: 6 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
        <span style={{ color: 'rgba(255,255,255,0.5)', fontSize: 12, fontFamily: "'IBM Plex Mono', monospace" }}>BUDGET USAGE</span>
        <span style={{ color, fontSize: 12, fontFamily: "'IBM Plex Mono', monospace", fontWeight: 600 }}>{pct.toFixed(0)}%</span>
      </div>
      <div style={{ height: 6, background: 'rgba(255,255,255,0.08)', borderRadius: 99, overflow: 'hidden' }}>
        <div style={{
          height: '100%', width: `${pct}%`, background: color,
          borderRadius: 99, transition: 'width 1s ease',
          boxShadow: `0 0 12px ${color}66`,
        }} />
      </div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 6 }}>
        <span style={{ color: 'rgba(255,255,255,0.3)', fontSize: 11, fontFamily: "'IBM Plex Mono', monospace" }}>${spent.toFixed(2)} spent</span>
        <span style={{ color: 'rgba(255,255,255,0.3)', fontSize: 11, fontFamily: "'IBM Plex Mono', monospace" }}>${limit} limit</span>
      </div>
    </div>
  );
}
