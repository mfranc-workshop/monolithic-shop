namespace MicroShop.EmailHelpers
{
    public enum EmailType
    {
        PaymentAccepted,
        PaymentRefused,
        OrderSend,
        OrderReceived,
        OrderDelayed,
        TransferReceived,
        WaitingForTransfer
    }
}