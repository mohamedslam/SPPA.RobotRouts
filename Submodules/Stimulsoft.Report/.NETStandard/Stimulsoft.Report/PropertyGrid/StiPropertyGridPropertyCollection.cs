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

namespace Stimulsoft.Report.PropertyGrid
{
    public sealed class StiPropertyGridPropertyCollection
    {
        #region Fields
        private int pos;
        private StiPropertyCategories[] categories;
        private StiPropertyObject[][] properties;
        #endregion

        #region Methods
        public void Add(StiPropertyCategories cat, StiPropertyObject[] objs)
        {
            categories[pos] = cat;
            properties[pos] = objs;
            pos++;
        }
        #endregion

        public StiPropertyGridPropertyCollection(int size)
        {
            categories = new StiPropertyCategories[size];
            properties = new StiPropertyObject[size][];
        }
    }
}