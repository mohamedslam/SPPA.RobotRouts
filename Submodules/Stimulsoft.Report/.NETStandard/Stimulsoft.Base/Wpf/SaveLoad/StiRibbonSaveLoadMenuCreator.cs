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

namespace Stimulsoft.Base.Wpf.SaveLoad
{
    public static class StiRibbonSaveLoadMenuCreator
    {
        #region Properties
        public static bool AllowSaveThisFileWindow { get; private set; }
        #endregion

        #region Methods
        public static IStiRibbonOpenMenuControl CreateOpenControl(IStiRibbonMenuControl ribbonMenu)
        {
            try
            {
                var type = Type.GetType("Stimulsoft.Controls.Wpf.Ribbon22.StiRibbonOpenUserControl, Stimulsoft.Controls.Wpf");
                if (type == null) return null;

                var res = StiActivator.CreateObject(type, new object[] { ribbonMenu }) as IStiRibbonOpenMenuControl;
                AllowSaveThisFileWindow = true;

                return res;
            }
            catch
            {

            }

            return null;
        }

        public static IStiRibbonSaveMenuControl CreateSaveControl(IStiRibbonMenuControl ribbonMenu)
        {
            try
            {
                var type = Type.GetType("Stimulsoft.Controls.Wpf.Ribbon22.StiRibbonSaveAsUserControl, Stimulsoft.Controls.Wpf");
                if (type == null) return null;

                var res = StiActivator.CreateObject(type, new object[] { ribbonMenu }) as IStiRibbonSaveMenuControl;
                AllowSaveThisFileWindow = true;

                return res;
            }
            catch
            {

            }

            return null;
        }

        public static IStiSaveThisFileWindow CreateSaveThisFileWindow(IStiRibbonMenuDesignerControl designerControl)
        {
            try
            {
                var type = Type.GetType("Stimulsoft.Controls.Wpf.Ribbon22.StiSaveThisFileWindow, Stimulsoft.Controls.Wpf");
                if (type == null) return null;

                return StiActivator.CreateObject(type, new object[] { designerControl }) as IStiSaveThisFileWindow;
            }
            catch
            {

            }

            return null;
        }
        #endregion
    }
}