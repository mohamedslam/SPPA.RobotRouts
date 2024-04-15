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

namespace Stimulsoft.Base.Server
{
    public interface IStiAccountUser
    {
        #region Properties
        byte[] Picture { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string UserName { get; set; }

        string UserKey { get; set; }

        string SessionKey { get; set; }

        DateTime? SessionDate { get; set; }
        
        DateTime Created { get; set; }

        StiDesignerSpecification DesignerSpecification { get; set; }

        bool Activated { get; set; }

        bool Expired { get; set; }

        int Days { get; set; }
        #endregion

        #region Methods
        void SaveToFile();
        #endregion
    }
}
