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
using System.Text;
using System.IO;
using System.ComponentModel;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.CrossTab;

namespace Stimulsoft.Report.Web
{
	/// <summary>
	/// This class describes groups of components.
	/// </summary>
	[StiToolbox(false)]
	public class StiGroup : StiContainer, ICloneable
	{
		#region StiService override
		[Browsable(false)]
		public sealed override Type ServiceType
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
            var group = (StiGroup)this.MemberwiseClone();
			group.Components.Clear();
			foreach (StiComponent comp in this.Components)group.Components.Add((StiComponent)comp.Clone());
			return group;
		}
		#endregion

		#region Methods
		public string ToString(string application)
		{
            var sr = new StiSerializing(new StiReportObjectStringConverter());

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
			sr.Serialize(this, stringWriter, application, StiSerializeTypes.SerializeToDesigner);
			
			stringWriter.Close();
			return sb.ToString();
		}


		public static StiGroup CreateFromString(string text, string application)
		{
            var stringReader = new StringReader(text);

            var sr = new StiSerializing(new StiReportObjectStringConverter());
            var group = new StiGroup();
			sr.Deserialize(group, stringReader, application);
			stringReader.Close();
			return group;
		}


		public static StiComponentsCollection GetSelectedComponents(bool isSelectedFinded,
			int level, StiContainer cont, StiComponentsCollection allComps,
			Hashtable lists)
		{
            var comps = new StiComponentsCollection();
			foreach (StiComponent comp in cont.Components)
			{
                var point = comp as StiPointPrimitive;

                //flag only for this component
                bool isSelectedFinded2 = isSelectedFinded;

				bool isAdded = false;
				if (level == 0 && comp.IsSelected)
				{
					comps.Add(comp);
					allComps.Add(comp);
					isSelectedFinded2 = true;
					//isAdded = true;
				}

				if (level != 0)
				{
					allComps.Add(comp);
					isAdded = true;
				}

				if (isAdded)
				{
					if (point != null && point.ReferenceToGuid != null)
					{
						lists[point.ReferenceToGuid] = comp;
					}
				}

                var cont2 = comp as StiContainer;
				if (cont2 != null)
				{
					int prevLevel = level;
					if (isSelectedFinded2)level ++;
					comps.AddRange(GetSelectedComponents(isSelectedFinded2, level, cont2, allComps, lists));
					level = prevLevel;
				}
			}
			return comps;
		}
	

		public static StiGroup GetGroupFromPage(StiPage page)
		{
            var pg = (StiPage)page.Clone();

            var allComps = new StiComponentsCollection();
            var lists = new Hashtable();
            var comps = GetSelectedComponents(false, 0, pg, allComps, lists);

			if (lists.Count > 0)
			{
                var addedList = new Hashtable();
                foreach (StiComponent comp in comps)
                {
                    if (comp is StiCrossLinePrimitive)
                    {
                        var crossLine = comp as StiCrossLinePrimitive;
                        addedList[crossLine.Guid] = crossLine;
                    }
                }
				foreach (StiComponent comp in pg.Components)
				{
                    var line = comp as StiCrossLinePrimitive;
					if (line != null && lists[line.Guid] != null && addedList[line.Guid] == null)
					{
						comps.Add(line);
						allComps.Add(line);
						addedList[line.Guid] = line;
					}
				}
			}

			#region Move top components to page
			foreach (StiComponent comp in comps)
			{
				if (comp.Parent != comp.Page)
				{
					comp.DisplayRectangle = comp.ComponentToPage(comp.DisplayRectangle);
				}
			}
			#endregion

            var group = new StiGroup();
            var newUnit = new StiHundredthsOfInchUnit();

            #region Reset selection from child components
            foreach (StiComponent comp in comps)
            {
                StiContainer cont = comp as StiContainer;
                if (cont != null)
                {
                    ResetSelection(cont);
                }
            }
            #endregion

            #region Convert units
            foreach (StiComponent comp in allComps)
			{
				if (comp is StiCrossLinePrimitive)continue;
                comp.Linked = false;    //fix
                comp.Page = null;
				comp.Parent = null;
                
				if (comp is StiContainer)
                    ((StiContainer)comp).Convert(pg.Unit, newUnit, false, false);
                else
                    comp.Convert(pg.Unit, newUnit);
			}
			#endregion

			foreach (StiComponent comp in comps)
			{
				group.Components.Add(comp);
			}

			return group;
		}

        private static void ResetSelection(StiContainer cont)
        {
            foreach (StiComponent comp in cont.Components)
            {
                comp.Reset();
                var cont2 = comp as StiContainer;
                if (cont2 != null)
                {
                    ResetSelection(cont2);
                }
            }
        }


		private static StiComponentsCollection GetAllComps(StiComponentsCollection comps)
		{
			StiComponentsCollection comps2 = new StiComponentsCollection();
			foreach (StiComponent comp in comps)
			{
				comps2.Add(comp);

				StiContainer cont = comp as StiContainer;
				if (cont != null)
				{
					comps2.AddRange(GetAllComps(cont.Components));
				}
			}
			return comps2;
		}


		public StiComponentsCollection InsertIntoPage(StiPage page)
		{
            var comps = new StiComponentsCollection();

			StiUnit oldUnit = new StiHundredthsOfInchUnit();
			page.ResetSelection();

            var allComps = GetAllComps(this.Components);

			#region Reasign Guid
			foreach (StiComponent comp in allComps)
			{
                var guid = comp as IStiComponentGuid;
				if (guid != null)
				{
					string oldGuid = guid.Guid;
					guid.NewGuid();

					foreach (StiComponent comp2 in allComps)
					{
                        var guidReference = comp2 as IStiComponentGuidReference;
						if (guidReference != null && guidReference.ReferenceToGuid == oldGuid)
						{
							guidReference.ReferenceToGuid = guid.Guid;
						}

						var crossHeader = comp2 as StiCrossHeader;
						if (crossHeader != null && crossHeader.TotalGuid == oldGuid)
                        {
							crossHeader.TotalGuid = guid.Guid;
						}
					}
				}
			}
			#endregion

            var compsReport = page.Report.GetComponents();
            var compsReportHash = new Hashtable();
            foreach (StiComponent comp in compsReport)
            {
                compsReportHash[comp.Name] = comp;
            }

			foreach (StiComponent comp in allComps)
			{				
				if (!(comp is StiCrossLinePrimitive))
				{
                    if (comp is StiContainer)
                        ((StiContainer)comp).Convert(oldUnit, page.Unit, false, false);
                    else
                        comp.Convert(oldUnit, page.Unit);
				}
				comp.Page = page;

				if (Components.IndexOf(comp) != -1)
				{
					if (!(comp is StiPointPrimitive))page.Components.Add(comp);
					comp.Select();
				}

				//if (!StiNameCreation.IsValidName(page.Report, comp.Name))
                if (compsReportHash[comp.Name] != null)
                {
                    comp.Name = StiNameCreation.CreateName(page.Report, StiNameCreation.GenerateName(page.Report, comp.LocalizedName, comp.GetType()));
                    if (comp is StiGauge)
                        Stimulsoft.Report.Gauge.Helpers.StiGaugeHelper.CheckGaugeName(comp as StiGauge);
				}

                var cont = comp as StiContainer;
				if (cont != null)
				{
					cont.Components.SetParent(cont);
				}

				if (!(comp is StiPointPrimitive))comps.Add(comp);
			}

			return comps;
		}

		#endregion

		public StiGroup()
		{
			Components.Clear();
		}
	}
}
