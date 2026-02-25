import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { subscriptionsApi } from '../api/subscriptions';
import { CURRENCIES, CATEGORIES, BILLING_CYCLES } from '../lib/utils';
import type { BillingCycle, SubscriptionCategory } from '../types';

const schema = z.object({
  name: z.string().min(1, 'Name is required').max(100),
  amount: z.coerce.number().positive('Amount must be positive'),
  currencyCode: z.string().length(3),
  billingCycle: z.enum(['Weekly', 'Monthly', 'Quarterly', 'Yearly']),
  nextBillingDate: z.string().min(1, 'Billing date required'),
  category: z.enum(['Streaming', 'Music', 'Gaming', 'SaaS', 'Cloud', 'Security', 'Other']),
  notes: z.string().optional(),
});

type FormValues = z.infer<typeof schema>;

interface AddSubscriptionModalProps {
  onClose: () => void;
}

const inputStyle: React.CSSProperties = {
  width: '100%',
  background: 'rgba(255,255,255,0.06)',
  border: '1px solid rgba(255,255,255,0.12)',
  borderRadius: 10,
  padding: '10px 14px',
  color: '#fff',
  fontSize: 14,
  outline: 'none',
  fontFamily: "'IBM Plex Mono', monospace",
  boxSizing: 'border-box',
};

const labelStyle: React.CSSProperties = {
  color: 'rgba(255,255,255,0.4)',
  fontSize: 11,
  letterSpacing: '0.08em',
  textTransform: 'uppercase',
  fontFamily: "'IBM Plex Mono', monospace",
  marginBottom: 6,
  display: 'block',
};

export function AddSubscriptionModal({ onClose }: AddSubscriptionModalProps) {
  const queryClient = useQueryClient();
  // Use local date (not UTC) to avoid timezone issues
  const d = new Date();
  const today = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;

  const { register, handleSubmit, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      currencyCode: 'USD',
      billingCycle: 'Monthly',
      category: 'SaaS',
      nextBillingDate: today,
    },
  });

  const mutation = useMutation({
    mutationFn: subscriptionsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['subscriptions'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
      onClose();
    },
  });

  const onSubmit = (data: FormValues) => {
    mutation.mutate({
      ...data,
      billingCycle: data.billingCycle as BillingCycle,
      category: data.category as SubscriptionCategory,
    });
  };

  return (
    <div
      style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 999 }}
      onClick={onClose}
    >
      <div
        style={{ background: '#141418', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 20, padding: 32, width: 460, maxWidth: '90vw' }}
        onClick={e => e.stopPropagation()}
      >
        <div style={{ fontFamily: "'Inter', sans-serif", fontSize: 20, fontWeight: 700, color: '#fff', marginBottom: 24 }}>
          Add Subscription
        </div>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
            <div>
              <label style={labelStyle}>Service Name</label>
              <input style={inputStyle} placeholder="e.g. Netflix" {...register('name')} />
              {errors.name && <p style={{ color: '#f87171', fontSize: 11, marginTop: 4 }}>{errors.name.message}</p>}
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
              <div>
                <label style={labelStyle}>Amount</label>
                <input style={inputStyle} type="number" step="0.01" placeholder="9.99" {...register('amount')} />
                {errors.amount && <p style={{ color: '#f87171', fontSize: 11, marginTop: 4 }}>{errors.amount.message}</p>}
              </div>
              <div>
                <label style={labelStyle}>Currency</label>
                <select style={{ ...inputStyle, appearance: 'none' }} {...register('currencyCode')}>
                  {CURRENCIES.map(c => <option key={c} value={c}>{c}</option>)}
                </select>
              </div>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
              <div>
                <label style={labelStyle}>Billing Cycle</label>
                <select style={{ ...inputStyle, appearance: 'none' }} {...register('billingCycle')}>
                  {BILLING_CYCLES.map(c => <option key={c}>{c}</option>)}
                </select>
              </div>
              <div>
                <label style={labelStyle}>Category</label>
                <select style={{ ...inputStyle, appearance: 'none' }} {...register('category')}>
                  {CATEGORIES.map(c => <option key={c}>{c}</option>)}
                </select>
              </div>
            </div>

            <div>
              <label style={labelStyle}>Next Billing Date</label>
              <input style={inputStyle} type="date" {...register('nextBillingDate')} />
            </div>

            <div>
              <label style={labelStyle}>Notes (optional)</label>
              <input style={inputStyle} placeholder="Optional notes..." {...register('notes')} />
            </div>
          </div>

          {mutation.isError && (
            <div style={{ marginTop: 14, padding: '10px 14px', background: 'rgba(248,113,113,0.08)', border: '1px solid rgba(248,113,113,0.2)', borderRadius: 8 }}>
              {(() => {
                const err = mutation.error as any;
                if (err?.code === 'ERR_NETWORK') return <p style={{ color: '#f87171', fontSize: 12 }}>Cannot connect to server.</p>;
                const apiErrors: string[] = err?.response?.data?.errors ?? [];
                const msg: string = err?.response?.data?.message ?? 'Failed to add subscription.';
                return apiErrors.length > 0
                  ? <ul style={{ color: '#f87171', fontSize: 12, paddingLeft: 16, margin: 0 }}>{apiErrors.map((e, i) => <li key={i}>{e}</li>)}</ul>
                  : <p style={{ color: '#f87171', fontSize: 12, margin: 0 }}>{msg}</p>;
              })()}
            </div>
          )}

          <div style={{ display: 'flex', gap: 12, marginTop: 28 }}>
            <button
              type="button"
              onClick={onClose}
              style={{ flex: 1, padding: '12px', background: 'rgba(255,255,255,0.06)', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 10, color: 'rgba(255,255,255,0.6)', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace", fontSize: 13 }}
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={mutation.isPending}
              style={{ flex: 1, padding: '12px', background: 'linear-gradient(135deg, #a78bfa, #818cf8)', border: 'none', borderRadius: 10, color: '#fff', cursor: 'pointer', fontFamily: "'IBM Plex Mono', monospace", fontSize: 13, fontWeight: 600, opacity: mutation.isPending ? 0.7 : 1 }}
            >
              {mutation.isPending ? 'Adding...' : 'Add Subscription'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
