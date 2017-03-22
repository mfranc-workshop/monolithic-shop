using System.Threading.Tasks;
using RestEase;

namespace MicroShop.Services
{
    public enum EmailType
    {
        PaymentAccepted = 0,
        PaymentRefused,
        OrderSend,
        OrderReceived,
        OrderDelayed,
        TransferReceived,
        WaitingForTransfer
    }

    public interface IEmailService
    {
        [Post("email")]
        Task<bool> SendEmail(string email, EmailType type);
    }
}
