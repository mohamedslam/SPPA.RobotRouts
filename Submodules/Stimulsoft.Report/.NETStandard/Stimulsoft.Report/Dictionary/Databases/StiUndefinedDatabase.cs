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


namespace Stimulsoft.Report.Dictionary
{
    public class StiUndefinedDatabase : StiSqlDatabase
	{
        #region Methods.override
        public override StiDatabase CreateNew()
        {
            return new StiUndefinedDatabase();
        }
        #endregion

		#region StiService.override
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Undefined SqlDatabase";
	    #endregion

		public StiUndefinedDatabase() : this(string.Empty, string.Empty)
		{
		}
		
		public StiUndefinedDatabase(string name, string connectionString) : base(name, connectionString)
		{
		}

        public StiUndefinedDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias, connectionString, promptUserNameAndpassword)
		{
		}

        public StiUndefinedDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }	
	}
}