# 🏨 Hotels Management System (HMS)

A fully-featured **Hotel-Centric Management Platform** built with **ASP.NET Core Web API**. The system handles hotel operations including room management, guest reservations, manager administration, JWT-based authentication, and role-based authorization.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Data Models](#data-models)
- [Entity Relationships](#entity-relationships)
- [Business Rules](#business-rules)
- [API Endpoints](#api-endpoints)
- [Authentication & Roles](#authentication--roles)
- [Architecture](#architecture)
- [Getting Started](#getting-started)

---

## Overview

HMS is a hotel-centric REST API that provides:

- **Hotel management** — create, update, filter, and delete hotels
- **Room management** — add rooms to hotels, search by price and availability
- **Manager management** — register managers, assign to hotels, JWT auth
- **Guest management** — register guests with unique credentials
- **Reservation management** — book one or multiple rooms, update dates, cancel bookings

---

## Tech Stack

| Layer            | Technology                          |
|------------------|--------------------------------------|
| Framework        | ASP.NET Core Web API                 |
| ORM              | Entity Framework Core (Code First)   |
| Database         | SQL Server                           |
| Authentication   | JWT (JSON Web Tokens)                |
| Authorization    | Role-Based (`Admin`, `Manager`, `Guest`) |
| Mapping          | AutoMapper / Mapster                 |
| DI Container     | Built-in ASP.NET Core DI             |
| Error Handling   | Global Exception Handling Middleware |

---

## Data Models

### 🏨 Hotel

| Field     | Type    | Notes        |
|-----------|---------|--------------|
| `Id`      | int     | Primary Key  |
| `Name`    | string  |              |
| `Rating`  | int     | Range: 1–5   |
| `Country` | string  |              |
| `City`    | string  |              |
| `Address` | string  |              |

---

### 👔 Manager

| Field            | Type   | Notes                        |
|------------------|--------|------------------------------|
| `Id`             | int    | PK, Identity (auto-increment)|
| `FirstName`      | string |                              |
| `LastName`       | string |                              |
| `PersonalNumber` | string | Unique                       |
| `Email`          | string | Unique                       |
| `PhoneNumber`    | string |                              |
| `HotelId`        | int    | FK → Hotel                   |

---

### 🛏️ Room

| Field     | Type    | Notes                             |
|-----------|---------|-----------------------------------|
| `Id`      | int     | Primary Key                       |
| `Name`    | string  |                                   |
| `Price`   | decimal | Must be > 0                       |
| `HotelId` | int     | FK → Hotel                        |

> ⚠️ Rooms do **not** have an `IsAvailable` field. Availability is determined dynamically from active reservations.

---

### 👤 Guest

| Field            | Type   | Notes   |
|------------------|--------|---------|
| `Id`             | int    | PK, Identity |
| `FirstName`      | string |         |
| `LastName`       | string |         |
| `PersonalNumber` | string | Unique  |
| `PhoneNumber`    | string | Unique  |

---

### 📅 Reservation

| Field          | Type     | Notes          |
|----------------|----------|----------------|
| `Id`           | int      | Primary Key    |
| `CheckInDate`  | DateTime | ≥ Today        |
| `CheckOutDate` | DateTime | > CheckInDate  |
| `GuestId`      | int      | FK → Guest     |

---

### 🔗 ReservationRoom *(Join Table)*

| Field           | Type | Notes                    |
|-----------------|------|--------------------------|
| `ReservationId` | int  | FK → Reservation         |
| `RoomId`        | int  | FK → Room                |

---

## Entity Relationships

```
Hotel ──────── 1 : M ──────── Manager
Hotel ──────── 1 : M ──────── Room
Guest ──────── 1 : M ──────── Reservation
Reservation ── M : M ──────── Room  (via ReservationRoom)
```

---

## Business Rules

### Hotel
- Create, update (name, address, rating), delete
- **Delete** only if the hotel has **no rooms** and **no active reservations**
- Filter hotels by `country`, `city`, `rating`

### Room
- Add rooms to a hotel with price validation (`> 0`)
- **Delete** only if the room has **no active or future reservations**
- Search rooms by **price range** and **availability on a specific date**

### Manager
- Register with unique `Email` and `PersonalNumber`
- Assign to a hotel
- **Delete** only if the hotel has **at least one other manager**
- Authenticates via **JWT**

### Guest
- Register with unique `PersonalNumber` and `PhoneNumber`
- **Delete** only if no active or future reservations exist

### Reservation
| Action   | Rules |
|----------|-------|
| **Create** | Authorized guest only · CheckIn ≥ Today · CheckOut > CheckIn · All rooms must be free on chosen dates |
| **Update** | Dates only · Must not overlap existing reservations |
| **Cancel** | Delete the reservation |
| **Search** | Filter by hotel, guest, room, or date range · Show active and completed |

---

## API Endpoints

### Hotels (Hotel-Centric)

```
GET    /api/hotels                              # Get all hotels (filterable)
GET    /api/hotels/{hotelId}                    # Get hotel by ID
POST   /api/hotels                              # Create hotel
PUT    /api/hotels/{hotelId}                    # Update hotel
DELETE /api/hotels/{hotelId}                    # Delete hotel

POST   /api/hotels/{hotelId}/rooms              # Add room to hotel
GET    /api/hotels/{hotelId}/rooms/{roomId}     # Get room by ID
PUT    /api/hotels/{hotelId}/rooms/{roomId}     # Update room
DELETE /api/hotels/{hotelId}/rooms/{roomId}     # Delete room

POST   /api/hotels/{hotelId}/managers           # Add manager to hotel

POST   /api/hotels/{hotelId}/reservations       # Create reservation
```

### Auth

```
POST   /api/auth/register    # Register (Manager or Guest)
POST   /api/auth/login       # Login → returns JWT token
```

### Reservations

```
GET    /api/reservations                        # Search/filter reservations
PUT    /api/reservations/{id}                   # Update reservation dates
DELETE /api/reservations/{id}                   # Cancel reservation
```

---

## Authentication & Roles

JWT tokens are issued on login and must be included in the `Authorization` header:

```
Authorization: Bearer <your_token>
```

### Role Permissions

| Action                        | Admin | Manager | Guest |
|-------------------------------|:-----:|:-------:|:-----:|
| Manage all hotels & rooms     | ✅    | ⚠️ own hotel only | ❌ |
| Manage managers               | ✅    | ❌      | ❌    |
| View hotels & rooms           | ✅    | ✅      | ✅    |
| Create / cancel reservation   | ✅    | ✅      | ✅ own only |
| View all reservations         | ✅    | ✅      | ❌    |

---

## Architecture

The project follows a clean **layered architecture** with separation of concerns:

```
HMS/
├── HMS.API/                    # Presentation Layer
│   ├── Controllers/
│   ├── Middleware/             # Global Exception Handler
│   └── Program.cs
│
├── HMS.Application/            # Business Logic Layer
│   ├── Services/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Mappings/               # AutoMapper / Mapster profiles
│
├── HMS.Domain/                 # Domain Entities
│   ├── Entities/
│   │   ├── Hotel.cs
│   │   ├── Manager.cs
│   │   ├── Room.cs
│   │   ├── Guest.cs
│   │   ├── Reservation.cs
│   │   └── ReservationRoom.cs
│   └── Enums/
│
├── HMS.Infrastructure/         # Data Access Layer
│   ├── Repositories/
│   │   └── GenericRepository.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Migrations/
│   │   └── Seed/
│   └── JWT/                    # Token generation
│
└── HMS.sln
```

### Design Patterns Used

- ✅ **Generic Repository Pattern**
- ✅ **Service Layer** (business logic separation)
- ✅ **Dependency Injection**
- ✅ **Global Exception Handling Middleware**
- ✅ **Uniform API Response Wrapper**
- ✅ **EF Core Code First + Migrations + Seed Data**

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Setup

```bash
# 1. Clone the repository
git clone https://github.com/your-username/hms.git
cd hms

# 2. Configure the connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=HmsDb;Trusted_Connection=True;"
}

# 3. Apply database migrations
dotnet ef database update --project HMS.Infrastructure --startup-project HMS.API

# 4. Run the application
dotnet run --project HMS.API
```

### JWT Configuration (`appsettings.json`)

```json
"JWT": {
  "SecretKey": "your_super_secret_key_here",
  "Issuer": "HMS",
  "Audience": "HMS_Users",
  "ExpiresInMinutes": 60
}
```

API will be available at: `https://localhost:5001` with Swagger at `/swagger`

---

## 📌 Notes

- All API responses follow a **uniform response wrapper** structure
- HTTP status codes strictly follow REST conventions (`200`, `201`, `400`, `401`, `403`, `404`, `409`, `500`)
- Room availability is **calculated dynamically** — no `IsAvailable` column in the database
- Seed data is applied automatically on first migration

---

*Built for educational and practical purposes using ASP.NET Core Web API.*
