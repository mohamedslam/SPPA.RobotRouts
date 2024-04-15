#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Maps
{
    public class StiMapSvg
    {
        #region Properties
        public string Key { get; set; }
        public string Data { get; set; }
        public string EnglishName { get; set; }
        public string ISOCode { get; set; }
        // Internal use only
        public string Value { get; set; }

        // for EnglishName
        [DefaultValue(null)]
        public double? ShowHiddenTextIfZoom { get; set; }
        public Rectangle Rect { get; set; }
        [DefaultValue(false)]
        public bool SetMaxWidth { get; set; }
        [DefaultValue(false)]
        public bool SkipText { get; set; }
        [DefaultValue(StiTextHorAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiTextHorAlignment HorAlignment { get; set; } = StiTextHorAlignment.Center;
        [DefaultValue(StiVertAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Center;

        // for ISOCode
        public Rectangle RectIso { get; set; }
        [DefaultValue(null)]
        public bool? SkipTextIso { get; set; }
        [DefaultValue(null)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiTextHorAlignment? HorAlignmentIso { get; set; }
        [DefaultValue(null)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiVertAlignment? VertAlignmentIso { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{this.Key} {ISOCode}";
        }
        #endregion
    }
}