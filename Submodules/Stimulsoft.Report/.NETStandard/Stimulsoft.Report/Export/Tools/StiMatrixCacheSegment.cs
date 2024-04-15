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

namespace Stimulsoft.Report.Export
{
    public class StiMatrixCacheSegment
    {
        #region Fields
        private int segmentHeight;
        #endregion

        #region Properties
        private string cacheGuid;
        /// <summary>
        /// Gets or sets a cache guid of segment.
        /// </summary>
        public string CacheGuid
        {
            get
            {
                if (cacheGuid == null)
                    NewCacheGuid();

                return cacheGuid;
            }
            set
            {
                cacheGuid = value;
            }
        }

        private StiMatrixLineData[] lines;
        public StiMatrixLineData[] Lines
        {
            get
            {
                if (lines == null)
                    lines = new StiMatrixLineData[segmentHeight];

                return lines;
            }
            set
            {
                lines = value;
            }
        }

        public bool IsSaved { get; set; }
        #endregion

        #region Methods
        public void NewCacheGuid()
        {
            cacheGuid = global::System.Guid.NewGuid().ToString().Replace("-", "");
        }

        public void Clear()
        {
            if (lines == null) return;

            foreach (var line in lines)
            {
                if (line == null) continue;

                line.Bookmarks = null;
                line.BordersX = null;
                line.BordersY = null;
                line.Cells = null;
                line.CellsMap = null;
                line.CellStyles = null;
            }

            lines = null;
        }
        #endregion

        public StiMatrixCacheSegment(int height)
        {
            this.segmentHeight = height;
        }
    }
}