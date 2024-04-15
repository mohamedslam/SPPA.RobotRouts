namespace SPPA.Domain.Exceptions;

public class MfLicenseException : MfCommonException
{
    public MfLicenseException(
        string debugMsg,
        string message,
        Dictionary<string, string>? args = null
    )
        : base(new MfErrorModel(message, args, null, debugMsg))
    {
    }

}
