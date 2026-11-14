import { useCallback, useEffect, useState } from "react";
import { analysesApi } from "../api/analyses";
import type {
  ActionStatus,
  DisruptionAnalysis as Analysis,
  DisruptionAction,
  IncidentPriority,
} from "../types/analysis";
import "./DisruptionAnalysis.css";

interface Props {
  disruptionId: number;
  isResolved: boolean;
}

const priorityLabels: Record<IncidentPriority, string> = {
  Low: "Niski",
  Medium: "Średni",
  High: "Wysoki",
  Critical: "Krytyczny",
};

const statusLabels: Record<ActionStatus, string> = {
  Proposed: "Oczekuje na decyzję",
  Accepted: "Zaakceptowano",
  Rejected: "Odrzucono",
  Completed: "Wykonano",
};

function ActionButtons({
  action,
  disabled,
  onChange,
}: {
  action: DisruptionAction;
  disabled: boolean;
  onChange: (status: ActionStatus) => void;
}) {
  if (action.status === "Proposed") {
    return (
      <div className="ai-action-buttons">
        <button
          type="button"
          disabled={disabled}
          onClick={() => onChange("Accepted")}
        >
          Akceptuj
        </button>
        <button
          type="button"
          className="ai-secondary-button"
          disabled={disabled}
          onClick={() => onChange("Rejected")}
        >
          Odrzuć
        </button>
      </div>
    );
  }

  if (action.status === "Accepted") {
    return (
      <div className="ai-action-buttons">
        <button
          type="button"
          disabled={disabled}
          onClick={() => onChange("Completed")}
        >
          Oznacz jako wykonane
        </button>
      </div>
    );
  }

  return null;
}

export function DisruptionAnalysis({ disruptionId, isResolved }: Props) {
  const [analyses, setAnalyses] = useState<Analysis[]>([]);
  const [loading, setLoading] = useState(true);
  const [analyzing, setAnalyzing] = useState(false);
  const [pendingActionId, setPendingActionId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const loadHistory = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const history = await analysesApi.getHistory(disruptionId);
      setAnalyses(history);
    } catch {
      setError("Nie udało się pobrać historii analiz.");
    } finally {
      setLoading(false);
    }
  }, [disruptionId]);

  useEffect(() => {
    void loadHistory();
  }, [loadHistory]);

  async function handleAnalyze() {
    setAnalyzing(true);
    setError(null);

    try {
      const analysis = await analysesApi.analyze(disruptionId);
      setAnalyses((previous) => [analysis, ...previous]);
    } catch {
      setError(
        "Analiza AI nie powiodła się. Sprawdź dostępność usługi i spróbuj ponownie.",
      );
    } finally {
      setAnalyzing(false);
    }
  }

  async function handleActionStatus(actionId: number, status: ActionStatus) {
    setPendingActionId(actionId);
    setError(null);

    try {
      const updated = await analysesApi.updateActionStatus(actionId, status);

      setAnalyses((previous) =>
        previous.map((analysis) => ({
          ...analysis,
          actions: analysis.actions.map((action) =>
            action.id === updated.id ? updated : action,
          ),
        })),
      );
    } catch {
      setError("Nie udało się zapisać decyzji dyspozytora.");
    } finally {
      setPendingActionId(null);
    }
  }

  return (
    <section className="ai-panel">
      <div className="ai-panel-header">
        <div>
          <h3>Analiza AI</h3>
          <p>
            Rekomendacje wspierają decyzje dyspozytora. Nie są wykonywane
            automatycznie.
          </p>
        </div>

        <button
          type="button"
          disabled={analyzing || loading || isResolved}
          onClick={() => void handleAnalyze()}
        >
          {analyzing ? "Analizowanie..." : "Analizuj AI"}
        </button>
      </div>

      {isResolved && (
        <p className="ai-muted">
          Zgłoszenie zostało rozwiązane. Możesz przeglądać wcześniejsze analizy.
        </p>
      )}

      {error && (
        <div className="ai-error" role="alert">
          {error}
          <button type="button" onClick={() => setError(null)}>
            Zamknij
          </button>
        </div>
      )}

      {loading ? (
        <p className="ai-muted">Ładowanie analiz...</p>
      ) : analyses.length === 0 ? (
        <p className="ai-muted">
          Brak analiz. Uruchom AI, aby uzyskać rekomendacje.
        </p>
      ) : (
        <div className="ai-history">
          {analyses.map((analysis, index) => (
            <article className="ai-analysis" key={analysis.id}>
              <div className="ai-analysis-heading">
                <span
                  className={`ai-priority ai-priority-${analysis.priority.toLowerCase()}`}
                >
                  Priorytet: {priorityLabels[analysis.priority]}
                </span>

                <span className="ai-muted">
                  {new Date(analysis.createdAt).toLocaleString("pl-PL")}
                </span>
              </div>

              <p className="ai-summary">{analysis.summary}</p>

              <h4>Rekomendowane działania</h4>

              <div className="ai-actions">
                {analysis.actions.map((action) => (
                  <div className="ai-action" key={action.id}>
                    <div className="ai-action-heading">
                      <strong>{action.title}</strong>
                      <span className="ai-muted">
                        {statusLabels[action.status]}
                      </span>
                    </div>

                    <p>{action.reason}</p>

                    <ActionButtons
                      action={action}
                      disabled={pendingActionId !== null}
                      onChange={(status) =>
                        void handleActionStatus(action.id, status)
                      }
                    />
                  </div>
                ))}
              </div>

              {index < analyses.length - 1 && (
                <div className="ai-history-divider" />
              )}
            </article>
          ))}
        </div>
      )}
    </section>
  );
}
