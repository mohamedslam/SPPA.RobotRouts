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
using System.IO;

namespace Stimulsoft.Base
{
    public sealed class StiDataAdaptersRepositary
	{
	    public static string GetRootDataAdaptersFolder()
	    {
	        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stimulsoft", "DataAdapters");

	        if (!Directory.Exists(directory))
	            Directory.CreateDirectory(directory);

	        return directory;
	    }

	    public static string GetDataAdapterFolder(string assemblyName)
	    {
	        var directory = GetRootDataAdaptersFolder();
	        var file = assemblyName.EndsWithInvariantIgnoreCase("exe") || assemblyName.EndsWithInvariantIgnoreCase("dll") 
	            ? Path.GetFileNameWithoutExtension(assemblyName) 
	            : assemblyName;

	        return Path.Combine(directory, file);
	    }

        public static string GetDataAdapterVersionFolder(string assemblyName, string version)
        {
            var directory = GetRootDataAdaptersFolder();
            var file = assemblyName.EndsWithInvariantIgnoreCase("exe") || assemblyName.EndsWithInvariantIgnoreCase("dll")
                ? Path.GetFileNameWithoutExtension(assemblyName)
                : assemblyName;

            return Path.Combine(directory, $"{file}-{version}");
        }

        public static string GetDataAdapterPathFromRoot(string assembly)
        {
            var path = Path.Combine(GetRootDataAdaptersFolder(), assembly);
            if (File.Exists(path))
                return path;
            return null;
        }

        public static string GetDataAdapterPath(string assembly)
	    {
	        foreach (var directory in Directory.EnumerateDirectories(GetRootDataAdaptersFolder()))
	        {
	            var path = Path.Combine(directory, assembly);
                if (File.Exists(path))
                    return path;
	        }
            return null;
	    }
	}
}
