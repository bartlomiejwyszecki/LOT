# Auth Module - Implementation Plan

## Overview

The Auth module provides comprehensive authentication and authorization for the LogisticsTracker system. It supports:
- **Local authentication** (email/password with email verification)
- **OAuth 2.0 integration** (Google, Microsoft, Azure B2C)
- **Role-based access control** (User, Merchant, Recipient, Carrier, Courier, Admin, SuperAdmin)
- **JWT token management** (access tokens + refresh tokens)
- **Password management** (reset, change, secure hashing with bcrypt)
- **External provider account linking** (multiple OAuth providers per user)

---

## Architecture & Design Pattern

### Pattern: Clean Architecture (Modular Monolith)

### Layered Architecture
```
┌─────────────────────────────────────────────┐
│  API Layer                                  │ 
├─────────────────────────────────────────────┤
│  Application Layer                          │ 
├─────────────────────────────────────────────┤
│  Domain Layer                               │ 
├─────────────────────────────────────────────┤
│  Infrastructure Layer                       │ 
└─────────────────────────────────────────────┘
```
- **Domain Layer**: Defines User entity with business rules (password validation, email verification, role assignment). 
- **Application Layer**: Implements IAuthenticationService orchestrating domain logic.
- **Infrastructure Layer**: EF Core persistence, OAuth provider implementations, bcrypt hashing, JWT token generation.
- **API Layer**: ASP.NET controllers mapping HTTP requests to Application services.

## Principles:

1. **Dependency Inversion**: All dependencies point inward toward Domain. Infrastructure and Application depend on Domain, never the reverse.
2. **Independence**: Domain has no external framework dependencies (no EF, no ASP.NET, no HTTP).
3. **Testability**: Each layer tested in isolation without infrastructure dependencies.
4. **Separation of Concerns**: Each layer has a single, clear responsibility.

---

## Project Structure plan

