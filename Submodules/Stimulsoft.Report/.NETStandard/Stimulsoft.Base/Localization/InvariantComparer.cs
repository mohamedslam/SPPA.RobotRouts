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

using System.Collections;
using System.Globalization;

namespace Stimulsoft.Base.Localization
{
	internal class InvariantComparer : IComparer
	{
		#region IComparer
		public int Compare(object a, object b)
		{
			var strA = a as string;
			var strB = b as string;

		    return strA != null && strB != null 
		        ? compareInfo.Compare(strA, strB) 
		        : Comparer.Default.Compare(a, b);
		}
        #endregion

        #region Fields.Static
        internal static readonly InvariantComparer Default;
        #endregion

        #region Fields
        private CompareInfo compareInfo;
		#endregion

		internal InvariantComparer()
	    {
	        this.compareInfo = CultureInfo.InvariantCulture.CompareInfo;
	    }

        static InvariantComparer()
	    {
	        Default = new InvariantComparer();
	    }

    }
}
