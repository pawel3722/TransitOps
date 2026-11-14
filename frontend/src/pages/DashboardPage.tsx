import { Fragment, useCallback, useEffect, useState } from "react";
import { disruptionsApi } from "../api/disruptions";
import { DisruptionAnalysis } from "../components/DisruptionAnalysis";
import { DisruptionForm } from "../components/DisruptionForm";
import { disruptionTypeName, type Disruption, type DisruptionInput } from "../types/disruption";
import "../App.css";


export function DashboardPage() {
  const [items, setItems] = useState<Disruption[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [editing, setEditing] = useState<Disruption | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [expandedAnalysisId, setExpandedAnalysisId] = useState<number | null>(
    null,
  );

  const load = useCallback(async () => {
    try {
      setError("");
      setItems(await disruptionsApi.list());
    } catch (e) {
      setError(e instanceof Error ? e.message : "Nie udało się pobrać danych.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const save = async (input: DisruptionInput) => {
    setSaving(true);
    try {
      if (editing) await disruptionsApi.update(editing.id, input);
      else await disruptionsApi.create(input);
      setShowForm(false);
      setEditing(null);
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Nie udało się zapisać.");
    } finally {
      setSaving(false);
    }
  };

  const remove = async (item: Disruption) => {
    if (!window.confirm(`Usunąć zgłoszenie ${item.line ? `linii ${item.line}` : "dotyczące całej sieci"}?`)) return;
    try {
      await disruptionsApi.remove(item.id);
      setExpandedAnalysisId((current) =>
        current === item.id ? null : current,
      );
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Nie udało się usunąć.");
    }
  };

  const toggleStatus = async (item: Disruption) => {
    try {
      await disruptionsApi.update(item.id, {
        line: item.line,
        type: item.type,
        description: item.description,
        delayMinutes: item.delayMinutes,
        status: item.status === 0 ? 1 : 0,
      });
      await load();
    } catch (e) {
      setError(
        e instanceof Error ? e.message : "Nie udało się zmienić statusu.",
      );
    }
  };

  const active = items.filter((item) => item.status === 0).length;

  return (
    <div className="shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">T</span>
          <span>
            TransitOps <b>AI</b>
          </span>
        </div>
        <div className="nav-item selected">▦ &nbsp; Centrum operacyjne</div>
        <div className="sidebar-bottom">
          TRANSPORT OPERATIONS
          <br />
          <small>Demo environment</small>
        </div>
      </aside>

      <main className="main">
        <header className="topbar">
          <span>Panel dyspozytora</span>
          <span className="environment">● Środowisko demonstracyjne</span>
        </header>

        <section className="content">
          <div className="heading">
            <div>
              <div className="eyebrow">OPERATIONS OVERVIEW</div>
              <h1>Centrum zakłóceń</h1>
              <p>
                Monitoruj incydenty i zarządzaj sytuacją w sieci transportowej.
              </p>
            </div>
            <button
              onClick={() => {
                setEditing(null);
                setShowForm(true);
              }}
            >
              + Nowe zgłoszenie
            </button>
          </div>

          <div className="stats">
            <div className="stat">
              <span>Wszystkie zgłoszenia</span>
              <strong>{items.length}</strong>
              <small>Łącznie w systemie</small>
            </div>
            <div className="stat">
              <span>Aktywne zakłócenia</span>
              <strong className="red">{active}</strong>
              <small>Wymagają uwagi</small>
            </div>
            <div className="stat">
              <span>Rozwiązane</span>
              <strong className="green">{items.length - active}</strong>
              <small>Zamknięte incydenty</small>
            </div>
          </div>

          {error && (
            <div className="alert" role="alert">
              {error}{" "}
              <button
                className="link"
                onClick={() => {
                  setError("");
                  void load();
                }}
              >
                Spróbuj ponownie
              </button>
            </div>
          )}

          <div className="panel">
            <div className="panel-head">
              <div>
                <h2>Zgłoszone zakłócenia</h2>
                <p>Przegląd zdarzeń w sieci</p>
              </div>
              <button
                className="secondary"
                onClick={() => {
                  setLoading(true);
                  void load();
                }}
              >
                ↻ Odśwież
              </button>
            </div>

            {loading ? (
              <div className="empty">Ładowanie danych…</div>
            ) : items.length === 0 ? (
              <div className="empty">
                Brak zgłoszeń. Dodaj pierwsze zakłócenie, aby rozpocząć.
              </div>
            ) : (
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>LINIA</th>
                      <th>RODZAJ</th>
                      <th>OPIS</th>
                      <th>STATUS</th>
                      <th>AKCJE</th>
                    </tr>
                  </thead>
                  <tbody>
                    {items.map((item) => (
                      <Fragment key={item.id}>
                        <tr>
                          <td>
                            <span className="line-pill">{item.line || "Cała sieć"}</span>
                          </td>
                          <td>
                            {disruptionTypeName(item.type)}
                            {item.type === 0 && item.delayMinutes != null ? (
                              <small className="muted">
                                {" "}
                                · {item.delayMinutes} min
                              </small>
                            ) : null}
                          </td>
                          <td className="description">{item.description}</td>
                          <td>
                            <span
                              className={`status ${item.status === 0 ? "active" : "resolved"}`}
                            >
                              {item.status === 0 ? "● Aktywne" : "✓ Rozwiązane"}
                            </span>
                          </td>
                          <td>
                            <div className="actions">
                              <button
                                className="link"
                                onClick={() => {
                                  setEditing(item);
                                  setShowForm(true);
                                }}
                              >
                                Edytuj
                              </button>
                              <button
                                className="link"
                                onClick={() => void toggleStatus(item)}
                              >
                                {item.status === 0 ? "Rozwiąż" : "Otwórz"}
                              </button>
                              <button
                                className="link danger"
                                onClick={() => void remove(item)}
                              >
                                Usuń
                              </button>
                              <button
                                type="button"
                                className="link"
                                aria-expanded={expandedAnalysisId === item.id}
                                onClick={() =>
                                  setExpandedAnalysisId((current) =>
                                    current === item.id ? null : item.id,
                                  )
                                }
                              >
                                {expandedAnalysisId === item.id
                                  ? "Ukryj AI"
                                  : "Analiza AI"}
                              </button>
                            </div>
                          </td>
                        </tr>
                        {expandedAnalysisId === item.id && (
                          <tr className="analysis-row">
                            <td colSpan={5}>
                              <DisruptionAnalysis
                                disruptionId={item.id}
                                isResolved={item.status === 1}
                              />
                            </td>
                          </tr>
                        )}
                      </Fragment>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </section>
      </main>

      {showForm && (
        <div
          className="overlay"
          onMouseDown={(e) => {
            if (e.target === e.currentTarget) setShowForm(false);
          }}
        >
          <div
            className="modal"
            role="dialog"
            aria-modal="true"
            aria-label={editing ? "Edytuj zakłócenie" : "Nowe zakłócenie"}
          >
            <div className="modal-head">
              <h2>{editing ? "Edytuj zakłócenie" : "Nowe zgłoszenie"}</h2>
              <button
                className="icon-button"
                onClick={() => setShowForm(false)}
                aria-label="Zamknij"
              >
                ✕
              </button>
            </div>
            <DisruptionForm
              key={editing?.id ?? "new"}
              existing={editing}
              saving={saving}
              onSubmit={save}
              onCancel={() => setShowForm(false)}
            />
          </div>
        </div>
      )}
    </div>
  );
}
