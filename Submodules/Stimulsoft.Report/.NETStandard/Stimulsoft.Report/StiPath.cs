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
using System.IO;
namespace Stimulsoft.Report
{
	public static class StiPath
	{
		public static string GetPath(string path)
		{	
			if (string.IsNullOrEmpty(path))return string.Empty;

			string dirPath = string.Empty;
			
			// If path is not absolute (path root is absent)
			if (Path.GetPathRoot(path).Trim() == "")
			{
				// Convert it to absolut by adding app dir path
				dirPath = StiOptions.Configuration.ApplicationDirectory;
				
				// Make sure app dir path contains trailing slash
                if (!(dirPath.EndsWith("" + Path.DirectorySeparatorChar, StringComparison.InvariantCulture)
                    || dirPath.EndsWith("" + Path.AltDirectorySeparatorChar, StringComparison.InvariantCulture)))
					dirPath += Path.DirectorySeparatorChar;
				dirPath += path;
			}
			else
			{
				dirPath = path;
			}
			
			// Fix path (if both type of directory separators used - change to one)
            if (dirPath.IndexOf(Path.DirectorySeparatorChar) >= 0 &&
                path.IndexOf(Path.DirectorySeparatorChar) >= 0)
			{
				dirPath = dirPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
			return dirPath;
			
		}
	}
}