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
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Events;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.PropertyGrid;
using System.Threading.Tasks;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// 
    /// </summary>
    [StiServiceBitmap(typeof(StiDatabase), "Stimulsoft.Report.Bmp.Dictionary.Database.bmp")]
    [StiServiceCategoryBitmap(typeof(StiDatabase), "Stimulsoft.Report.Bmp.Dictionary.Database.bmp")]
    public abstract class StiDatabase :
        StiService,
        IStiInherited,
        IStiPropertyGridObject,
        IStiJsonReportObject,
        IStiAppConnection,
        IStiName,
        IStiAlias
    {
        #region enum Order
        public enum Order
        {
            Name = 100,
            Alias = 200,
            ConnectionString = 300,
            FirstRowIsHeader = 350,
            PathSchema = 400,
            PathData = 500,
            XmlType = 600,
            PromptUserNameAndPassword = 700,
            SaveDataInReportResources = 800
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiDatabase;

        [Browsable(false)]
        public string PropName => Name;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }

        /// <summary>
        /// Return events collection of this component.
        /// </summary>
        public virtual StiEventsCollection GetEvents()
        {
            var events = new StiEventsCollection();

            if (ConnectingEvent != null)
                events.Add(ConnectingEvent);

            if (ConnectedEvent != null)
                events.Add(ConnectedEvent);

            if (DisconnectingEvent != null)
                events.Add(DisconnectingEvent);

            if (DisconnectedEvent != null)
                events.Add(DisconnectedEvent);

            return events;
        }
        #endregion

        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiDatabase
            jObject.AddPropertyBool("Inherited", Inherited);
            jObject.AddPropertyJObject("ConnectingEvent", ConnectingEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ConnectedEvent", ConnectedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("DisconnectingEvent", DisconnectingEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("DisconnectedEvent", DisconnectedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Inherited":
                        this.Inherited = property.DeserializeBool();
                        break;

                    case "ConnectingEvent":
                        this.ConnectingEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ConnectedEvent":
                        this.ConnectedEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "DisconnectingEvent":
                        this.DisconnectingEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "DisconnectedEvent":
                        this.DisconnectedEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Name":
                        this.Name = property.DeserializeString();
                        break;

                    case "Alias":
                        this.Alias = property.DeserializeString();
                        break;

                    case "Key":
                        this.Key = property.DeserializeString();
                        break;
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        [StiSerializable]
        public bool Inherited { get; set; }
        #endregion

        #region IStiAppConnection
        string IStiAppConnection.GetName()
        {
            return Name;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            Key = StiKeyHelper.GetOrGeneratedKey(Key);

            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }
        #endregion

        #region IStiName
        /// <summary>
        /// Gets or sets a name of the database.
        /// </summary>
        [StiSerializable]
        [StiOrder((int)Order.Name)]
        [Description("Gets or sets a name of the database.")]
        public string Name { get; set; }
        #endregion

        #region IStiAlias
        /// <summary>
        /// Gets or sets an alias of the database.
        /// </summary>
        [StiSerializable]
        [StiOrder((int)Order.Alias)]
        [Description("Gets or sets an alias of the database.")]
        public string Alias { get; set; }
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service category.
        /// </summary>
        public sealed override string ServiceCategory => Loc.Get("PropertyMain", "CategoryConnections");

        /// <summary>
        /// Gets a service type.
        /// </summary>
        public sealed override Type ServiceType => typeof(StiDatabase);

        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => CreateConnector().Name;
        #endregion

        #region Events
        #region Connecting
        /// <summary>
        /// Occurs when connection is activating.
        /// </summary>
        public event EventHandler Connecting;

        /// <summary>
        /// Raises the Connecting event for this connection.
        /// </summary>
        protected virtual void OnConnecting(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the Connecting event for this connection.
        /// </summary>
        public void InvokeConnecting()
        {
            OnConnecting(EventArgs.Empty);
            Connecting?.Invoke(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(null, this, ConnectingEvent);
        }

        /// <summary>
        /// Occurs when connection is activating.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Browsable(false)]
        [Description("Occurs when connection is activating.")]
        public StiConnectingEvent ConnectingEvent { get; set; } = new StiConnectingEvent();
        #endregion

        #region Connected
        /// <summary>
        /// Occurs when connection is activated.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Raises the Connected event for this component.
        /// </summary>
        protected virtual void OnConnected(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the Connected event for this connection.
        /// </summary>
        public void InvokeConnected()
        {
            OnConnected(EventArgs.Empty);
            Connected?.Invoke(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(null, this, ConnectedEvent);
        }

        /// <summary>
        /// Occurs when connection is activated.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Browsable(false)]
        [Description("Occurs when connection is activated.")]
        public StiConnectedEvent ConnectedEvent { get; set; } = new StiConnectedEvent();
        #endregion

        #region Disconnecting
        /// <summary>
        /// Occurs when connection is deactivating.
        /// </summary>
        public event EventHandler Disconnecting;

        /// <summary>
        /// Raises the Disconnecting event for this connection.
        /// </summary>
        protected virtual void OnDisconnecting(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the Disconnecting event for this connection.
        /// </summary>
        public void InvokeDisconnecting()
        {
            OnDisconnecting(EventArgs.Empty);
            Disconnecting?.Invoke(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(null, this, DisconnectingEvent);
        }

        /// <summary>
        /// Occurs when connection is deactivating.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Browsable(false)]
        [Description("Occurs when connection is deactivating.")]
        public StiDisconnectingEvent DisconnectingEvent { get; set; } = new StiDisconnectingEvent();
        #endregion

        #region Disconnected
        /// <summary>
        /// Occurs when connection is deactivated.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Raises the Disconnected event for this connection.
        /// </summary>
        protected virtual void OnDisconnected(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the Disconnected event for this connection.
        /// </summary>
        public void InvokeDisconnected()
        {
            OnDisconnected(EventArgs.Empty);
            Disconnected?.Invoke(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(null, this, DisconnectedEvent);
        }

        /// <summary>
        /// Occurs when connection is deactivated.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Browsable(false)]
        [Description("Occurs when connection is deactivated.")]
        public StiDisconnectedEvent DisconnectedEvent { get; set; } = new StiDisconnectedEvent();
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }

        /// <summary>
        /// Gets a connection type.
        /// </summary>
        [Browsable(false)]
        public virtual StiConnectionType ConnectionType => StiConnectionType.Other;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        [Browsable(false)]
        public virtual Stimulsoft.Base.StiConnectionOrder ConnectionOrder
        {
            get
            {
                return CreateConnector().ConnectionOrder;
            }
        }

        /// <summary>
        /// Allows exceptions from the retrieving schema methods and data.
        /// </summary>
        [Browsable(false)]
        public bool AllowException { get; set; }
        #endregion

        #region Methods
        public abstract StiDatabase CreateNew();

        /// <summary>
        /// Retrieve database schema and create associated data sources in the report dictionary for this database.
        /// </summary>
        public void Synchronize(StiReport report)
        {
            ApplyDatabaseInformation(GetDatabaseInformation(report), report);
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public abstract StiDataConnector CreateConnector(string connectionString = null);

        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public abstract void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report);

        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public abstract void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll);

        /// <summary>
        /// Returns full database information.
        /// </summary>
        public virtual StiDatabaseInformation GetDatabaseInformation(StiReport report)
        {
            return null;
        }

        /// <summary>
        /// Returns full database information.
        /// </summary>
        public virtual StiDatabaseInformation GetDatabaseInformation()
        {
            return GetDatabaseInformation(null);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool onlyAlias)
        {
            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias))
                return $"{Alias} [{ServiceName}]";

            if (Name == Alias || string.IsNullOrWhiteSpace(Alias))
                return $"{Name} [{ServiceName}]";

            return $"{Name} [{Alias}]";
        }

        /// <summary>
        /// Calls the form for database edition.
        /// </summary>
        /// <param name="newDatabase"></param>
        /// <returns>Result of editing.</returns>
        public abstract DialogResult Edit(StiDictionary dictionary, bool newDatabase);

        /// <summary>
        /// Calls the form for database edition.
        /// </summary>
        /// <param name="newDatabase"></param>
        /// <returns>Result of editing.</returns>
        public abstract Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase);

        /// <summary>
        /// Registers the database in dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary in which is registered database.</param>
        /// <param name="loadData">Load the data or no.</param>
        public abstract void RegData(StiDictionary dictionary, bool loadData);
        #endregion

        /// <summary>
        /// Creates a new object of the type StiDatabase.
        /// </summary>
        public StiDatabase() : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDatabase.
        /// </summary>
        /// <param name="name">Name of database.</param>
        public StiDatabase(string name) : this(name, name)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDatabase.
        /// </summary>
        /// <param name="name">Name of database.</param>
        /// <param name="alias">Alias of database.</param>
        public StiDatabase(string name, string alias)
        {
            this.Name = name;
            this.Alias = alias;
        }

        /// <summary>
        /// Creates a new object of the type StiDatabase.
        /// </summary>
        /// <param name="name">Name of database.</param>
        /// <param name="alias">Alias of database.</param>
        /// <param name="key">Key string.</param>
        public StiDatabase(string name, string alias, string key) : this(name, alias)
        {
            this.Key = key;
        }
    }
}
