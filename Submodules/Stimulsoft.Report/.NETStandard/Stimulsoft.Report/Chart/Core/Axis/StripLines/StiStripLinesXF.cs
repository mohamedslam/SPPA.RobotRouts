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
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiStripLinesXF : CollectionBase
    {
        public object Clone()
        {
            var newLines = new StiStripLinesXF();
            foreach (StiStripLineXF line in base.List)
                newLines.Add(line.Clone() as StiStripLineXF);
            
            return newLines;
        }

		public void Add(object valueObject, double value)
		{
            Add(new StiStripLineXF(valueObject, value));
		}

        public void Add(StiStripLineXF line)
        {
            base.List.Add(line);
        }

        public void AddRange(StiStripLineXF[] lines)
        {
            foreach (StiStripLineXF line in lines) Add(line);
        }

        public bool Contains(StiStripLineXF value)
        {
            return List.Contains(value);
        }

        public int IndexOf(StiStripLineXF value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, StiStripLineXF value)
        {
            List.Insert(index, value);
        }

        public void Remove(StiStripLineXF value)
        {
            List.Remove(value);
        }

        public void Reverse()
        {
            InnerList.Reverse();
        }

        public StiStripLineXF this[int index]
        {
            get
            {
                return (StiStripLineXF)InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }
    }
}
