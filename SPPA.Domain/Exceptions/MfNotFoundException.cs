namespace SPPA.Domain.Exceptions;

public class MfNotFoundException : MfCommonException
{
    public MfNotFoundException(string debugMsg)
        : base(new MfErrorModel("server-error.not-found.common", null, null, debugMsg))
    {
    }

    public MfNotFoundException(
        string debugMsg,
        string message,
        Dictionary<string, string>? args = null
    )
        : base(new MfErrorModel(message, args, null, debugMsg))
    {
    }

}
