#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dashboard.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dashboard
{
    [JsonObject]
    [RefreshProperties(RefreshProperties.All)]
    public class StiElementLayout :
        IStiJsonReportObject,
        ICloneable
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("FullScreenButton", FullScreenButton, true);
            jObject.AddPropertyBool("SaveButton", SaveButton, true);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FullScreenButton":
                        this.FullScreenButton = property.DeserializeBool();
                        break;

                    case "SaveButton":
                        this.SaveButton = property.DeserializeBool();
                        break;
                }
            }
        }

        internal static StiElementLayout CreateFromJsonObject(JObject jObject, IStiElement element)
        {
            var layout = new StiElementLayout(element);

            layout.LoadFromJsonObject(jObject);

            return layout;
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault => FullScreenButton && SaveButton;
        #endregion

        #region Properties
        [DefaultValue(true)]
        [StiSerializable]
        public bool FullScreenButton
        {
            get
            {
                return Layout != null ? Layout.ShowFullScreenButton : true;
            }
            set
            {
                if (Layout != null)
                    Layout.ShowFullScreenButton = value;
            }
        }

        [DefaultValue(true)]
        [StiSerializable]
        public bool SaveButton
        {
            get
            {
                return Layout != null ? Layout.ShowSaveButton : true;
            }
            set
            {
                if (Layout != null)
                    Layout.ShowSaveButton = value;
            }
        }

        private IStiInteractionLayout Layout => (element as IStiElementInteraction)?.DashboardInteraction as IStiInteractionLayout;
        #endregion

        #region Fields
        private IStiElement element;
        #endregion

        public StiElementLayout()
        {
        }

        public StiElementLayout(IStiElement element)
        {
            this.element = element;
        }
    }
}