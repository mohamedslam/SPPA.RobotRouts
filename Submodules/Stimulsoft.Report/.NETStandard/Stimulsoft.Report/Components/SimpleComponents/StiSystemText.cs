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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class that realizes a category - SystemText. 
    /// </summary>
    [StiToolbox(false)]
	public class StiSystemText : StiText
	{
        #region StiComponent.Properties
        public override StiComponentId ComponentId => StiComponentId.StiSystemText;
        #endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.SystemText;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiSystemText");
		#endregion

		#region this
		/// <summary>
		/// Creates a new component of the type StiSystemText.
		/// </summary>
		public StiSystemText() : this(RectangleD.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiSystemText.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiSystemText(RectangleD rect) : this(rect, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiSystemText.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		/// <param name="text">Text expression.</param>
		public StiSystemText(RectangleD rect, string text) : base(rect, text)
		{
			PlaceOnToolbox = true;
		}
		#endregion
	}
}
