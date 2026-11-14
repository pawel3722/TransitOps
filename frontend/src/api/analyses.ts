
import type {
  ActionStatus,
  DisruptionAction,
  DisruptionAnalysis,
} from "../types/analysis";

const API_BASE_URL =
  import.meta.env.VITE_API_URL ?? "http://localhost:5230";

async function request<T>(
  path: string,
  options?: RequestInit,
): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
  });

  if (!response.ok) {
    throw new Error(
      `Żądanie nie powiodło się (HTTP ${response.status}).`,
    );
  }

  return (await response.json()) as T;
}

export const analysesApi = {
  getHistory(disruptionId: number) {
    return request<DisruptionAnalysis[]>(
      `/api/disruptions/${disruptionId}/analyses`,
    );
  },

  analyze(disruptionId: number) {
    return request<DisruptionAnalysis>(
      `/api/disruptions/${disruptionId}/analyze`,
      { method: "POST" },
    );
  },

  updateActionStatus(
    actionId: number,
    status: ActionStatus,
  ) {
    return request<DisruptionAction>(
      `/api/actions/${actionId}/status`,
      {
        method: "PATCH",
        body: JSON.stringify({ status }),
      },
    );
  },
};
