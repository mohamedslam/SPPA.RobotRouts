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

using System.Collections.Generic;

namespace Stimulsoft.Base
{
    /// <summary>
    /// Provides access to the application function.
    /// </summary>
    public interface IStiAppFunction : IStiAppCell
    {
        /// <summary>
        /// Returns a name of the function.
        /// </summary>
        /// <returns>The name of the function.</returns>
        string GetName();

        /// <summary>
        /// Invokes this function with specified arguments.
        /// </summary>
        /// <param name="arguments">The list of arguments.</param>
        /// <returns>The result of the function calling.</returns>
        object Invoke(IEnumerable<object> arguments);
    }
}