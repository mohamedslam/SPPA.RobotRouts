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
    /// A class to output columns on the Panel.
    /// </summary>
    internal class StiColumnsOnPanel
    {
        #region Properties
        internal StiEngine Engine { get; }

        /// <summary>
        /// Returns the number of columns in the current container for output. If there is no columns then returns 0.
        /// </summary>
        internal int Count
        {
            get
            {
                var panel = Engine.TemplateContainer as StiPanel;
                return panel != null ? panel.Columns : 0;
            }
        }

        /// <summary>
        /// Returns the direction of columns output in the current output container.
        /// </summary>
        internal bool RightToLeft
        {
            get
            {
                var panel = Engine.TemplateContainer as StiPanel;
                return panel != null && panel.RightToLeft;
            }
        }

        /// <summary>
        /// Returns interval between two columns in the current output container.
        /// </summary>
        internal double ColumnGaps
        {
            get
            {
                var panel = Engine.TemplateContainer as StiPanel;
                return panel != null ? panel.ColumnGaps : 0;
            }
        }

        /// <summary>
        /// A number of a column in the currrent output container. Numbering starts with 1.
        /// </summary>
        internal int CurrentColumn { get; set; } = 1;
        #endregion

        #region Methods 
        /// <summary>
        /// Returns the width of a column in the current output container.
        /// </summary>
        /// <returns></returns>
        internal double GetColumnWidth()
        {
            var panel = Engine.TemplateContainer as StiPanel;
            return panel != null ? panel.GetColumnWidth() : 0;
        }
        #endregion

        public StiColumnsOnPanel(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
