#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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

namespace Stimulsoft.Report.Maps
{
    public class StiMapData : INotifyPropertyChanged
    {
        #region Fields
        private double valueDouble;
        private bool isValueInit;
        #endregion

        #region Properties
        public string Key { get; private set; }

        public string Gss { get; set; }

        private string value;
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    InvokeValueChanged();
                }
            }
        }

        private string group;
        public string Group
        {
            get
            {
                return group;
            }
            set
            {
                if (group != value)
                {
                    group = value;
                    InvokeValueChanged();
                }
            }
        }

        public string Name { get; set; }

        private string color = null;
        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    InvokeValueChanged();
                }
            }
        }
        #endregion

        #region Methods
        public double? GetValue()
        {
            if (!isValueInit)
            {
                isValueInit = true;
                double resValue;
                if (double.TryParse(Value, out resValue))
                {
                    valueDouble = resValue;
                }
            }

            return valueDouble;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Key, this.Group);
        }

        public StiMapData Clone()
        {
            return new StiMapData(this.Key)
            {
                Gss = this.Gss,
                value = this.value,
                group = this.group,
                Name = this.Name,
                color = this.color,
            };
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        internal void InvokePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region Events
        internal event EventHandler ValuesChanged;
        private void InvokeValueChanged()
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }        
        #endregion

        public StiMapData(string key)
        {
            this.Key = key;
        }
    }
}