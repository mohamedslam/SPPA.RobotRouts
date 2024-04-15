using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiRadarGridLinesCoreXF : 
        IStiApplyStyle, 
        ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (GridLines.AllowApplyStyle)
            {
                if (this.GridLines is IStiRadarGridLinesVert)
                {
                    this.GridLines.Color = style.Core.GridLinesVertColor;
                }
                else
                {
                    this.GridLines.Color = style.Core.GridLinesHorColor;
                }
            }
        }
        #endregion

        #region Properties
        private IStiRadarGridLines gridLines;
        public IStiRadarGridLines GridLines
        {
            get
            {
                return gridLines;
            }
            set
            {
                gridLines = value;
            }
        }
        #endregion

        public StiRadarGridLinesCoreXF(IStiRadarGridLines gridLines)
        {
            this.gridLines = gridLines;
        }
	}
}
