using API.Repositories.Implementations;
using API.Repositories.Interfaces;

namespace API.UnitOfWorks
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
        IUnitImagesRepository UnitImagesRepository { get; }
        IUnitReviewRepository UnitReviewRepository { get; }
        Task SaveAsync();
    }
}













