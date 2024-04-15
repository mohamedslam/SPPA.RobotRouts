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

namespace Stimulsoft.Report.Dictionary
{
    public abstract class StiDataTransformationMeter
	{
        #region Methods
        public override string ToString()
        {
            var name = GetType().Name;
            return string.IsNullOrWhiteSpace(Expression)
                ? name
                : $"{name}-\"{Expression}\"";
        }

        public virtual int GetUniqueCode()
	    {
	        unchecked
	        {
	            var hashCode = Expression != null ? Expression.GetHashCode() : 0;
	            hashCode = (hashCode * 397) ^ GetType().GetHashCode();
	            hashCode = (hashCode * 397) ^ (Label != null ? Label.GetHashCode() : 0);
	            return hashCode;
	        }
        }
        #endregion

        #region Properties
        public string Expression { get; set; }

	    public string Label { get; set; }
        #endregion

        protected StiDataTransformationMeter(string expression, string label)
	    {
	        Expression = expression;
            Label = label;
	    }
    }
}
