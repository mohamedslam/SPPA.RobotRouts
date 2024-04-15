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

using Stimulsoft.Base.Meters;
using Stimulsoft.Base.Serializing;
using System.ComponentModel;

namespace Stimulsoft.Report.Dictionary
{
    public class StiMeasureTransformationMeter : 
        StiDataTransformationMeter,
        IStiMeasureMeter
    {
        #region Methods
        public override int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = Expression != null ? Expression.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();
                hashCode = (hashCode * 397) ^ (Label != null ? Label.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets unically object identificator.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }
        #endregion

        public StiMeasureTransformationMeter(string expression, string label, string key)
            : base(expression, label)
        {
            Key = key;
        }
    }
}
