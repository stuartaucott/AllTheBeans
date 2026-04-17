import { msalInstance, loginRequest } from '../auth/msal';

const base = import.meta.env.VITE_API_BASE;

async function authHeader(): Promise<Record<string, string>> {
    const account = msalInstance.getAllAccounts()[0];
    if (!account) return {};
    try {
        const result = await msalInstance.acquireTokenSilent({ ...loginRequest, account });
        return { Authorization: `Bearer ${result.accessToken}` };
    } catch {
        return {};
    }
}

export interface Bean {
    id: string;
    name: string;
    description: string;
    country: string;
    colour: string;
    cost: number;
    imageUrl: string;
    isBeanOfTheDay: boolean;
}

export interface BeanSearch {
    query?: string;
    country?: string;
    colour?: string;
    minCost?: number;
    maxCost?: number;
    page?: number;
    pageSize?: number;
}

function qs(o: Record<string, unknown>): string {
    const p = new URLSearchParams();
    for (const [k, v] of Object.entries(o))
        if (v !== undefined && v !== '') p.set(k, String(v));
    const s = p.toString();
    return s ? `?${s}` : '';
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
    const headers = {
        'Content-Type': 'application/json',
        ...(await authHeader()),
        ...(init.headers ?? {})
    };
    const res = await fetch(`${base}${path}`, { ...init, headers });
    if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
    return res.status === 204 ? (undefined as T) : (res.json() as Promise<T>);
}

export const BeansApi = {
    search: (s: BeanSearch) =>
        request<Bean[]>(`/api/beans${qs(s as Record<string, unknown>)}`),
    get: (id: string) =>
        request<Bean>(`/api/beans/${id}`),
    beanOfTheDay: () =>
        request<Bean>('/api/bean-of-the-day'),
    placeOrder: (body: {
        customerName: string;
        customerEmail: string;
        shippingAddress: string;
        beanId: string;
        quantity: number;
    }) =>
        request<{ id: string }>('/api/orders', { method: 'POST', body: JSON.stringify(body) })
};