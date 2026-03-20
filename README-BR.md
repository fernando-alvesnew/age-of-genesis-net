# Projeto Recrutador - API .NET

Recorte do sistema original em Laravel para um projeto em **C# .NET 8**, focado em:

- Login (email ou login)
- Pagamento com cartao de credito via PagSeguro

## Arquitetura

- `Domain`: entidades e regras centrais.
- `Application`: casos de uso.
- `Infrastructure`: acesso SQL Server, JWT e cliente PagSeguro.
- `Controllers`: interface HTTP.

Separacao de responsabilidades seguindo boas praticas de SOLID e DDD tatico.

## Endpoints

- `GET /health`
- `POST /api/login`
- `POST /api/payments/credit-card`

## Variaveis de ambiente

Use `.env` com base no `.env.example`.

Campos principais:

- `JWT_SECRET`
- `PAGSEGURO_BASE_URL`
- `PAGSEGURO_TOKEN`
- `SQLSERVER_DSN`

## Rodar com Docker

```bash
docker compose up --build
```

API: `http://localhost:8080`  
Swagger: `http://localhost:8080/swagger`

## Rodar local sem Docker

```bash
dotnet restore ./src/RecruiterApi/RecruiterApi.csproj
dotnet run --project ./src/RecruiterApi/RecruiterApi.csproj
```

## Testes

```bash
dotnet test ./RecruiterApi.sln
```

## Exemplo de login

```json
{
  "login": "usuario_ou_email",
  "password": "123456"
}
```

## Exemplo de pagamento

```json
{
  "userId": 1,
  "storeCartId": 12,
  "creditCardHolder": "Nome Sobrenome",
  "cpfForCard": "12345678910",
  "encryptedCard": "CARD_ENCRYPTED_DATA",
  "amount": 1500,
  "description": "Pedido #12",
  "customerEmail": "player@email.com",
  "notificationUrl": "https://seu-dominio.com/api/payment-notification",
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
