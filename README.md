# BIABank - Banking System Simulation

A comprehensive banking system simulation built with ASP.NET Core 9.0, featuring customer account management, transactions, KYC verification, and an admin portal. The system implements clean architecture principles with JWT authentication and real-time notifications.

## Tech Stack

### Backend (API)
- **Framework**: ASP.NET Core 9.0
- **Database**: SQLite with Entity Framework Core 9.0
- **Authentication**: JWT Bearer Tokens
- **Real-time Communication**: SignalR
- **API Documentation**: OpenAPI/Swagger
- **Email Service**: SMTP Integration

### Frontend (MVC)
- **Framework**: ASP.NET Core MVC 9.0
- **Authentication**: JWT Bearer Tokens
- **Real-time Updates**: SignalR Client
- **UI**: Razor Views with Bootstrap

### Infrastructure
- **Containerization**: Docker & Docker Compose
- **Reverse Proxy**: Nginx
- **SSL/TLS**: HTTPS Support
- **Database Storage**: Persistent SQLite Volume

## Features

### Core Banking Features
- **Account Management**: Create, view, and manage bank accounts
- **Transactions**:
  - Deposits  
  - Withdrawals  
  - Fund transfers  
  - Bill payments  
- **Customer Management**: Full customer profile management
- **Transaction History**: Audit trail of all financial operations
- **Reports & Analytics**: Financial reports and operational insights

### Customer Features
- **Account Management**: Open and manage personal bank accounts
- **Transactions**: Perform deposits, withdrawals, transfers, and bill payments
- **KYC Verification**: Customer identity verification system
- **Real-time Notifications**: Instant transaction alerts via SignalR
- **Transaction History**: View detailed transaction records

![Customer Interface](Image/Customer.png)

### Admin Portal
- **User Management**: Manage customer accounts, roles, and permissions
- **Operations Monitoring**: Track and audit all banking activities
- **KYC Review**: Approve or reject customer verification requests
- **System Administration**: Configure system settings and access controls
- **Reporting & Analytics**: Monitor financial and operational performance

![Admin Portal](Image/Admin.png)

### Security Features
- JWT-based authentication and authorization
- Role-Based Access Control (RBAC) for admin and customer roles
- Permission-based authorization
- Secure password hashing and validation
- SSL/TLS (HTTPS) encryption
- CORS policy configuration
- SQL injection prevention via Entity Framework

### Technical Features
- Responsive web interface
- Real-time updates using SignalR
- Docker containerization
- Environment-based configuration
- Database migrations
- Nginx reverse proxy
- Email notifications via SMTP
- Secure API architecture with MVC pattern

## Architecture
The application follows a clean architecture pattern with clear separation of concerns:
```
   ┌─────────────┐
   │   Browser   │
   └──────┬──────┘
          │
┌─────────▼──────────┐
│       NGINX        │
└────┬────────────┬──┘
     │            │
┌────▼─────┐      │
│   MVC    │──────│
│(Frontend)│      │
└──────────┘  ┌───▼────────┐
              │    API     │
              │ (Backend)  │
              └───┬────────┘
                  │
              ┌───▼──────┐
              │ Database │
              └──────────┘
```

## Clean Architecture Layers

- **Presentation**: MVC Web App (Razor Views, Controllers)
- **Application**: Business logic, services, use cases, DTOs
- **Domain**: Core entities, value objects, business rules
- **Infrastructure**: EF Core, SQLite, SMTP, external integrations

## Configuration

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```env
# JWT Configuration
JWT_SECRET_KEY=your-secret-key-here

# Email Configuration
EMAIL_SMTP_HOST=smtp.example.com
EMAIL_SMTP_PORT=587
EMAIL_USERNAME=your-email@example.com
EMAIL_PASSWORD=your-email-password
EMAIL_FROM=noreply@biabank.com
EMAIL_FROM_NAME=BIABank

# Application Settings
APP_BASE_URL=https://yourdomain.com
```
### Database Connection

The application uses SQLite by default. The connection string is configured in:
- **Development**: `appsettings.Development.json`
- **Production**: Docker environment variables

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/get-started)

### Quick Start with Docker (Recommended)

1. **Clone the repository**
```bash
git clone https://github.com/xuanhoang24/BIABanking
cd BIABanking
```

2. **Configure environment variables**

Create a `.env` file in the root directory  
(see [Configuration → Environment Variables](#environment-variables)).

For development, create a `docker-compose.override.yml` file:
```yaml
services:
  api:
    ports:
      - "5000:8080"

  web:
    ports:
      - "7000:8080"
```

3. **Run with Docker Compose**

For development (without nginx):
```bash
docker-compose up -d
```

For production (with nginx and SSL):
```bash
docker-compose --profile prod up -d
```

4. **Access the application**

**Development mode:**
- Web Application: http://localhost:7000
- API Base: http://localhost:5000

**Production mode (with nginx):**
- Web Application: https://localhost (or https://biabanking.site)
- API Base: https://localhost/api (or https://biabanking.site/api)

> **Note**: The API doesn't have a root landing page. Use the web application to interact with the system, or access specific API endpoints like `/api/auth/login`.

## Support
For issues and questions, please open an issue in the repository.