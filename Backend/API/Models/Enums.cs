namespace API.Models
{



    public enum TargetLocation
    {
        MainGate, AquaPark, Pool_A
    }

    public enum AccessType
    {
        Gate, Facility
    }
    public enum AmenityName
    {
        WIFI, PoolAccess, AC
    }



    public enum PaymentStatus
    {
        Pending, Refunded, Completed
    }
    public enum BookingStatus
    {
        Pending, Accepted, Rejected, Cancelled, Completed
    }
    public enum VerificationStatus
    {
        NotVerified=0 ,Pending=1, Verified=2, Rejected=3
    }

    public enum DocumentType
    {
        OwnershipContract, NationaId_Front, NationaId_Back 
    }
    public enum TransactionStatus
    {
        Success, Failed, Pending
    }
    public enum PaymentMethod
    {
        Cash, CreditCard, DebitCard, BankTransfer, PayPal
    }
    public enum UnitType
    {
        Apartment, Chalet, Villa,
    }
    public enum ReviewStatus
    {
        Approved, Pending, Rejected
    }
}