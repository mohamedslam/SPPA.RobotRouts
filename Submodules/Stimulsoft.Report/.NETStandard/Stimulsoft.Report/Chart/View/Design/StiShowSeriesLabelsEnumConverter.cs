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

using Stimulsoft.Base.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart.Design
{
    /// <summary>
    /// Provides a type converter to convert Enum objects to and from various other representations.
    /// </summary>
    public class StiShowSeriesLabelsEnumConverter : StiEnumConverter
    {
        #region Methods
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.Values == null)
            {
                var values = Enum.GetValues(this.Type);

                var newValues = new List<StiShowSeriesLabels>();
                foreach (StiShowSeriesLabels obj in values)
                {
                    var type = obj.GetType();
                    var memInfo = type.GetMember(obj.ToString());
                    var attributes = memInfo[0].GetCustomAttributes(typeof (ObsoleteAttribute), false);
                    if (attributes.Length > 0)continue;
                    
                    newValues.Add(obj);
                }

                values = newValues.ToArray();

                var comparer = this.Comparer;
                if (comparer != null) 
                    Array.Sort(values, 0, values.Length, comparer);

                this.Values = new StandardValuesCollection(values);
            }
            return this.Values;
        }
        #endregion

        public StiShowSeriesLabelsEnumConverter(Type type)
        {
            this.Type = type;
        }

        public StiShowSeriesLabelsEnumConverter()
        {
        }
    }
}