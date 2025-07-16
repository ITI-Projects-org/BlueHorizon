using Village_System.Models;
using Village_System.Repositories.Implementations;
using Village_System.Repositories.Interfaces;

namespace Village_System.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        public VillageSystemDbContext _context { get; }
        public UnitOfWork(VillageSystemDbContext context)
        {
            _context = context;
        }
        public IAccessPermissionRepository accessPermissionRepository;
        public IAccessPermissionRepository AccessPermissionRepository
        {
            get
            {
                if (accessPermissionRepository == null)
                {
                    accessPermissionRepository = new AccessPermissionRepository(_context);
                };
                return accessPermissionRepository;
            }
        }



        public IAmenityRepository amenityRepository;
        public IAmenityRepository AmenityRepository
        {
            get
            {
                if (amenityRepository == null)
                {
                    amenityRepository = new AmenityRepository(_context);
                };
                return amenityRepository;
            }
        }

        public IBookingRepository bookingRepository ;
        public IBookingRepository BookingRepository
        {
            get
            {
                if (bookingRepository == null)
                {
                    bookingRepository = new BookingRepository(_context);
                };
                return bookingRepository;
            }
        }

        public IMessageRepository messageRepository;
        public IMessageRepository MessageRepository
        {
            get
            {
                if (messageRepository == null)
                {
                    messageRepository = new MessageRepository(_context);
                };
                return messageRepository;
            }
        }

        public IOwnerReviewRepository ownerReviewRepository;
        public IOwnerReviewRepository OwnerReviewRepository
        {
            get
            {
                if (ownerReviewRepository == null)
                {
                    ownerReviewRepository = new OwnerReviewRepository(_context);
                };
                return ownerReviewRepository;
            }
        }

        public IOwnerVerificationDocumentRepository ownerVerificationDocumentRepository;
        public IOwnerVerificationDocumentRepository OwnerVerificationDocumentRepository
        {
            get
            {
                if (ownerVerificationDocumentRepository == null)
                {
                    ownerVerificationDocumentRepository = new OwnerVerificationDocumentRepository(_context);
                };
                return ownerVerificationDocumentRepository;
            }
        }


        public IPaymentTransactionRepository paymentTransactionRepository;
        public IPaymentTransactionRepository PaymentTransactionRepository
        {
            get
            {
                if (paymentTransactionRepository == null)
                {
                    paymentTransactionRepository = new PaymentTransactionRepository(_context);
                };
                return paymentTransactionRepository;
            }
        }
        public IQRCodeRepository qRCodeRepository;
        public IQRCodeRepository QRCodeRepository
        {
            get
            {
                if (qRCodeRepository == null)
                {
                    qRCodeRepository = new QRCodeRepository(_context);
                };
                return qRCodeRepository;
            }
        }
        public IUnitAmenityRepository unitAmenityRepository ;
        public IUnitAmenityRepository UnitAmenityRepository
        {
            get
            {
                if (unitAmenityRepository == null)
                {
                    unitAmenityRepository = new UnitAmenityRepository(_context);
                };
                return unitAmenityRepository;
            }
        }

        public IUnitRepository unitRepository;
        public IUnitRepository UnitRepository
        {
            get
            {
                if (unitRepository == null)
                {
                    unitRepository = new UnitRepository(_context);
                };
                return unitRepository;
            }
        }
        public IUnitReviewRepository unitReviewRepository;
        public IUnitReviewRepository UnitReviewRepository
        {
            get
            {
                if (unitReviewRepository == null)
                {
                    unitReviewRepository = new UnitReviewRepository(_context);
                };
                return unitReviewRepository;
            }
        }

        public IOwnerRepository ownerRepository;
        public IOwnerRepository OwnerRepository
        {
            get
            {
                if (ownerRepository == null)
                {
                    ownerRepository = new OwnerRepository(_context);
                };
                return ownerRepository;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
