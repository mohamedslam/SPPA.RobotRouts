#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiImageElementHelper
    {
        #region Fields
        private IStiImageElement imageElement;
        #endregion

        #region Helper Methods
        private Hashtable GetImageElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(imageElement as StiComponent);
            properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(imageElement, 1, 1, true));

            return properties;
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {   
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetImageElementJSProperties(parameters);
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyValue = parameters["propertyValue"];

            switch (parameters["propertyName"] as string)
            {
                case "imageSrc":
                    {
                        imageElement.Image = !String.IsNullOrEmpty(propertyValue as string) ? Convert.FromBase64String(((string)propertyValue).Substring(((string)propertyValue).IndexOf("base64,") + 7)) : null;
                        break;
                    }
                case "imageUrl":
                    {
                        imageElement.ImageHyperlink = StiEncodingHelper.DecodeString(propertyValue as string);
                        break;
                    }
                case "icon":
                    {
                        imageElement.Icon = (StiFontIcons?)(!string.IsNullOrEmpty(propertyValue as string) ? Enum.Parse(typeof(StiFontIcons), (string)propertyValue) : null);
                        break;
                    }
                case "iconColor":
                    {
                        imageElement.IconColor = StiReportEdit.StrToColor(propertyValue as string);
                        break;
                    }
            }
        }
        #endregion

        #region Constructor
        public StiImageElementHelper(IStiImageElement imageElement)
        {
            this.imageElement = imageElement;
        }
        #endregion   
    }
}
