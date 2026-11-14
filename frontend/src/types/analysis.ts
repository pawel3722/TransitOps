
export type IncidentPriority = "Low" | "Medium" | "High" | "Critical";

export type ActionStatus =
  | "Proposed"
  | "Accepted"
  | "Rejected"
  | "Completed";

export interface DisruptionAction {
  id: number;
  title: string;
  reason: string;
  status: ActionStatus;
}

export interface DisruptionAnalysis {
  id: number;
  disruptionId: number;
  priority: IncidentPriority;
  summary: string;
  createdAt: string;
  actions: DisruptionAction[];
}
