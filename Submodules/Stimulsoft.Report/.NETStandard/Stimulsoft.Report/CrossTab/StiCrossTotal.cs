#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;

namespace Stimulsoft.Report.CrossTab
{
	/// <summary>
	/// Summary description for StiCrossRowTotal.
	/// </summary>
	public class StiCrossTotal : StiCrossField
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TextFormat");
            jObject.RemoveProperty("HideZeros");

            return jObject;
        }
        #endregion

        #region Properties Browsable(false)
        [StiNonSerialized]
		[Browsable(false)]
		public override StiFormatService TextFormat
		{
			get 
			{
				return base.TextFormat;
			}
			set 
			{
				base.TextFormat = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override bool HideZeros
		{
			get 
			{
				return base.HideZeros;
			}
			set 
			{
				base.HideZeros = value;
			}
		}

		[Browsable(true)]
		public override bool Enabled
		{
			get 
			{
				return base.Enabled;
			}
			set 
			{
				base.Enabled = value;
			}
		}

        public override string CellText => Text;
        #endregion

        #region IStiPropertyGridObject
	    public override StiComponentId ComponentId => StiComponentId.StiCrossTotal;
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossTotal();
        }
        #endregion

        public StiCrossTotal()
		{
			Brush = new StiSolidBrush(Color.WhiteSmoke);
		}
	}
}