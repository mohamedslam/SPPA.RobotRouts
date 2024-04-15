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

using Stimulsoft.Base;
using System;
using System.Reflection;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Helpers
{
    public class StiWinFormRunner : IStiFormRunner
    {
        #region Fields
        private Form form;
        #endregion

        #region IStiFormRunner
        public object OwnerWindow { get; set; }

        public void ShowDialog()
        {
            form.Owner = OwnerWindow as Form;
            InvokeComplete(form.ShowDialog() == DialogResult.OK);
        }

        public object this[string key]
        {
            get
            {
                var type = form.GetType();
                var property = type.GetProperty(key);
                return property.GetValue(form, new object[0]);
            }
            set
            {
                var type = form.GetType();
                var property = type.GetProperty(key);
                property.SetValue(form, value, new object[0]);
            }
        }

        public void Create(string formType, string assemblyName)
        {
            Create(formType, assemblyName, new object[0]);
        }

        public void Create(string formType, string assemblyName, params object[] args)
        {
            var type = Type.GetType($"{formType}, {assemblyName}, {StiVersion.VersionInfo}");
            if (type == null)
                throw new Exception($"Assembly '{assemblyName}' is not found!");

            this.form =
                StiActivator.CreateObject(type, args) as Form;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (form != null)
            {
                form.Dispose();
            }
        }
        #endregion

        #region Events
        public event StiShowDialogCompleteEventHandler Complete;

        private void InvokeComplete(bool dialogResult)
        {
            Complete?.Invoke(this, new StiShowDialogCompleteEvetArgs(dialogResult));
        }
        #endregion
    }
}