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

using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Design.Forms.Toolbox
{
    public class StiFormsToolboxItemRefl
    {
        #region Fields
        private object baseObject; 
        #endregion

        #region Methods
        public string Key()
        {
            var type = baseObject.GetType();
            var info = type.GetProperty("Key");
            return info.GetValue(baseObject).ToString();
        }

        public string LocalizedName()
        {
            var type = baseObject.GetType();
            var info = type.GetProperty("LocalizedName");
            return info.GetValue(baseObject).ToString();
        }

        public Bitmap ToolboxIcon()
        {
            var type = baseObject.GetType();
            var info = type.GetProperty("ToolboxIcon");
            return info.GetValue(baseObject) as Bitmap;
        }

        public Bitmap ToolboxSmallIcon()
        {
            var type = baseObject.GetType();
            var info = type.GetProperty("ToolboxSmallIcon");
            return info.GetValue(baseObject) as Bitmap;
        }

        public object CreateNewElement()
        {
            var type = baseObject.GetType();
            var method = type.GetMethod("CreateNewElement");
            return method.Invoke(baseObject, null);
        }
        #endregion

        public StiFormsToolboxItemRefl(object baseObject)
        {
            this.baseObject = baseObject;
        }
    }
}
