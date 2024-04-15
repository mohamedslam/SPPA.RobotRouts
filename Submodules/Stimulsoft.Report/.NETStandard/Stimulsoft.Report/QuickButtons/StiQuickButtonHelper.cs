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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.QuickButtons
{
	/// <summary>
	/// Summary description for StiQuickButtonHelper.
	/// </summary>
	public class StiQuickButtonHelper
	{		
		/// <summary>
		/// Returns quick button rectangle.
		/// </summary>
		/// <param name="buttonIndex">An index of the quick button.</param>
		/// <param name="rect">A rectangle of the component.</param>
		/// <returns>Returns a rectangle of the quick button.</returns>
		public static RectangleD GetQuickButtonRect(StiQuickButton[] buttons, int buttonIndex, RectangleD rect)
		{
			var index = 0;
			var posLeft = rect.Left + StiScale.I1;
			var posRight = rect.Right - StiScale.I11;
			foreach (var button in buttons)
			{
				if (index == buttonIndex)
				{
                    if (StiOptions.Viewer.Pins.QuickButtonsRightToLeft)
                    {
                        var pos = posLeft;

                        if (button.Alignment == StiQuickButtonAlignment.Left)
                            pos = posRight;

                        return new RectangleD(pos, rect.Top + StiScale.I3, StiScale.I10, StiScale.I10);
                    }
                    else
                    {
                        var pos = posRight;

                        if (button.Alignment == StiQuickButtonAlignment.Left)
                            pos = posLeft;

                        return new RectangleD(pos, rect.Top + StiScale.I3, StiScale.I10, StiScale.I10);
                    }                    
				}

                if (StiOptions.Viewer.Pins.QuickButtonsRightToLeft)
                {
                    if (button.Alignment == StiQuickButtonAlignment.Right)
                        posLeft += StiScale.I12;
                    else
                        posRight -= StiScale.I12;
                }
                else
                {
                    if (button.Alignment == StiQuickButtonAlignment.Left)
                        posLeft += StiScale.I12;
                    else
                        posRight -= StiScale.I12;
                }

				index++;
			}
			return RectangleD.Empty;
		}

		public static StiQuickButton[] GetQuickButtons(Type type)
		{
			var attrs = type.GetCustomAttributes(typeof(StiQuickButtonAttribute), false) as StiQuickButtonAttribute[];

            if (attrs == null || attrs.Length == 0)
                return null;

            if (attrs.Length > 1)
				Array.Sort(attrs, new StiQuickButtonAttribute.Sorter());

			var quickButtons = new StiQuickButton[attrs.Length];

			var index = 0;
			foreach (var attr in attrs)
			{
				var quickButtonType = Type.GetType(attr.QuickButtonTypeName);
				quickButtons[index] = StiActivator.CreateObject(quickButtonType, new object[0]) as StiQuickButton;
                index++;
			}

			return quickButtons;
		}

		/// <summary>
		/// Checks visibility of the quick button rectangle.
		/// </summary>
		/// <param name="rect">A rectangle of the component.</param>
		public static bool CheckVisibleQuickButtons(RectangleD rect)
		{
            switch(StiOptions.Designer.ShowQuickButtons)
            {
                case StiQuickButtonVisibility.None:
                    return false;

                case StiQuickButtonVisibility.Always:
                    return rect.Height >= StiScale.I4 && rect.Width >= StiScale.I4;

                case StiQuickButtonVisibility.HideSmallSize:
                    return rect.Height >= StiScale.I(25) && rect.Width >= StiScale.I(25);

                default :
                    return rect.Height >= StiScale.I4;
            }			
		}
	}
}