#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports	 	 										}
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

using Stimulsoft.Base.Design;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Dictionary
{
    public sealed class StiRestrictions : IStiDefault
    {
        #region Fields
        private Hashtable restrictionsDataSource;
        private Hashtable restrictionsDataRelation;
        private Hashtable restrictionsDataColumn;
        private Hashtable restrictionsDatabase;
        private Hashtable restrictionsVariable;
        private Hashtable restrictionsTotal;
        private Hashtable restrictionsBusinessObject;
        #endregion

        #region IStiDefault
        /// <summary>
        /// Gets true of this class has a default state.
        /// </summary>
        public bool IsDefault
        {
            get
            {
                return
                    restrictionsDataSource == null || restrictionsDataSource.Count == 0 &&
                    restrictionsDataRelation == null || restrictionsDataRelation.Count == 0 &&
                    restrictionsDataColumn == null || restrictionsDataColumn.Count == 0 &&
                    restrictionsDatabase == null || restrictionsDatabase.Count == 0 &&
                    restrictionsVariable == null || restrictionsVariable.Count == 0 &&
                    restrictionsTotal == null || restrictionsTotal.Count == 0 &&
                    restrictionsBusinessObject == null || restrictionsBusinessObject.Count == 0;
            }
        }
        #endregion

        #region Methods
        public void Clear()
        {
            if (restrictionsDataSource != null)
            {
                restrictionsDataSource.Clear();
                restrictionsDataSource = null;
            }

            if (restrictionsDataRelation != null)
            {
                restrictionsDataRelation.Clear();
                restrictionsDataRelation = null;
            }

            if (restrictionsDataColumn != null)
            {
                restrictionsDataColumn.Clear();
                restrictionsDataColumn = null;
            }

            if (restrictionsDatabase != null)
            {
                restrictionsDatabase.Clear();
                restrictionsDatabase = null;
            }

            if (restrictionsVariable != null)
            {
                restrictionsVariable.Clear();
                restrictionsVariable = null;
            }

            if (restrictionsTotal != null)
            {
                restrictionsTotal.Clear();
                restrictionsTotal = null;
            }

            if (restrictionsBusinessObject != null)
            {
                restrictionsBusinessObject.Clear();
                restrictionsBusinessObject = null;
            }
        }

        private Hashtable GetHashtable(StiDataType dataType, bool create)
        {
            switch (dataType)
            {
                case StiDataType.Database:
                    if (restrictionsDatabase == null && create)
                        restrictionsDatabase = new Hashtable();

                    return restrictionsDatabase;

                case StiDataType.DataColumn:
                    if (restrictionsDataColumn == null && create)
                        restrictionsDataColumn = new Hashtable();

                    return restrictionsDataColumn;

                case StiDataType.DataRelation:
                    if (restrictionsDataRelation == null && create)
                        restrictionsDataRelation = new Hashtable();

                    return restrictionsDataRelation;

                case StiDataType.DataSource:
                    if (restrictionsDataSource == null && create)
                        restrictionsDataSource = new Hashtable();

                    return restrictionsDataSource;

                case StiDataType.BusinessObject:
                    if (restrictionsBusinessObject == null && create)
                        restrictionsBusinessObject = new Hashtable();

                    return restrictionsBusinessObject;

                case StiDataType.Total:
                    if (restrictionsTotal == null && create)
                        restrictionsTotal = new Hashtable();

                    return restrictionsTotal;

                default:
                    if (restrictionsVariable == null && create)
                        restrictionsVariable = new Hashtable();

                    return restrictionsVariable;
            }
        }

        public void Add(string name, StiDataType dataType, StiRestrictionTypes type)
        {
            var hashtable = GetHashtable(dataType, true);
            var list = hashtable[name] as List<StiRestrictionTypes>;
            if (list == null)
            {
                list = new List<StiRestrictionTypes>();
                hashtable[name] = list;
            }
            list.Add(type);
        }


        public bool IsAllowEdit(string name, StiDataType dataType)
        {
            var restrictions = GetHashtable(dataType, false);
            if (restrictions == null || restrictions[name] == null) return true;

            var list = restrictions[name] as List<StiRestrictionTypes>;
            if (list != null)
            {
                foreach (var type in list)
                {
                    if ((type & StiRestrictionTypes.DenyEdit) != 0)
                        return false;
                }
            }
            return true;
        }

        public bool IsAllowDelete(string name, StiDataType dataType)
        {
            var restrictions = GetHashtable(dataType, false);
            if (restrictions == null || restrictions[name] == null) return true;

            var list = restrictions[name] as List<StiRestrictionTypes>;
            if (list != null)
            {
                foreach (var type in list)
                {
                    if ((type & StiRestrictionTypes.DenyDelete) != 0)
                        return false;
                }
            }
            return true;
        }

        public bool IsAllowShow(string name, StiDataType dataType)
        {
            var restrictions = GetHashtable(dataType, false);
            if (restrictions == null || restrictions[name] == null) return true;

            var list = restrictions[name] as List<StiRestrictionTypes>;
            if (list != null)
            {
                foreach (var type in list)
                {
                    if ((type & StiRestrictionTypes.DenyShow) != 0)
                        return false;
                }
            }
            return true;
        }

        public bool IsAllowMove(string name, StiDataType dataType)
        {
            var restrictions = GetHashtable(dataType, false);
            if (restrictions == null || restrictions[name] == null) return true;

            var list = restrictions[name] as List<StiRestrictionTypes>;
            if (list != null)
            {
                foreach (var type in list)
                {
                    if ((type & StiRestrictionTypes.DenyMove) != 0)
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}