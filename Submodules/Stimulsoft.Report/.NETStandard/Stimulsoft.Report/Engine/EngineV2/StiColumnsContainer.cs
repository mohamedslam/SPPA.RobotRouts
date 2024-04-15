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
using System.Collections.Generic;
using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class describes a container which contains databands. These databands are output in the column mode on the databand.
    /// </summary>
    internal class StiColumnsContainer : StiContainer
    {
        #region Fields
        /// <summary>
        /// Number of bands in a container. Containers markers of the beginning/end of a group are not considered.
        /// </summary>
        private int countOfItems;
        #endregion

        #region Properties
        internal int Columns { get; set; }

        internal double ColumnWidth { get; set; }

        internal double ColumnGaps { get; set; }

        internal StiColumnDirection ColumnDirection { get; set; } = StiColumnDirection.AcrossThenDown;

        internal bool RightToLeft { get; set; }

        internal int MinRowsInColumn { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a specified container into the container of columns. The CountOfItems field is increased on 1.
        /// </summary>
        /// <param name="container">A container that should be added.</param>
        public void AddContainer(StiContainer container)
        {
            this.Components.Add(container);
            if (container is StiLevelContainer) return;
            this.countOfItems++;
        }

        /// <summary>
        /// Returns the height of space to place a container into the current 
        /// container of colimns.
        /// </summary>
        /// <param name="currentHeight">Current height of the container of columns.</param>
        /// <param name="container">A container that will be placed in the container of columns.</param>
        /// <returns>Necessary space.</returns>
        public double HowMuchAdditionalSpaceNeeded(double currentHeight, StiContainer container)
        {
            this.countOfItems++;
            this.Components.Add(container);

            /* Call the FinishColumns method to indicate space to place 
             * entire current container.*/

            var requredSpace = FinishColumns(true);
            this.Components.Remove(container);
            this.countOfItems--;

            return (double)Math.Max((decimal)requredSpace - (decimal)currentHeight, 0);
        }

        /// <summary>
        /// Finishes column formation in the container of columns. All containers are placed on their proper places.
        /// </summary>
        /// <returns>Returns the maximal height</returns>
        public double FinishColumns()
        {
            return FinishColumns(false);
        }

        /// <summary>
        /// Finishes column formation in the container of columns. All containers are placed on their proper places.
        /// </summary>
        /// <returns>Returns the maximal height</returns>
        private double FinishColumns(bool onlyCalc)
        {
            double positionY = 0;
            var column = 1;
            var columns = this.Columns;

            var columnWidth = this.ColumnWidth;
            var columnGaps = this.ColumnGaps;
            var fullColumnWidth = columnWidth + columnGaps;

            #region AcrossThenDown
            if (this.ColumnDirection == StiColumnDirection.AcrossThenDown)
            {
                var componentIndex = 0;
                var startLine = 0;
                var endLine = 0;
                lock (((ICollection) Components).SyncRoot)
                {
                    foreach (StiContainer component in Components)
                    {
                        #region Ignore containers-markers
                        if (component is StiLevelContainer)
                        {
                            componentIndex++;
                            endLine++;
                            //continue;
                        }
                        #endregion

                        else
                        {
                            if (this.Page != null && this.Page.UnlimitedHeight && this.Page.UnlimitedBreakable)
                            {
                                var pageHeight = this.Page.PageHeight - this.Page.Margins.Top - this.Page.Margins.Bottom;
                                if ((int) ((this.Top + positionY) / pageHeight) !=
                                    (int) ((this.Top + positionY + component.Height) / pageHeight))
                                {
                                    positionY += pageHeight - (this.Top + positionY) % pageHeight;
                                }
                            }

                            double dLeft = 0;
                            if (this.RightToLeft)
                                dLeft = component.Parent.Width - fullColumnWidth * column + columnGaps;
                            else
                                dLeft = fullColumnWidth * (column - 1);

                            component.Left = dLeft;
                            component.Width = dLeft - component.Left + columnWidth; //rounding error compensation
                            component.Top = positionY;

                            componentIndex++;
                            endLine++;

                            column++;
                        }

                        if (column > columns || componentIndex == Components.Count)
                        {
                            double maxHeight = 0;

                            #region Find maximal height
                            for (var index = startLine; index < endLine; index++)
                            {
                                var cont = Components[index] as StiContainer;
                                if (cont is StiLevelContainer) continue;
                                maxHeight = Math.Max(cont.Height, maxHeight);
                            }
                            #endregion

                            #region Set maximal height to all containers
                            if (!onlyCalc)
                            {
                                for (var index = startLine; index < endLine; index++)
                                {
                                    var cont = Components[index] as StiContainer;
                                    if (cont is StiLevelContainer) continue;
                                    lock (((ICollection) cont.Components).SyncRoot)
                                    {
                                        foreach (StiComponent comp in cont.Components)
                                        {
                                            if (comp.Bottom == cont.Height)
                                                comp.Height = maxHeight - comp.Top;
                                        }
                                    }

                                    cont.Height = maxHeight;
                                }
                            }
                            #endregion

                            positionY += maxHeight;
                            column = 1;

                            startLine = componentIndex;
                            endLine = componentIndex;
                        }
                    }
                }
            }
            #endregion

            #region DownThenAcross
            else
            {
                //Create the array for storing the number of rows in each column
                var itemsPerColumn = new int[columns];
                
                //Number of rows per one column
                var tempItemsPerColumn = this.countOfItems / columns;

                //Minimal number of rows in a column
                var tempMinRowsInColumn = Math.Min(this.MinRowsInColumn, this.countOfItems);
                
                //If the minimal number of rows in a column is more than the whole number of rows, then decrease
                //the minimal number of rows in a column to the whole number of rows
                if (tempMinRowsInColumn > 0)
                    tempItemsPerColumn = Math.Max(tempMinRowsInColumn, tempItemsPerColumn);

                //Remember the number of columns in each column in the array
                for (var index = 0; index < columns; index++)
                {
                    itemsPerColumn[index] = tempItemsPerColumn;
                }

                if (this.MinRowsInColumn > 0)
                {
                    //If the minimal number of rows is less or equal to the number of rows in one column, 
                    //then make additional distribution of rows by columns
                    if (tempMinRowsInColumn <= tempItemsPerColumn)
                    {
                        //The number of not distributed rows
                        tempItemsPerColumn = this.countOfItems - tempItemsPerColumn * columns;

                        //Add rows to columns untill there are free rows
                        for (var index = 0; index < tempItemsPerColumn; index++)
                        {
                            itemsPerColumn[index]++;
                        }
                    }
                }
                else
                {
                    //new version, first columns must be full, last column is remains.
                    var mod = this.countOfItems - tempItemsPerColumn * columns;
                    if (mod > 0)
                    {
                        tempItemsPerColumn++;
                        for (var index = 0; index < columns; index++)
                        {
                            itemsPerColumn[index] = tempItemsPerColumn;
                        }
                    }
                }

                var currentItem = 0;
                double posY = 0;

                #region Creates an array of collections. Containers as columns will be stored there. One collection - one column.
                var listOfColumns = new List<StiContainer>[columns];
                for (var index = 0; index < columns; index++)
                {
                    listOfColumns[index] = new List<StiContainer>();
                }
                #endregion

                double storedHeight = -1;
                //True если было использовано свойство CanGrow и мы имеем разные высоты у контейнеров
                var usedCanGrow = false;

                lock (((ICollection) Components).SyncRoot)
                {
                    foreach (StiContainer component in Components)
                    {
                        #region Ignore containers-markers
                        if (component is StiLevelContainer) continue;
                        #endregion

                        #region Check the containers height
                        if (!usedCanGrow)
                        {
                            if (storedHeight == -1)
                                storedHeight = component.Height;
                            else if (storedHeight != component.Height)
                                usedCanGrow = true;
                        }
                        #endregion

                        currentItem++;

                        if (currentItem > itemsPerColumn[column - 1])
                        {
                            currentItem = 1;
                            column++;

                            posY = 0;
                        }

                        if (this.Page != null && this.Page.UnlimitedHeight && this.Page.UnlimitedBreakable)
                        {
                            var pageHeight = this.Page.PageHeight - this.Page.Margins.Top - this.Page.Margins.Bottom;
                            if ((int) ((this.Top + posY) / pageHeight) !=
                                (int) ((this.Top + posY + component.Height) / pageHeight))
                            {
                                posY += pageHeight - (this.Top + posY) % pageHeight;
                            }
                        }

                        double dLeft = 0;
                        if (this.RightToLeft)
                            dLeft = component.Parent.Width - fullColumnWidth * column + columnGaps;
                        else
                            dLeft = fullColumnWidth * (column - 1);

                        component.Left = dLeft;
                        component.Width = dLeft - component.Left + columnWidth; //rounding error compensation

                        component.Top = posY;
                        posY += component.Height;
                        positionY = Math.Max(positionY, posY);

                        //Add a component into the list of containers
                        listOfColumns[column - 1].Add(component);
                    }
                }

                #region If the CanGrow property was used then align rows by height in containers
                if (usedCanGrow)
                {
                    #region Find the maximal number of rows in one of columns
                    var maxRows = 0;
                    for (var columnIndex = 0; columnIndex < columns; columnIndex++)
                    {
                        var list = listOfColumns[columnIndex];
                        maxRows = Math.Max(list.Count, maxRows);
                    }
                    #endregion

                    /* A table is used to store changed heights of containers. It is used in the 
                     * mode of calculation of the common height.*/
                    var containerHeights = new Hashtable();

                    #region Correct heights of containers
                    for (var rowIndex = 0; rowIndex < maxRows; rowIndex++)
                    {
                        double maxHeight = 0;

                        #region Find maximal height in a line
                        for (var columnIndex = 0; columnIndex < columns; columnIndex++)
                        {
                            if (listOfColumns[columnIndex].Count > rowIndex)
                            {
                                var container = listOfColumns[columnIndex][rowIndex];

                                #region Игнорируем контейнеры - маркеры
                                if (container is StiLevelContainer) continue;
                                #endregion

                                maxHeight = Math.Max(maxHeight, container.Height);
                            }
                        }
                        #endregion

                        #region Correct heights of containers
                        for (var columnIndex = 0; columnIndex < columns; columnIndex++)
                        {
                            if (listOfColumns[columnIndex].Count <= rowIndex) continue;

                            var container = listOfColumns[columnIndex][rowIndex];

                            #region Ignore containers-markers
                            if (container is StiLevelContainer) continue;
                            #endregion

                            if (!onlyCalc)
                            {
                                if (container.Height != maxHeight)
                                {
                                    lock (((ICollection)container.Components).SyncRoot)
                                    {
                                        foreach (StiComponent comp in container.Components)
                                        {
                                            if (comp.Bottom == container.Height)
                                                comp.Height = maxHeight - comp.Top;
                                        }
                                    }
                                    container.Height = maxHeight;
                                }
                            }
                            else
                            {
                                containerHeights[container] = maxHeight;
                            }
                        }
                        #endregion
                    }
                    #endregion

                    positionY = 0;

                    #region Correct position of containers with taking changes heights into consideration
                    for (var columnIndex = 0; columnIndex < columns; columnIndex++)
                    {
                        posY = 0;
                        lock (((ICollection) listOfColumns[columnIndex]).SyncRoot)
                        {
                            foreach (var cont in listOfColumns[columnIndex])
                            {
                                #region Ignore containers-markers
                                if (cont is StiLevelContainer) continue;
                                #endregion

                                cont.Top = posY;
                                var contHeight = onlyCalc ? (double)containerHeights[cont] : cont.Height;
                                posY += contHeight;
                            }
                        }

                        positionY = Math.Max(positionY, posY);
                    }
                    #endregion
                }
                #endregion

            }
            #endregion

            return positionY;
        }

        /// <summary>
        /// Returns the number of a column on the current moment. The number starts with 1.
        /// For the DownThenAcross 1 is always returned.
        /// </summary>
        /// <returns>The number of an output column.</returns>
        public int GetCurrentColumn()
        {
            if (this.ColumnDirection == StiColumnDirection.DownThenAcross)
                return 1;
            
            //The number of entire lines
            var lines = this.countOfItems / this.Columns;
            return this.countOfItems - lines * this.Columns + 1;
        }

        /// <summary>
        /// Returns the number of output databands in the last row.
        /// The method can be used only for the  AcrossThenDown mode.
        /// </summary>
        /// <returns>The number of output databands</returns>
        public int GetLengthOfLastRow()
        {
            //StiDataBand dataBand = this.ContainerInfoV2.ParentBand as StiDataBand;
            var oldCount = this.countOfItems;
            var lines = oldCount / this.Columns;
            var newCount = lines * this.Columns;

            return oldCount == newCount ? this.Columns : oldCount - newCount;
        }
        #endregion

        /// <summary>
        /// Need remove.
        /// </summary>
        public StiColumnsContainer()
        {
        }
    }
}
