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

namespace Stimulsoft.Report.Components.Table
{
    public interface IStiTableCell
    {
        #region Properties
        int[] JoinCells { get; set; }

        int ParentJoin { get; set; }

        bool Join { get; set; }

        int ID { get; set; }

        int JoinWidth { get; set; }

        int JoinHeight { get; set; }

        bool Merged { get; }

        bool ChangeTopPosition { get; }

        bool ChangeLeftPosition { get; }

        bool ChangeRightPosition { get; }

        StiTablceCellType CellType { get; set; }

        StiDockStyle CellDockStyle { get; set; }

        int Column { get; set; }

        bool FixedWidth { get; set; }

        object TableTag { get; set; }

        StiComponent ParentJoinCell { get; set; }
        #endregion

        #region Methods
        StiComponent GetJoinComponentByGuid(int id);

        StiComponent GetJoinComponentByIndex(int index);

        bool ContainsGuid(int id);

        void SetJoinSize();

        double GetRealHeightAfterInsertRows();

        double GetRealHeight();

        double GetRealTop();

        double GetRealWidth();

        double GetRealLeft();
        #endregion
    }
}
