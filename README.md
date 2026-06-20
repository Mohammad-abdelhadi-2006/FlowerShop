# Flower Shop API

A RESTful e-commerce backend for a flower shop, built with **ASP.NET Core Web API** and **Entity Framework Core**. It exposes endpoints for managing products, users, and orders, including order creation with live stock checks and price snapshots.

> This is the **backend** of the Flower Shop project. The Flutter mobile client lives here: [flower_shop_app](https://github.com/Mohammad-abdelhadi-2006/flower_shop_app)

---

## Tech Stack

- **.NET 10** / ASP.NET Core Web API
- **C#**
- **Entity Framework Core** (Code-First, Migrations)
- **SQL Server** (LocalDB / SQL Express)
- **BCrypt.Net** — password hashing
- **Scalar** — interactive API documentation

---

## Architecture

The project follows a clean, layered structure so each part has one job:

```
Request → Controller → Service → DbContext → SQL Server
```

- **Controllers** — thin; they only handle HTTP and return status codes.
- **Services** — hold all the business logic (stock checks, price snapshots, hashing).
- **DTOs** — separate the API shape from the database entities (the client never sees `PasswordHash`, and never sets the `Id`).
- **DbContext / EF Core** — data access and migrations.

---

## Features

- **Products** — full CRUD with input validation (`Name` required, `Price` and `StockQuantity` ranges).
- **Users** — full CRUD, with:
  - Passwords hashed using **BCrypt** (never stored or returned in plain text).
  - Duplicate-email protection.
- **Orders** — create an order from a cart of items, with:
  - **Stock validation** — rejects the order if a product doesn't have enough stock.
  - **Price snapshot** — the unit price is stored at order time, so old orders keep their original price even if the product price changes later.
  - **Order total** calculated from the line items.

---

## API Endpoints

| Method | Endpoint            | Description                  |
|--------|---------------------|------------------------------|
| GET    | `/api/product`      | Get all products             |
| GET    | `/api/product/{id}` | Get a product by id          |
| POST   | `/api/product`      | Create a product             |
| PUT    | `/api/product/{id}` | Update a product             |
| DELETE | `/api/product/{id}` | Delete a product             |
| GET    | `/api/user`         | Get all users                |
| GET    | `/api/user/{id}`    | Get a user by id             |
| POST   | `/api/user`         | Create a user (hashed password) |
| PUT    | `/api/user/{id}`    | Update a user                |
| DELETE | `/api/user/{id}`    | Delete a user                |
| GET    | `/api/order`        | Get all orders               |
| GET    | `/api/order/{id}`   | Get an order by id           |
| POST   | `/api/order`        | Create an order (stock check + price snapshot) |
| DELETE | `/api/order/{id}`   | Delete an order              |

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (Express or LocalDB)

### 1. Clone the repo
```bash
git clone https://github.com/Mohammad-abdelhadi-2006/FlowerShop.git
cd FlowerShop/FlowerShop.API
```

### 2. Add your connection string
The real connection string is **not** committed. Create a file named `appsettings.Development.json` inside `FlowerShop.API/` with:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=FlowerShopDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply the database migrations
```bash
dotnet ef database update
```

### 4. Run the API
```bash
dotnet run
```

### 5. Open the API docs
Navigate to the Scalar URL printed in the console (e.g. `https://localhost:5158/scalar/v1`) and try the endpoints.

---

## Screenshots

**Create a product** — `POST /api/product` returns `201 Created`:

![Create Product](screenshots/Screenshot%202.png)

**Create a user** — `POST /api/user` returns `201 Created`. Note the response never exposes the password or its hash:

![Create User](screenshots/Screenshot%201.png)

**Create an order** — `POST /api/order` returns `201 Created`, with the calculated total and a price snapshot on each item:

![Create Order](screenshots/Screenshot%203.png)

---

## Author

**Mohammad Abdelhadi** — [GitHub](https://github.com/Mohammad-abdelhadi-2006)
