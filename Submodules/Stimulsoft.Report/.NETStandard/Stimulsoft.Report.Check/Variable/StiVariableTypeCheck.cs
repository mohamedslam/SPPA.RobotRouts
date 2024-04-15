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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;
using System;

namespace Stimulsoft.Report.Check
{
    public class StiVariableTypeCheck : StiVariableCheck
    {
        #region Properties
        public override string ShortMessage => StiLocalizationExt.Get("CheckVariable", "StiVariableTypeCheckShort");

        public override string LongMessage => StiLocalizationExt.Get("CheckVariable", "StiVariableTypeCheckLong");

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        private Type GetSimplifiedType(Type type)
        {
            if (
                type == typeof(sbyte) ||
                type == typeof(byte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(sbyte?) ||
                type == typeof(byte?) ||
                type == typeof(short?) ||
                type == typeof(ushort?) ||
                type == typeof(int?) ||
                type == typeof(uint?) ||
                type == typeof(long?) ||
                type == typeof(ulong?) ||
                type == typeof(ByteList) ||
                type == typeof(ShortList) ||
                type == typeof(IntList) ||
                type == typeof(LongList))
            {
                type = typeof(long);
            }
            else if (
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(double?) ||
                type == typeof(float?) ||
                type == typeof(DoubleList) ||
                type == typeof(FloatList))
            {
                type = typeof(double);
            }
            else if (
                type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(DecimalList))
            {
                type = typeof(decimal);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(DateTimeList))
            {
                type = typeof(DateTime);
            }
            else if (type == typeof(bool) || type == typeof(bool?) || type == typeof(BoolList))
            {
                type = typeof(bool);
            }
            else if (type == typeof(char) || type == typeof(char?) || type == typeof(CharList))
            {
                type = typeof(char);
            }
            else if (type == typeof(string) || type == typeof(StringList))
            {
                type = typeof(string);
            }

            return type;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            var variable = obj as StiVariable;
            if (variable != null && variable.DialogInfo != null && variable.DialogInfo.BindingValue && 
                variable.DialogInfo.BindingVariable != null)
            {
                var column = StiDataColumn.GetDataColumnFromColumnName(report.Dictionary, variable.DialogInfo.KeysColumn);
                var columnBinding = StiDataColumn.GetDataColumnFromColumnName(report.Dictionary, variable.DialogInfo.BindingValuesColumn, true);
                if (column != null && columnBinding != null && 
                    !(GetSimplifiedType(column.Type).Equals(GetSimplifiedType(variable.Type)) &&
                    GetSimplifiedType(columnBinding.Type).Equals(GetSimplifiedType(variable.Type)) &&
                    GetSimplifiedType(variable.DialogInfo.BindingVariable.Type).Equals(GetSimplifiedType(variable.Type))))
                {
                    var check = new StiVariableTypeCheck();

                    check.Element = obj;
                    check.Actions.Add(new StiEditPropertyAction());

                    return check;
                }
            }

            return null;
        }
        #endregion
    }
}
