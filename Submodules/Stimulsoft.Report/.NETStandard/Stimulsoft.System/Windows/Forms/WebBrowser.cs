using System;

namespace Stimulsoft.System.Windows.Forms
{
    public class WebBrowser : Control
    {
        public bool AllowWebBrowserDrop { get; set; }

        public bool ScrollBarsEnabled { get; set; }

        public Uri Url { get; set; }

        public bool ScriptErrorsSuppressed { get; set; }

        public string DocumentText { get; set; }

        public WebBrowserReadyState ReadyState { get; }
    }
}