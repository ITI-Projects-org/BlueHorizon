# BlueHorizon: Tourist Village Rental Platform

A sophisticated **B2B2C rental marketplace** specifically designed for tourist villages (similar to Marassi, North Coast Egypt). The platform operates as a **multi-sided marketplace** connecting property owners with travelers seeking vacation rentals.

## 📌 Business Model & Platform Overview

**BlueHorizon** facilitates seamless interaction between **property owners** and **tenants/guests** through a comprehensive rental management system. The platform ensures legal compliance, secure transactions, and quality user experiences.

### **Core Business Logic:**

- **Property Owners** list their units after rigorous legal verification
- **Tenants/Guests** search, book, and pay for accommodations securely
- **Platform** generates revenue through commission-based model
- **Admins** verify ownership documents and moderate platform content

### **Revenue Streams:**

1. **Commission-based**: Platform takes percentage from each successful booking
2. **Verification fees**: Document processing and legal validation charges
3. **Premium listings**: Enhanced visibility and marketing for property owners

---

## 🏗️ Technical Architecture

### **Overall Architecture Pattern: N-Layer Architecture**

```
┌─────────────────────────────────────────┐
│           Frontend (Angular)            │
├─────────────────────────────────────────┤
│              API Layer                  │
│         (ASP.NET Core Web API)          │
├─────────────────────────────────────────┤
│           Business Logic                │
│        (Services & UnitOfWork)          │
├─────────────────────────────────────────┤
│          Data Access Layer              │
│       (Repositories & EF Core)          │
├─────────────────────────────────────────┤
│             Database                    │
│          (SQL Server)                   │
└─────────────────────────────────────────┘
```

## ⚙️ Backend Technical Stack & Architecture

### **Core Technologies:**

- **Framework**: ASP.NET Core 9.0 Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT + ASP.NET Core Identity
- **Real-time Communication**: SignalR for in-app chat
- **Cloud Services**: Cloudinary for optimized image storage
- **QR Code**: Dynamic generation with time-based validation
- **Testing Framework**: xUnit for comprehensive unit testing

### **Key Design Patterns Implemented:**

#### **1. Repository Pattern**

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

- **Purpose**: Abstracts data access logic from business logic
- **Implementation**: Each entity has specific repository inheriting from GenericRepository
- **Benefits**: Enhanced testability, separation of concerns, maintainability

#### **2. Unit of Work Pattern**

```csharp
public class UnitOfWork : IUnitOfWork
{
    private BlueHorizonDbContext _context;
    public IBookingRepository BookingRepository { get; }
    public IUnitRepository UnitRepository { get; }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

- **Purpose**: Manages multiple repositories and ensures atomic transactions
- **Benefits**: Data consistency, transaction management, reduced database calls

#### **3. Dependency Injection Pattern**

```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
```

- **Purpose**: Achieves loose coupling and enhanced testability
- **Scope Management**: Scoped for request lifetime, Transient for stateless services

#### **4. Table Per Hierarchy (TPH) Inheritance**

```csharp
builder.Entity<ApplicationUser>()
    .HasDiscriminator<string>("UserType")
    .HasValue<ApplicationUser>("Base")
    .HasValue<Tenant>("Tenant")
    .HasValue<Owner>("Owner")
    .HasValue<Admin>("Admin");
