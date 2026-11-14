export const disruptionTypes = [
  { value: 0, label: "Opóźnienie" },
  { value: 1, label: "Awaria pojazdu" },
  { value: 2, label: "Zamknięcie przystanku" },
  { value: 3, label: "Wypadek lub kolizja" },
  { value: 4, label: "Utrudnienia drogowe" },
  { value: 5, label: "Zablokowanie trasy" },
  { value: 6, label: "Awaria infrastruktury" },
  { value: 7, label: "Problem z obsadą" },
  { value: 8, label: "Zdarzenie z pasażerem" },
  { value: 9, label: "Warunki atmosferyczne" },
  { value: 10, label: "Inne" },
] as const;
export type DisruptionType = (typeof disruptionTypes)[number]["value"];
export const disruptionTypeName = (type: number): string =>
  disruptionTypes.find((item) => item.value === type)?.label ?? "Nieznany typ";
export type DisruptionStatus = 0 | 1;
export interface Disruption {
  id: number;
  line: string;
  type: DisruptionType;
  description: string;
  delayMinutes: number | null;
  status: DisruptionStatus;
  createdAt: string;
}
export interface DisruptionInput {
  line: string;
  type: DisruptionType;
  description: string;
  delayMinutes: number | null;
  status: DisruptionStatus;
}