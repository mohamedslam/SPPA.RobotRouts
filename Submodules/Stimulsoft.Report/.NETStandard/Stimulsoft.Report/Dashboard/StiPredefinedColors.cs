#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System.Collections.Generic;

using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiPredefinedColors
    {
        public static List<Color[]> Sets = new List<Color[]>
        {
            StiColor.Get("2f528f", "3960a7", "406dbb", "6d89cb", "9eadd8", "c0c9e4"),
            StiColor.Get("ae5a21", "ca6a28", "e2772e", "ef9164", "f3b29a", "f6ccbe"),
            StiColor.Get("787878", "8c8c8c", "9d9d9d", "b1b1b1", "c6c6c6", "d8d8d8"),
            StiColor.Get("bc8c00", "daa400", "f3b700", "ffc859", "ffd695", "ffe2bc"),
            StiColor.Get("41719c", "4c84b6", "5694cb", "7aa9da", "a4c0e3", "c4d5eb"),
            StiColor.Get("507e32", "5f933b", "6ba543", "88b76e", "acca9e", "c9dbc1"),
            StiColor.Get("5f5f5f", "b3b3b3", "898989", "212121", "dadada", "aaaaaa"),
            StiColor.Get("c0c9e4", "9eadd8", "6d89cb", "406dbb", "3960a7", "2f528f"),
            StiColor.Get("f6ccbe", "f3b29a", "ef9164", "e2772e", "ca6a28", "ae5a21"),
            StiColor.Get("d8d8d8", "c6c6c6", "b1b1b1", "9d9d9d", "8c8c8c", "787878"),
            StiColor.Get("ffe2bc", "ffd695", "ffc859", "f3b700", "daa400", "bc8c00"),
            StiColor.Get("c4d5eb", "a4c0e3", "7aa9da", "5694cb", "4c84b6", "41719c"),
            StiColor.Get("c9dbc1", "acca9e", "88b76e", "6ba543", "5f933b", "507e32")
        };

        public static List<Color[]> NegativeSets = new List<Color[]>
        {
            StiColor.Get("ee0e1b", "d20213", "b3030f", "9d040c", "7f0005", "6b0004"),
            StiColor.Get("f36642", "f24e29", "e64926", "d74222", "c93c1e", "af3017"),
            StiColor.Get("de3a70", "c81a57", "c81a57", "b21852", "9c144e", "780d46"),
            StiColor.Get("718792", "5b7481", "4f6570", "41525b", "344047", "252e32")
        };
    }
}
