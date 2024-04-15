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

using System;
using System.Data;

namespace Stimulsoft.Base
{
    public class StiXmlConnector : StiFileDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.XmlDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.XmlDataSource;

        public override string Name => "XML";

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable => true;

        /// <summary>
        /// A type of the file which can be processed with this connection helper.
        /// </summary>
        public override StiFileType FileType => StiFileType.Xml;
        #endregion

        #region Methods
        /// <summary>
        /// Returns DataSet based on specified options.
        /// </summary>
        public override DataSet GetDataSet(StiFileDataOptions options)
        {
            var xmlOptions = options as StiXmlOptions;
            if (xmlOptions == null) 
                throw new NotSupportedException("Only StiXmlOptions accepted!");

            if (xmlOptions.Content == null || xmlOptions.Content.Length == 0)
                options.DataSet = null;
            //ADO.NET XML
            else if (xmlOptions.IsAdoNet)
            {
                var dataSet = new DataSet { EnforceConstraints = false };
                dataSet.Read(xmlOptions.Schema, xmlOptions.Content);
                options.DataSet = dataSet;
            }
            else
            {
                //XML
                options.DataSet = StiBaseOptions.DefaultJsonConverterVersion == StiJsonConverterVersion.ConverterV2 
                    ? StiJsonToDataSetConverterV2.GetDataSetFromXml(xmlOptions.Content) 
                    : StiJsonToDataSetConverter.GetDataSetFromXml(xmlOptions.Content);
            }

            return options.DataSet;
        }
        #endregion

        #region Methods.Static
        public static StiXmlConnector Get()
        {
            return new StiXmlConnector();
        }
        #endregion

        private StiXmlConnector()
        {
        }
    }
}