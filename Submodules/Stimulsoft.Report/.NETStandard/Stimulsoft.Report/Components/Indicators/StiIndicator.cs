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

using Stimulsoft.Base;
using System.ComponentModel;
using System.Linq;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiIndicatorConverter))]
    public abstract class StiIndicator : IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            return jObject;
        }

        public abstract void LoadFromJsonObject(JObject jObject);

        internal static StiIndicator CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").DeserializeString();

            StiIndicator indicator = null;
            switch (ident)
            {
                case "StiIconSetIndicator":
                    indicator = new StiIconSetIndicator();
                    break;

                case "StiDataBarIndicator":
                    indicator = new StiDataBarIndicator();
                    break;
            }

            indicator.LoadFromJsonObject(jObject);
            return indicator;
        }
        #endregion
    }
}
