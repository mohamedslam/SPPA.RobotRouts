#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes the abstract class that allows to save / load pages.
	/// </summary>
	[StiServiceBitmap(typeof(StiSLService), "Stimulsoft.Report.Bmp.SL.SLPage.bmp")]
	public abstract class StiPageSLService : StiSLService
	{
		/// <summary>
		/// Saves a page in the stream.
		/// </summary>
		/// <param name="page">Page for saving.</param>
		/// <param name="stream">Stream to save page.</param>
		public abstract void Save(StiPage page, Stream stream);

		/// <summary>
		/// Loads a page from the stream.
		/// </summary>
		/// <param name="page">The page in which loading will be done.</param>
		/// <param name="stream">Stream to load pages.</param>
		public abstract void Load(StiPage page, Stream stream);
	}
}
