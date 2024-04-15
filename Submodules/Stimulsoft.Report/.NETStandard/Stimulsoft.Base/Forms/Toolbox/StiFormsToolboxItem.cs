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

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#else
using System.Drawing;
#endif

namespace Stimulsoft.Base.Forms.Toolbox
{
    public class StiFormsToolboxItem
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

        public Bitmap Image()
        {
            var type = baseObject.GetType();
            var info = type.GetProperty("DesignerImage");
            return info.GetValue(baseObject) as Bitmap;
        }

        public object CreateNewElement()
        {
            var type = baseObject.GetType();
            var method = type.GetMethod("CreateNewElement");
            return method.Invoke(baseObject, null);
        }
        #endregion

        public StiFormsToolboxItem(object baseObject)
        {
            this.baseObject = baseObject;
        }
    }
}
