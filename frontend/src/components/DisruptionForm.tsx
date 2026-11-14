import { useState, type FormEvent } from "react";
import type {
  Disruption,
  DisruptionInput,
  DisruptionType,
} from "../types/disruption";
import { disruptionTypes } from "../types/disruption";

const initial: DisruptionInput = {
  line: "",
  type: 0,
  description: "",
  delayMinutes: 10,
  status: 0,
};
interface Props {
  existing?: Disruption | null;
  saving: boolean;
  onSubmit: (value: DisruptionInput) => Promise<void>;
  onCancel: () => void;
}
export function DisruptionForm({
  existing,
  saving,
  onSubmit,
  onCancel,
}: Props) {
  const [form, setForm] = useState<DisruptionInput>(
    existing
      ? {
          line: existing.line,
          type: existing.type,
          description: existing.description,
          delayMinutes: existing.delayMinutes,
          status: existing.status,
        }
      : initial,
  );
  const [error, setError] = useState("");
  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (!form.description.trim()) {
      setError("Wypełnij opis zdarzenia.");
      return;
    }
    if (form.type === 0 && (!form.delayMinutes || form.delayMinutes < 1)) {
      setError("Opóźnienie musi być większe od zera.");
      return;
    }
    setError("");
    await onSubmit({
      ...form,
      line: form.line.trim(),
      description: form.description.trim(),
      delayMinutes: form.type === 0 ? form.delayMinutes : null,
    });
  };
  return (
    <form className="form" onSubmit={submit}>
      <div className="form-grid">
        <label>
          Linia lub linie (opcjonalnie)
          <input
            maxLength={20}
            value={form.line}
            onChange={(e) => setForm({ ...form, line: e.target.value })}
            placeholder="np. 12 lub 12, 15; pozostaw puste dla całej sieci"
          />
        </label>
        <label>
          Rodzaj
          <select
            value={form.type}
            onChange={(e) =>
              setForm({
                ...form,
                type: Number(e.target.value) as DisruptionType,
              })
            }
          >
            {disruptionTypes.map((type) => (
              <option key={type.value} value={type.value}>
                {type.label}
              </option>
            ))}
          </select>
        </label>
      </div>
      {form.type === 0 && (
        <label>
          Opóźnienie (min)
          <input
            type="number"
            min="1"
            max="1440"
            value={form.delayMinutes ?? ""}
            onChange={(e) =>
              setForm({
                ...form,
                delayMinutes: e.target.value ? Number(e.target.value) : null,
              })
            }
            required
          />
        </label>
      )}
      <label>
        Opis
        <textarea
          rows={3}
          maxLength={1000}
          value={form.description}
          onChange={(e) => setForm({ ...form, description: e.target.value })}
          placeholder="Co się wydarzyło?"
          required
        />
      </label>
      {existing && (
        <label>
          Status
          <select
            value={form.status}
            onChange={(e) =>
              setForm({ ...form, status: Number(e.target.value) as 0 | 1 })
            }
          >
            <option value={0}>Aktywne</option>
            <option value={1}>Rozwiązane</option>
          </select>
        </label>
      )}
      {error && <p className="error">{error}</p>}
      <div className="form-actions">
        <button type="button" className="secondary" onClick={onCancel}>
          Anuluj
        </button>
        <button disabled={saving} type="submit">
          {saving
            ? "Zapisywanie…"
            : existing
              ? "Zapisz zmiany"
              : "Dodaj zgłoszenie"}
        </button>
      </div>
    </form>
  );
}
