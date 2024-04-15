namespace SPPA.Domain.Exceptions;

public class MfNotAcceptableException : MfCommonException
{
    public MfNotAcceptableException(
        string debugMsg,
        string message,
        Dictionary<string, string>? args = null
    )
        : base(new MfErrorModel(message, args, null, debugMsg))
    {
    }

}
