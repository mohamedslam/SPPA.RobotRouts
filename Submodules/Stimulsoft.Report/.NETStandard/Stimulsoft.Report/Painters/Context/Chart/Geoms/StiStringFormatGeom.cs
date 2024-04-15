#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Text;

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Base.Context
{
    public class StiStringFormatGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("IsGeneric", IsGeneric));
            jObject.Add(new JProperty("Alignment", Alignment.ToString()));
            jObject.Add(new JProperty("FormatFlags", FormatFlags.ToString()));
            jObject.Add(new JProperty("HotkeyPrefix", HotkeyPrefix.ToString()));
            jObject.Add(new JProperty("LineAlignment", LineAlignment.ToString()));
            jObject.Add(new JProperty("Trimming", Trimming.ToString()));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public bool IsGeneric { get; set; } = false;

        public StringAlignment Alignment { get; set; } = StringAlignment.Near;

        public StringFormatFlags FormatFlags { get; set; } = (StringFormatFlags)0;

        public HotkeyPrefix HotkeyPrefix { get; set; } = HotkeyPrefix.None;

        public StringAlignment LineAlignment { get; set; } = StringAlignment.Near;

        public StringTrimming Trimming { get; set; } = StringTrimming.None;
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.StringFormat;
        #endregion

        public StiStringFormatGeom(StringFormat sf)
        {
            this.Alignment = sf.Alignment;
            this.FormatFlags = sf.FormatFlags;
            this.HotkeyPrefix = sf.HotkeyPrefix;
            this.LineAlignment = sf.LineAlignment;
            this.Trimming = sf.Trimming;
        }
    }
}
