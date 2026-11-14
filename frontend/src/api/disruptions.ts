import type { Disruption, DisruptionInput } from '../types/disruption';
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5230';

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}/api/disruptions${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json; charset=utf-8', ...init?.headers },
  });
  if (!response.ok) {
    let message = `Błąd API (${response.status})`;
    try {
      const body = await response.json();
      if (body.errors) message = Object.values(body.errors).flat().join(' ');
      else if (body.title) message = body.title;
    } catch { /* Response is not JSON */ }
    throw new Error(message);
  }
  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export const disruptionsApi = {
  list: () => request<Disruption[]>(''),
  create: (input: DisruptionInput) => request<Disruption>('', { method: 'POST', body: JSON.stringify(input) }),
  update: (id: number, input: DisruptionInput) => request<void>(`/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  remove: (id: number) => request<void>(`/${id}`, { method: 'DELETE' }),
};