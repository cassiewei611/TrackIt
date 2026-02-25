import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '../api/client';
import type { Team } from '../types';

export function TeamsPage() {
  const [showCreate, setShowCreate] = useState(false);
  const [teamName, setTeamName] = useState('');
  const queryClient = useQueryClient();

  const { data: teams = [], isLoading } = useQuery({
    queryKey: ['teams'],
    queryFn: () => apiClient.get<Team[]>('/teams').then(r => r.data),
  });

  const createMutation = useMutation({
    mutationFn: (name: string) => apiClient.post<Team>('/teams', { name }).then(r => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teams'] });
      setShowCreate(false);
      setTeamName('');
    },
  });

  const cardStyle: React.CSSProperties = {
    background: 'rgba(255,255,255,0.03)',
    border: '1px solid rgba(255,255,255,0.07)',
    borderRadius: 16,
    padding: 24,
  };

  return (
    <div style={{ padding: '32px 0' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', marginBottom: 32 }}>
        <div>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 24, fontWeight: 700 }}>Teams</div>
          <div style={{ color: 'rgba(255,255,255,0.35)', fontSize: 12, marginTop: 4 }}>Collaborate on shared subscriptions</div>
        </div>
        <button
          onClick={() => setShowCreate(true)}
          style={{ padding: '10px 20px', background: 'linear-gradient(135deg, #a78bfa, #818cf8)', border: 'none', borderRadius: 10, color: '#fff', cursor: 'pointer', fontSize: 13, fontFamily: "'IBM Plex Mono', monospace", fontWeight: 600 }}
        >
          + New Team
        </button>
      </div>

      {showCreate && (
        <div style={{ ...cardStyle, marginBottom: 20, border: '1px solid rgba(167,139,250,0.3)' }}>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 16, fontWeight: 600, marginBottom: 16 }}>Create Team</div>
          <div style={{ display: 'flex', gap: 10 }}>
            <input
              value={teamName}
              onChange={e => setTeamName(e.target.value)}
              placeholder="Team name..."
              style={{ flex: 1, background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.12)', borderRadius: 10, padding: '10px 14px', color: '#fff', fontSize: 14, outline: 'none', fontFamily: "'IBM Plex Mono', monospace" }}
            />
            <button
              onClick={() => teamName.trim() && createMutation.mutate(teamName.trim())}
              disabled={createMutation.isPending}
              style={{ padding: '10px 20px', background: 'linear-gradient(135deg, #a78bfa, #818cf8)', border: 'none', borderRadius: 10, color: '#fff', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace", fontSize: 13 }}
            >
              Create
            </button>
            <button
              onClick={() => setShowCreate(false)}
              style={{ padding: '10px 20px', background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 10, color: 'rgba(255,255,255,0.5)', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace", fontSize: 13 }}
            >
              Cancel
            </button>
          </div>
        </div>
      )}

      {isLoading ? (
        <div style={{ textAlign: 'center', color: 'rgba(255,255,255,0.3)', padding: '40px 0' }}>Loading teams...</div>
      ) : teams.length === 0 ? (
        <div style={{ ...cardStyle, textAlign: 'center', padding: '60px 24px' }}>
          <div style={{ fontSize: 48, marginBottom: 16 }}>👥</div>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 18, fontWeight: 600, marginBottom: 8 }}>No teams yet</div>
          <div style={{ color: 'rgba(255,255,255,0.4)', fontSize: 13 }}>Create a team to share subscriptions with your colleagues</div>
        </div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: 16 }}>
          {teams.map(team => (
            <div key={team.id} style={cardStyle}>
              <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 18, fontWeight: 700, marginBottom: 8 }}>{team.name}</div>
              {team.description && <div style={{ color: 'rgba(255,255,255,0.4)', fontSize: 13, marginBottom: 12 }}>{team.description}</div>}
              <div style={{ display: 'flex', alignItems: 'center', gap: 8, color: 'rgba(255,255,255,0.4)', fontSize: 12 }}>
                <span style={{ background: 'rgba(167,139,250,0.1)', border: '1px solid rgba(167,139,250,0.2)', borderRadius: 99, padding: '2px 10px', color: '#a78bfa' }}>
                  {team.memberCount} {team.memberCount === 1 ? 'member' : 'members'}
                </span>
                <span>·</span>
                <span>Created {new Date(team.createdAt).toLocaleDateString()}</span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
