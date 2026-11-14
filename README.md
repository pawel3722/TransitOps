# TransitOps AI

Aplikacja demonstracyjna wspierająca obsługę zakłóceń w komunikacji miejskiej. Kierowca zgłasza zdarzenie, a dyspozytor przegląda zgłoszenia, uruchamia analizę Gemini i podejmuje decyzje dotyczące proponowanych działań.

> **Charakter projektu:** prototyp rekrutacyjny (MVP), nie system przeznaczony do rzeczywistego prowadzenia ruchu. Rekomendacje AI są propozycjami — nie są wykonywane automatycznie.

## Funkcjonalności

- Formularz zgłoszenia zdarzenia dla kierowcy (`/report`).
- Panel dyspozytora (`/dashboard`): lista zgłoszeń, statystyki, edycja, usuwanie i zmiana statusu.
- 11 kategorii zdarzeń, m.in. opóźnienia, awarie, kolizje, blokady tras i zdarzenia z pasażerami.
- Pole linii umożliwia podanie kilku oznaczeń (np. `12, 15`) lub pozostawienie go pustego dla zdarzeń sieciowych / o nieustalonych liniach.
- Analiza zdarzenia przez Google Gemini: priorytet, podsumowanie i propozycje działań z uzasadnieniem.
- Decyzje dyspozytora: akceptacja lub odrzucenie propozycji; zaakceptowane działanie można oznaczyć jako wykonane.
- Historia analiz przechowywana w SQLite.

## Technologie

| Warstwa | Technologia |
| --- | --- |
| Frontend | React 19, TypeScript, Vite 8, React Router |
| Backend | ASP.NET Core (.NET 10), Web API |
| Dane | SQLite, Entity Framework Core 10 |
| AI | Google Gemini API (REST, odpowiedź strukturalna JSON) |
| Testy | xUnit (projekt `TransitOps.Tests`) |

## Wymagania

- .NET SDK 10
- Node.js w wersji zgodnej z Vite 8 (Node.js 20.19+ lub 22.12+)
- npm
- Klucz Google Gemini API do korzystania z analiz AI

## Uruchomienie lokalne

### 1. Backend

W terminalu, z katalogu głównego repozytorium:

```powershell
cd backend/TransitOps.Api
dotnet restore
```

Skonfiguruj klucz Gemini **lokalnie**, za pomocą .NET User Secrets:

```powershell
dotnet user-secrets set "Gemini:ApiKey" "TWOJ_KLUCZ_API"
```

Model Gemini jest konfigurowany w `appsettings.json` (`Gemini:Model`). W razie potrzeby wybierz model dostępny dla Twojego klucza API.

Zastosuj istniejące migracje bazy danych:

```powershell
dotnet ef database update
```

Jeżeli nie masz narzędzia `dotnet ef`, zainstaluj je poleceniem `dotnet tool install --global dotnet-ef` (lub zaktualizuj istniejącą instalację).

Uruchom API:

```powershell
dotnet run --launch-profile http
```

Backend nasłuchuje pod adresem `http://localhost:5230`. Domyślna baza SQLite (`transitops.db`) powstaje w katalogu roboczym backendu po zastosowaniu migracji.

### 2. Frontend

W **drugim terminalu**, z katalogu głównego repozytorium:

```powershell
cd frontend
npm install
npm run dev
```

Otwórz `http://localhost:5173`.

Adres API można zmienić, tworząc plik `frontend/.env.local`:

```dotenv
VITE_API_URL=http://localhost:5230
```

Nie umieszczaj klucza Gemini w zmiennych `VITE_*` — są dostępne w kodzie przeglądarki. Zapytania do Gemini wykonuje backend.

## Scenariusz demonstracyjny

1. Otwórz `/report` i zgłoś np. awarię autobusu linii 12.
2. Przejdź do `/dashboard` i odśwież listę zgłoszeń.
3. Rozwiń **Analiza AI** przy zgłoszeniu i uruchom analizę.
4. Zapoznaj się z priorytetem i rekomendacjami Gemini.
5. Zaakceptuj lub odrzuć rekomendację. Zaakceptowaną można oznaczyć jako wykonaną.
6. Uruchom ponowną analizę, aby zobaczyć historię analiz tego zgłoszenia.

## API — najważniejsze endpointy

| Metoda | Endpoint | Funkcja |
| --- | --- | --- |
| `GET` | `/api/disruptions` | Lista zgłoszeń |
| `GET` | `/api/disruptions/{id}` | Szczegóły zgłoszenia |
| `POST` | `/api/disruptions` | Utworzenie zgłoszenia |
| `PUT` | `/api/disruptions/{id}` | Aktualizacja zgłoszenia |
| `DELETE` | `/api/disruptions/{id}` | Usunięcie zgłoszenia |
| `POST` | `/api/disruptions/{id}/analyze` | Nowa analiza AI |
| `GET` | `/api/disruptions/{id}/analyses` | Historia analiz |
| `PATCH` | `/api/actions/{id}/status` | Zmiana statusu rekomendacji |

Przykładowe żądanie zmiany statusu rekomendacji:

```http
PATCH /api/actions/1/status
Content-Type: application/json

{"status":"Accepted"}
```

Dopuszczalne przejścia: `Proposed → Accepted / Rejected` oraz `Accepted → Completed`.

## Testy i budowanie

Z katalogu głównego repozytorium:

```powershell
dotnet test backend/TransitOps.Tests
```

```powershell
dotnet build backend/TransitOps.Api
cd frontend
npm run build
npm run lint
```

Testy logiki rekomendacji wykorzystują bazę testową, a nie prawdziwe wywołania Gemini.

## Architektura

- `frontend/src/pages` — widoki kierowcy i dyspozytora.
- `frontend/src/components` — formularz i panel analizy AI.
- `frontend/src/api` — komunikacja z backendem.
- `backend/TransitOps.Api/Controllers` — endpointy HTTP.
- `backend/TransitOps.Api/Services` — logika aplikacji i integracja z Gemini.
- `backend/TransitOps.Api/Models`, `DTOs`, `Data` — model domenowy, kontrakty API i EF Core.
- `backend/TransitOps.Api/Migrations` — migracje schematu SQLite.

## Ograniczenia prototypu

- Brak logowania i podziału uprawnień — role kierowcy i dyspozytora są rozdzielone widokami, a nie autoryzacją API.
- Oznaczenia wielu linii są przechowywane jako tekst, nie jako osobne rekordy powiązane relacją.
- Dane o lokalizacji pojazdów, rozkładach i zasobach dyspozytorskich nie są pobierane z rzeczywistych systemów.
- Odpowiedzi AI mogą zawierać błędy i wymagają weryfikacji przez człowieka.
- Lokalna konfiguracja CORS zezwala na frontend pod `http://localhost:5173`; wdrożenie wymaga dostosowania konfiguracji.

## Bezpieczeństwo konfiguracji

Nie zapisuj w repozytorium kluczy API, plików `.env.local`, baz `*.db` ani katalogów `bin`, `obj`, `node_modules` i `.vs`. Klucz Gemini jest odczytywany przez backend z konfiguracji `.NET`, a lokalnie powinien być ustawiony przez User Secrets.
