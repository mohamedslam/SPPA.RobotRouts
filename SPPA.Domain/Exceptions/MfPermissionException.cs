namespace SPPA.Domain.Exceptions;

public class MfPermissionException : MfCommonException
{
    public MfPermissionException(
        string debugMsg,
        string message,
        Dictionary<string, string>? args = null
    )
        : base(new MfErrorModel(message, args, null, debugMsg))
    {
    }

}
