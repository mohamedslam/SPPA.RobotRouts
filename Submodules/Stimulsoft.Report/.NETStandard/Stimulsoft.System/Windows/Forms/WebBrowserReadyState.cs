using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    //
    // Summary:
    //     Specifies constants that define the state of the System.Windows.Forms.WebBrowser
    //     control.
    public enum WebBrowserReadyState
    {
        //
        // Summary:
        //     No document is currently loaded.
        Uninitialized = 0,
        //
        // Summary:
        //     The control is loading a new document.
        Loading = 1,
        //
        // Summary:
        //     The control has loaded and initialized the new document, but has not yet received
        //     all the document data.
        Loaded = 2,
        //
        // Summary:
        //     The control has loaded enough of the document to allow limited user interaction,
        //     such as clicking hyperlinks that have been displayed.
        Interactive = 3,
        //
        // Summary:
        //     The control has finished loading the new document and all its contents.
        Complete = 4
    }
}
