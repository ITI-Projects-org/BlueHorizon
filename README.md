# Tourist Village Rental System

A web-based platform for managing rentals of residential units within a tourist village, inspired by communities like Marassi in Egypt's North Coast.

## 📌 Overview

The system facilitates interaction between two user types: **owners** and **tenants**. Owners can list properties (after legal verification), manage availability, pricing, and generate QR codes for access. Tenants can search, book, communicate with owners, and review units.

---

## ⚙️ Technologies Used

- **Frontend:** Angular
- **Backend:** ASP.NET Core Web API
- **Database:** SQL Server
- **Authentication:** JWT
- **Payment Integration:** Third-party gateway
- **QR Code:** Secure generation and validation

---

## 🧱 Core Features

### 👤 Owner Module

- Register with ID + ownership documents
- Add/Edit/Delete unit listings
- Manage pricing and availability (calendar view)
- Accept/reject booking requests
- Generate time-limited QR codes for tenants
- Track earnings + past rentals
- View ratings/reviews left by tenants

### 🧍 Tenant Module

- Browse/filter/search units
- View detailed unit + owner info
- In-app messaging with owners
- Book units and make secure payments
- Receive QR codes for access
- Rate/review units and owners

### 🛠️ System Core

- Secure login and registration
- Admin panel for document review, verification
- Commission management
- Notification system (bookings, payments, reviews)
- QR Code validation for gate/facility entry
- Messaging system (in-app chat)
- Rating system for units and owners

---

## 🗄️ Database Design (Conceptual)

### 👥 Users:

- User, Owner, Tenant (inheritance or 1:1 relation)

### 📦 Listings:

- Unit, Amenities, UnitAmenity (M\:N)

### 🧾 Transactions:

- Booking, QR_Code, Access_Permission
- Payment_Transaction

### 💬 Communication:

- Message (Sender/Receiver linked to User)

### 🌟 Reviews:

- UnitReview, OwnerReview (linked to BookingID)

---

## 🧪 Usage Scenarios

### ✅ Owner Registration & Verification

- Upload ownership contract + ID
- Admin approves/rejects verification

### 🏠 Listing a Unit

- Post-verification owners list full unit details + photos
- Set pricing and availability

### 🔍 Tenant Booking

- Filter/search units, view details, message owner
- Book + pay via secure gateway
- Receive QR code for stay duration

### 🚪 QR Code Usage

- Scanned at village gates or facilities
- Validates duration + access permissions

### ⭐ Tenant Review

- Post-stay: rate unit and owner
- System recalculates averages

### ❌ Owner Rejects Booking

- Booking status set to Rejected
- Tenant notified + refunded

---

## 🚀 Running the Project

### 1. Clone Repository

```bash
git clone https://github.com/your-org/tourist-village-rental.git
```

### 2. Navigate to Folders

- `/backend` → ASP.NET Core Web API
- `/frontend` → Angular frontend

### 3. Backend Setup

```bash
cd backend
# Restore dependencies
dotnet restore
# Run the API
dotnet run
```

### 4. Frontend Setup

```bash
cd frontend
npm install
ng serve --open
```

---

## 👥 Team Members

- Owner Verification Admin
- Frontend Developers (Angular)
- Backend Developers (API, QR, Payments)
- QA / Testers

---

## 📁 Branch Strategy

- `main`: Production
- `dev`: Integration
- `frontend-*`: Angular features
- `backend-*`: API features

---

## 📌 Notes

- All booking/payment data must be secure
- QR codes are valid only during rental window
- No listings possible before owner verification
- Admin can moderate all reviews

---

## 📧 Contact

For technical questions or support, please contact the project admin.
