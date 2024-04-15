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
using System.ComponentModel;

namespace Stimulsoft.Report.Events
{
    /// <summary>
    /// Represents the method that handles the GetProperties event.
    /// </summary>
    public delegate void StiGetPropertiesEventHandler(object sender, StiGetPropertiesEventArgs e);

    /// <summary>
    /// Describes an argument for the event GetProperties.
    /// </summary>
    public class StiGetPropertiesEventArgs : EventArgs
    {
        public object[] SelectedObjects { get; set; }

        public PropertyDescriptorCollection Properties { get; set; }

        public System.Windows.Forms.PropertySort PropertySort { get; set; }

        public Base.StiGuiMode GuiMode { get; set; }

        public StiGetPropertiesEventArgs(object[] selectedObjects, PropertyDescriptorCollection properties, System.Windows.Forms.PropertySort propertySort, Base.StiGuiMode guiMode)
        {
            this.SelectedObjects = selectedObjects;
            this.Properties = properties;
            this.PropertySort = propertySort;
            this.GuiMode = guiMode;
        }
    }
}
