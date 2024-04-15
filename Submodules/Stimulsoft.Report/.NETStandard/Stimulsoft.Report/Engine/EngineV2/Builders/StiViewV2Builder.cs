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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiViewV2Builder : StiComponentV2Builder
	{
		public override StiComponent InternalRender(StiComponent masterComp)
		{
			var masterView = masterComp as StiView;
			var imageComp = (StiView)masterView.Clone(false);

            if (StiOptions.Engine.ImageCache.Enabled)
            {
                var masterImage = masterView as StiImage;
                if (!(masterImage != null && masterImage.ExistImage()))
                {
                    imageComp.ResetImageToDraw();
                    imageComp.Guid = null;
                }
            }

            if (!imageComp.ExistImageToDraw())
		        imageComp.PutImageToDraw(masterView.GetImageFromSource());

		    return imageComp;
		}
	}
}
