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
using Stimulsoft.Report.Components;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Engine
{
	public class StiImageV1Builder : StiViewV1Builder
	{
		#region Methods.Render
		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			var masterImage = masterComp as StiImage; 
			var result = base.InternalRender(masterImage, ref renderedComponent, outContainer);

			var image = renderedComponent as StiImage;

			#region Correct image for break
			if (image.ExistImageToDraw() && masterImage.Stretch && image.CanBreak)
			{
				var rect = masterImage.GetPaintRectangle(true, false);
				var bitmap = new Bitmap((int)Math.Round((decimal)rect.Width), (int)Math.Round((decimal)rect.Height));
			    using (var g = Graphics.FromImage(bitmap))
			    {
			        using (var gdiImage = image.TakeGdiImage())
			        {
			            if (gdiImage != null)
			                g.DrawImage(gdiImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
			        }
			    }

			    image.PutImageToDraw(bitmap);
			}
			#endregion

			return result;
		}
		#endregion
	}
}
