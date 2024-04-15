#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Stimulsoft.Base
{
    public static class StiFriendlyData
    {
        #region Methods
        public static List<DataSet> GetFromFolder(string folder)
        {
            if (!Directory.Exists(folder)) 
                return null;

            var dataSets = new List<DataSet>();

            foreach (var fileXsd in Directory.EnumerateFiles(folder, "*.xsd"))
            {
                var dataSet = new DataSet(Path.GetFileNameWithoutExtension(fileXsd));
                dataSet.ReadXmlSchema(fileXsd);
                dataSets.Add(dataSet);
            }

            foreach (var fileXml in Directory.EnumerateFiles(folder, "*.xml"))
            {
                var fileName = Path.GetFileNameWithoutExtension(fileXml);
                var dataSet = dataSets.FirstOrDefault(d => d.DataSetName.ToLowerInvariant() == fileName.ToLowerInvariant());

                if (dataSet == null)
                {
                    dataSet = new DataSet(fileName);
                    dataSets.Add(dataSet);
                }
                dataSet.ReadXml(fileXml);
            }
            return dataSets;
        }
        #endregion
    }
}
