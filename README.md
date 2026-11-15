# TransitOps AI

TransitOps to prototyp aplikacji do obsługi zakłóceń w komunikacji miejskiej. Przygotowałem go jako zadanie rekrutacyjne. Chciałem połączyć prosty proces zgłaszania zdarzeń z panelem dyspozytora, w którym AI pomaga ocenić sytuację i zaproponować dalsze kroki.

AI nie podejmuje decyzji za dyspozytora — każdą propozycję można zaakceptować lub odrzucić, a zaakceptowane działanie oznaczyć jako wykonane.

## Demo

Nagranie działania aplikacji: [demo.mp4](demo.mp4).

## Co działa

- Zgłaszanie zdarzeń przez kierowcę (`/report`).
- Panel dyspozytora (`/dashboard`) z listą zgłoszeń, edycją i zmianą statusu.
- 11 kategorii zakłóceń, m.in. opóźnienia, awarie, kolizje i blokady tras.
- Zgłoszenia dotyczące jednej, kilku lub nieustalonych linii.
- Analiza zgłoszenia przez Gemini: priorytet, podsumowanie i proponowane działania wraz z uzasadnieniem.
- Akceptowanie, odrzucanie i oznaczanie rekomendacji jako wykonanych.
- Historia analiz zapisywana w bazie danych.

## Technologie

- **Backend:** C#, ASP.NET Core (.NET 10), Entity Framework Core, SQLite
- **Frontend:** React, TypeScript, Vite
- **AI w aplikacji:** Google Gemini API
- **Testy:** xUnit

## Uruchomienie

Potrzebne są .NET SDK 10, Node.js zgodny z Vite 8, npm oraz klucz API Gemini (do funkcji analizy).

### Backend

```bash
cd backend/TransitOps.Api
dotnet restore
dotnet user-secrets set "Gemini:ApiKey" "TWOJ_KLUCZ_API"
dotnet ef database update
dotnet run --launch-profile http
```

API będzie dostępne pod adresem `http://localhost:5230`. Jeśli polecenie `dotnet ef` nie jest dostępne, należy zainstalować narzędzie `dotnet-ef`. Model Gemini można zmienić w `appsettings.json` (`Gemini:Model`).

### Frontend

W drugim terminalu:

```bash
cd frontend
npm install
npm run dev
```

Aplikacja będzie dostępna pod adresem `http://localhost:5173`. Jeśli backend działa pod innym adresem, można ustawić `VITE_API_URL` w pliku `frontend/.env.local`.

Klucz Gemini jest przechowywany po stronie backendu — nie należy umieszczać go w zmiennych `VITE_*` ani commitować do repozytorium.

## Testy

```bash
dotnet test backend/TransitOps.Tests
```

Testy obejmują logikę obsługi rekomendacji. Nie wykonują rzeczywistych zapytań do Gemini.

## Wykorzystanie AI

W projekcie korzystałem z AI na dwa różne sposoby:

- **Google Gemini API** jest częścią działania aplikacji. Na podstawie zgłoszenia przygotowuje podsumowanie, ocenę priorytetu i propozycje działań. Wyniki są zapisywane, ale ich realizacja wymaga decyzji dyspozytora.
- **ChatGPT (OpenAI)** służył mi jako wsparcie podczas tworzenia projektu — przy omawianiu rozwiązań, pisaniu i poprawianiu kodu, diagnozowaniu błędów oraz przygotowaniu testów i dokumentacji. Wygenerowane propozycje weryfikowałem w działającej aplikacji.

## Ograniczenia

To demonstracyjny MVP, a nie system do rzeczywistego prowadzenia ruchu. Nie ma uwierzytelniania ani integracji z danymi GPS, rozkładami jazdy czy zasobami przewoźnika. Rekomendacje AI mogą być niepełne lub błędne, dlatego wymagają oceny człowieka.
