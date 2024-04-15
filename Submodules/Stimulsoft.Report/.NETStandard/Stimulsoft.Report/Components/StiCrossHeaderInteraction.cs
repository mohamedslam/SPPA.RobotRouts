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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Engine;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    public class StiCrossHeaderInteraction : StiInteraction
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCrossHeaderInteraction
            jObject.AddPropertyBool("CollapsingEnabled", CollapsingEnabled);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CollapsingEnabled":
                        this.CollapsingEnabled = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

		#region Properties
		[Browsable(false)]
		public override bool IsDefault => base.IsDefault && !CollapsingEnabled;
	    #endregion

        #region Properties.Collapsing
	    /// <summary>
		/// Gets or sets value which indicates whether it is allowed or not data collapsing in the report viewer.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether it is allowed or not data collapsing in the report viewer.")]
		[StiOrder(StiPropertyOrder.InteractionCollapsingEnabled)]
		[StiEngine(StiEngineVersion.EngineV2)]
		public virtual bool CollapsingEnabled { get; set; }
	    #endregion
	}
}