```

- **Purpose**: Efficient user role management with single table
- **Benefits**: Performance optimization for user queries, simplified schema

#### **5. DTO (Data Transfer Object) Pattern**

- **Purpose**: Secure data serialization and well-defined API contracts
- **Implementation**: AutoMapper for efficient object-to-object mapping
- **Benefits**: API versioning, security, performance optimization

## 🎨 Frontend Technical Stack & Architecture

### **Core Technologies:**

- **Framework**: Angular 20 (Latest stable version)
- **State Management**: RxJS Observables & Service-based architecture
- **UI Framework**: Bootstrap 5 with custom responsive design
- **Real-time Features**: SignalR Client for instant messaging
- **HTTP Communication**: HttpClient with sophisticated interceptors
- **Authentication**: JWT token management with automatic refresh
- **Icons**: Bootstrap Icons + FontAwesome integration
- **Animations**: Angular Animations API for smooth UX

### **Frontend Architecture Patterns:**

#### **1. Component-Based Architecture**

```
src/app/
├── Components/          # Reusable UI components
├── Pages/              # Route-specific page components
├── Services/           # Business logic & HTTP communication
├── Guards/            # Route protection & authorization
├── Interceptors/      # HTTP request/response handling
├── Models/           # TypeScript interfaces & types
└── Pipes/           # Data transformation utilities
```

#### **2. Service Pattern with Dependency Injection**

```typescript
@Injectable({ providedIn: "root" })
export class AuthService {
  private baseUrl = "https://localhost:7083/api/Authentication";

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Login`, credentials).pipe(
      tap((response) => this.handleLoginSuccess(response)),
      catchError(this.handleError)
    );
  }
}
```

#### **3. Guard Pattern for Security**

```typescript
@Injectable({ providedIn: "root" })
export class AuthGuard implements CanActivate {
  canActivate(): boolean {
    if (this.auth.isLoggedIn()) return true;
    this.router.navigate(["/login"]);
    return false;
  }
}
```

- **Implementation**: Role-based guards (Admin, Owner, Tenant)
- **Purpose**: Route-level security and user experience optimization

#### **4. Interceptor Pattern for HTTP Management**

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }
  return next(req);
};
```

#### **5. Reactive Programming with RxJS**

- **Observable Pattern**: Asynchronous data streams throughout the application
- **Benefits**: Reactive UI updates, sophisticated error handling, memory management

---

## 🗃️ Database Design & Entity Relationships

### **Entity Relationship Architecture:**

```
ApplicationUser (Base Class - TPH Inheritance)
├── Owner (Inherits from ApplicationUser)
│   ├── Units (1:N relationship)
│   ├── OwnerVerificationDocuments (1:N)
│   └── OwnerReviews (1:N received reviews)
├── Tenant (Inherits from ApplicationUser)
│   ├── Bookings (1:N relationship)
│   ├── UnitReviews (1:N written reviews)
│   └── Messages (1:N sent/received)
└── Admin (Inherits from ApplicationUser)
    └── Administrative functions

Unit (Core Business Entity)
├── Bookings (1:N relationship)
├── UnitAmenities (M:N via junction table)
├── UnitReviews (1:N relationship)
├── UnitImages (1:N relationship)
└── QRCode (1:1 via Booking relationship)

