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
using System.Linq;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Stimulsoft.Base
{
	/// <summary>
	/// Class contains statistic methods to find assemblies.
	/// </summary>
	public sealed class StiAssemblyFinder
	{
		/// <summary>
		/// Gets assembly by it name.
		/// </summary>
		public static Assembly GetAssembly(string assemblyName, bool useAssemblyFromDataAdaptersRepository = true)
		{
            var resCurrentDirectory = Directory.GetCurrentDirectory();

            try
            {
                var location = typeof(StiAssemblyFinder).Assembly.Location;
                if (!string.IsNullOrWhiteSpace(location) && StiBaseOptions.AllowSetCurrentDirectory)
                {
                    try
                    {
                        location = Path.GetDirectoryName(location);
                        Directory.SetCurrentDirectory(location);
                    }
                    catch
                    {
                    }
                }

                var asmName = assemblyName;
                if (asmName.EndsWithInvariantIgnoreCase(".dll") || asmName.EndsWithInvariantIgnoreCase(".exe"))
                    asmName = Path.GetFileNameWithoutExtension(assemblyName);

                var assembly = GetAssemblyFromCurrentDomain(asmName);
                if (assembly != null)
                    return assembly;

                if (useAssemblyFromDataAdaptersRepository)
                    assembly = GetAssemblyFromDataAdaptersRepository(assemblyName);
                if (assembly != null)
                    return assembly;

                assembly = GetAssemblyFromAssemblyLoad(asmName);
                if (assembly != null)
                    return assembly;

                assembly = GetAssemblyFromAssemblyLoadWithSpecifiedVersion(asmName);
                if (assembly != null)
                    return assembly;

                assembly = GetAssemblyFromAssemblyLoadFrom(assemblyName);
                if (assembly != null)
                    return assembly;

                if (assemblyName != asmName)
                {
                    assembly = GetAssemblyFromAssemblyLoad(assemblyName);
                    if (assembly != null)
                        return assembly;
                }

                return GetAssemblyFromAssemblyLoadWithPartialName(asmName);
            }
            finally
            {
                if (StiBaseOptions.AllowSetCurrentDirectory)
                {
                    Directory.SetCurrentDirectory(resCurrentDirectory);
                }
            }
		}

	    private static Assembly GetAssemblyFromCurrentDomain(string name)
	    {
	        return string.IsNullOrEmpty(name)
	            ? null
	            : AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == name);
	    }

	    private static Assembly GetAssemblyFromDataAdaptersRepository(string name)
	    {
	        try
	        {
	            var assemblyPath = StiDataAdaptersRepositary.GetDataAdapterPath(name);
	            if (assemblyPath != null && File.Exists(assemblyPath))
	                return Assembly.LoadFrom(assemblyPath);
	        }
            catch (Exception)
            {
            }
	        return null;
	    }

        internal static Assembly GetAssemblyFromDataAdaptersFolder(string assemblyName, string assemblyFolder)
        {
            try
            {
                var path = Path.Combine(assemblyFolder, assemblyName);
                var assemblyPath = StiDataAdaptersRepositary.GetDataAdapterPathFromRoot(path);
                if (assemblyPath != null && File.Exists(assemblyPath))
                    return Assembly.LoadFrom(assemblyPath);
            }
            catch (Exception)
            {
            }
            return null;
        }

        private static Assembly GetAssemblyFromAssemblyLoad(string name)
	    {
	        try
	        {
	            return Assembly.Load(name);
	        }
	        catch (FileNotFoundException)
	        {
	        }
	        return null;
	    }

	    private static Assembly GetAssemblyFromAssemblyLoadWithSpecifiedVersion(string name)
	    {
	        try
	        {
	            if (name.ToLowerInvariant().StartsWith("stimulsoft", StringComparison.InvariantCulture))
	            {
	                var version = typeof(StiAssemblyFinder).AssemblyQualifiedName;
	                var index = version.IndexOf(",", StringComparison.InvariantCulture);
	                version = version.Substring(index + 1);
	                version = version.Replace("Stimulsoft.Base", name);
	                return Assembly.Load(version);
	            }
	        }
	        catch (FileNotFoundException)
	        {
	        }
            return null;
	    }

	    private static Assembly GetAssemblyFromAssemblyLoadFrom(string name)
	    {
	        try
	        {
	            if (File.Exists(name))
	                return Assembly.LoadFrom(name);
	            
	        }
	        catch (FileNotFoundException)
	        {
	        }
            return null;
	    }

	    private static Assembly GetAssemblyFromAssemblyLoadWithPartialName(string name)
	    {
#pragma warning disable 618,612
	        return Assembly.LoadWithPartialName(name);
	    }
	}
}
