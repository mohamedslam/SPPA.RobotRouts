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

namespace Stimulsoft.Base
{
    public abstract class StiDataConnector
    {
        #region Properties.Static
        /// <summary>
        /// If true then database information will be required from database with help of DataAdapter (instead Connection.GetSchema).
        /// </summary>
        public static bool AdvancedRetrievalModeOfDatabaseSchema { get; set; }

        /// <summary>
        /// If true then in GetSchema method, column reference by index else by name.
        /// </summary>
        public static bool GetSchemaColumnsMode { get; set; } = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a type of the connector.
        /// </summary>
        public abstract StiConnectionIdent ConnectionIdent { get; }

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public abstract StiConnectionOrder ConnectionOrder { get; }

        /// <summary>
        /// The name of this connector.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public virtual string[] NuGetPackages => null;

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public virtual string NuGetVersion => null;

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public abstract bool IsAvailable { get; }
        #endregion

        #region Methods
        public virtual void ResetSettings()
        {
        }

        /// <summary>
        /// Return an array of the data connectors which can be used also to access data for this type of the connector.
        /// </summary>
        public virtual StiDataConnector[] GetFamilyConnectors()
        {
            return new[] { this };
        }
        #endregion
    }
}
