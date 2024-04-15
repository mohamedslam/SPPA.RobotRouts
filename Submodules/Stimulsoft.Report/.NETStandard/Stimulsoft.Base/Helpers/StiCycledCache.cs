#region Copyright (C) 2003-2021 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
{	                         										}
{																	}
{	Copyright (C) 2003-2021 Stimulsoft     							}
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
#endregion Copyright (C) 2003-2021 Stimulsoft

using System;
using System.Collections.Generic;

namespace Stimulsoft.Base.Helpers
{
    internal class StiCycledCache
    {
        #region Fields
        private Dictionary<string, IDisposable> cache = new Dictionary<string, IDisposable>(); 
        private Queue<string> queue = new Queue<string>();
        private object locker = new object();
        #endregion

        #region Properties
        public int Count { get; set; }
        #endregion

        #region Methods
        public void Add(string key, IDisposable value)
        {
            lock (locker)
            {
                if (cache.Count >= this.Count)
                {
                    var lastKey = queue.Dequeue();

                    var lastValue = cache[lastKey];
                    cache.Remove(lastKey);
                    lastValue.Dispose();
                }

                queue.Enqueue(key);
                cache.Add(key, value);
            }
        }

        public object Get(string key)
        {
            if (cache.ContainsKey(key))
                return cache[key];

            return null;
        } 
        #endregion

        public StiCycledCache(int count)
        {
            this.Count = count;
        }
    }
}
