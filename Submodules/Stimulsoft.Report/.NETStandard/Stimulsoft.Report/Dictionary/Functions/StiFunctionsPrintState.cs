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

using System;

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunctionsPrintState
	{
        public static bool IsNull(object dataSource, string dataColumn)
        {
            if (!(dataSource is StiDataSource || dataSource is StiBusinessObject))
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source or name of Business Object.");

            if (dataColumn == null)
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");

            StiDataSource dataSourceObject = dataSource as StiDataSource;
            if (dataSourceObject != null)
            {
                object value = dataSourceObject[dataColumn];
                return value == null || value == DBNull.Value;
            }

            StiBusinessObject businessObject = dataSource as StiBusinessObject;
            object value2 = businessObject[dataColumn];
            return value2 == null || value2 == DBNull.Value;
        }

        public static bool NextIsNull(object dataSource, string dataColumn)
        {
            if (!(dataSource is StiDataSource || dataSource is StiBusinessObject))
                throw new ArgumentNullException(
                    "Function NextIsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source or name of Business Object.");

            if (dataColumn == null)
                throw new ArgumentNullException(
                    "Function NextIsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");

            object value = Next(dataSource, dataColumn);

            return value == null || value == DBNull.Value;
        }

        public static bool PreviousIsNull(object dataSource, string dataColumn)
        {
            if (!(dataSource is StiDataSource || dataSource is StiBusinessObject))
                throw new ArgumentNullException(
                    "Function PreviousIsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source or name of Business Object.");

            if (dataColumn == null)
                throw new ArgumentNullException(
                    "Function PreviousIsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");

            object value = Previous(dataSource, dataColumn);

            return value == null || value == DBNull.Value;
        }

        public static object Next(object dataSource, string dataColumn)
        {
            if (!(dataSource is StiDataSource || dataSource is StiBusinessObject))
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source or name of Business Object.");

            if (dataColumn == null)
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");

            StiDataSource dataSourceObject = dataSource as StiDataSource;
            StiBusinessObject businessObject = dataSource as StiBusinessObject;

            string guid = Guid.NewGuid().ToString();
            if (dataSourceObject != null)
                dataSourceObject.SaveState(guid);
            if (businessObject != null)
                businessObject.SaveState(guid);

            object value = null;

            try
            {
                if (dataSourceObject != null)
                {
                    dataSourceObject.Next();
                    if (dataSourceObject.IsEof) return null;

                    value = dataSourceObject[dataColumn];
                }
                if (businessObject != null)
                {
                    businessObject.Next();
                    if (businessObject.IsEof) return null;

                    value = businessObject[dataColumn];
                }
            }
            finally
            {
                if (dataSourceObject != null)
                    dataSourceObject.RestoreState(guid);
                if (businessObject != null)
                    businessObject.RestoreState(guid);
            }

            return value;
        }

        public static object Previous(object dataSource, string dataColumn)
        {
            if (!(dataSource is StiDataSource || dataSource is StiBusinessObject))
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source or name of Business Object.");

            if (dataColumn == null)
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");

            StiDataSource dataSourceObject = dataSource as StiDataSource;
            StiBusinessObject businessObject = dataSource as StiBusinessObject;

            string guid = Guid.NewGuid().ToString();
            if (dataSourceObject != null)
                dataSourceObject.SaveState(guid);
            if (businessObject != null)
                businessObject.SaveState(guid);

            object value = null;

            try
            {
                if (dataSourceObject != null)
                {
                    dataSourceObject.Prior();
                    if (dataSourceObject.IsBof) return null;

                    value = dataSourceObject[dataColumn];
                }
                if (businessObject != null)
                {
                    businessObject.Prior();
                    if (businessObject.IsBof) return null;

                    value = businessObject[dataColumn];
                }

            }
            finally
            {
                if (dataSourceObject != null)
                    dataSourceObject.RestoreState(guid);
                if (businessObject != null)
                    businessObject.RestoreState(guid);
            }

            return value;
        }
	}
}
