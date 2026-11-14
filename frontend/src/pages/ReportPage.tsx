import { useState } from "react";
import { Link } from "react-router-dom";
import { disruptionsApi } from "../api/disruptions";
import { DisruptionForm } from "../components/DisruptionForm";
import type { DisruptionInput } from "../types/disruption";

export function ReportPage() {
  const [saving, setSaving] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState("");

  const submit = async (input: DisruptionInput) => {
    setSaving(true);
    setError("");

    try {
      await disruptionsApi.create({
        ...input,
        status: 0,
      });

      setSuccess(true);
    } catch (e) {
      setError(
        e instanceof Error ? e.message : "Nie udało się wysłać zgłoszenia.",
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="report-page">
      <div className="report-card">
        <div className="eyebrow">TRANSITOPS AI</div>
        <h1>Zgłoś zakłócenie</h1>
        <p>
          Formularz dla kierowców i pracowników terenowych. Zgłoszenie zostanie
          przekazane do dyspozytora.
        </p>

        {success ? (
          <div role="status">
            <h2>Zgłoszenie przyjęte</h2>
            <p>Incydent został zapisany w systemie.</p>
            <button onClick={() => setSuccess(false)}>
              Dodaj kolejne zgłoszenie
            </button>
          </div>
        ) : (
          <>
            {error && (
              <div className="alert" role="alert">
                {error}
              </div>
            )}
            <DisruptionForm
              existing={null}
              saving={saving}
              onSubmit={submit}
              onCancel={() => setSuccess(false)}
            />
          </>
        )}

        <p className="report-footer">
          <Link to="/dashboard">Przejdź do panelu dyspozytora</Link>
        </p>
      </div>
    </div>
  );
}
