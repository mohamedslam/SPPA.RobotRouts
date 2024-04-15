using System;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class DateTimePicker : Control
    {
        public string CustomFormat { get; set; }
        public Font CalendarFont { get; set; }
        public global::System.Drawing.Color CalendarForeColor { get; set; }
        public LeftRightAlignment DropDownAlign { get; set; }
        public bool ShowUpDown { get; set; }
        public DateTime MaxDate { get; set; }
        public DateTimePickerFormat Format { get; set; }
        public DateTime Value { get; set; }
        public DateTime MinDate { get; set; }
        public EventHandler ValueChanged { get; set; }
    }
}
