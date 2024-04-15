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

using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System;

namespace Stimulsoft.Report.Engine
{
    public class StiSparklineV2Builder : StiComponentV2Builder
    {
        #region Methods
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            if (masterComp.Height > masterComp.Page.Height || masterComp.Height > masterComp.Parent.Height)
                masterComp.Height = Math.Min(masterComp.Page.Height, masterComp.Parent.Height);
        }

        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterSparkline = masterComp as StiSparkline;
            var childSparkline = masterSparkline.Clone() as StiSparkline;

            childSparkline.Values = GetValuesFromValueDataColumn(masterSparkline);

            return childSparkline;
        }

        internal static decimal[] GetValuesFromValueDataColumn(StiSparkline sparkline, int maxRows = 0)
        {
            if (string.IsNullOrWhiteSpace(sparkline.ValueDataColumn))
                return null;

            var variableValue = sparkline.Report[sparkline.ValueDataColumn];
            if (variableValue != null)
                return GetValuesFromValue(variableValue);

            var variable = sparkline.Report.Dictionary.Variables[sparkline.ValueDataColumn];
            variableValue = variable?.ValueObject;
            if (variableValue != null)            
                return GetValuesFromValue(variableValue);

            #region DataSource
            var dataSource = sparkline.GetDataSource();
            if (dataSource != null)
            {
                var dataColumn = StiDataColumn.GetDataColumnFromColumnName(sparkline.Report.Dictionary, sparkline.ValueDataColumn);
                if (dataColumn != null && dataColumn.IsArray())
                    return ListExt.ToDecimalArray(dataSource[dataColumn.Name]);

                dataSource.SaveState("SparklineRender_DataColumn");

                if (!string.IsNullOrWhiteSpace(sparkline.DataRelationName))
                    dataSource.SetDetails(sparkline.DataRelationName);

                var values = new decimal[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(sparkline.Report.Dictionary, sparkline.ValueDataColumn);
                    values[posIndex] = StiValueHelper.TryToDecimal(data);

                    if (maxRows > 0 && posIndex > maxRows) break;
                    dataSource.Next();
                }
                dataSource.RestoreState("SparklineRender_DataColumn");

                return values;
            }
            #endregion

            #region Business Object
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(sparkline.Report.Dictionary, sparkline.ValueDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("SparklineRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new decimal[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(sparkline.Report.Dictionary, sparkline.ValueDataColumn);
                    values[posIndex] = StiValueHelper.TryToDecimal(data);
                    if (maxRows > 0 && posIndex > maxRows) break;

                    businessObject.Next();
                }
                businessObject.RestoreState("SparklineRender_DataColumn");
                return values;
            }
            #endregion

            return null;
        }

        private static decimal[] GetValuesFromValue(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalArray(value);
            else
                return new decimal[] { StiValueHelper.TryToDecimal(value) };
        }
        #endregion
    }
}
