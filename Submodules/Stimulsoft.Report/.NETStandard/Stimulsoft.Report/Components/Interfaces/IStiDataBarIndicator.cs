using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Interface describes base properties for Data Bar Indicator.
    /// </summary>
    public interface IStiDataBarIndicator         
    {
        /// <summary>
        /// Gets or sets value which indicates which type of brush will be used for Data Bar indicator drawing.
        /// </summary>        
        StiBrushType BrushType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a color of positive values for data bar indicator.
        /// </summary>
        Color PositiveColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a color of negative values for data bar indicator.
        /// </summary>
        Color NegativeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a border color of positive values for data bar indicator.
        /// </summary>
        Color PositiveBorderColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a border color of negative values for data bar indicator.
        /// </summary>
        Color NegativeBorderColor
        {
            get;
            set;
        }
                
        /// <summary>
        /// Gets or sets value which indicates that border is drawing.
        /// </summary>        
        bool ShowBorder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value which direction data bar will be filled by brush, from left to right or from right to left.
        /// </summary>        
        StiDataBarDirection Direction
        {
            get;
            set;
        }
    }
}
