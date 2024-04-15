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
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Describes the grid coumn collection.
	/// </summary>
	public class StiGridColumnsCollection : 
        CollectionBase,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiGridColumn column in List)
            {
                jObject.AddPropertyJObject(index.ToString(), column.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var column = new StiGridColumn();
                column.LoadFromJsonObject((JObject)property.Value);

                List.Add(column);
            }
        }
        #endregion

		#region Collection

		protected override void OnInsert(int index, object value)
		{
            ((StiGridColumn)value).GridColumnCollection = this;
		}

		public void Add(StiGridColumn column)
		{
			List.Add(column);
		}

		public bool Contains(StiGridColumn column)
		{
			return List.Contains(column);
		}

	
		public int IndexOf(StiGridColumn column)
		{
			return List.IndexOf(column);
		}

		public void Insert(int index, StiGridColumn column)
		{
			List.Insert(index, column);
		}

		public void Remove(StiGridColumn column)
		{
			List.Remove(column);
		}
		
		
		public StiGridColumn this[int index]
		{
			get
			{
				return (StiGridColumn)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion

		#region this
		private StiGridControl gridControl = null;
		public StiGridControl GridControl
		{
			get
			{
				return gridControl;
			}
		}
		
		public StiGridColumnsCollection(StiGridControl gridControl)
		{
			this.gridControl = gridControl;
		}

		public StiGridColumnsCollection() : this(null)
		{
		}
		#endregion
	}
}