```
api/LogisticsTracker/Modules/Auth/
│
├── Auth.Domain/
│   ├── Entities/
│   │   ├── Entity.cs                     
│   │   ├── User.cs                       
│   │   └── * ?? ExternalProviderLink.cs       
│   ├── ValueObjects/
│   │   ├── Role.cs                      
│   │   ├── Email.cs                      
│   │   └── ?? Password.cs                   
│   ├── Exceptions/
│   │   ├── AuthDomainException.cs
│   │   ├── InvalidEmailException.cs
│   │   ├── WeakPasswordException.cs
│   │   └── UserAlreadyExistsException.cs
│   └── Auth.Domain.csproj
│
├── Auth.Application/
│   ├── Services/
│   │   ├── IAuthenticationService.cs     # Main service interface
│   │   ├── AuthenticationService.cs      # Orchestrates auth workflows
│   │   ├── IJwtTokenService.cs
│   │   ├── JwtTokenService.cs
│   │   ├── IPasswordService.cs
│   │   ├── PasswordService.cs
│   │   ├── IEmailVerificationService.cs
│   │   └── EmailVerificationService.cs
│   ├── DTOs/
│   │   ├── RegisterRequestDto.cs
│   │   ├── SelectRoleRequestDto.cs
│   │   ├── LoginRequestDto.cs
│   │   ├── LoginResponseDto.cs
│   │   ├── * ExternalLoginRequestDto.cs
│   │   ├── RefreshTokenRequestDto.cs
│   │   ├── PasswordResetRequestDto.cs
│   │   └── UserProfileDto.cs
│   ├── Interfaces/
│   │   ├── IUserRepository.cs
│   │   ├── * IExternalProviderRepository.cs
│   │   └── IEmailService.cs
│   ├── DependencyInjection.cs
│   └── Auth.Application.csproj
│
├── Auth.Infrastructure/
│   ├── Persistence/
│   │   ├── AuthDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── UserConfiguration.cs
│   │   │   └── * ExternalProviderLinkConfiguration.cs
│   │   ├── Repositories/
│   │   │   ├── UserRepository.cs
│   │   │   └── * ExternalProviderRepository.cs
│   │   ├── Migrations/
│   │   └── Seeds/
│   │       └── AuthDbContextSeed.cs
│   ├── * OAuth/
│   │   ├── IOAuthProvider.cs
│   │   ├── OAuthProviderFactory.cs
│   │   ├── Providers/
│   │   │   ├── GoogleOAuthProvider.cs
│   │   │   ├── MicrosoftOAuthProvider.cs
│   │   │   └── AzureB2COAuthProvider.cs
│   │   └── Models/
│   │       ├── OAuthSettings.cs
│   │       ├── OAuthTokenResponse.cs
│   │       └── OAuthUserInfo.cs
│   ├── Services/
│   │   ├── PasswordHashingService.cs
│   │   ├── JwtTokenService.cs
│   │   ├── RefreshTokenService.cs
│   │   ├── EmailVerificationService.cs
│   │   └── EmailService.cs
│   ├── DependencyInjection.cs
│   └── Auth.Infrastructure.csproj
│
├── Auth.Api/
│   ├── Controllers/
│   │   └── AuthController.cs
│   ├── Requests/
│   │   ├── RegisterRequest.cs
│   │   ├── SelectRoleRequest.cs
│   │   ├── LoginRequest.cs
│   │   ├── * ExternalLoginRequest.cs
│   │   ├── RefreshTokenRequest.cs
│   │   ├── PasswordResetRequest.cs
│   │   └── ChangePasswordRequest.cs
│   ├── Responses/
│   │   ├── LoginResponse.cs
│   │   ├── UserResponse.cs
│   │   ├── TokenResponse.cs
│   │   └── ApiResponse.cs
│   ├── AuthApiExtensions.cs
│   └── Auth.Api.csproj
│
├── Auth.Tests/
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── UserTests.cs
│   │   │   └── * ExternalProviderLinkTests.cs
│   │   └── ValueObjects/
│   │       ├── EmailTests.cs
│   │       └── PasswordTests.cs
│   ├── Application/
│   │   ├── Services/
│   │   │   ├── AuthenticationServiceTests.cs
│   │   │   ├── JwtTokenServiceTests.cs
│   │   │   └── PasswordServiceTests.cs
│   │   └── Fixtures/
│   │       ├── AuthTestDataBuilder.cs
│   │       └── AuthFixture.cs
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── UserRepositoryTests.cs
│   │   │   ├── AuthDbContextMigrationTests.cs
│   │   │   └── DatabaseFixture.cs
│   │   └── * OAuth/
│   │       ├── GoogleOAuthProviderTests.cs
│   │       ├── MicrosoftOAuthProviderTests.cs
│   │       └── AzureB2COAuthProviderTests.cs
│   ├── Integration/
│   │   ├── AuthenticationFlowTests.cs
│   │   ├── * ExternalProviderIntegrationTests.cs
│   │   └── TokenRefreshTests.cs
│   └── Auth.Tests.csproj
```

---

## Module Functionality

### 1. Local User Registration & Login

**Registration**:
- Email + password input with validation
- Password requirements: min 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special char
- Email verification token generated and sent
- User created with default "User" role (unverified state)

**Email Verification**:
- Two-step process: token sent to email, user clicks link or provides token
- Token must be valid and not expired
- Email marked as verified in database

**Role Selection**:
- After email verification, user selects role
- Available roles: Merchant, Recipient, Carrier, Courier (User is default)
- Admin/SuperAdmin assigned by system only

**Login**:
- Email + password authentication
- Password verified against bcrypt hash
- JWT access token issued (~15 min expiration)
- Refresh token issued (~7 days expiration)

### 2. * OAuth 2.0 / OpenID Connect Integration (Phase 2 - plan for future)

**Supported Providers**:
- **Azure B2C** (primary provider)
- **Google OAuth 2.0**
- **Microsoft OAuth 2.0**

