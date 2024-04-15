using System;

namespace LatexMath2MathML
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(String message)
        {
            this.Message = message;
        }

        public String Message { get; private set; }
    }
}
