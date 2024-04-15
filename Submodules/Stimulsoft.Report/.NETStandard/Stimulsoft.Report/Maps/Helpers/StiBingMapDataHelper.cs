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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Maps.Helpers
{
    public static class StiBingMapDataHelper
    {
        #region Methods
        public static void CreateImageBackground(StiMap map, List<string> pushPins, Size size)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                try
                {
                    var image = StiBingMapHelper.GetImage(size, pushPins);
                    StiMapDrawingCache.StoreLastImage(map, image);
                    StiComponentProgressHelper.Remove(map);
                }
                catch
                {
                    StiComponentProgressHelper.Remove(map, true);
                }
            };
            StiComponentProgressHelper.Add(map);
            worker.RunWorkerAsync();
        }
        #endregion
    }
}
