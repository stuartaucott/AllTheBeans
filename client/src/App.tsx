import { Routes, Route, Link } from 'react-router-dom';
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from '@azure/msal-react';
import { loginRequest } from './auth/msal';
import BeansPage from './pages/BeansPage';
import BeanDetailPage from './pages/BeanDetailPage';
import OrderPage from './pages/OrderPage';

export default function App() {
    const { instance, accounts } = useMsal();

    return (
        <div className="layout">
            <header className="top">
                <h1><Link to="/">All The Beans</Link></h1>
                <nav>
                    <AuthenticatedTemplate>
                        <span className="user">{accounts[0]?.username}</span>
                        <button onClick={() => instance.logoutRedirect()}>Sign out</button>
                    </AuthenticatedTemplate>
                    <UnauthenticatedTemplate>
                        <button onClick={() => instance.loginRedirect(loginRequest)}>Sign in with Microsoft</button>
                    </UnauthenticatedTemplate>
                </nav>
            </header>
            <main>
                <Routes>
                    <Route path="/" element={<BeansPage />} />
                    <Route path="/beans/:id" element={<BeanDetailPage />} />
                    <Route path="/beans/:id/order" element={<OrderPage />} />
                </Routes>
            </main>
        </div>
    );
} 