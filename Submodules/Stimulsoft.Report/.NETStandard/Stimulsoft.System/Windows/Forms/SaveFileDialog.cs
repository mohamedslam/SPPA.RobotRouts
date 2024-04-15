using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class SaveFileDialog : IDisposable
    {

        public string Filter { get; set; }

        public int FilterIndex { get; set; }

        public string FileName { get; set; }

        public bool RestoreDirectory { get; set; }
        public string Title { get; set; }
        public string DefaultExt { get; set; }
        public string InitialDirectory { get; set; }

        public DialogResult ShowDialog()
        {
            return DialogResult.Abort;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public SaveFileDialog()
        {

        }
    }
}
