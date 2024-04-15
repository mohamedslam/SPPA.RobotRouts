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

using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    public class StiMultiConditionContainer
	{
	    [StiSerializable(StiSerializationVisibility.List)]
		public StiFiltersCollection Filters { get; set; } = new StiFiltersCollection();

	    /// <summary>
		/// Gets or sets filter mode.
		/// </summary>
		[DefaultValue(StiFilterMode.And)]
		[StiSerializable]
		[Browsable(false)]
		public StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

	    [StiSerializable]
		[DefaultValue(true)]
		public bool Enabled { get; set; } = true;

	    /// <summary>
		/// Gets or sets a color to draw text.
		/// </summary>
		[StiSerializable]
		public Color TextColor { get; set; } = Color.Red;

	    /// <summary>
		/// Gets or sets a color to draw background of text.
		/// </summary>
		[StiSerializable]
		public Color BackColor { get; set; } = Color.Transparent;

	    /// <summary>
		/// Gets or sets font of text.
		/// </summary>
		[StiSerializable]
		public Font Font { get; set; } = new Font("Arial",8);

	    [StiSerializable]
		[DefaultValue(false)]
		public bool CanAssignExpression { get; set; }

	    [StiSerializable]
		[DefaultValue("")]
		public string AssignExpression { get; set; } = string.Empty;

	    [StiSerializable]
        [DefaultValue("")]
        public string Style { get; set; } = string.Empty;

	    [StiSerializable]
        [DefaultValue(StiConditionBorderSides.NotAssigned)]
        public StiConditionBorderSides BorderSides { get; set; } = StiConditionBorderSides.NotAssigned;

	    [StiSerializable]
        [DefaultValue(StiConditionPermissions.All)]
        public StiConditionPermissions Permissions { get; set; } = StiConditionPermissions.All;

	    [StiSerializable]
        [DefaultValue(false)]
        public bool BreakIfTrue { get; set; }
	}
}
