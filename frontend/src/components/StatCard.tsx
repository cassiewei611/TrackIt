interface StatCardProps {
  label: string;
  value: string | number;
  sub?: string;
  accent: string;
}

export function StatCard({ label, value, sub, accent }: StatCardProps) {
  return (
    <div style={{
      background: 'rgba(255,255,255,0.04)',
      border: '1px solid rgba(255,255,255,0.08)',
      borderRadius: 16,
      padding: '24px 28px',
      flex: 1,
      minWidth: 0,
      position: 'relative',
      overflow: 'hidden',
    }}>
      <div style={{
        position: 'absolute', top: 0, left: 0, right: 0, height: 2,
        background: accent,
      }} />
      <div style={{ color: 'rgba(255,255,255,0.45)', fontSize: 11, letterSpacing: '0.08em', textTransform: 'uppercase', fontFamily: "'IBM Plex Mono', monospace", marginBottom: 10 }}>{label}</div>
      <div style={{ fontSize: 32, fontWeight: 700, color: '#fff', fontFamily: "'Inter', sans-serif", lineHeight: 1 }}>{value}</div>
      {sub && <div style={{ color: 'rgba(255,255,255,0.4)', fontSize: 12, marginTop: 8, fontFamily: "'IBM Plex Mono', monospace" }}>{sub}</div>}
    </div>
  );
}
