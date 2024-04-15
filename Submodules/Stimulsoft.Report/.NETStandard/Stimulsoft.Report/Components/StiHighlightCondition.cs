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
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// This class is Obsolete. Please use Conditions.
	/// </summary>
	[RefreshProperties(RefreshProperties.All)]
	public class StiHighlightCondition : ICloneable
	{
		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			var hl =  (StiHighlightCondition)MemberwiseClone();

		    hl.TextBrush =	(StiBrush)TextBrush.Clone();
			hl.Brush =		(StiBrush)Brush.Clone();
			hl.Condition =	(StiExpression)Condition.Clone();

			return hl;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The brush of the component, which is used to display text.
		/// </summary>
		[StiCategory("Appearance")]
		[StiSerializable]
		[Description("The brush of the component, which is used to display text.")]
		public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.Red);

		/// <summary>
		/// The brush, which is used to draw background.
		/// </summary>
		[StiCategory("Appearance")]
		[StiSerializable]
		[Description("The brush, which is used to draw background.")]
		public StiBrush Brush { get; set; } = new StiSolidBrush(Color.Transparent);

	    /// <summary>
		/// Gets or sets the expression for calculation of the condition.
		/// </summary>
		[StiCategory("Behavior")]
		[StiSerializable]
		[Description("Gets or sets the expression for calculation of the condition.")]
		public StiExpression Condition { get; set; } = new StiExpression();

	    /// <summary>
		/// Gets or sets font of component.
		/// </summary>
		[StiCategory("Appearance")]
		[StiSerializable]
		[Description("Gets or sets font of component.")]
		public Font Font { get; set; } = new Font("Arial",8);
	    #endregion
    }
}