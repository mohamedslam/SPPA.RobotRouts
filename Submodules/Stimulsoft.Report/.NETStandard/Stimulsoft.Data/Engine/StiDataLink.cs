#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Design;
using System.ComponentModel;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    [TypeConverter(typeof(StiDataLinkConverter))]
    [StiSerializable]
    public class StiDataLink : IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty(nameof(Key), Key);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Key):
                        Key = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        public string ParentTable { get; set; }

        public string ChildTable { get; set; }

        public string[] ParentColumns { get; set; }

        public string[] ChildColumns { get; set; }

        public string[] ParentKeys
        {
            get
            {
                if (string.IsNullOrEmpty(ParentTable) || ParentColumns == null)
                    return null;

                return ParentColumns.ToList()
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => $"{ParentTable}.{c}")
                    .ToArray();
            }
        }

        public string[] ChildKeys
        {
            get
            {
                if (string.IsNullOrEmpty(ChildTable) || ChildColumns == null)
                    return null;

                return ChildColumns.ToList()
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => $"{ChildTable}.{c}")
                    .ToArray();
            }
        }
        
        /// <summary>
        /// Gets or sets the key to the data relation in the report dictionary.
        /// </summary>
        public string Key { get; set; }

        public bool Active { get; set; }
        #endregion

        #region Methods
        public static StiDataLink LoadFromJson(JObject json)
        {
            var link = new StiDataLink();
            link.LoadFromJsonObject(json);
            return link;
        }

        public override string ToString()
        {
            return $"{NullStr(ParentTable)}->{NullStr(ChildTable)} : {NullStr(ParentColumns)}->{NullStr(ChildColumns)}";
        }

        private string NullStr(string str)
        {
            return str ?? "";
        }

        private string NullStr(string[] strs)
        {
            if (strs == null || strs.Length == 0)
                return "";

            return string.Join(", ", strs.ToArray());
        }
        #endregion

        public StiDataLink()
        {
        }

        public StiDataLink(string key)
        {
            Key = key;
        }

        public StiDataLink(string parentTable, string childTable, string[] parentColumns, string[] childColumns, bool active)
        {
            ParentTable = parentTable;
            ChildTable = childTable;
            ParentColumns = parentColumns;
            ChildColumns = childColumns;
            Active = active;
        }

        public StiDataLink(string parentTable, string childTable, string[] parentColumns, string[] childColumns, bool active, string key) :
            this(parentTable, childTable, parentColumns, childColumns, active)
        {
            Key = key;
        }
    }
}