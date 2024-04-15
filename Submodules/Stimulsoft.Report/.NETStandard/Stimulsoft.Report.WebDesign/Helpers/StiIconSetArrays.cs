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

using System;
using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Web
{
    internal class StiIconSetArrays
    {
        public static Hashtable GetItems()
        {
            Hashtable iconSetArrays = new Hashtable();
            int count = Enum.GetValues(typeof(StiIconSet)).Length;
            for (int i = 0; i < count; i++)
            {
                var icons = StiIconSetHelper.GetIconSet(((StiIconSet)i));                
                ArrayList iconsArray = new ArrayList();
                if (icons != null) iconsArray.AddRange(icons);
                iconSetArrays[((StiIconSet)i).ToString()] = iconsArray.Count > 0 ? iconsArray : null;
            }

            return iconSetArrays;
        }
    }
}