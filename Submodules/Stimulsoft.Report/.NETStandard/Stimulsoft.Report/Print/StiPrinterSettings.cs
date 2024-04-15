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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base.Design;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Print
{
    public class StiPrinterSettings :
        IStiSerializeToCodeAsClass,
        IStiJsonReportObject,
        IStiDefault
    {
        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault
        {
            get
            {
                return 
                    PrintDialogResult == DialogResult.None &&
                    Copies == 1 &&
                    Collate &&
                    Duplex == Duplex.Default &&
                    ShowDialog &&
                    PrinterName != null && PrinterName.Length == 0;
            }
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyEnum("PrintDialogResult", PrintDialogResult, DialogResult.None);
            jObject.AddPropertyInt("Copies", Copies, 1);
            jObject.AddPropertyBool("Collate", Collate, true);
            jObject.AddPropertyEnum("Duplex", Duplex, Duplex.Default);
            jObject.AddPropertyBool("ShowDialog", ShowDialog, true);
            jObject.AddPropertyStringNullOrEmpty("PrinterName", PrinterName);

            if (jObject.Count == 0)
                return null;

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PrintDialogResult":
                        this.PrintDialogResult = property.DeserializeEnum<DialogResult>();
                        break;

                    case "Copies":
                        this.Copies = property.DeserializeInt();
                        break;

                    case "Collate":
                        this.Collate = property.DeserializeBool();
                        break;

                    case "Duplex":
                        this.Duplex = property.DeserializeEnum<Duplex>(); 
                        break;

                    case "ShowDialog":
                        this.ShowDialog = property.DeserializeBool();
                        break;

                    case "PrinterName":
                        this.PrinterName = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public DialogResult PrintDialogResult { get; set; } = DialogResult.None;

        /// <summary>
        /// Gets or sets copies number of report for printing.
        /// </summary>
        [DefaultValue(1)]
        [StiSerializable]
        [Description("Gets or sets copies number of report for printing.")]
        public int Copies { get; set; } = 1;

        /// <summary>
        /// Gets os sets value which indicates that collate mode of printing will be used or not.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that collate mode of printing will be used or not.")]
        public bool Collate { get; set; } = true;

        /// <summary>
        /// Gets os sets mode of duplex printing.
        /// </summary>
        [DefaultValue(Duplex.Default)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets mode of duplex printing.")]
        public Duplex Duplex { get; set; } = Duplex.Default;

        /// <summary>
        /// Gets or sets value which indicates that print dialog will be shown or not.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that print dialog will be shown or not.")]
        public bool ShowDialog { get; set; } = true;

        /// <summary>
        /// Gets or sets name of printer which will be used for report printing.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets name of printer which will be used for report printing.")]
        [Editor("Stimulsoft.Report.Print.Design.StiPrinterNameEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string PrinterName { get; set; } = "";
        #endregion
    }
}