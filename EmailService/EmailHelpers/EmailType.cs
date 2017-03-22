namespace EmailService.EmailHelpers
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
}