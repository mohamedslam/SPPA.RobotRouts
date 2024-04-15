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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using System.Globalization;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the collection of variables.
    /// </summary>
    public class StiVariablesCollection :
        CollectionBase,
        ICloneable,
        IComparer
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObjectEx(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            var index = 0;
            foreach (StiVariable variable in List)
            {
                jObject.AddPropertyJObject(index.ToString(), variable.SaveToJsonObjectEx());
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObjectEx(JObject jObject, StiReport report)
        {
            foreach (var property in jObject.Properties())
            {
                var variable = new StiVariable();
                variable.LoadFromJsonObjectEx((JObject)property.Value, report);

                List.Add(variable);
            }
        }
        #endregion

        #region IComparer
        private int directionFactor = 1;
        int IComparer.Compare(object x, object y)
        {
            var var1 = x as StiVariable;
            var var2 = y as StiVariable;

            var catFactor = var1.Category.CompareTo(var2.Category) * directionFactor;
            if (catFactor != 0) 
                return catFactor;

            return StiOptions.Designer.SortDictionaryByAliases
                ? var1.Alias.CompareTo(var2.Alias) * directionFactor
                : var1.Name.CompareTo(var2.Name) * directionFactor;
        }
        #endregion

        #region Sort
        public void Sort()
        {
            Sort(StiSortOrder.Asc);
        }

        public void Sort(StiSortOrder order)
        {
            directionFactor = order == StiSortOrder.Asc ? 1 : -1;

            base.InnerList.Sort(this);
        }
        #endregion

        #region Collection
        public List<StiVariable> ToList()
        {
            return this.Cast<StiVariable>().ToList();
        }

        public StiVariable Add(string name, object value)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(name, value));
        }

        public StiVariable Add(string category, string name, object value)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(category, name, value));
        }

        public StiVariable Add(string category, string name, string alias, object value)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(category, name, alias, value));
        }

        public StiVariable Add(string category, string name, string alias, object value, bool readOnly)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(category, name, alias, value, readOnly));
        }

        public StiVariable Add(string category, string name, string alias, Type type, string value, bool readOnly)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(category, name, alias, type, value, readOnly));
        }

        public StiVariable Add(string category)
        {
            return AddInternal(new StiVariable(category));
        }

        public StiVariable Add(string category, string name, Type type, string value, bool readOnly)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(category, name, type, value, readOnly));
        }

        public StiVariable Add(string category, string name, Type type)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(category, name, type));
        }

        public StiVariable Add(string name, Type type)
        {
            if (name.Length > 0 &&
                this.Contains(name)) this.Remove(name);

            return AddInternal(new StiVariable(name, type));
        }

        public StiVariable Add(StiVariable variable)
        {
            if (!string.IsNullOrWhiteSpace(variable.Name) && this.Contains(variable.Name)) 
                this.Remove(variable.Name);

            return AddInternal(variable);
        }

        public void AddRange(StiVariable[] variables)
        {
            foreach (var variable in variables)
            {
                if (variable.Name.Length > 0 &&
                    this.Contains(variable.Name)) this.Remove(variable.Name);

                AddInternal(variable);
            }
        }

        public void AddRange(StiVariablesCollection variables)
        {
            foreach (StiVariable variable in variables)
            {
                Add(variable);
            }
        }

        private StiVariable AddInternal(StiVariable variable)
        {
            List.Add(variable);

            return variable;
        }

        public bool Contains(StiVariable variable)
        {
            return List.Contains(variable);
        }

        public bool Contains(string name)
        {
            name = name.ToLowerInvariant();
            foreach (StiVariable variable in this)
            {
                if (variable.Name.ToLowerInvariant() == name) 
                    return true;
            }
            return false;
        }

        public bool ContainsCategory(string name)
        {
            name = name.ToLowerInvariant();
            foreach (StiVariable variable in this)
            {
                if (variable.Category.ToLowerInvariant() == name) 
                    return true;
            }
            return false;
        }

        public int IndexOf(StiVariable variable)
        {
            return List.IndexOf(variable);
        }

        public int IndexOf(string name)
        {
            name = name.ToLowerInvariant();
            var index = 0;
            foreach (StiVariable variable in this)
            {
                if (variable.Name.ToLowerInvariant() == name)
                    return index;

                index++;
            }
            return -1;
        }

        public void Insert(int index, StiVariable variable)
        {
            List.Insert(index, variable);
        }

        public void Remove(StiVariable variable)
        {
            if (variable.Category.Length > 0 && GetVariablesCount(variable.Category) == 1) 
                variable.Name = "";

            else 
                List.Remove(variable);
        }

        public void Remove(string name)
        {
            name = name.ToLowerInvariant();
            var index = 0;
            while (index < List.Count)
            {
                var variable = List[index] as StiVariable;
                if (variable.Name.ToLowerInvariant() == name)                
                    List.RemoveAt(index);                
                else 
                    index++;
            }
        }

        public StiVariable this[int index]
        {
            get
            {
                return (StiVariable)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public StiVariable this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();

                foreach (StiVariable variable in List)
                {
                    if (variable.Name.ToLowerInvariant() == name) 
                        return variable;
                }

                return null;
            }
            set
            {
                name = name.ToLowerInvariant();
                for (var index = 0; index < List.Count; index++)
                {
                    var variable = List[index] as StiVariable;

                    if (variable.Name.ToLowerInvariant() == name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                Add(value);
            }
        }

        public StiVariable[] Items => (StiVariable[])InnerList.ToArray(typeof(StiVariable));
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var variables = new StiVariablesCollection();

            foreach (StiVariable variable in List)
            {
                variables.Add((StiVariable)variable.Clone());
            }

            return variables;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Moves all variables from "fromCategory" category to position of "toCategory" category.
        /// </summary>
        public void MoveCategoryTo(string fromCategory, string toCategory)
        {
            if (fromCategory == toCategory)
                return;

            var fromIndex = GetFirstCategoryIndex(fromCategory);
            var toIndex = GetFirstCategoryIndex(toCategory);

            if (fromIndex == toIndex)return;

            var fromVariables = new List<StiVariable>();
            foreach (StiVariable variable in this)
            {
                if (variable.Category == fromCategory)
                    fromVariables.Add(variable);
            }

            RemoveCategory(fromCategory);

            var index = fromIndex > toIndex 
                ? GetFirstCategoryIndex(toCategory) 
                : GetLastCategoryIndex(toCategory) + 1;

            foreach (var variable in fromVariables)
            {
                this.Insert(index, variable);
                index++;
            }
        }

        public int GetFirstCategoryIndex(string category)
        {
            var index = 0;

            foreach (StiVariable variable in this)
            {
                if (category == variable.Category)
                    return index;

                index++;
            }
            return -1;
        }

        public int GetLastCategoryIndex(string category)
        {
            var selectedIndex = -1;
            var index = 0;
            var find = false;

            foreach (StiVariable variable in this)
            {
                if (category == variable.Category)
                {
                    selectedIndex = index;
                    find = true;
                }
                else if (find) break;

                index++;
            }
            return selectedIndex;
        }

        public void RenameCategory(string oldName, string newName)
        {
            oldName = oldName.ToLower(CultureInfo.InvariantCulture);
            foreach (StiVariable variable in List)
            {
                if (variable.Category.ToLowerInvariant() == oldName)
                    variable.Category = newName;
            }
        }

        public List<StiVariable> GetAllVariablesOfCategory(string category)
        {
            category = category.ToLowerInvariant();
            return List.Cast<StiVariable>()
                .Where(v => !v.IsCategory && v.Category.ToLowerInvariant() == category)
                .ToList();            
        }

        public void RemoveCategory(string category)
        {
            var pos = 0;
            while (pos < Items.Length)
            {
                if (Items[pos].Category == category) 
                    RemoveAt(pos);
                else 
                    pos++;
            }
        }

        public int GetVariablesCount(string category)
        {
            var count = 0;
            foreach (StiVariable variable in List)
            {
                if (variable.Category == category)
                    count++;
            }

            return count;
        }

        public string GenerateEmptyCategoryName()
        {
            var baseName = StiLocalization.Get("PropertyMain", "Category");

            var index = 1;
            while (true)
            {
                var newName = baseName + index;

                if (!this.ContainsCategory(newName))
                    return newName;

                index++;
            }
        }
        #endregion
    }
}