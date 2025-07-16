using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace Village_System.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
        public int Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string PaymentGatewayReference { get; set; }
    }
}
public enum TransactionStatus
{
    Success, Failed, Pending
}
public enum PaymentMethod
{
    Cash, CreditCard, DebitCard, BankTransfer, PayPal
}
