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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class to output columns on the DataBand.
    /// </summary>
    internal class StiColumnsOnDataBand
    {
        #region Properties
        internal StiEngine Engine { get; }

        /// <summary>
        /// Gets or sets value which indicates about current state of column on DataBand mode.
        /// </summary>
        public bool Enabled { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a container of columns to the current page. 
        /// A container of columns is used to output columns on a databand.
        /// </summary>
        /// <param name="dataBand">A DataBand that contains columns.</param>
        /// <returns></returns>
        internal StiColumnsContainer RenderColumns(StiDataBand dataBand)
        {
            //Add a container of columns only in a case when columns are present
            if (dataBand.Columns > 1)
            {
                //Creates a container for columns
                var columnsContainer = CreateColumns(dataBand);
                
                //Записываем контейнер в страницу
                Engine.RenderContainer(columnsContainer);
                
                //Enable mode of button output on a DataBand
                Enabled = true;

                return columnsContainer;
            }
            return null;
        }

        /// <summary>
        /// Returns a container of columns that is the last on a page.
        /// If after a container of columns other bands were output then return null.
        /// </summary>
        /// <returns>Container of columns.</returns>
        internal StiColumnsContainer GetColumns(StiContainer container = null)
        {
            if (Engine.ContainerForRender == null) return null;
            var countOfComponents = Engine.ContainerForRender.Components.Count;
            if (countOfComponents <= 0) return null;

            for (var index = countOfComponents - 1; index >= 0; index--)
            {
                var comp = Engine.ContainerForRender.Components[index];
                if (comp is StiColumnsContainer)
                {
                    if (container != null && "Columns" + container.Name != comp.Name) return null;
                    return comp as StiColumnsContainer;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates and returns a container to output columns on a Databand.
        /// </summary>
        /// <param name="dataBand">A DataBand that describes columns.</param>
        /// <returns>Created container.</returns>
        public StiColumnsContainer CreateColumns(StiDataBand dataBand)
        {
            return new StiColumnsContainer
            {
                Name = "Columns" + dataBand.Name,
                Width = dataBand.Parent.Width,
                Height = 0,
                Columns = dataBand.Columns,
                ColumnDirection = dataBand.ColumnDirection,
                ColumnGaps = dataBand.ColumnGaps,
                ColumnWidth = dataBand.GetColumnWidth(),
                MinRowsInColumn = dataBand.MinRowsInColumn,
                RightToLeft = dataBand.RightToLeft
            };
        }
        #endregion

        public StiColumnsOnDataBand(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