**OAuth Registration**:
- User redirected to provider consent screen
- Provider returns authorization code
- System exchanges code for user info (server-to-server)
- User created automatically with email from OAuth provider
- Default "User" role assigned

**Provider Linking**:
- Authenticated users can link multiple OAuth providers
- Single local account can have multiple external identities
- Prevents duplicate accounts

### 3. Token Management

**Access Token**:
- JWT format (not stored in DB)
- Short-lived (~15 minutes)
- Contains: user ID, email, role
- Signature verified on each API request

**Refresh Token**:
- Long-lived (~7 days)
- Used to obtain new access token without re-login
- Optionally stored in DB for revocation capability
- Rotated on each refresh for security

**Refresh Flow**:
- Client sends refresh token
- System validates it
- New access + refresh token pair issued
- Old refresh token invalidated

### 4. Password Management

**Reset Password**:
- User requests reset with email
- Reset token generated with expiration
- Email sent with reset link
- User submits new password with valid token
- Password updated, token invalidated

**Change Password**:
- Authenticated user provides current + new password
- Current password verified
- New password validated and stored (bcrypt)

### 5. User Profile Management

**Get Current User**:
- Extract user ID from JWT claims (stateless, no DB call)
- Return user profile with role

**Get User by ID**:
- Admin-only endpoint
- Fetch from database

### 6. Role-Based Access Control

- JWT contains role claim
- API endpoints protection with `[Authorize(Roles = "...")]` attributes
- Roles checked on each request

---

## * Azure B2C Integration (Phase 2 - plan for future)

### Overview

Azure B2C is the **primary OAuth provider** handling identity verification, social login, and password management.

```
Frontend App
    ↓
Auth API (Auth Module)
    ↓
┌──────────────────────────┐
│   Azure B2C Tenant       │
│ • Authentication         │
│ • Email verification     │
│ • Social login           │
│ • Password reset         │
│ • MFA support           │
└──────────────────────────┘
    ↓
Local Database
(Cache user data)
```

### Integration Flow

1. **User Initiates Login**:
   - Frontend redirects to Azure B2C login endpoint
   - User authenticates (local account or social provider)

2. **Authorization Code Exchange**:
   - Azure B2C redirects back with authorization code
   - Auth API exchanges code for tokens (backend call, no browser)
   - Prevents token exposure in browser URL

3. **User Sync**:
   - Extract user info from Azure B2C token (email, name, Azure B2C ID)
   - Check if user exists in local database
   - Create or update local user record
   - Link local user to Azure B2C ID via ExternalProviderLink

4. **Token Issuance**:
   - System generates its own JWT (not Azure B2C token)
   - JWT contains user context (ID, email, role)
   - Used for subsequent API authorization

### Configuration

**appsettings.json**:
```json
...TO BE IMPLEMENTED
```

### Benefits

- ✅ Azure manages password security (BCRYPT, salting, policies)
- ✅ Social login (Google, Microsoft, Facebook, etc.) out-of-box
- ✅ Multi-factor authentication (MFA) available
- ✅ GDPR/compliance built-in
- ✅ Automatic scaling, no server maintenance
- ✅ Custom policies for complex authentication flows

---

## Local Storage Strategy

### Database Design

**Users Table**:
| Column | Type | Purpose |
|--------|------|---------|
| Id | GUID | Primary key |
| Email | VARCHAR(256) | Unique constraint, indexed |
| PasswordHash | VARCHAR(MAX) | bcrypt hash (null for OAuth-only users) |
| FirstName | VARCHAR(100) | User display name |
| LastName | VARCHAR(100) | User display name |
| Role | INT | Enum value (1=SuperAdmin, 2=Admin, etc.) |
| IsEmailVerified | BIT | Email verification flag |
| EmailVerificationToken | VARCHAR(MAX) | Verification token (null after verified) |
| EmailVerificationTokenExpiry | DATETIME | When token expires |
| PasswordResetToken | VARCHAR(MAX) | Reset token (null unless reset pending) |
| PasswordResetTokenExpiry | DATETIME | When reset token expires |
| IsActive | BIT | Soft delete flag |
| CreatedAt | DATETIME | User creation timestamp |
| UpdatedAt | DATETIME | Last modification timestamp |

