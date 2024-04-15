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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes a collection component.
    /// </summary>
    public class StiComponentsCollection :
        CollectionBase,
        IStiStateSaveRestore,
        ICloneable,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiComponent component in List)
            {
                jObject.AddPropertyJObject(index.ToString(), component.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var propJObject = (JObject)property.Value;
                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

                var service = StiOptions.Services.Components.FirstOrDefault(x => x.GetType().Name == ident);

                if (service == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var component = service.CreateNew();

                Add(component);
                component.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var comps = new StiComponentsCollection();

            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiComponent comp in this)
                {
                    comps.Add(comp.Clone() as StiComponent);
                }
            }

            return comps;
        }
        #endregion

        #region Methods
        internal StiComponent GetComponentByName(string componentName, StiContainer container)
        {
            lock (((ICollection)container.Components).SyncRoot)
            {
                foreach (StiComponent component in container.Components)
                {
                    if (component.Name == componentName) 
                        return component;

                    var cont = component as StiContainer;
                    if (cont != null)
                    {
                        var comp = GetComponentByName(componentName, cont);
                        if (comp != null) 
                            return comp;
                    }
                }
            }
            return null;
        }

        internal StiComponent GetComponentByGuid(string guid, StiContainer container)
        {
            lock (((ICollection)container.Components).SyncRoot)
            {
                foreach (StiComponent component in container.Components)
                {
                    if (component.Guid == guid)
                        return component;

                    var cont = component as StiContainer;
                    if (cont != null)
                    {
                        var comp = GetComponentByGuid(guid, cont);
                        if (comp != null)
                            return comp;
                    }
                }
            }
            return null;
        }

        public StiPage GetPageByAlias(string alias)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    if (page.Alias == alias) 
                        return page;
                }
            }
            return null;
        }

        public void SetParent(StiContainer parent)
        {
            this.parent = parent;

            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiComponent comp in this)
                {
                    comp.Parent = parent;
                    
                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.Components.SetParent(cont);
                }
            }
        }

        public List<StiComponent> ToList()
        {
            return this.Cast<object>().Cast<StiComponent>().ToList();
        }

        public List<StiSubReport> SelectAllSubReports()
        {
            return this.Cast<object>().Cast<StiComponent>()
                .Where(c => c is StiSubReport)
                .Cast<StiSubReport>().ToList();
        }

        public List<StiChart> SelectAllCharts()
        {
            return this.Cast<object>().Cast<StiComponent>()
                .Where(c => c is StiChart)
                .Cast<StiChart>().ToList();
        }

        private void AddCore(StiComponent component)
        {
            if (parent != null)
            {
                component.Parent = parent;
                
                if (parent.Page != null)
                {
                    component.Page = parent.Page;

                    if (string.IsNullOrEmpty(component.Name))
                    {
                        if (component.Report != null && component.Report.IsDesigning)
                        {
                            component.Name = StiNameCreation.CreateName(parent.Report,
                                StiNameCreation.GenerateName(component));
                        }
                        else
                        {
                            component.Name = StiNameCreation.CreateSimpleName(parent.Report,
                                StiNameCreation.GenerateName(component));
                        }
                    }
                }
            }

            List.Add(component);
        }

        public void Add(StiComponent component)
        {
            AddCore(component);
            InvokeComponentAdded(component, EventArgs.Empty);
        }

        public void AddRange(StiComponentsCollection components)
        {
            lock (((ICollection)components).SyncRoot)
            {
                foreach (StiComponent comp in components)
                {
                    Add(comp);
                }
            }
        }

        public void AddRange(StiComponent[] components)
        {
            lock (components.SyncRoot)
            {
                foreach (StiComponent comp in components)
                {
                    Add(comp);
                }
            }
        }

        public bool Contains(StiComponent component)
        {
            return List.Contains(component);
        }

        public int IndexOf(StiComponent component)
        {
            return List.IndexOf(component);
        }

        public int IndexOf(string name)
        {
            name = name.ToLowerInvariant();
            int index = 0;

            lock (List.SyncRoot)
            {
                foreach (StiComponent component in List)
                {
                    if (component.Name.ToLowerInvariant() == name) 
                        return index;

                    index++;
                }
            }
            return -1;
        }

        public void InsertRange(int index, StiComponentsCollection components)
        {
            lock (((ICollection)components).SyncRoot)
            {
                foreach (StiComponent comp in components)
                {
                    Insert(index, comp);
                }
            }
        }

        public void Insert(int index, StiComponent component)
        {
            if (parent != null)
            {
                component.Parent = parent;
                
                if (parent.Page != null)                
                    component.Page = parent.Page;                
            }

            List.Insert(index, component);
            InvokeComponentAdded(component, EventArgs.Empty);
        }

        public void Remove(StiComponentsCollection components)
        {
            lock (((ICollection)components).SyncRoot)
            {
                foreach (StiComponent component in components)
                {
                    Remove(component);
                }
            }
        }

        public void Remove(StiComponent component)
        {
            Remove(component, true);
        }

        public void Remove(StiComponent component, bool clearParent)
        {
            if (clearParent && component.Page != null && component.Report != null)            
                component.Parent = null;            

            if (List.Contains(component)) 
                List.Remove(component);

            InvokeComponentRemoved(component, EventArgs.Empty);
        }

        public StiComponent this[int index]
        {
            get
            {
                return List[index] as StiComponent;
            }
            set
            {
                List[index] = value;
            }
        }

        public StiComponent this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();
                lock (List.SyncRoot)
                {
                    foreach (StiComponent component in List)
                    {
                        if (component.Name.ToLowerInvariant() == name)
                            return component;
                    }
                }
                return null;
            }
            set
            {
                name = name.ToLowerInvariant();
                for (int index = 0; index < List.Count; index++)
                {
                    var comp = List[index] as StiComponent;
                    if (comp.Name.ToLowerInvariant() == name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                AddCore(value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            List.CopyTo(array, index);
        }
        #endregion

        #region Events
        #region ComponentAdded
        public event EventHandler ComponentAdded;

        protected virtual void OnComponentAdded(EventArgs e)
        {
        }

        public virtual void InvokeComponentAdded(object sender, EventArgs e)
        {
            this.OnComponentAdded(e);
            
            ComponentAdded?.Invoke(sender, e);
        }
        #endregion

        #region ComponentRemoved
        public event EventHandler ComponentRemoved;

        protected virtual void OnComponentRemoved(EventArgs e)
        {
        }

        public virtual void InvokeComponentRemoved(object sender, EventArgs e)
        {
            this.OnComponentRemoved(e);
            
            ComponentRemoved?.Invoke(sender, e);
        }
        #endregion
        #endregion        

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public virtual void SaveState(string stateName)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiComponent comp in this)
                {
                    comp.SaveState(stateName);
                }
            }
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiComponent comp in this)
                {
                    comp.RestoreState(stateName);
                }
            }
        }

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiComponent comp in this)
                {
                    comp.ClearAllStates();
                }
            }
        }
        #endregion

        #region Methods.Sort
        public void SortBySelectionTick()
        {
            if (Count <= 1)return;
            
            int pos = 1;
            while (pos < this.Count)
            {
                if (this[pos - 1].SelectionTick > this[pos].SelectionTick)
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;
                    
                    if (pos != 1) 
                        pos--;
                }
                else 
                    pos++;
            }
        }


        public void SortByPriority()
        {
            if (Count <= 1)return;

            int pos = 1;
            while (pos < this.Count)
            {
                if (this[pos - 1].Priority > this[pos].Priority)
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;
                    
                    if (pos != 1)
                        pos--;
                }
                else 
                    pos++;
            }
        }

        public void SortByTopPosition()
        {
            if (Count <= 1)return;

            bool needSort = true;
            var thisCount = this.Count;

            if (thisCount > 100)
            {
                needSort = false;
                var lastValue = this[0].Top;
                var hash = new Hashtable();
                for (int index = 0; index < thisCount; index++)
                {
                    var currentValue = this[index].Top;
                    hash[currentValue] = null;
                    
                    if (currentValue < lastValue) 
                        needSort = true;

                    lastValue = currentValue;
                }

                var hashKeysCount = hash.Keys.Count;
                hash.Clear();

                if (needSort && (hashKeysCount < thisCount / 2))
                {
                    #region Optimized sorting
                    for (int index = 0; index < Count; index++)
                    {
                        var comp = this[index];
                        var currentValue = comp.Top;
                        var comps = (List<StiComponent>)hash[currentValue];
                        if (comps == null)
                        {
                            comps = new List<StiComponent>();
                            hash[currentValue] = comps;
                        }
                        comps.Add(comp);
                    }

                    var keys = new object[hash.Keys.Count];
                    hash.Keys.CopyTo(keys, 0);
                    Array.Sort(keys);

                    int counter = 0;
                    foreach (var key in keys)
                    {
                        var comps = (List<StiComponent>)hash[key];
                        foreach (var comp in comps)
                        {
                            this[counter++] = comp;
                        }
                    }
                    hash.Clear();

                    needSort = false;
                    #endregion
                }
            }

            if (needSort)
            {
                #region Standard sorting
                int pos = 1;
                while (pos < thisCount)
                {
                    if (this[pos - 1].Top > this[pos].Top)
                    {
                        var swapComp = this[pos - 1];
                        this[pos - 1] = this[pos];
                        this[pos] = swapComp;
                        
                        if (pos != 1) 
                            pos--;
                    }
                    else 
                        pos++;
                }
                #endregion
            }
        }

        public void SortByBottomPosition()
        {
            if (Count <= 1)return;
            
            var pos = 1;
            while (pos < this.Count)
            {
                if (this[pos - 1].Bottom > this[pos].Bottom)
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;
                    
                    if (pos != 1) 
                        pos--;
                }
                else 
                    pos++;
            }
        }

        public void SortByLeftPosition()
        {
            if (Count <= 1)return;
            
            var pos = 1;
            while (pos < this.Count)
            {
                if (this[pos - 1].Left > this[pos].Left)
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;

                    if (pos != 1) 
                        pos--;
                }
                else 
                    pos++;
            }
        }

        public void SortByRightPosition()
        {
            if (Count <= 1)return;
            
            var pos = 1;
            while (pos < this.Count)
            {
                if (this[pos - 1].Right > this[pos].Right)
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;
                    
                    if (pos != 1) 
                        pos--;
                }
                else 
                    pos++;
            }
        }

        public void SortBandsByTopPosition()
        {
            if (Count <= 1)return;
            
            var pos = 1;
            while (pos < this.Count)
            {

                if (
                    this[pos - 1] is StiBand &&
                    this[pos] is StiBand &&
                    this[pos - 1].DockStyle == this[pos].DockStyle &&
                    ((this[pos - 1].DockStyle == StiDockStyle.Top && this[pos - 1].Top > this[pos].Top) ||
                    (this[pos - 1].DockStyle == StiDockStyle.Bottom && this[pos - 1].Top < this[pos].Top)))
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;
                    
                    if (pos != 1) 
                        pos--;
                }
                else 
                    pos++;
            }
        }

        public void SortBandsByLeftPosition()
        {
            if (Count <= 1)return;
            
            int pos = 1;
            while (pos < Count)
            {
                if (this[pos - 1] is StiBand && this[pos] is StiBand && this[pos - 1].Left > this[pos].Left)
                {
                    var swapComp = this[pos - 1];
                    this[pos - 1] = this[pos];
                    this[pos] = swapComp;
                    
                    if (pos != 1) 
                        pos--;
                }
                else 
                    pos++;
            }
        }
        #endregion

        #region Fields
        private StiContainer parent;
        #endregion

        public StiComponentsCollection(StiContainer parent)
        {
            this.parent = parent;
        }

        public StiComponentsCollection()
        {
            this.parent = null;
        }
    }
}
