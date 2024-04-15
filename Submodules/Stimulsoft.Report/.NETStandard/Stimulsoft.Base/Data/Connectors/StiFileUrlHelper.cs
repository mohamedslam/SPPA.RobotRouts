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
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Base
{
    public static class StiFileUrlHelper
    {
        public static byte[] Get(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            byte[] content = null;

            #region Load from File
            try
            {
                if (File.Exists(path))
                    content = File.ReadAllBytes(path);
            }
            catch
            {
            }
            #endregion

            #region Load from Url
            try
            {
                if (content == null)
                    content = StiDownloadCache.Get(path);
            }
            catch
            {
            }
            #endregion

            return content;
        }
    }
}
