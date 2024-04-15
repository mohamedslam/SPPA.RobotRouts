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
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Base;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Design;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Class which contains schema information about business object which registered in report dictionary.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiBusinessObjectConverter))]
    public class StiBusinessObject : 
        ICloneable,
        IStiStateSaveRestore,
        IStiEnumerator,
        IStiInherited,
        IStiPropertyGridObject, 
        IStiJsonReportObject,
        IStiName,
        IStiAlias
    {
        #region enum Order
        public enum Order
        {
            Name = 100,
            Alias = 200,
            Category = 300,
            Columns = 400
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // StiBusinessObject
            jObject.AddPropertyBool("Inherited", Inherited);
            jObject.AddPropertyJObject("BusinessObjects", BusinessObjects.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Columns", Columns.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("Guid", Guid);
            jObject.AddPropertyStringNullOrEmpty("Category", Category);
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Inherited":
                        this.Inherited = property.DeserializeBool();
                        break;

                    case "BusinessObjects":
                        this.BusinessObjects.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Columns":
                        this.Columns.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Guid":
                        this.Guid = property.DeserializeString();
                        break;

                    case "Category":
                        this.Category = property.DeserializeString();
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
        #endregion

        #region IStiPropertyGridObject
		[Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiBusinessObject;

        [Browsable(false)]
        public string PropName => this.Name;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.Category()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion
       
        #region IStiStateSaveRestore
        private StiStatesManager states;
        /// <summary>
        /// Gets the component states manager.
        /// </summary>
        protected StiStatesManager States
        {
            get
            {
                return states ?? (states = new StiStatesManager());
            }
        }

        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public virtual void SaveState(string stateName)
        {
            States.PushInt(stateName, this, "positionValue", positionValue);
            States.PushBool(stateName, this, "isEofValue", IsEof);
            States.PushBool(stateName, this, "isBofValue", IsBof);
            States.PushBool(stateName, this, "isEmpty", IsEmpty);

            States.Push(stateName, this, "current", currentObject);
            States.Push(stateName, this, "businessObjectValue", businessObjectValue);
            States.Push(stateName, this, "specPrevValue", specPrevValue);
            States.Push(stateName, this, "specNextValue", specNextValue);
            States.Push(stateName, this, "specStoredCurrentValue", specStoredCurrentValue);
            States.Push(stateName, this, "enumerator", enumerator);
            States.Push(stateName, this, "FilteredCount", FilteredCount);

            States.PushBool(stateName, this, "isEnumeratorCreated", isEnumeratorCreated);
            States.PushBool(stateName, this, "specNextValueRead", specNextValueRead);
            States.PushBool(stateName, this, "specMoveNextResult", specMoveNextResult);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            if (!States.IsExist(stateName, this)) return;

            positionValue = States.PopInt(stateName, this, "positionValue");
            IsBof = States.PopBool(stateName, this, "isBofValue");
            IsEof = States.PopBool(stateName, this, "isEofValue");
            IsEmpty = States.PopBool(stateName, this, "isEmpty");

            currentObject = States.Pop(stateName, this, "current");
            businessObjectValue = States.Pop(stateName, this, "businessObjectValue");
            specPrevValue = States.Pop(stateName, this, "specPrevValue");
            specNextValue = States.Pop(stateName, this, "specNextValue");
            specStoredCurrentValue = States.Pop(stateName, this, "specStoredCurrentValue");
            enumerator = States.Pop(stateName, this, "enumerator") as IEnumerator;
            FilteredCount = States.Pop(stateName, this, "FilteredCount") as int?;

            isEnumeratorCreated = States.PopBool(stateName, this, "isEnumeratorCreated");
            specNextValueRead = States.PopBool(stateName, this, "specNextValueRead");
            specMoveNextResult = States.PopBool(stateName, this, "specMoveNextResult");
        }

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            states = null;
        }
        #endregion

        #region IStiInherited
        [Browsable(false)]
        [DefaultValue(false)]
        [StiSerializable]
        public bool Inherited { get; set; }
        #endregion

        #region IStiEnumerator
        protected int positionValue;
        /// <summary>
        /// Gets the current position.
        /// </summary>
        [Browsable(false)]
        public virtual int Position
        {
            get
            {
                return positionValue;
            }
            set
            {
                if (value == positionValue) return;

                this.First();
                var count = value;
                while (count > 0)
                {
                    count--;
                    this.Next();
                }
            }
        }

        /// <summary>
        /// Gets count of elements.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                CheckEnumerator();

                if (FilteredCount != null)
                    return FilteredCount.Value;

                #region ICollection
                if (BusinessObjectValue is ICollection)
                    return ((ICollection)BusinessObjectValue).Count;
                #endregion

                #region IListSource
                if (BusinessObjectValue is IListSource)
                {
                    var listSource = BusinessObjectValue as IListSource;
                    var list = listSource.GetList();
                    if (list != null)
                        return list.Count;
                }
                #endregion

                #region Array
                if (BusinessObjectValue is Array)
                {
                    var array = BusinessObjectValue as Array;
                    return array.Length;
                }
                #endregion

                #region Calculate count of elements in enumerator 
                try
                {
                    this.SaveState("CalculateCount");
                    var count = 0;
                    
                    //EnumeratorReset();
                    CreateEnumerator();

                    if (enumerator != null)
                    {
                        if (!IsEof) count++;

                        while (enumerator.MoveNext())
                            count++;
                    }

                    return count;
                }
                finally
                {
                    this.RestoreState("CalculateCount");
                }
                #endregion
            }
        }

        /// <summary>
        /// Gets value indicates that this position specifies to the beginning of data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsBof { get; set; }

        /// <summary>
        /// Gets value indicates that this position specifies to the data end.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsEof { get; set; }

        /// <summary>
        /// Gets value indicates that no data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsEmpty { get; private set; }

        private void EnumeratorReset()
        {
            if (!StiOptions.Engine.AllowUseResetMethodInBusinessObject) return;

            if (enumerator != null && enumerator.GetType().GetMethod("Reset") != null)
            {
                try
                {
                    enumerator.Reset();
                }
                catch
                {
                    //Try to create new enumerator
                    this.enumerator = StiBusinessObjectHelper.GetEnumeratorFromObject(this.BusinessObjectValue);
                    FilteredCount = null;
                }
            }
            else
            {
                if (!previousResetException)
                {
                    try
                    {
                        enumerator.Reset();
                    }
                    catch
                    {
                        previousResetException = true;
                    }
                }

                if (previousResetException)
                {
                    //Try to create new enumerator
                    this.enumerator = StiBusinessObjectHelper.GetEnumeratorFromObject(this.BusinessObjectValue);
                    FilteredCount = null;
                }
            }
        }

        /// <summary>
        /// Sets a position at the beginning.
        /// </summary>
        public virtual void First()
        {
            specNextValue = null;
            specNextValueRead = false;
            specPrevValue = null;

            positionValue = 0;
            IsEof = false;
            IsBof = true;

            if (enumerator != null)
            {
                EnumeratorReset();
                var value = enumerator.MoveNext();
                if (!value)
                {
                    IsEmpty = true;
                    IsEof = true;
                    currentObject = null;
                    specPrevValue = null;
                }
                else
                {
                    currentObject = enumerator.Current;
                    specPrevValue = null;
                    IsEmpty = false;
                }
            }
            else
            {
                currentObject = null;
                specPrevValue = null;
                IsEmpty = true;
                IsEof = true;
            }
        }

        /// <summary>
        /// Sets a position on the previous element.
        /// </summary>
        public virtual void Prior()
        {
            throw new Exception("StiBusinessObject does not support IStiEnumerator.Prior");
        }

        /// <summary>
        /// Sets a position on the next element.
        /// </summary>
        public virtual void Next()
        {
            if (enumerator == null) return;
            
            #region Если ранее произведено упреждающее чтение данных
            if (specNextValueRead)
            {
                var value = specMoveNextResult;

                if (!value)
                {
                    IsEof = true;
                    currentObject = null;
                    specPrevValue = null;
                }
                else
                {
                    this.specPrevValue = this.Current;
                    currentObject = specNextValue;
                    IsBof = false;
                    IsEof = false;

                    if (!IsEof)
                        positionValue++;
                }
                specNextValueRead = false;
                specNextValue = null;
            }
            #endregion

            #region Чтение без упреждения
            else
            {
                specNextValue = null;
                specNextValueRead = false;

                var value = enumerator.MoveNext();

                if (!value)
                {
                    IsEof = true;
                    specPrevValue = currentObject;  //fix
                    currentObject = null;
                }
                else
                {
                    specPrevValue = currentObject;
                    currentObject = enumerator.Current;
                    IsBof = false;
                    IsEof = false;

                    if (!IsEof)
                        positionValue++;
                }
            }
            #endregion
        }

        /// <summary>
        /// Sets a position on the last element.
        /// </summary>
        public virtual void Last()
        {
            //simple
            while (!IsEof)
            {
                Next();
            }
        }
        #endregion

        #region IClone
        public object Clone()
        {
            var clonedBusinessObject = (StiBusinessObject)this.MemberwiseClone();
            clonedBusinessObject.Columns = new StiDataColumnsCollection(clonedBusinessObject);
            clonedBusinessObject.BusinessObjects = new StiBusinessObjectsCollection(this.dictionary, clonedBusinessObject);

            foreach (StiDataColumn column in this.Columns)
            {
                var clonedColumn = (StiDataColumn)column.Clone();
                clonedColumn.BusinessObject = clonedBusinessObject;
                clonedBusinessObject.Columns.Add(clonedColumn);
            }

            foreach (StiBusinessObject obj in this.BusinessObjects)
            {
                var clonedObj = (StiBusinessObject)obj.Clone();
                clonedObj.ParentBusinessObject = clonedBusinessObject;
                clonedBusinessObject.BusinessObjects.Add(clonedObj);
            }

            return clonedBusinessObject;
        }
        #endregion

        #region IStiName
        /// <summary>
        /// Gets or sets a name of the business object.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [StiOrder((int)Order.Name)]
        [Description("Gets or sets a name of the business object.")]
        public string Name { get; set; }
        #endregion

        #region IStiAlias
        /// <summary>
        /// Gets or sets an alias of the business object.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [StiOrder((int)Order.Alias)]
        [Description("Gets or sets an alias of the business object.")]
        public string Alias { get; set; }
        #endregion

        #region Properties
        //value, calculated in CreateEnumerator.FilterData method
        internal int? FilteredCount { get; set; }

        internal object currentObject;
        [Browsable(false)]
        public object Current
        {
            get
            {
                CheckEnumerator();

                if (currentObject == null && IsEof)
                    return specPrevValue;

                return currentObject;
            }
        }

        private StiReport Report => Dictionary?.Report;

        /// <summary>
        /// Gets or sets collection of the business objects.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public StiBusinessObjectsCollection BusinessObjects { get; set; }

        /// <summary>
        /// Gets or sets a column collection of the business object.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Data")]
        [Description("Gets or sets a column collection of the business object.")]
        [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiDataColumnsCollectionConverter))]
        [StiOrder((int)Order.Columns)]
        public virtual StiDataColumnsCollection Columns { get; set; }

        /// <summary>
        /// Gets or sets guid of business object.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets a category name of the business object.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [StiOrder((int)Order.Category)]
        [Description("Gets or sets a category name of the business object.")]
        public string Category { get; set; }

        private object businessObjectValue;
        /// <summary>
        /// Gets or sets business object.
        /// </summary>
        [Browsable(false)]
        public object BusinessObjectValue
        {
            get
            {
                if (ParentBusinessObject != null && ParentBusinessObject.specTotalsCalculation)
                    businessObjectValue = ParentBusinessObject[this.Name];

                return businessObjectValue;
            }
            set
            {
                businessObjectValue = value;
            }
        }

        private StiDictionary dictionary;
        /// <summary>
        /// Gets or sets the dictionary in which this Business Object is located.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public StiDictionary Dictionary
        {
            get
            {
                return dictionary;
            }
            set
            {
                dictionary = value;
                this.BusinessObjects.dictionary = this.dictionary;

                if (value != null)
                {
                    foreach (StiBusinessObject businessObject in this.BusinessObjects)
                    {
                        businessObject.Dictionary = this.dictionary;
                    }
                }
            }
        }

        [Browsable(false)]
        public StiBusinessObject ParentBusinessObject { get; set; }

        [Browsable(false)]
        public StiDataBand OwnerBand { get; set; }

        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }
        #endregion

        #region Static properties
        /// <summary>
        /// Дополнительный список имён полей для метода StiBusinessObjectHelper.GetValueFromClass().
        /// Если поле в этом списке - то при получении значения этого поля null передаётся как есть.
        /// Для остальных полей вместо null передаётся тип этого поля, это надо для работы с анонимными типами.
        /// </summary>
        [Browsable(false)]
        public static Hashtable FieldsIgnoreList { get; set; } = new Hashtable();
        #endregion

        #region Methods
        /// <summary>
        /// Returns level of data in hierarchical band.
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            if (RowToLevel == null) return 0;

            var value = RowToLevel[this.Current];
            if (value is int) return (int)value;
            return 0;
        }

        private void CheckEnumerator()
        {
            if (!isEnumeratorCreated)
                SetDetails();
        }

        public void SetPrevValue()
        {
            this.specStoredCurrentValue = this.Current;
            this.currentObject = specPrevValue;
            this.specSetPrevValue = true;
        }

        public void SetNextValue()
        {
            this.specStoredCurrentValue = this.Current;
            if (!this.specNextValueRead)
            {
                this.specMoveNextResult = enumerator.MoveNext();
                if (this.specMoveNextResult)
                {
                    this.specNextValue = enumerator.Current;
                    this.currentObject = this.specNextValue;
                }
                else
                {
                    this.specNextValue = null;
                    this.currentObject = null;
                }

                this.specNextValueRead = true;
            }
            else
                this.currentObject = this.specNextValue;
            
            this.specSetNextValue = true;
        }

        public void RestoreCurrentValue()
        {
            this.currentObject = this.specStoredCurrentValue;
            this.specSetPrevValue = false;
            this.specSetNextValue = false;
        }

        public StiBusinessObject GetTopParentBusinessObject()
        {
            var businessObject = this;
            while (businessObject.ParentBusinessObject != null)
            {
                businessObject = businessObject.ParentBusinessObject;
            }
            return businessObject;
        }

        public void CreateEnumerator()
        {
            specSetPrevValue = false;
            specSetNextValue = false;
            FilteredCount = null;
            if (this.ParentBusinessObject == null)
            {
                this.BusinessObjectValue = this.GetBusinessObjectData();
                this.enumerator = StiBusinessObjectHelper.GetEnumeratorFromObject(this.BusinessObjectValue);
                
                if (this.enumerator != null)
                {
                    if (OwnerBand != null)
                    {
                        var list = new ArrayList();
                        try
                        {
                            enumerator.Reset();
                        }
                        catch
                        {
                        }

                        while (enumerator.MoveNext())
                        {
                            list.Add(enumerator.Current);
                        }
                        this.enumerator = list.GetEnumerator();
                    }                                
                }
            }
            else
            {
                var value = this.ParentBusinessObject.Current;
                if (value == null)
                {
                    First();    //for set IsBof,IsEof
                    return;
                }
                this.BusinessObjectValue = StiBusinessObjectHelper.GetValueFromObject(value, this.Name);
               
                this.enumerator = StiBusinessObjectHelper.GetEnumeratorFromObject(this.BusinessObjectValue);
            }

            #region Apply Filters and Sortings
            if (OwnerBand != null && OwnerBand.Report.EngineVersion == StiEngineVersion.EngineV2)
            {
                OwnerBand.DataBandInfoV2.GroupHeaderCachedResults = null;
                OwnerBand.DataBandInfoV2.GroupFooterCachedResults = null;
            }   
            if (this.enumerator != null)
            {
                FilterData();
                if (OwnerBand is StiHierarchicalBand)
                {
                    var hierarchicalSort =
                        new StiHierarchicalBusinessObjectSort(this, OwnerBand as StiHierarchicalBand, OwnerBand.Sort);
                    hierarchicalSort.Process();
                }
                else
                    SortData();
            }
            #endregion

            First();
            isEnumeratorCreated = true;

            SortDataByGroups();
        }

        private void SortData()
        {
            if (OwnerBand == null || OwnerBand.Sort == null || OwnerBand.Sort.Length <= 0 || this.enumerator == null) return;

            var listOfDetailRows = new ArrayList();
            try
            {
                enumerator.Reset();
            }
            catch
            {
            }

            while (enumerator.MoveNext())
            {
                listOfDetailRows.Add(enumerator.Current);
            }

            if (OwnerBand is StiHierarchicalBand)
            {
                var dataSort = new StiHierarchicalBusinessObjectSort(this, OwnerBand as StiHierarchicalBand, OwnerBand.Sort);
                listOfDetailRows.Sort(dataSort);
            }
            else
            {
                var dataSort = new StiBusinessObjectSort(OwnerBand.Sort, null, null);
                listOfDetailRows.Sort(dataSort);
            }

            this.enumerator = listOfDetailRows.GetEnumerator();
        }

        private void SortDataByGroups()
        {
            if (OwnerBand == null || this.enumerator == null) return;
            if (!StiDataHelper.NeedGroupSort(OwnerBand)) return;

            #region Prepare group conditions
            var groupHeaderComponents = OwnerBand.Report.EngineVersion == StiEngineVersion.EngineV1
                ? OwnerBand.DataBandInfoV1.GroupHeaderComponents
                : OwnerBand.DataBandInfoV2.GroupHeaders;

            var groupCount = 0;
            foreach (StiGroupHeaderBand groupHeader in groupHeaderComponents)
            {
                if (groupHeader.SortDirection != StiGroupSortDirection.None)
                    groupCount++;
            }

            var listOfDetailRows = new ArrayList();
            try
            {
                enumerator.Reset();
            }
            catch
            {
            }
            var counter = 0;
            while (enumerator.MoveNext())
            {
                counter++;
            }

            var conditions = new object[counter, groupCount + 1, 2];
            var rowToConditions = new Hashtable();
            var index = 0;
            var storeCurrentObject = currentObject;
            specSortGroup = true;

            try
            {
                enumerator.Reset();
            }
            catch
            {
            }

            while (enumerator.MoveNext())
            {
                var currentRow = enumerator.Current;
                listOfDetailRows.Add(currentRow);
                rowToConditions[currentRow] = index;
                currentObject = currentRow;

                var groupIndex = 0;
                foreach (StiGroupHeaderBand groupHeader in groupHeaderComponents)
                {
                    if (groupHeader.SortDirection == StiGroupSortDirection.None) continue;

                    if (OwnerBand.Report.EngineVersion == StiEngineVersion.EngineV1)
                        conditions[index, groupIndex, 0] = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(groupHeader);
                    else
                        conditions[index, groupIndex, 0] = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(groupHeader);

                    conditions[index, groupIndex, 1] = groupHeader.SortDirection;
                    groupIndex++;
                }

                conditions[index, groupCount, 0] = index;
                conditions[index, groupCount, 1] = StiGroupSortDirection.Ascending;

                index++;
            }
            specSortGroup = false;
            currentObject = storeCurrentObject;
            #endregion

            var dataSort = new StiBusinessObjectSort(OwnerBand.Sort, rowToConditions, conditions);
            listOfDetailRows.Sort(dataSort);

            this.enumerator = listOfDetailRows.GetEnumerator();
            First();
        }

        public void FilterData()
        {
            if (this.enumerator == null) return;

            var filter = StiDataHelper.GetFilterEventHandler(OwnerBand, this);
            if (OwnerBand == null || filter == null) return;

            isEnumeratorCreated = true;

            var resPos = Position;
            var resLine = Dictionary.Report.Line;
            var listOfDetailRows = new ArrayList();

            specFilterData = true;

            if (filter is StiFilterEventHandler)
            {
                var pos = 0;
                try
                {
                    enumerator.Reset();
                }
                catch
                {
                }

                while (enumerator.MoveNext())
                {
                    Position = pos;
                    if (pos == 0 && positionValue == 0) currentObject = enumerator.Current;
                    Dictionary.Report.Line = pos + 1;

                    try
                    {
                        var filterMethod = (StiFilterEventHandler)filter;
                        var args = new StiFilterEventArgs();
                        filterMethod(this, args);

                        if (args.Value)
                            listOfDetailRows.Add(enumerator.Current);
                    }
                    catch
                    {
                    }
                    pos++;
                }
                this.enumerator = listOfDetailRows.GetEnumerator();
                FilteredCount = listOfDetailRows.Count;
            }
            else if (filter is StiParser.StiFilterParserData)
            {
                var data = (StiParser.StiFilterParserData)filter;

                var pos = 0;
                try
                {
                    enumerator.Reset();
                }
                catch
                {
                }

                while (enumerator.MoveNext())
                {
                    Position = pos;
                    if (pos == 0 && positionValue == 0) currentObject = enumerator.Current;
                    Dictionary.Report.Line = pos + 1;

                    try
                    {
                        var result = StiParser.ParseTextValue(data.Expression, data.Component);
                        if (result is bool && (bool) result)
                            listOfDetailRows.Add(enumerator.Current);
                    }
                    catch
                    {
                    }
                    pos++;
                }
                this.enumerator = listOfDetailRows.GetEnumerator();
                FilteredCount = listOfDetailRows.Count;
            }

            specFilterData = false;

            Position = resPos;
            Dictionary.Report.Line = resLine;

            isEnumeratorCreated = false;
        }

        private void DestroyEnumerator()
        {
            isEnumeratorCreated = false;
            FilteredCount = null;
        }

        /// <summary>
		/// Set the details data for Business Objects.
		/// </summary>
        public void SetDetails()
        {
            UpdateChilds();
            CreateEnumerator();
        }

        private void UpdateChilds()
        {
            isEnumeratorCreated = false;
            foreach (StiBusinessObject child in BusinessObjects)
            {
                child.UpdateChilds();
            }
        }

        private object GetBusinessObjectDataFromParent(StiBusinessObject businessObject)
        {
            foreach (var businessObjectData in businessObject.dictionary.Report.BusinessObjectsStore)
            {
                if (businessObjectData.Name == businessObject.Name)
                    return businessObjectData.BusinessObjectValue;
            }
            return null;
        }

        /// <summary>
        /// Returns the index of a column in the data source.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Index.</returns>
        public int GetColumnIndex(string columnName)
        {
            var index = 0;
            foreach (StiDataColumn column in this.Columns)
            {
                if (column.NameInSource == columnName)
                    return index;

                index++;
            }
            index = 0;

            foreach (StiDataColumn column in this.Columns)
            {
                if (column.Name == columnName || column.Alias == columnName)
                    return index;

                index++;
            }
            return -1;
        }

        public object GetBusinessObjectData()
        {
            return GetBusinessObjectData(false);
        }

        public object GetBusinessObjectData(bool isColumnsRetrieve)
        {
            if (this.ParentBusinessObject == null)
                return GetBusinessObjectDataFromParent(this);

            #region Get parent business object
            var parents = new List<StiBusinessObject>();
            var current = this;
            while (current.ParentBusinessObject != null)
            {
                parents.Insert(0, current);
                current = current.ParentBusinessObject;
            }
            #endregion

            var currentData = GetBusinessObjectDataFromParent(current);
            if (currentData is IEnumerable)
            {
                var currentData2 = StiBusinessObjectHelper.GetElementFromEnumerable(currentData as IEnumerable);

                if (currentData2 == null)
                    currentData = StiBusinessObjectHelper.GetElementType(currentData.GetType());
                else
                    currentData = currentData2;
                
            }
            foreach (var businessObject in parents)
            {
                currentData = StiBusinessObjectHelper.GetElementFromObject(currentData, businessObject.Name, isColumnsRetrieve);
                current = businessObject;
            }
            return currentData;
        }

        /// <summary>
        /// Returns the text representation of the Business Object.
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            if (StiOptions.Dictionary.ShowOnlyAliasForBusinessObject)
                return StiBusinessObjectHelper.GetBusinessObjectFullAlias(this);

            var fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(this);

            if (Name == Alias)
                return fullName;

            var fullAlias = StiBusinessObjectHelper.GetBusinessObjectFullAlias(this);
            return $"{fullName} [{fullAlias}]";
        }

        /// <summary>
        /// Returns the name of the Business Object.
        /// </summary>
        /// <returns></returns>
        public string GetCorrectFullName()
        {
            if (this.ParentBusinessObject == null)
                return Name;

            return $"{this.ParentBusinessObject.GetCorrectFullName()}.{StiNameValidator.CorrectName(this.Name, Report)}";
        }

        /// <summary>
        /// Returns the text view of the Business Object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool onlyAlias)
        {
            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias))
                return Alias;

            if (Name == Alias || string.IsNullOrWhiteSpace(Alias))
                return Name;

            return $"{Name} [{Alias}]";
        }

        public void Connect()
        {
            UpdateChilds();
            CreateEnumerator();
        }

        public void Disconnect()
        {
            DestroyEnumerator();
            this.businessObjectValue = null;
            this.RowToLevel = null;
        }
        #endregion

        #region Fields
        internal bool isEnumeratorCreated;
        private object specPrevValue;
        private object specNextValue;
        private bool specNextValueRead;
        private bool specMoveNextResult;
        private object specStoredCurrentValue;
        internal IEnumerator enumerator;
        protected internal Hashtable RowToLevel;
        internal bool specSetPrevValue;
        internal bool specSetNextValue;
        internal bool specFilterData;
        internal bool specSortGroup;
        internal bool specTotalsCalculation = false;
        internal bool previousResetException;
        #endregion

        #region Properties
        [Browsable(false)]
        public object this[string name]
        {
            get
            {
                var stateIsEnumeratorCreated = this.isEnumeratorCreated;

                try
                {
                    var value = this.Current;

                    //fix, если одно из свойств specSetPrevValue, specSetNextValue, specFilterData, specSortGroup или specTotalsCalculation установлено,
                    //значить базовый бизнес-объект изменялся, и требуется получить новое значение текущего бизнес-объекта
                    var tempBO = ParentBusinessObject;
                    while (tempBO != null)
                    {
                        if (tempBO.specSetPrevValue || tempBO.specSetNextValue || tempBO.specFilterData || tempBO.specSortGroup || tempBO.specTotalsCalculation)
                        {
                            value = ParentBusinessObject[this.Name];
                            break;
                        }

                        tempBO = tempBO.ParentBusinessObject;
                    }

                    if (value == null && IsEof)
                        value = specPrevValue;

                    if (value == null)
                        return null;

                    object result = null;

                    if (businessObjectValue is ITypedList)
                        result = StiBusinessObjectHelper.GetValueFromITypedList(businessObjectValue as ITypedList, name, value);

                    else if (businessObjectValue is ICustomTypeDescriptor)
                        return StiBusinessObjectHelper.GetValueFromICustomTypeDescriptor(businessObjectValue as ICustomTypeDescriptor, name, value);

                    else
                        result = StiBusinessObjectHelper.GetValueFromObject(value, name);

                    if (result is Type)
                    {
                        var column = this.Columns[name];
                        if (column != null && column.Type != typeof(Type)) return null;
                    }
                    return result;
                }
                finally
                {
                    #region Если до обращения источник не был инициализорован, то после его инициализации и чтения данных, сбрасываем его в исходное состояние
                    if (!stateIsEnumeratorCreated)
                    {
                        this.BusinessObjectValue = null;
                        this.enumerator = null;
                        isEnumeratorCreated = false;
                        FilteredCount = null;
                    }
                    #endregion
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates new instance of StiBusinessObject class.
        /// </summary>
        public StiBusinessObject()
            : this(string.Empty, string.Empty, string.Empty, null)
        {
        }

        /// <summary>
        /// Creates new instance of StiBusinessObject class.
        /// </summary>
        /// <param name="category">Category of business object.</param>
        /// <param name="name">Name of business object.</param>
        /// <param name="alias">Alias of business object.</param>
        /// <param name="guid">Business object.</param>
        public StiBusinessObject(string category, string name, string alias, string guid)
        {
            this.Category = category;
            this.Name = name;
            this.Alias = alias;
            
            this.Columns = new StiDataColumnsCollection(this);
            this.BusinessObjects = new StiBusinessObjectsCollection(null, this);

            if (guid == null)
                guid = global::System.Guid.NewGuid().ToString().Replace("-", "");

            this.Guid = guid;
        }

        public StiBusinessObject(string category, string name, string alias, string guid, string key)
            : this(category, name, alias, guid)
        {
            this.Key = key;
        }
    }
}