Booking (Transaction Entity)
├── PaymentTransaction (1:1)
├── QRCode (1:1)
├── AccessPermissions (1:N)
├── UnitReview (1:1 optional)
└── OwnerReview (1:1 optional)
```

### **Key Database Design Principles:**

- **Referential Integrity**: Comprehensive foreign key constraints
- **Cascade Deletes**: Strategic cascade rules for data consistency
- **Precision Decimals**: Financial calculations with proper decimal precision
- **Unique Constraints**: Email uniqueness, booking integrity validation
- **Indexing Strategy**: Optimized queries for search and filtering operations

### **Business Rules Enforced at Database Level:**

- **Verification Requirement**: Units cannot be listed without owner verification
- **Booking Validation**: Prevents double-booking through database constraints
- **Review Integrity**: Reviews linked to actual bookings only
- **QR Code Security**: Time-limited access with automatic expiration

---

## 🧱 Core Business Features

### 👤 **Owner Module - Property Management**

- **Registration & Verification**: Upload ownership documents + government ID
- **Property Listings**: Add/Edit/Delete unit listings with rich media
- **Dynamic Pricing**: Manage pricing strategies and seasonal rates
- **Availability Management**: Interactive calendar view for booking control
- **Booking Management**: Accept/reject booking requests with instant notifications
- **QR Code Generation**: Create time-limited access codes for verified tenants
- **Financial Dashboard**: Track earnings, commission deductions, payout history
- **Review Management**: Monitor and respond to guest reviews and ratings
- **Communication Hub**: Direct messaging with potential and confirmed guests

### 🧍 **Tenant Module - Guest Experience**

- **Advanced Search**: Filter by location, amenities, price range, availability
- **Detailed Listings**: Comprehensive unit information with owner profiles
- **Interactive Communication**: In-app messaging system with property owners
- **Secure Booking**: Multi-step booking process with payment integration
- **Digital Access**: Receive QR codes for seamless property access
- **Review System**: Rate and review both units and property owners
- **Booking History**: Track past and upcoming reservations
- **Notification Center**: Real-time updates on booking status and messages
- **Wishlist Management**: Save favorite properties for future booking

### 🛠️ **Administrative Core System**

- **User Management**: Comprehensive user registration and authentication
- **Verification Workflow**: Document review and approval process for owners
- **Content Moderation**: Review and moderate user-generated content
- **Commission Management**: Automated calculation and distribution of platform fees
- **Analytics Dashboard**: Platform performance metrics and business intelligence
- **Security Monitoring**: QR Code validation and access control systems
- **Communication Oversight**: Message monitoring and dispute resolution
- **Rating Algorithm**: Sophisticated rating calculation and display system

---

## � Advanced Features & Integrations

### **1. Real-time Communication System (SignalR)**

```csharp
[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string receiverId, string message)
    {
        await Clients.User(receiverId).SendAsync("ReceiveMessage",
            Context.UserIdentifier, message, DateTime.UtcNow);
    }
}
```

- **Instant Messaging**: Real-time chat between owners and tenants
- **Connection Management**: Automatic reconnection and presence tracking
- **Security**: JWT-based authentication for WebSocket connections

### **2. Dynamic QR Code System**

- **Time-based Generation**: Codes valid only during rental period
- **Encrypted Payload**: Secure data encoding with tamper detection
- **Multi-level Access**: Different permissions for gates, facilities, amenities
- **Audit Trail**: Complete access logging for security and analytics

### **3. Cloud Image Management (Cloudinary)**

```csharp
public class PhotoService : IPhotoService
{
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Transformation = new Transformation().Width(800).Height(600).Crop("fill")
        };
        return await _cloudinary.UploadAsync(uploadParams);
    }
}
```

### **4. AI-Powered Features**

- **Smart Recommendations**: Machine learning-based unit suggestions
- **Dynamic Pricing**: AI-driven pricing optimization based on demand
- **Content Moderation**: Automated review filtering and sentiment analysis
- **Fraud Detection**: Pattern recognition for suspicious booking activities

### **5. Payment Integration Architecture**

- **Multiple Gateways**: Support for various payment providers
- **Secure Processing**: PCI-compliant payment handling
- **Commission Calculation**: Automated platform fee management
- **Refund Management**: Streamlined refund processing for cancellations

---

## 📊 Performance & Scalability Architecture

### **Backend Performance Optimizations:**

- **Entity Framework Optimizations**: Lazy loading with strategic eager loading
- **Connection Pooling**: Efficient database connection management
- **Caching Strategy**: Multi-level caching (memory, distributed, CDN)
- **Asynchronous Operations**: Non-blocking I/O throughout the application
- **Database Indexing**: Strategic indexing for search and filtering operations

### **Frontend Performance Features:**

- **OnPush Change Detection**: Optimized Angular change detection strategy
- **Lazy Loading Routes**: Code splitting for faster initial page loads
- **Service Workers**: Progressive Web App capabilities for offline functionality
- **Image Optimization**: Lazy loading and responsive image delivery
- **Bundle Optimization**: Tree shaking and minification strategies

### **Scalability Considerations:**

- **Microservices Ready**: Modular architecture supporting future decomposition
- **Horizontal Scaling**: Stateless design enabling load balancing
- **CDN Integration**: Global content delivery for media assets
- **Database Sharding**: Prepared for horizontal database scaling

---

## 🔒 Comprehensive Security Implementation

### **Authentication & Authorization Framework:**

- **Multi-layer Security**: JWT tokens with refresh token rotation
- **Role-based Access Control**: Granular permissions (Owner, Tenant, Admin)
- **Session Management**: Secure token storage with automatic expiration
- **Two-factor Authentication**: Optional 2FA for enhanced security

### **Data Protection Measures:**

- **Input Validation**: Comprehensive server-side and client-side validation
- **SQL Injection Prevention**: Entity Framework parameterized queries
- **XSS Protection**: Angular's built-in sanitization + custom guards
- **CSRF Protection**: Anti-forgery tokens for state-changing operations
- **Data Encryption**: Sensitive data encryption at rest and in transit

### **API Security:**

- **Rate Limiting**: Protection against API abuse and DDoS attacks
- **CORS Configuration**: Controlled cross-origin resource sharing
- **HTTPS Enforcement**: SSL/TLS encryption for all communications
- **API Versioning**: Backward compatibility and security updates

---

## 🧪 Testing Strategy & Quality Assurance

### **Backend Testing Framework:**

- **Unit Tests**: xUnit framework for business logic validation
- **Integration Tests**: API endpoint testing with test databases
- **Repository Tests**: Data access layer validation and mocking
- **Security Tests**: Authentication and authorization verification

### **Frontend Testing Approach:**

- **Component Tests**: Angular Testing Utilities for UI components
- **Service Tests**: HTTP service mocking and error handling
- **E2E Tests**: End-to-end user journey validation
- **Performance Tests**: Load testing and performance profiling

### **Quality Assurance Metrics:**

- **Code Coverage**: Minimum 80% coverage requirement
- **Performance Benchmarks**: Response time and throughput monitoring
- **Security Audits**: Regular penetration testing and vulnerability assessment
- **User Acceptance Testing**: Stakeholder validation of business requirements

---

## 🗄️ Database Design (Detailed Implementation)

### 👥 **User Management System:**

- **ApplicationUser**: Base identity class with ASP.NET Core Identity integration
- **Owner, Tenant, Admin**: Inherited classes using Table-Per-Hierarchy pattern
- **Profile Management**: Extended user properties and preferences

### 📦 **Property Listing System:**

- **Unit**: Core property entity with rich metadata
- **Amenities**: Standardized amenity catalog
- **UnitAmenity**: Many-to-many relationship management
- **UnitImages**: Optimized image storage with Cloudinary integration

### 🧾 **Transaction Management:**

- **Booking**: Central booking entity with comprehensive business rules
- **PaymentTransaction**: Secure payment processing and audit trail
- **QRCode**: Dynamic access code generation and validation
- **AccessPermission**: Granular access control and logging

### 💬 **Communication System:**

- **Message**: Real-time messaging with SignalR integration
- **ChatMessage**: Enhanced chat features with read receipts
- **Notification**: System-wide notification management

### 🌟 **Review & Rating System:**

- **UnitReview**: Property-specific guest feedback
- **OwnerReview**: Host performance evaluation
- **Rating Algorithm**: Sophisticated scoring with weighted averages

---

## 🧪 Comprehensive Usage Scenarios

### ✅ **Owner Onboarding & Verification Process**

1. **Initial Registration**: Owner creates account with basic information
2. **Document Upload**: Submit ownership contracts and government identification
3. **Admin Review**: Verification team validates legal documents and ownership
4. **Approval Notification**: Owner receives approval status and can begin listing
5. **Profile Completion**: Add banking details for commission payouts

### 🏠 **Property Listing Management**

1. **Unit Creation**: Post-verification owners create detailed property listings
2. **Media Upload**: Add high-quality photos and virtual tour content
3. **Pricing Strategy**: Set base rates, seasonal pricing, and discount rules
4. **Availability Setup**: Configure calendar and booking rules
5. **Amenity Selection**: Choose from standardized amenity catalog
6. **Publication**: Unit goes live after final review

### 🔍 **Tenant Booking Journey**

1. **Search & Discovery**: Advanced filtering by location, dates, amenities, price
2. **Property Exploration**: View detailed listings, photos, reviews, location info
3. **Owner Communication**: Direct messaging for questions and special requests
4. **Booking Initiation**: Select dates and review pricing breakdown
5. **Secure Payment**: Process payment through integrated gateway
6. **Confirmation**: Receive booking confirmation and QR access codes
7. **Pre-arrival**: Access check-in instructions and property details

### 🚪 **QR Code Access System**

1. **Code Generation**: Automatic creation upon successful booking payment
2. **Time Validation**: Codes activate on check-in date, expire on checkout
3. **Multi-point Access**: Village gates, property entrance, amenity facilities
4. **Security Scanning**: Guards validate codes at entry points
5. **Access Logging**: Complete audit trail of all access attempts
6. **Emergency Override**: Admin capability for urgent access needs

### ⭐ **Review & Rating Process**

1. **Post-stay Trigger**: Automatic review invitation after checkout date
2. **Dual Review System**: Tenants review units, owners review guests
3. **Rating Categories**: Cleanliness, accuracy, communication, location, value
4. **Review Moderation**: AI and manual review for content appropriateness
5. **Score Calculation**: Weighted average with recency bias
6. **Public Display**: Approved reviews appear on property listings

### ❌ **Booking Cancellation & Dispute Resolution**

1. **Cancellation Request**: Either party can initiate cancellation
2. **Policy Application**: Automatic enforcement of cancellation terms
3. **Refund Processing**: Calculated refunds based on timing and policy
4. **Dispute Escalation**: Admin mediation for complex situations
5. **Resolution Documentation**: Complete audit trail of decisions
6. **Feedback Integration**: Learnings applied to policy improvements

### 🔧 **Administrative Oversight Functions**

1. **User Management**: Monitor and manage all platform participants
2. **Content Moderation**: Review and approve user-generated content
3. **Financial Oversight**: Commission tracking and payout management
4. **Security Monitoring**: Fraud detection and prevention measures
5. **Performance Analytics**: Platform metrics and business intelligence
6. **System Maintenance**: Updates, backups, and technical management

---

## 🚀 Development Setup & Deployment

### **1. Repository Management**

```bash
git clone https://github.com/ITI-Projects-org/BlueHorizon.git
cd BlueHorizon
```

### **2. Backend Environment Setup**

```bash
cd Backend/API

