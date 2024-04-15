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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base.Services;

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes an abstract class that allows to save, load and merge dictionaries of data.
	/// </summary>
	[StiServiceBitmap(typeof(StiSLService), "Stimulsoft.Report.Bmp.SL.SLDictionary.bmp")]
	public abstract class StiDictionarySLService : StiSLService
	{
		/// <summary>
		/// Saves a dictionary of data in the stream.
		/// </summary>
		/// <param name="dictionary">Dictionary of the data for saving.</param>
		/// <param name="stream">Stream to save dictionary of data.</param>
		public abstract void Save(StiDictionary dictionary, Stream stream);

		/// <summary>
		/// Loads a dictionay of data from the stream.
		/// </summary>
		/// <param name="dictionary">Dictionary of the data in which loading will be done.</param>
		/// <param name="stream">Stream to load dictionary of data.</param>
		public abstract void Load(StiDictionary dictionary, Stream stream);

		/// <summary>
		/// Merge dictionary of data from the stream.
		/// </summary>
		/// <param name="dictionary">Merge the dictionary of the data.</param>
		/// <param name="stream">Stream to merge the dictionary of the data.</param>
		public abstract void Merge(StiDictionary dictionary, Stream stream);
	}
}
