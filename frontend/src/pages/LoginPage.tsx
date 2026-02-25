import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../api/auth';
import { useAuthStore } from '../store/authStore';

const schema = z.object({
  email: z.string().email('Invalid email'),
  password: z.string().min(1, 'Password is required'),
});

type FormValues = z.infer<typeof schema>;

const inputStyle: React.CSSProperties = {
  width: '100%',
  background: 'rgba(255,255,255,0.06)',
  border: '1px solid rgba(255,255,255,0.12)',
  borderRadius: 10,
  padding: '12px 16px',
  color: '#fff',
  fontSize: 14,
  outline: 'none',
  fontFamily: "'IBM Plex Mono', monospace",
  boxSizing: 'border-box',
};

export function LoginPage() {
  const navigate = useNavigate();
  const login = useAuthStore(s => s.login);

  const { register, handleSubmit, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(schema),
  });

  const mutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      login(data.user, data.accessToken, data.refreshToken);
      navigate('/');
    },
  });

  const onSubmit = (data: FormValues) => mutation.mutate(data);

  return (
    <div style={{ minHeight: '100vh', background: '#0c0c10', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div style={{ width: 400, maxWidth: '90vw' }}>
        <div style={{ textAlign: 'center', marginBottom: 40 }}>
          <div style={{ width: 48, height: 48, background: 'linear-gradient(135deg, #a78bfa, #60a5fa)', borderRadius: 14, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 24, fontWeight: 700, fontFamily: "'Inter', sans-serif", margin: '0 auto 16px' }}>T</div>
          <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 28, fontWeight: 800 }}>Welcome back</div>
          <div style={{ color: 'rgba(255,255,255,0.4)', fontSize: 13, marginTop: 8 }}>Sign in to your TrackIt account</div>
        </div>

        <div style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.08)', borderRadius: 20, padding: 32 }}>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
              <div>
                <label style={{ color: 'rgba(255,255,255,0.4)', fontSize: 11, letterSpacing: '0.08em', textTransform: 'uppercase', display: 'block', marginBottom: 8 }}>Email</label>
                <input style={inputStyle} type="email" placeholder="you@example.com" {...register('email')} />
                {errors.email && <p style={{ color: '#f87171', fontSize: 11, marginTop: 4 }}>{errors.email.message}</p>}
              </div>
              <div>
                <label style={{ color: 'rgba(255,255,255,0.4)', fontSize: 11, letterSpacing: '0.08em', textTransform: 'uppercase', display: 'block', marginBottom: 8 }}>Password</label>
                <input style={inputStyle} type="password" placeholder="••••••••" {...register('password')} />
                {errors.password && <p style={{ color: '#f87171', fontSize: 11, marginTop: 4 }}>{errors.password.message}</p>}
              </div>
            </div>

            {mutation.isError && (
              <p style={{ color: '#f87171', fontSize: 12, marginTop: 16, textAlign: 'center' }}>
                {(mutation.error as any)?.code === 'ERR_NETWORK'
                  ? 'Cannot connect to server. Please ensure the backend is running.'
                  : ((mutation.error as any)?.response?.data?.message ?? 'Invalid email or password.')}
              </p>
            )}

            <button
              type="submit"
              disabled={mutation.isPending}
              style={{ width: '100%', marginTop: 28, padding: '14px', background: 'linear-gradient(135deg, #a78bfa, #818cf8)', border: 'none', borderRadius: 10, color: '#fff', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace", fontSize: 14, fontWeight: 600, opacity: mutation.isPending ? 0.7 : 1 }}
            >
              {mutation.isPending ? 'Signing in...' : 'Sign in'}
            </button>
          </form>

          <p style={{ textAlign: 'center', marginTop: 24, color: 'rgba(255,255,255,0.4)', fontSize: 13 }}>
            Don't have an account?{' '}
            <Link to="/register" style={{ color: '#a78bfa', textDecoration: 'none' }}>Sign up</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
