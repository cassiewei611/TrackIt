import { useState, useRef, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { CURRENCIES } from '../lib/utils';

interface LayoutProps {
  children: React.ReactNode;
  currency: string;
  onCurrencyChange: (c: string) => void;
}

export function Layout({ children, currency, onCurrencyChange }: LayoutProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuthStore();
  const [showMenu, setShowMenu] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  const tabs = [
    { id: 'dashboard', label: 'Dashboard', path: '/' },
    { id: 'subscriptions', label: 'Subscriptions', path: '/subscriptions' },
    { id: 'budget', label: 'Budget', path: '/budget' },
    { id: 'teams', label: 'Teams', path: '/teams' },
  ];

  const activeTab = tabs.find(t => t.path === location.pathname)?.id ?? 'dashboard';

  // Close menu when clicking outside
  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setShowMenu(false);
      }
    }
    if (showMenu) document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [showMenu]);

  const tabStyle = (tab: string): React.CSSProperties => ({
    padding: '8px 16px',
    borderRadius: 8,
    cursor: 'pointer',
    fontSize: 13,
    fontFamily: "'Inter', sans-serif",
    fontWeight: 500,
    background: activeTab === tab ? 'rgba(167,139,250,0.15)' : 'transparent',
    color: activeTab === tab ? '#a78bfa' : 'rgba(255,255,255,0.4)',
    border: activeTab === tab ? '1px solid rgba(167,139,250,0.3)' : '1px solid transparent',
    transition: 'all 0.15s',
  });

  const initials = user?.fullName
    ? user.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
    : '?';

  const handleSignOut = () => {
    setShowMenu(false);
    logout();
    navigate('/login');
  };

  return (
    <div style={{ minHeight: '100vh', background: '#0c0c10', color: '#fff', fontFamily: "'Inter', sans-serif" }}>
      <div style={{ position: 'fixed', inset: 0, pointerEvents: 'none', overflow: 'hidden', zIndex: 0 }}>
        <div style={{ position: 'absolute', top: -200, left: -200, width: 600, height: 600, background: 'radial-gradient(circle, rgba(167,139,250,0.06) 0%, transparent 70%)', borderRadius: '50%' }} />
        <div style={{ position: 'absolute', bottom: -200, right: -100, width: 500, height: 500, background: 'radial-gradient(circle, rgba(96,165,250,0.05) 0%, transparent 70%)', borderRadius: '50%' }} />
      </div>

      <div style={{ position: 'relative', zIndex: 1, maxWidth: 1200, margin: '0 auto', padding: '0 24px' }}>
        <header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '22px 0 18px', borderBottom: '1px solid rgba(255,255,255,0.06)' }}>
          {/* Logo */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 12, cursor: 'pointer' }} onClick={() => navigate('/')}>
            <div style={{ width: 34, height: 34, background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', borderRadius: 10, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 17, fontWeight: 800, fontFamily: "'Inter', sans-serif" }}>T</div>
            <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 18, fontWeight: 800, letterSpacing: '-0.03em' }}>TrackIt</div>
          </div>

          {/* Nav + controls */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <nav style={{ display: 'flex', gap: 2, marginRight: 8 }}>
              {tabs.map(t => (
                <button key={t.id} style={tabStyle(t.id)} onClick={() => navigate(t.path)}>{t.label}</button>
              ))}
            </nav>

            {/* Currency selector */}
            <select
              value={currency}
              onChange={e => onCurrencyChange(e.target.value)}
              style={{ background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 8, color: '#fff', padding: '7px 10px', fontSize: 12, cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace" }}
            >
              {CURRENCIES.map(c => <option key={c} value={c}>{c}</option>)}
            </select>

            {/* Avatar + dropdown */}
            <div ref={menuRef} style={{ position: 'relative' }}>
              <div
                onClick={() => setShowMenu(v => !v)}
                style={{
                  width: 36, height: 36, borderRadius: '50%',
                  background: showMenu
                    ? 'linear-gradient(135deg, #818cf8, #a78bfa)'
                    : 'linear-gradient(135deg, #a78bfa, #818cf8)',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  fontSize: 13, fontWeight: 700, cursor: 'pointer',
                  boxShadow: showMenu ? '0 0 0 3px rgba(167,139,250,0.3)' : 'none',
                  transition: 'box-shadow 0.15s',
                  userSelect: 'none',
                }}
              >
                {initials}
              </div>

              {showMenu && (
                <div style={{
                  position: 'absolute', top: 'calc(100% + 10px)', right: 0,
                  width: 240,
                  background: '#18181f',
                  border: '1px solid rgba(255,255,255,0.1)',
                  borderRadius: 14,
                  boxShadow: '0 20px 60px rgba(0,0,0,0.6)',
                  overflow: 'hidden',
                  zIndex: 100,
                  animation: 'fadeUp 0.15s ease forwards',
                }}>
                  {/* User info */}
                  <div style={{ padding: '16px 18px', borderBottom: '1px solid rgba(255,255,255,0.07)' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                      <div style={{
                        width: 40, height: 40, borderRadius: '50%',
                        background: 'linear-gradient(135deg, #a78bfa, #818cf8)',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        fontSize: 15, fontWeight: 700, flexShrink: 0,
                      }}>
                        {initials}
                      </div>
                      <div style={{ minWidth: 0 }}>
                        <div style={{ fontSize: 14, fontWeight: 600, color: '#fff', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                          {user?.fullName ?? 'User'}
                        </div>
                        <div style={{ fontSize: 11, color: 'rgba(255,255,255,0.4)', marginTop: 2, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                          {user?.email ?? ''}
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Account details */}
                  <div style={{ padding: '10px 0', borderBottom: '1px solid rgba(255,255,255,0.07)' }}>
                    <MenuItem
                      icon="💰"
                      label="Monthly Budget"
                      value={user?.monthlyBudgetLimit ? `$${user.monthlyBudgetLimit}` : 'Not set'}
                      onClick={() => { setShowMenu(false); navigate('/budget'); }}
                    />
                    <MenuItem
                      icon="🌐"
                      label="Display Currency"
                      value={currency}
                      onClick={() => setShowMenu(false)}
                    />
                    <MenuItem
                      icon="📋"
                      label="My Subscriptions"
                      onClick={() => { setShowMenu(false); navigate('/subscriptions'); }}
                    />
                    <MenuItem
                      icon="👥"
                      label="Teams"
                      onClick={() => { setShowMenu(false); navigate('/teams'); }}
                    />
                  </div>

                  {/* Sign out */}
                  <div style={{ padding: '8px 0' }}>
                    <button
                      onClick={handleSignOut}
                      style={{
                        width: '100%', display: 'flex', alignItems: 'center', gap: 12,
                        padding: '10px 18px', background: 'transparent', border: 'none',
                        cursor: 'pointer', textAlign: 'left', color: '#f87171',
                        fontSize: 13, fontFamily: "'Inter', sans-serif", fontWeight: 500,
                        transition: 'background 0.1s',
                      }}
                      onMouseEnter={e => (e.currentTarget.style.background = 'rgba(248,113,113,0.08)')}
                      onMouseLeave={e => (e.currentTarget.style.background = 'transparent')}
                    >
                      <span style={{ fontSize: 15 }}>→</span>
                      Sign out
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </header>

        {children}
        <div style={{ height: 48 }} />
      </div>
    </div>
  );
}

function MenuItem({ icon, label, value, onClick }: {
  icon: string;
  label: string;
  value?: string;
  onClick: () => void;
}) {
  const [hovered, setHovered] = useState(false);
  return (
    <button
      onClick={onClick}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      style={{
        width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        padding: '9px 18px', background: hovered ? 'rgba(255,255,255,0.05)' : 'transparent',
        border: 'none', cursor: 'pointer', textAlign: 'left',
        fontFamily: "'Inter', sans-serif", transition: 'background 0.1s',
      }}
    >
      <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
        <span style={{ fontSize: 14, width: 20, textAlign: 'center' }}>{icon}</span>
        <span style={{ fontSize: 13, color: 'rgba(255,255,255,0.7)', fontWeight: 500 }}>{label}</span>
      </div>
      {value && (
        <span style={{ fontSize: 11, color: 'rgba(255,255,255,0.35)', fontFamily: "'IBM Plex Mono', monospace" }}>{value}</span>
      )}
    </button>
  );
}
