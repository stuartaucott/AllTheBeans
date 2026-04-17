import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { BeansApi } from '../api/client';
import type { Bean } from '../api/client';

export default function BeanDetailPage() {
    const { id } = useParams<{ id: string }>();
    const [bean, setBean] = useState<Bean | null>(null);

    useEffect(() => { if (id) BeansApi.get(id).then(setBean); }, [id]);

    if (!bean) return <p>Loading...</p>;

    return (
        <article className="detail">
            <h2>{bean.name} {bean.isBeanOfTheDay && <span className="star">★ Bean of the Day</span>}</h2>
            <p><em>{bean.country} · {bean.colour}</em></p>
            <p>{bean.description}</p>
            <p><strong>£{bean.cost.toFixed(2)}</strong></p>
            <Link to={`/beans/${bean.id}/order`}><button>Buy now</button></Link>
        </article>
    );
} 