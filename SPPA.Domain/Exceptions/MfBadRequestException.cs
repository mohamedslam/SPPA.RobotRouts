namespace SPPA.Domain.Exceptions;

public class MfBadRequestException : MfCommonException
{
    public MfBadRequestException(string debugMsg)
        : base(new MfErrorModel("server-error.bad-request.common", null, null, debugMsg))
    {
    }

    public MfBadRequestException(
        string debugMsg,
        string message,
        Dictionary<string, string>? args = null,
        Dictionary<string, MfErrorValidationMessage>? validate = null
    )
        : base(new MfErrorModel(message, args, validate, debugMsg))
    {
    }

}