# FleetLinker: Advanced Fleet & Spare Parts Management System

![Banner](https://img.shields.io/badge/Status-In--Progress-orange?style=for-the-badge)
![.NET 9](https://img.shields.io/badge/.NET-9.0-blue?style=for-the-badge&logo=dotnet)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green?style=for-the-badge)
![Saudi Arabia](https://img.shields.io/badge/Market-Saudi--Arabia-darkgreen?style=for-the-badge)

FleetLinker is a robust, high-performance e-commerce and management platform specifically tailored for the **Saudi Arabian market**. It bridge the gap between clients, suppliers, and workshop owners, providing a seamless ecosystem for spare parts e-commerce, equipment rentals, and event management.

---

## 🚀 Key Features

- **🛒 E-commerce Engine**: Specialized cart and checkout system for spare parts and equipment.
- **🛠️ Service Management**: Advanced equipment request workflows and workshop integration.
- **📅 Event Booking**: Comprehensive event management system with real-time visitor tracking and capacity management.
- **🌍 Bilingual by Design**: Full native support for **Arabic (Default)** and English, including localized identity error messages.
- **💡 Smart Order Consolidation**: Unified billing system that merges equipment requests and spare part offers into single, manageable orders.
- **🛡️ Secure Identity**: Role-based access control (Client, Supplier, Workshop Owner) powered by ASP.NET Core Identity and JWT.
- **📦 Reliable Caching**: High-speed data retrieval using Redis integration with local fallback support.

---

## 🏗️ Technical Architecture

FleetLinker is built on **Clean Architecture** principles to ensure scalability, maintainability, and testability:

- **Domain Layer**: Pure business logic, entities, and core specifications.
- **Application Layer**: MediatR-powered CQRS pattern, DTOs, and validation logic.
- **Infrastructure Layer**: EF Core implementation, Redis caching, and third-party integrations (Nada Payment).
- **Presentation (API)**: ASP.NET Core 9.0 RESTful API with Swagger/OpenAPI documentation.

---

## 🛠️ Tech Stack

- **Framework**: .NET 9.0
- **Database**: Microsoft SQL Server
- **Caching**: Redis (StackExchange.Redis)
- **Object Mapping**: AutoMapper
- **Validation**: FluentValidation
- **Messaging**: MediatR (CQRS Pattern)
- **Authentication**: JWT & ASP.NET Core Identity
- **Documentation**: Swagger UI & Postman Collections

---

## 🚦 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Redis](https://redis.io/download) (Optional, falls back to Memory Cache)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/FleetLinker.git
   cd FleetLinker
   ```

2. **Update Configuration**:
   Edit `appsettings.json` in the `FleetLinker.API` project to configure your database connection and Redis settings.

3. **Apply Migrations**:
   ```bash
   dotnet ef database update --project FleetLinker.Infra --startup-project FleetLinker.API
   ```

4. **Run the Application**:
   ```bash
   dotnet run --project FleetLinker.API
   ```

---

## 📖 API Documentation

Once the application is running, you can access the interactive Swagger documentation at:
`http://localhost:<port>/index.html`

---

## 🇸🇦 Built for Saudi localized experience
FleetLinker is designed with a deep understanding of the local market requirements, offering seamless Arabic localization and Saudi-specific business workflows.

