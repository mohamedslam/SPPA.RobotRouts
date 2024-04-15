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

namespace Stimulsoft.Base
{
	public static class StiFileUtils
	{
		public static bool ProcessReadOnly(string path)
		{
		    if (!File.Exists(path)) return false;

		    var attr = File.GetAttributes(path);
		    if ((attr & FileAttributes.ReadOnly) <= 0) return false;

		    File.SetAttributes(path, (FileAttributes)(attr - FileAttributes.ReadOnly));

		    return true;
		}

		public static bool IsReadOnly(string path)
		{
			if (!File.Exists(path)) return false;

			var attr = File.GetAttributes(path);
			return (attr & FileAttributes.ReadOnly) > 0;
		}
    }
}
