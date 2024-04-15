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

using System;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
	public class StiComponentResourceHelper
    {
        public static object GetObject(ComponentResourceManager resources, string name)
        {
            var stepScaleName = StiScale.StepName;
            var resource = resources.GetObject($"{name}{stepScaleName}");
            if (resource != null)
                return resource;

            resource = resources.GetObject($"{name}");
            if (resource != null && resource is Image)
            {
                var image = resource as Image;
                var resizedImage = StiImageUtils.ResizeImage(image, StiScale.XXI(image.Width), StiScale.YYI(image.Height));
                image.Dispose();
                return resizedImage;
            }

            return resource;
        }

        public static Icon GetIcon(ComponentResourceManager resources, string name)
        {
            return GetObject(resources, name) as Icon;
        }

        public static Image GetImage(ComponentResourceManager resources, string name)
        {
            return GetObject(resources, name) as Image;
        }
    }
}
