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
using Stimulsoft.Report.Dictionary;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Events
{
	public delegate void StiEditCustomConnectionEventHandler(object sender, StiEditCustomConnectionEventArgs e);

	public class StiEditCustomConnectionEventArgs : EventArgs
	{
        #region Properties
        public StiDatabase Database { get; }

        public StiDictionary Dictionary { get; }

        public object Designer { get; }

        public bool IsNewConnection { get; }

        public DialogResult? DialogResult { get; set; }
        #endregion

        public StiEditCustomConnectionEventArgs(StiDatabase database, StiDictionary dictionary, object designer, bool isNewConnection)
		{
			this.Database = database;
            this.Dictionary = dictionary;
            this.Designer = designer;
            this.IsNewConnection = isNewConnection;
        }
	}
}