# Restore NuGet packages
dotnet restore

# Update database with migrations
dotnet ef database update

# Configure user secrets (development)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
dotnet user-secrets set "JwtKey" "your-jwt-secret-key"
dotnet user-secrets set "CloudinarySettings:CloudName" "your-cloudinary-name"

# Run the API server
dotnet run
```

### **3. Frontend Environment Setup**

```bash
cd Frontend

# Install dependencies
npm install

# Configure environment variables
# Update src/environments/environment.ts with API endpoints

# Start development server with SSL bypass
npm start

# Build for production
npm run build
```

### **4. Environment Configuration**

#### **Backend Configuration (appsettings.json)**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BlueHorizonDB;Trusted_Connection=true;"
  },
  "JwtKey": "your-super-secret-jwt-key-minimum-32-characters",
  "CloudinarySettings": {
    "CloudName": "your-cloudinary-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

#### **Frontend Configuration (environment.ts)**

```typescript
export const environment = {
  production: false,
  apiUrl: "https://localhost:7083/api",
  hubUrl: "https://localhost:7083/chathub",
};
```

### **5. Database Setup**

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Seed initial data (optional)
dotnet run --seed-data
```

---

**© 2025 BlueHorizon Platform - Tourist Village Rental System**
_Built with modern architecture principles for scalable, secure, and user-friendly vacation rental management._
