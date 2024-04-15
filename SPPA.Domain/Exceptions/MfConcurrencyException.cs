namespace SPPA.Domain.Exceptions;

public class MfConcurrencyException : MfCommonException
{
    public MfConcurrencyException(string debugMsg, Exception exception)
        : base(new MfErrorModel("server-error.concurrency.common", null, null, debugMsg), exception)
    {
    }

}
