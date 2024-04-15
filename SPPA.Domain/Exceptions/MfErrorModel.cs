namespace SPPA.Domain.Exceptions;

public class MfErrorModel
{
    public string Message { get; set; }

    public Dictionary<string, string>? Args { get; set; }

    public Dictionary<string, MfErrorValidationMessage>? Validation { get; set; }

    public string? DebugMsg { get; set; }

    public MfErrorModel(
        string message,
        Dictionary<string, string>? args,
        Dictionary<string, MfErrorValidationMessage>? validation,
        string? debugMsg

    )
    {
        DebugMsg = debugMsg;
        Message = message;
        Args = args;
        Validation = validation;
    }

}


public class MfErrorValidationMessage
{
    public string Message { get; set; }

    public Dictionary<string, string>? Args { get; set; }

    public string? DebugMsg { get; set; }

    public MfErrorValidationMessage(string message, Dictionary<string, string>? args, string? debugMsg)
    {
        Message = message;
        Args = args;
        DebugMsg = debugMsg;
    }

}
