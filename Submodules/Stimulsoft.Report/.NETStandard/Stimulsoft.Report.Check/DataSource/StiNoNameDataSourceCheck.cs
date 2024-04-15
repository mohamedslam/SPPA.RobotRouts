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

using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiNoNameDataSourceCheck : StiDataSourceCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataSource", "StiNoNameDataSourceCheckShort"));
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataSource", "StiNoNameDataSourceCheckLong"));
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Error;
            }
        }
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            StiDataSource dataSource = obj as StiDataSource;
            if (dataSource != null && string.IsNullOrEmpty(dataSource.Name))
            {
                StiNoNameDataSourceCheck check = new StiNoNameDataSourceCheck();
                check.Element = obj;

                check.Actions.Add(new StiGenerateNewNameDataSourceAction());
                check.Actions.Add(new StiDeleteDataSourceAction());

                return check;
            }
            return null;
        }
        #endregion
    }
}