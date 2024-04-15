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


namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class describesindex in a container. The index is represented as Index.IndexInColumnContainer, where 
    /// Index is the index in the current container of output, IndexInColumnContainer is the index in the container of columns. 
    /// If the index does not indicate the column container, then IndexInColumnContainer is -1.
    /// </summary>
    internal class StiIndex
    {
        public int Index { get; set; }

        public int IndexInColumnContainer { get; set; }

        public StiIndex(int index)
            : this(index, -1)
        {
        }

        public StiIndex(int index, int indexInColumnContainer)
        {
            this.Index = index;
            this.IndexInColumnContainer = indexInColumnContainer;
        }
    }
}
