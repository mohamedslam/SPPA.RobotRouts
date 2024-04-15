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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
	/// <summary>
	/// Class store information about form parameters.
	/// </summary>
	public class StiFormSettings
	{
		public static void Save(Form form)
		{
			Save(form.Name, form);
		}

		public static void Save(string name, Form form)
		{
		    if (StiOptions.Designer.DontSaveFormsSettings) return;

			var state = new StiWindowsSettings.StiWindowState
			{
				X = (int)(form.Left / StiScale.Factor),
				Y = (int)(form.Top / StiScale.Factor),
				Width = (int)(form.Width / StiScale.Factor),
				Height = (int)(form.Height / StiScale.Factor)
			};
			StiWindowsSettings.Set(name, state);
			StiWindowsSettings.Save();
		}

		public static void Load(Form form, bool skipLocation = false, bool skipSize = false)
		{
			Load(form.Name, form, skipLocation, skipSize);
		}

		public static void Load(string name, Form form, bool skipLocation = false, bool skipSize = false)
		{
		    if (StiOptions.Designer.DontSaveFormsSettings) return;

			var state = StiWindowsSettings.Get(name);
			if (state == null) return;

			int width = StiScale.XXI(state.Width);
			int height = StiScale.YYI(state.Height);

            width = Math.Min(width, Screen.FromControl(form).Bounds.Width);
		    height = Math.Min(height, Screen.FromControl(form).Bounds.Height);

		    form.AutoScaleMode = AutoScaleMode.None;
            if (!skipSize)
            {
                form.Width = width;
                form.Height = height;
            }

            if (!skipLocation && Screen.AllScreens.Length == 1)
            {
				var left = StiScale.XXI(state.X);
				var top = StiScale.YYI(state.Y);

				var primary = Screen.PrimaryScreen;
                if (primary.Bounds.IntersectsWith(new Rectangle(state.X, top, width, height)))
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.Left = left;
                    form.Top = top;
                }
            }
		}

	    public static Size LoadSize(Form form)
	    {
	        if (StiOptions.Designer.DontSaveFormsSettings) return form.Size;

			var state = StiWindowsSettings.Get(form.Name);
			if (state == null) return form.Size;

            int width = StiScale.XXI(state.Width);
			int height = StiScale.YYI(state.Height);

            width = Math.Min(width, Screen.FromControl(form).Bounds.Width);
	        height = Math.Min(height, Screen.FromControl(form).Bounds.Height);

	        return new Size(width, height);
	    }
	}
}
