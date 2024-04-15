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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Engine
{
	public class StiComponentV1Builder : StiV1Builder
	{
		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			masterComp.IsRendered = false;
			masterComp.RenderedCount = 0;

			StiComponentHelper.FillComponentPlacement(masterComp);
		}

		/// <summary>
		/// Clears a component after rendering.
		/// </summary>
		public override void UnPrepare(StiComponent masterComp)
		{
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent into consideration and without taking 
		/// Conditions into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">Rendered component.</param>
		/// <param name="outContainer">Panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			renderedComponent = masterComp.Clone(false) as StiComponent;
			outContainer.Components.Add(renderedComponent);			
			renderedComponent.InvokeEvents();
			return true;
		}

		/// <summary>
		/// Renders a component in the specified container with taking events generation into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component which is being rendered.</param>
		/// <param name="outContainer">A container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
		public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			#region Conditions
			IStiTextBrush textBrush = masterComp as IStiTextBrush;
			IStiBrush brush = masterComp as IStiBrush;
			IStiFont font = masterComp as IStiFont;

			StiBrush savedTextBrush = null;
			StiBrush savedBrush = null;
			Font savedFont = null;

			if (textBrush != null)savedTextBrush = textBrush.TextBrush;
			if (brush != null)savedBrush = brush.Brush;
			if (font != null)savedFont = font.Font;
			bool savedEnabled = masterComp.Enabled;
			#endregion
								
			masterComp.InvokeBeforePrint(masterComp, EventArgs.Empty);
			
			bool result = true;
			if (masterComp.IsEnabled)
			{
				masterComp.RenderedCount++;
				bool isNewGuidCreated = masterComp.DoBookmark();
				masterComp.DoPointer(!isNewGuidCreated);
				result = masterComp.InternalRender(ref renderedComponent, outContainer);
			}

			masterComp.InvokeAfterPrint(masterComp, EventArgs.Empty);
			
			#region Conditions
			if (textBrush != null)textBrush.TextBrush = savedTextBrush;
			if (brush != null)brush.Brush = savedBrush;
			if (font != null)font.Font = savedFont;
			masterComp.Enabled = savedEnabled;
			#endregion

			return result;
		}

		/// <summary>
		/// Renders a component in the specified container with taking events generation into consideration.
		/// </summary>
		/// <param name="outContainer">A Panel in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering is finished or not.</returns>
		public override bool Render(StiComponent masterComp, StiContainer outContainer)
		{
			StiComponent renderedComponent = null;
			return masterComp.Render(ref renderedComponent, outContainer);
		}
		#endregion
	}
}
