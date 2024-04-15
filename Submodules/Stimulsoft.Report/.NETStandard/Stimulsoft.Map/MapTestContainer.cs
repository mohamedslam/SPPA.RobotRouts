#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stimulsoft.Map
{
    public class MapTestContainer
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double? TextScale { get; set; }

        public List<StiMapSvg1> Paths { get; set; } = new List<StiMapSvg1>();
    }

    public class StiMapSvg1
    {
        #region Properties
        public string Key { get; set; }
        public string EnglishName { get; set; }
        public string Data { get; set; }

        [DefaultValue(null)]
        public string ISOCode { get; set; }
        public RectangleD? Rect { get; set; }

        [DefaultValue(false)]
        public bool SetMaxWidth { get; set; }

        [DefaultValue(false)]
        public bool SkipText { get; set; }

        [DefaultValue(null)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiTextHorAlignment? HorAlignment { get; set; }

        [DefaultValue(null)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiVertAlignment? VertAlignment { get; set; }
        #endregion

        #region Methods
        public override string ToString() => this.Key;
        #endregion
    }
}