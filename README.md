# Recruiter Project - .NET API

## Limitations of This Scope

- Does not represent the full complexity of the production project.
- Does not include all game/platform modules.
- Designed to demonstrate good engineering practices within a focused scope.

## Note for Recruiters

If needed, I can elaborate in an interview on:

- architectural trade-offs;
- domain modeling decisions;
- testing strategy and incremental evolution.


This is a focused extraction from the original Laravel system, rebuilt in **C# .NET 8** with:

- Login module (email or login)
- Credit card payment module using PagSeguro

## Architecture

- `Domain`: core entities/rules
- `Application`: use cases
- `Infrastructure`: SQL Server persistence, JWT, PagSeguro HTTP client
- `Controllers`: HTTP adapters

The project follows separation of concerns aligned with SOLID and tactical DDD.

## Endpoints

- `GET /health`
- `POST /api/login`
- `POST /api/payments/credit-card`

## Environment

Copy `.env.example` to `.env` and update:

- `JWT_SECRET`
- `PAGSEGURO_BASE_URL`
- `PAGSEGURO_TOKEN`
- `SQLSERVER_DSN`

## Run with Docker

```bash
docker compose up --build
```

API: `http://localhost:8080`  
Swagger: `http://localhost:8080/swagger`

## Run locally (without Docker)

```bash
dotnet restore ./src/RecruiterApi/RecruiterApi.csproj
dotnet run --project ./src/RecruiterApi/RecruiterApi.csproj
```

## Tests

```bash
dotnet test ./RecruiterApi.sln
```

## Login payload example

```json
{
  "login": "user_or_email",
  "password": "123456"
}
```

## Credit card payment payload example

```json
{
  "userId": 1,
  "storeCartId": 12,
  "creditCardHolder": "John Doe",
  "cpfForCard": "12345678910",
  "encryptedCard": "CARD_ENCRYPTED_DATA",
  "amount": 1500,
  "description": "Order #12",
  "customerEmail": "player@email.com",
  "notificationUrl": "https://your-domain.com/api/payment-notification",
  "items": [
    {
      "referenceId": "item_1",
      "name": "Item A",
      "quantity": 1,
      "unitAmount": 1500
    }
  ]
}
```
