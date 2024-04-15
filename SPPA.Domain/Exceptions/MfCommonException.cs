namespace SPPA.Domain.Exceptions;

public abstract class MfCommonException : Exception
{
    public MfErrorModel ViewModel { get; set; }

    public MfCommonException(MfErrorModel model)
        : base(model.DebugMsg)
    {
        ViewModel = model;
    }

    public MfCommonException(MfErrorModel model, Exception exception)
        : base(model.DebugMsg, exception)
    {
        ViewModel = model;
    }
}
