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

namespace Stimulsoft.Base.Design
{
    /// <summary>
    /// Represents the method that handles the StiProcessDescriptionEvent.
    /// </summary>
    public delegate void StiProcessDescriptionEventHandler(object sender, StiProcessDescriptionEventArgs e);

    /// <summary>
    /// Describes an argument for the event StiProcessDescriptionEvent.
    /// </summary>
    public class StiProcessDescriptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets component type.
        /// </summary>
        public Type ComponentType { get; }

        /// <summary>
        /// Gets or sets property description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the StiProcessDescriptionEventArgs class.
        /// </summary>
        public StiProcessDescriptionEventArgs(Type componentType, string name, string description)
        {
            this.ComponentType = componentType;
            this.Name = name;
            this.Description = description;
        }
    }
}
