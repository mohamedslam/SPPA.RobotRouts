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

using System;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Base
{
    public static class StiExceptionProvider
    {
        public static IStiCustomExceptionProvider CustomExceptionProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether not to show the message from engine of the message.
        /// </summary>
        public static bool HideMessages { get; set; }

        public static bool DisableSendError { get; set; }

        public static string ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the value, which indicates not to show the exception from engine of the exception.
        /// </summary>
        public static bool HideExceptions { get; set; }

        public static bool Show(Exception exception, bool rethrow = false, Form owner = null)
        {
            if (CustomExceptionProvider != null)
            {
                CustomExceptionProvider.Show(exception);
            }
            else
            {
#if !NETSTANDARD
                if (!HideMessages)
                {
                    if (owner == null)
                        owner = Form.ActiveForm;

                    using (var form = new StiExceptionPreviewForm(exception))
                    {
                        if (owner != null)
                            form.Owner = owner;
                        else
                            form.StartPosition = FormStartPosition.CenterScreen;

                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            using (var form1 = new StiExceptionForm(exception))
                            {
                                if (owner != null)
                                    form1.Owner = owner;
                                else
                                    form1.StartPosition = FormStartPosition.CenterScreen;

                                form1.ShowDialog();
                            }
                        }
                    }
                }
                else
#endif
                {
                    if (rethrow)
                        return !HideExceptions;

                    if (!HideExceptions)
                        throw exception;
                }
            }
            return false;
        }
    }
}
