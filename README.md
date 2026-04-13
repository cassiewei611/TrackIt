# TrackIT

A personal subscription tracking app. Add your subscriptions, see your monthly and yearly spend, track renewals, and set a budget limit — all in one place.

## Features

- **Dashboard** — monthly total, yearly projection, budget usage, spending by category, and subscriptions renewing within 7 days
- **Subscription management** — add, edit, and delete subscriptions with billing cycle, category, and notes
- **Split cost** — divide a shared subscription across multiple people
- **Currency conversion** — view all totals in your preferred currency (USD, EUR, GBP, DKK, and more)
- **Budget limit** — set a monthly cap and see how much of it you have used
- **Spend timeline** — line chart showing monthly spend over the past 6 months
- **JWT authentication** — register and login with secure token-based auth

## Tech Stack

**Backend** — ASP.NET Core 8 Web API
- Clean Architecture (Domain / Application / Infrastructure / API)
- CQRS with MediatR
- Entity Framework Core + SQL Server
- FluentValidation + pipeline behaviors
- Serilog file logging
- Live exchange rates via [Frankfurter API](https://www.frankfurter.app/)

**Frontend** — React + TypeScript (Vite)
- TanStack Query for data fetching and caching
- Recharts for line and pie charts
- Zustand for auth state
- React Router

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | No | Create account |
| POST | `/api/auth/login` | No | Login and receive JWT |
| GET | `/api/subscriptions` | Yes | List subscriptions |
| POST | `/api/subscriptions` | Yes | Add a subscription |
| PUT | `/api/subscriptions/{id}` | Yes | Update a subscription |
| DELETE | `/api/subscriptions/{id}` | Yes | Delete a subscription |
| GET | `/api/dashboard/summary` | Yes | Dashboard summary |
| GET | `/api/dashboard/timeline` | Yes | Monthly spend timeline |

## Running Locally

**Prerequisites:** .NET 8 SDK, Node 18+, SQL Server (or Docker)

**Backend**
```bash
cd backend
dotnet run --project src/TrackIt.API
```
Swagger UI available at `http://localhost:5000/swagger`

**Frontend**
```bash
cd frontend
npm install
npm run dev
```

## Running with Docker

Starts SQL Server, backend, and frontend together:

```bash
docker-compose up --build
```

Frontend will be at `http://localhost:3000`, API at `http://localhost:5000`.