**ExternalProviderLinks Table**:
| Column | Type | Purpose |
|--------|------|---------|
| Id | GUID | Primary key |
| UserId | GUID | Foreign key to Users |
| Provider | VARCHAR(50) | "AzureB2C", "Google", "Microsoft" |
| ExternalUserId | VARCHAR(256) | Provider's user ID |
| Email | VARCHAR(256) | Email from OAuth provider |
| LinkedAt | DATETIME | When provider was linked |

### Performance Optimization

- Clustered index on Users.Id (primary key)
- Non-clustered index on Users.Email (login queries)
- Non-clustered index on ExternalProviderLinks.(Provider, ExternalUserId)
- Foreign key index on ExternalProviderLinks.UserId

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register with email/password |
| POST | `/api/auth/verify-email` | Verify email with token |
| POST | `/api/auth/select-role` | Select role after registration |
| POST | `/api/auth/login` | Login with email/password |
| GET | `/api/auth/login/external/{provider}` | Initiate OAuth login |
| POST | `/api/auth/callback/{provider}` | OAuth callback handler |
| POST | `/api/auth/link-provider/{provider}` | Link additional OAuth provider |
| POST | `/api/auth/refresh-token` | Refresh access token |
| POST | `/api/auth/logout` | Logout (optional, for audit) |
| POST | `/api/auth/password/reset-request` | Request password reset |
| POST | `/api/auth/password/reset` | Reset password with token |
| POST | `/api/auth/password/change` | Change password (authenticated) |
| GET | `/api/auth/me` | Get current user profile |

---

## Future: Event Bus Integration (Phase 2+)

**Example Scenario**:
```
User registers in Auth module
    ↓
UserRegisteredEvent published to event bus
    ↓
Multiple subscribers react asynchronously:
  • Notifications module: Send welcome email
  • Profile module: Create user profile
  • Analytics module: Log signup event
  • Reporting module: Update user stats
```

### Future Implementation Architecture

```
Auth Module                         Other Modules
┌─────────────────────────┐       ┌──────────────────────┐
│ Domain Events           │       │ Event Subscribers    │
│ • UserRegisteredEvent   │       │ • Notifications      │
│ • RoleSelectedEvent     │ ────→ │ • Profile            │
│ • PasswordResetEvent    │       │ • Analytics          │
└─────────────────────────┘       │ • Reporting          │
        ↓                         └──────────────────────┘
┌─────────────────────────┐
│  Event Bus / Mediator   │
│  (MassTransit, RabbitMQ)│
└─────────────────────────┘
        ↓
┌─────────────────────────┐
│  Event Store (optional) │
│  (Audit trail, replay)  │
└─────────────────────────┘
```

### Technology Options for Event Bus (Phase 2+)

| Technology | Use Case | Pros | Cons |
|-----------|----------|------|------|
| **MediatR** | In-process, sync/async | Simple, no infrastructure | Limited to single process |
| **RabbitMQ** | Distributed, async | Reliable, proven | Requires broker setup |
| **Azure Service Bus** | Cloud-native | Integrated with Azure B2C | Azure-dependent |
| **Mass Transit** | Abstraction layer | Provider-agnostic, features | Learning curve |
| **NServiceBus** | Enterprise | Full-featured, reliable | Licensed (commercial) |

### Implementation Roadmap

**Stage 1 (Current - Auth Module Only)**

**Stage 2 (External Providers)**:

**Stage 2+ (Event bus integration)**:
- Integrate MediatR / RabitMQ or other Event Bus technologies
- Add domain events to Auth
- Publish events within same process

**Stage 3 (When async/distributed needed)**:
- Replace with RabbitMQ or Azure Service Bus
- Async event processing
- Scalable across multiple servers