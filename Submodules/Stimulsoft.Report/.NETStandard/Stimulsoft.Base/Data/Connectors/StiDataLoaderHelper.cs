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
using System.Linq;
using System.IO;
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;
using System.Net;
using System.Collections.Specialized;

namespace Stimulsoft.Base
{
    public static class StiDataLoaderHelper
    {
        #region class Data
        public class Data
        {
            public string Name { get; }

            public byte[] Array { get; }

            public List<Data> ToList()
            {
                return new List<Data> { this };
            }

            public Data(string name, byte[] array)
            {
                this.Name = name;
                this.Array = array;
            }
        }
        #endregion

        public static List<Data> LoadMultiple(string path, string filter)
        {
            if (string.IsNullOrEmpty(path)) 
                return null;

            if (Directory.Exists(path))
            {
                return Directory
                    .EnumerateFiles(path, filter, SearchOption.TopDirectoryOnly)
                    .Select(f => new Data(Path.GetFileNameWithoutExtension(f), File.ReadAllBytes(f))).ToList();
            }

            var data = LoadSingle(path);
            return data?.ToList();
        }

        public static Data LoadSingle(string path, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            if (!string.IsNullOrEmpty(path) && !(path.StartsWith(@"http://") || path.StartsWith(@"https://")) && File.Exists(path))
                return new Data(Path.GetFileNameWithoutExtension(path), File.ReadAllBytes(path));

            var uri = new Uri(path);
            return new Data(Path.GetFileNameWithoutExtension(uri.LocalPath), StiBytesFromURL.Load(path, cookieContainer, headers));
        }
    }
}
