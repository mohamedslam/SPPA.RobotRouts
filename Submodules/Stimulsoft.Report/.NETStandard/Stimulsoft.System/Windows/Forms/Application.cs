using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.System.Windows.Forms
{
    public class Application
    {
        public static string ProductName { get; }
        public static List<Form> OpenForms { get; set; }

        public static CultureInfo CurrentCulture { get; set; } = Thread.CurrentThread.CurrentCulture;

        public static void DoEvents()
        {
            throw new NotImplementedException();
        }

        public static void Run(Form form)
        {
            throw new NotImplementedException();
        }
    }
}
