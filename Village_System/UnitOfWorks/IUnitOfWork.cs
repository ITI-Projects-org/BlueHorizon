using Village_System.Repositories.Implementations;
using Village_System.Repositories.Interfaces;

namespace Village_System.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IAccessPermissionRepository AccessPermissionRepository { get; }
        IOwnerRepository OwnerRepository { get; }
        IAmenityRepository AmenityRepository { get; }
        IBookingRepository BookingRepository { get; }
        IMessageRepository MessageRepository { get; }
        IOwnerReviewRepository OwnerReviewRepository { get; }
        IOwnerVerificationDocumentRepository OwnerVerificationDocumentRepository { get; }
        IPaymentTransactionRepository PaymentTransactionRepository { get; }
        IQRCodeRepository QRCodeRepository { get; }
        IUnitAmenityRepository UnitAmenityRepository { get; }
        IUnitRepository UnitRepository { get; }
        IUnitReviewRepository UnitReviewRepository { get; }
        Task SaveAsync();
    }
}













