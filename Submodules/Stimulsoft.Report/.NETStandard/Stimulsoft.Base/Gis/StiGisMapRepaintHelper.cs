#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Stimulsoft.Base.Gis
{
    public static class StiGisMapRepaintHelper
    {
        #region Fields
        public delegate IStiGisMapSimpleView CreateViewDelegate(StiGeoMapProviderType provider, StiGisMapData data, InvokeTickDelegate invokeTick);
        public delegate IStiGisMapControl CreateWinFormsControlDelegate();
        public delegate IStiGisMapControl CreateWpfControlDelegate(object fontFamily);
        public delegate void InvokeTickDelegate();

        private static CreateViewDelegate createViewMethod;
        private static CreateWinFormsControlDelegate createWinFormsControlMethod;
        private static CreateWpfControlDelegate createWpfControlMethod;
        private static Dictionary<object, IStiGisMapSimpleView> cache = new Dictionary<object, IStiGisMapSimpleView>();
        private static bool isInit;
        #endregion

        #region Events
        public static event EventHandler Tick;
        #endregion

        #region Methods
        public static void Init()
        {
            if (isInit) return;

            try
            {
                var type = StiTypeFinder.GetType($"Stimulsoft.Map.Gis.StiGisBridge, Stimulsoft.Map, {StiVersion.VersionInfo}");
                if (type != null)
                {
                    var method = type.GetMethod("CreateDesignViewDefault", BindingFlags.Static | BindingFlags.Public);
                    if (method == null)
                        throw new Exception("Method 'CreateDesignViewDefault' not found");
                    createViewMethod = method.Invoke(null, null) as CreateViewDelegate;

                    method = type.GetMethod("CreateWinFormsControl", BindingFlags.Static | BindingFlags.Public);
                    if (method == null)
                        throw new Exception("Method 'CreateWinFormsControl' not found");
                    createWinFormsControlMethod = method.Invoke(null, null) as CreateWinFormsControlDelegate;

                    method = type.GetMethod("CreateWpfControl", BindingFlags.Static | BindingFlags.Public);
                    if (method == null)
                        throw new Exception("Method 'CreateWpfControl' not found");
                    createWpfControlMethod = method.Invoke(null, null) as CreateWpfControlDelegate;

                    isInit = true;
                }
            }
            catch
            {
            }
        }

        public static IStiGisMapControl CreateWinFormsControl()
        {
            return (createWinFormsControlMethod != null)
                ? createWinFormsControlMethod()
                : null ;
        }

        public static IStiGisMapControl CreateWpfControl(object fontFamily)
        {
            return (createWpfControlMethod != null)
                ? createWpfControlMethod(fontFamily)
                : null;
        }

        private static void InvokeTick()
        {
            Tick?.Invoke(null, EventArgs.Empty);
        }

        public static List<object> FetchAllComponents()
        {
            lock (cache)
            {
                if (cache.Count > 0)
                {
                    var result = new List<object>();
                    foreach (var pair in cache)
                    {
                        if (pair.Value.IsComplete)
                            result.Add(pair.Key);
                    }

                    return result;
                }
            }

            return null;
        }

        public static IStiGisMapSimpleView Get(object element, StiGeoMapProviderType provider, StiGisMapData data)
        {
            lock (cache)
            {
                IStiGisMapSimpleView view;
                if (cache.TryGetValue(element, out view))
                {
                    if (view.IsChanged(provider, data))
                    {
                        // restart data retrieval
                        view.Restart(provider, data);
                        return null;
                    }
                    else
                    {
                        if (view.IsComplete)
                            return view;
                    }
                }
                else
                {
                    if (createViewMethod == null)
                        throw new NotSupportedException();

                    view = createViewMethod(provider, data, InvokeTick);
                    cache.Add(element, view);
                    view.Run();

                    return null;
                }
            }

            return null;
        }

        public static IStiGisMapSimpleView GetForExport(StiGeoMapProviderType provider, StiGisMapData data)
        {
            return createViewMethod(provider, data, InvokeTick);
        }
        #endregion
    }
}
