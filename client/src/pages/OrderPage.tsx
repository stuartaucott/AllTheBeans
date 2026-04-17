import { FormEvent, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useIsAuthenticated, useMsal } from '@azure/msal-react';
import { BeansApi } from '../api/client';
import { loginRequest } from '../auth/msal';

export default function OrderPage() {
    const { id } = useParams<{ id: string }>();
    const isAuthed = useIsAuthenticated();
    const { instance } = useMsal();
    const nav = useNavigate();

    const [form, setForm] = useState({
        customerName: '',
        customerEmail: '',
        shippingAddress: '',
        quantity: 1
    });
    const [error, setError] = useState<string | null>(null);

    if (!isAuthed) {
        return (
            <div>
                <p>Sign in to place an order.</p>
                <button onClick={() => instance.loginRedirect(loginRequest)}>Sign in</button>
            </div>
        );
    }

    async function submit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        try {
            const { id: orderId } = await BeansApi.placeOrder({ ...form, beanId: id! });
            alert(`Order ${orderId} placed!`);
            nav('/');
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Order failed');
        }
    }

    return (
        <form className="order" onSubmit={submit}>
            <h2>Place order</h2>
            <label>Name
                <input required value={form.customerName}
                    onChange={e => setForm(f => ({ ...f, customerName: e.target.value }))} />
            </label>
            <label>Email
                <input type="email" required value={form.customerEmail}
                    onChange={e => setForm(f => ({ ...f, customerEmail: e.target.value }))} />
            </label>
            <label>Shipping address
                <textarea required value={form.shippingAddress}
                    onChange={e => setForm(f => ({ ...f, shippingAddress: e.target.value }))} />
            </label>
            <label>Quantity
                <input type="number" min={1} max={1000} value={form.quantity}
                    onChange={e => setForm(f => ({ ...f, quantity: Number(e.target.value) }))} />
            </label>
            {error && <p style={{ color: 'crimson' }}>{error}</p>}
            <button type="submit">Place order</button>
        </form>
    );
} 