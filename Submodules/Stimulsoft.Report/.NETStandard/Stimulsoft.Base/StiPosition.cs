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

namespace Stimulsoft.Base
{
    /// <summary>
    /// Saving of the positions in text.
    /// </summary>
    public struct StiPosition
    {
        /// <summary>
        /// Gets or sets line in text.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets column in text.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Creates position in text.
        /// </summary>
        /// <param name="line">Line in text.</param>
        /// <param name="column">Column in text.</param>
        public StiPosition(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }
    };
}