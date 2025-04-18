
# 📘 README – AlphaPortal Web API

## 🧾 Projektbeskrivning
AlphaPortal är ett ASP.NET Core Web API-projekt utvecklat med Entity Framework Core (Code First). Applikationen hanterar projekt, användare, klienter och statusar via RESTful endpoints. Syftet är att tillämpa moderna designmönster och säkerhetslösningar i ett web API.

## 🧱 Teknisk stack
| Del | Teknik |
|-----|--------|
| Backend | ASP.NET Core Web API (.NET 9) |
| Databas | SQL Server LocalDB (.mdf-fil via EF Core) |
| ORM | Entity Framework Core (Code First) |
| Säkerhet | API-nyckel via middleware |
| Autentisering | Identity + UserManager (frivilligt – extra) |
| Dokumentation | Swagger (OpenAPI) |
| Versionshantering | Git + GitHub |

## 🧩 Designmönster
Projektet implementerar:
- ✅ Repository Pattern (`UserRepository`, `ProjectRepository`, etc.)
- ✅ Service Pattern (`UserService`, `ProjectService`, `ClientService`)
- ✅ Lagerindelning: `Data`, `Domain`, `WebApi`

## ✅ Funktioner

| Funktion | Klar |
|----------|------|
| Skapa projekt | ✅ |
| Hämta alla projekt + sortering | ✅ |
| Hämta projekt via ID | ✅ |
| Uppdatera / radera projekt | ✅ |
| Hämta klient & tillhörande projekt | ✅ |
| Skapa och uppdatera användarprofil | ✅ |
| Registrera användare (frivillig funktion) | ✅ |
| Validering via `ModelState` | ✅ |
| Skydd via API-nyckel | ✅ |
| Swagger UI för test | ✅ |

## 🔐 API-säkerhet
API:et är skyddat med en API-nyckel som anges i `appsettings.json`:

```json
"ApiKey": "super-hemlig-nyckel"
```

Skicka nyckeln i headers:

```http
GET /api/Project
x-api-key: super-hemlig-nyckel
```