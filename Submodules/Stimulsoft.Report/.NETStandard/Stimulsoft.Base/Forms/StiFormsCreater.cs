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

using Stimulsoft.Base.Forms.Toolbox;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Base.Forms
{
    internal class StiFormsCreater
    {
        #region Methods
        internal static string GetTypeName(string name)
        {
            var type = Type.GetType("Stimulsoft.Form.Wpf.Designer.StiNameTypeProvider, Stimulsoft.Form.Wpf");

            return type.GetProperty(name).GetValue(null, null).ToString();
        }        

        internal static object GetFormPreviewControl(string formJson)
        {
            try
            {
                var typeName = GetTypeName("StiFormPreviewControl");
                return CreateObject(typeName, new object[] { formJson });
            }
            catch
            {
                return null;
            }
        }

        internal static object GetFormDesignControl(string formJson)
        {
            try
            {
                var typeName = GetTypeName("StiFormDesignSurfaceControl");
                return CreateObject(typeName, new object[] { formJson, true });
            }
            catch
            {
                return null;
            }
        }

        internal static object GetFormDesignWindow()
        {
            try
            {
                var typeName = GetTypeName("StiFormDesignerWindow");
                return CreateObject(typeName, new object[] { });
            }
            catch
            {
                return null;
            }
        }

        internal static object GetToolbox()
        {
            try
            {
                var typeName = GetTypeName("StiToolbox");
                return CreateObject(typeName, new object[] { });
            }
            catch
            {
                return null;
            }
        }

        public static List<StiFormsToolboxItem> GetToolboxItems()
        {
            try
            {
                var toolbox = GetToolbox();

                var type = toolbox.GetType();
                var info = type.GetProperty("Items");
                var baseList = info.GetValue(toolbox);
                var valueList = info.GetValue(toolbox) as ICollection;

                var list = new List<StiFormsToolboxItem>();

                foreach (var item in valueList)
                    list.Add(new StiFormsToolboxItem(item));

                return list;
            }
            catch
            {
                return null;
            }
        }

        private static object CreateObject(string typeName, object[] arg)
        {
            var type = Type.GetType($"{typeName}, Stimulsoft.Form.Wpf");
            return StiActivator.CreateObject(type, arg);
        }
        #endregion
    }
}
