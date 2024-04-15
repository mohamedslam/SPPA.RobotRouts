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

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
	public class StiMhtImageHost : StiHtmlImageHost
	{
		public override string GetImageString(Bitmap bmp)
		{
			var imageNumber = ImageCache.AddImageInt(bmp, ForcePng ? ImageFormat.Png : HtmlExport.imageFormat);
			var imageFormat = (ImageFormat)ImageCache.ImageFormatStore[imageNumber];

			var baseDir = string.Empty;
		    if (!string.IsNullOrEmpty(HtmlExport.fileName))
		        baseDir = Path.GetDirectoryName(HtmlExport.fileName);

		    if (baseDir.Length > 0 && baseDir[baseDir.Length - 1] != '\\')
		        baseDir += '\\';

			var dir = Path.GetFileNameWithoutExtension(HtmlExport.fileName) + ".files\\";

			var imageStr = Path.GetFileNameWithoutExtension(HtmlExport.fileName) + (imageNumber + 1) + "." + imageFormat.ToString();

			var output = "file:///" + baseDir + dir + imageStr;

			return StiExportUtils.StringToUrl(output.Replace("\\", "/").Replace(" ", "_"));
		}

		public StiMhtImageHost(StiHtmlExportService htmlExport) : base (htmlExport)
		{
			IsMhtExport = true;
			ForcePng = false;
		}
	}	
}
