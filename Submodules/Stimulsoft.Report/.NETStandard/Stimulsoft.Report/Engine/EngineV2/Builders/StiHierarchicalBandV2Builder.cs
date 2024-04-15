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

using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    public class StiHierarchicalBandV2Builder : StiDataBandV2Builder
    {
        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterHierarchical = masterComp as StiHierarchicalBand;

            if (IsCollapsed(masterHierarchical))
                return new StiContainer();

            var container = base.InternalRender(masterHierarchical) as StiContainer;
            if (container != null)
            {
                var ds = masterHierarchical.DataSource;
                if (ds != null && !ds.IsEmpty)
                {
                    var level = ds.GetLevel();
                    if (level > 0)
                        CreateIndention(masterHierarchical, container, level);

                    if ((container.Interaction != null) && (container.Interaction is StiBandInteraction) && ((StiBandInteraction)container.Interaction).CollapsingEnabled)
                    {
                        int pos = ds.Position;
                        ds.Position++;
                        var nextLevel = ds.GetLevel();
                        ds.Position = pos;
                        if (nextLevel <= level)
                        {
                            var bandInteraction = container.Interaction.Clone() as StiBandInteraction;
                            bandInteraction.CollapsingEnabled = false;
                            container.Interaction = bandInteraction;
                        }
                    }
                }

                var bo = masterHierarchical.BusinessObject;
                if ((bo != null) && !bo.IsEmpty)
                {
                    var level = bo.GetLevel();
                    if (level > 0)
                        CreateIndention(masterHierarchical, container, level);

                    if ((container.Interaction != null) && (container.Interaction is StiBandInteraction) && ((StiBandInteraction)container.Interaction).CollapsingEnabled)
                    {
                        int pos = bo.Position;
                        bo.Next();
                        var nextLevel = bo.GetLevel();
                        bo.Position = pos;
                        if (nextLevel <= level)
                        {
                            var bandInteraction = container.Interaction.Clone() as StiBandInteraction;
                            bandInteraction.CollapsingEnabled = false;
                            container.Interaction = bandInteraction;
                        }
                    }
                }
            }

            return container;
        }

        private bool IsCollapsed(StiHierarchicalBand band)
        {
            if (band.CollapsingIndex == 0 || band.Position == 0) return false;

            var resIndex = band.CollapsingIndex;
            var resPosition = band.Position;
            var level = 0;

            var useBusinessObject = !band.IsBusinessObjectEmpty;
            object storedBusinessObjectCurrent = null;

            if (!band.IsDataSourceEmpty)
                level = band.DataSource.GetLevel();

            if (useBusinessObject)
                level = band.BusinessObject.GetLevel();

            try
            {
                #region BusinessObject
                if (useBusinessObject)
                {
                    var bandPosition = band.Position;
                    storedBusinessObjectCurrent = band.BusinessObject.Current;

                    band.Position = 0;
                    var listOfDetailRows = new ArrayList();
                    while ((listOfDetailRows.Count < bandPosition) && !band.IsEof)
                    {
                        listOfDetailRows.Add(band.BusinessObject.Current);
                        band.Next();
                    }

                    while (true)
                    {
                        if (band.CollapsingIndex == 0 || bandPosition == 0)
                            return false;

                        band.CollapsingIndex--;
                        bandPosition--;

                        band.BusinessObject.currentObject = listOfDetailRows[bandPosition];

                        var currLevel = band.BusinessObject.GetLevel();
                        if (level <= currLevel) continue;

                        if (IsCollapsed(band, false)) return true;
                        level = currLevel;
                    }
                }
                #endregion

                #region DataSource
                if (!band.IsDataSourceEmpty)
                {
                    while (true)
                    {
                        if (band.CollapsingIndex == 0 || band.Position == 0)
                            return false;

                        band.CollapsingIndex--;
                        band.Position--;

                        var currLevel = band.DataSource.GetLevel();
                        if (level <= currLevel) continue;

                        if (IsCollapsed(band, false)) return true;
                        level = currLevel;
                    }
                }
                #endregion

                return false;
            }
            finally
            {
                if (useBusinessObject)
                    band.BusinessObject.currentObject = storedBusinessObjectCurrent;

                band.CollapsingIndex = resIndex;
                band.Position = resPosition;
            }
        }

        internal static void CreateIndention(StiHierarchicalBand masterHierarchical, StiContainer container, int level)
        {
            var dist = level * masterHierarchical.Report.Unit.ConvertFromHInches(masterHierarchical.Indent);

            lock (((ICollection) container.Components).SyncRoot)
            {
                foreach (StiComponent comp in container.Components)
                {
                    if (comp.Locked) continue;
                    comp.Left += dist;
                }
            }
        }
    }
}
