#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Server 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

namespace Stimulsoft.Base.Plans
{
    public abstract class StiPlan
    {
        #region Consts
        public const int K = 1024;
        public const int M = 1024 * K;        
        public const int DefaultResourceSize = 2 * M;
        public const int DefaultItems = 20;
        #endregion

        #region Properties
        public abstract StiPlanIdent Ident { get; }

        public abstract string Name { get; }

        public abstract bool IsBI { get; }

        public virtual bool AllowWhiteLabel => false;

        public virtual bool AllowSchedulers => false;

        public abstract int? MaxUsers { get; }        
        #endregion
    }
}
