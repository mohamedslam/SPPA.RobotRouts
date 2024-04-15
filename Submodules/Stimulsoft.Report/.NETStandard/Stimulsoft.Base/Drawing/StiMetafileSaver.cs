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

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Class realize methods for saving metafile.
	/// </summary>
	public sealed class StiMetafileSaver
	{
		public static void Save(Stream stream, Metafile metafile)
		{
			Metafile tempMetafile;
			using (var bitmap = new Bitmap(1, 1))
			using (var g = Graphics.FromImage(bitmap))
			{
				var ipHdc = g.GetHdc();
                var unit = GraphicsUnit.Pixel;
				tempMetafile = new Metafile(stream, ipHdc, metafile.GetBounds(ref unit),  MetafileFrameUnit.Pixel);
				g.ReleaseHdc(ipHdc);
			}

			using (var g = Graphics.FromImage(tempMetafile))
			{
                g.DrawImage(
                    metafile,
                    metafile.GetMetafileHeader().Bounds.Left,
                    metafile.GetMetafileHeader().Bounds.Top,
                    metafile.Width,
                    metafile.Height);                
			}
		}

		public static void Save(string fileName, Metafile metafile)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			var stream = new FileStream(fileName, FileMode.Create);
			Save(stream, metafile);
			stream.Flush();
			stream.Close();
		}
	}
}
