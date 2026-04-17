import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { BeansApi } from '../api/client';
import type { Bean } from '../api/client';

export default function BeanOfTheDay() {
    const [bean, setBean] = useState<Bean | null>(null);
    const [expanded, setExpanded] = useState(false);

    useEffect(() => {
        BeansApi.beanOfTheDay().then(setBean).catch(() => setBean(null));
    }, []);

    if (!bean) return null;

    return (
        <section className="bean-of-day">
            <small>Bean of the Day</small>
            <h2>{bean.name}</h2>
            <p>{expanded ? bean.description : `${bean.description.slice(0, 80)}...`}</p>
            <button onClick={() => setExpanded(e => !e)}>{expanded ? 'Show less' : 'Tell me more'}</button>{' '}
            <Link to={`/beans/${bean.id}`}><button>View</button></Link>
        </section>
    );
} 