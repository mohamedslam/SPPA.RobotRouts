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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Base.Helpers
{
    public static class StiComponentProgressHelper
    {
        #region Consts
        public const float ProgressDelta = 10;
        public const int TimerInterval = 50;
        #endregion

        #region Fields
        private static Timer timer = new Timer();
        private static bool lockCompletedProgressHandler;
        private static object lockObject = new object();
        private static ObservableCollection<IStiAppComponent> hash = new ObservableCollection<IStiAppComponent>();
        private static Dictionary<IStiAppComponent, long> dictTick = new Dictionary<IStiAppComponent, long>();
        #endregion

        #region Properties
        public static float CurrentValue { get; set; }
        #endregion

        #region Events
        #region Tick
        public static event EventHandler Tick;

        private static void InvokeTick()
        {
            Tick?.Invoke(null, EventArgs.Empty);
        }
        #endregion

        #region CompletedProgress
        public static event EventHandler CompletedProgress;

        private static void InvokeCompletedProgress(object comp)
        {
            if (lockCompletedProgressHandler) return;

            CompletedProgress?.Invoke(comp, EventArgs.Empty);
        }
        #endregion
        #endregion

        #region Handlers
        private static void ActiveProgress_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
                InvokeCompletedProgress(e.OldItems[0]);
            
            if (hash.Count > 0)
                timer.Start();
            else
                timer.Stop();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            CurrentValue += ProgressDelta;

            if (CurrentValue >= 360)
                CurrentValue = 0;

            InvokeTick();
        }
        #endregion

        #region Methods
        public static void Init()
        {
            timer.Interval = TimerInterval;
            timer.Tick += Timer_Tick;
            hash.CollectionChanged += ActiveProgress_CollectionChanged;
        }

        public static void Add(IStiAppComponent comp)
        {
            lock (lockObject)
            {
                if (!hash.Contains(comp))
                    hash.Add(comp);

                if (!dictTick.ContainsKey(comp))
                    dictTick.Add(comp, Environment.TickCount);
            }
        }

        public static StiProgressStatus Contains(IStiAppComponent comp)
        {
            lock (lockObject)
            {
                var exists = hash.Contains(comp);
                if (!exists)
                    return StiProgressStatus.None;

                if (!dictTick.ContainsKey(comp))
                    return StiProgressStatus.Long;

                if (IsHidenProgress(dictTick[comp]))
                    return StiProgressStatus.Hiden;

                if (IsLongProgress(dictTick[comp]))
                    return StiProgressStatus.Long;

                return StiProgressStatus.Short;
            }
        }

        public static bool IsActiveProgress(StiProgressStatus status)
        {
            return status == StiProgressStatus.Short || status == StiProgressStatus.Long;
        }

        public static bool IsLongProgress(long tick)
        {
            return (Environment.TickCount - tick) > 2000;
        }

        public static bool IsHidenProgress(long tick)
        {
            return (Environment.TickCount - tick) < 500;
        }

        public static void Remove(IStiAppComponent comp, bool lockCompledProgressr = false)
        {
            lock (lockObject)
            {
                lockCompletedProgressHandler = lockCompledProgressr;

                if (hash.Contains(comp))
                    hash.Remove(comp);

                if (dictTick.ContainsKey(comp))
                    dictTick.Remove(comp);

                lockCompletedProgressHandler = false;
            }
        }

        public static List<IStiAppComponent> FetchAllComponents()
        {
            lock (lockObject)
            {
                return hash.Where(c => c != null).ToList();
            }
        }

        public static void Dispose()
        {
            timer.Tick -= Timer_Tick;
            hash.CollectionChanged -= ActiveProgress_CollectionChanged;
        }
        #endregion
    }
}
