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
using System.Data;
using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Helpers
{
    public static class StiResourceArrayToDataSet
    {
        #region Methods
        public static DataSet Get(StiResourceType resourceType, byte[] array)
        {
            switch (resourceType)
            {
                case StiResourceType.Excel:
                    return StiExcelConnector.Get().GetDataSet(new StiExcelOptions(array));

                case StiResourceType.Csv:
                    return StiCsvConnector.Get().GetDataSet(new StiCsvOptions(array));

                case StiResourceType.Dbf:
                    return StiDBaseConnector.Get().GetDataSet(new StiDBaseOptions(array));

                case StiResourceType.Xml:
                    return StiXmlConnector.Get().GetDataSet(new StiXmlOptions(array));

                case StiResourceType.Json:
                    return StiJsonConnector.Get().GetDataSet(new StiJsonOptions(array));

                default:
                    throw new NotSupportedException($"'{resourceType}' resource type is not supported!");

            }
        }
        #endregion
    }
}
