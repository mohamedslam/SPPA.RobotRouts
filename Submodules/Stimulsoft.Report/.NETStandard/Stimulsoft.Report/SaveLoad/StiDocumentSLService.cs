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

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes the abstract class that allows to save / load documents.
	/// </summary>
	[StiServiceBitmap(typeof(StiSLService), "Stimulsoft.Report.Bmp.SL.SLDocument.bmp")]
	public abstract class StiDocumentSLService : StiSLService
	{
		/// <summary>
		/// Saves a document in the stream.
		/// </summary>
		/// <param name="report">Rendered for saving.</param>
		/// <param name="stream">Stream to save documents.</param>
		public abstract void Save(StiReport report, Stream stream);

		/// <summary>
		/// Loads a document from the stream.
		/// </summary>
		/// <param name="report">Report in which loading will be done.</param>
		/// <param name="stream">Stream to load documents.</param>
		public abstract void Load(StiReport report, Stream stream);

		/// <summary>
		/// If the provider handles with multitude of files then true. If does not then false.
		/// </summary>
		public abstract bool MultiplePages { get; }		
	}
}
