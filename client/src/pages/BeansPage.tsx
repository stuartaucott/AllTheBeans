import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { BeansApi } from '../api/client';
import type { Bean, BeanSearch } from '../api/client';
import BeanOfTheDay from '../components/BeanOfTheDay';

export default function BeansPage() {
  const [filters, setFilters] = useState<BeanSearch>({});
  const [beans, setBeans] = useState<Bean[] | null>(null);

  useEffect(() => {
    let cancelled = false;
    BeansApi.search(filters).then(data => {
      if (!cancelled) setBeans(data);
    });
    return () => { cancelled = true; };
  }, [filters]);

  return (
    <>
      <BeanOfTheDay />

      <div className="filters">
        <input
          placeholder="Search name or description"
          onChange={e => setFilters(f => ({ ...f, query: e.target.value }))}
        />
        <select onChange={e => setFilters(f => ({ ...f, colour: e.target.value || undefined }))}>
          <option value="">All roasts</option>
          <option>Light</option>
          <option>Medium</option>
          <option>Dark</option>
        </select>
        <input
          type="number" step="0.01" placeholder="Max cost"
          onChange={e => setFilters(f => ({ ...f, maxCost: e.target.value ? Number(e.target.value) : undefined }))}
        />
      </div>

      {beans === null ? <p>Loading...</p> : (
        <div className="grid">
          {beans.map(b => (
            <article key={b.id} className="card">
              <Link to={`/beans/${b.id}`}>
                <h3>{b.name}</h3>
                <p>{b.country} · {b.colour}</p>
                <strong>£{b.cost.toFixed(2)}</strong>
              </Link>
            </article>
          ))}
        </div>
      )}
    </>
  );
}