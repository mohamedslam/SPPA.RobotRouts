#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Base.Gis
{
    public interface IStiGisMapControl
    {
        void SetProviderType(StiGeoMapProviderType type, Color geometryColor, double geometryLineSize, 
            bool showPlacemark, StiLanguageType language, StiFontIcons icon, Color iconColor);

        void InitGeoCommands(List<string> commands, List<string> colors, List<object> lineSizes, List<string> descriptions);

        double Zoom { get; set; }

        bool AllowLocalCache { get; set; }
    }
}