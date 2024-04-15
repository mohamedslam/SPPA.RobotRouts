#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Runtime.InteropServices;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using System.Security;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Controls
{
    /// <summary>
	/// Represents a Windows rich text box control, with some impovements.
	/// </summary>
	[ToolboxItem(false)]
    [SuppressUnmanagedCodeSecurity]
    public class StiRichBoxControl : RichTextBox
    {
        #region struct PARAFORMAT
        [StructLayout(LayoutKind.Sequential)]
        private struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;

            // PARAFORMAT2 from here onwards.
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }
        #endregion

        #region struct CHARFORMATSTRUCT
        //[StructLayout(LayoutKind.Sequential)]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CHARFORMATSTRUCT
        {
            public int cbSize;
            public UInt32 dwMask;
            public UInt32 dwEffects;
            public Int32 yHeight;
            public Int32 yOffset;
            public Int32 crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName;
        }
        #endregion

        #region Fields.Static
        private static IntPtr moduleHandle;
        private static bool failLoadModule;
        private static bool needCheckClassName = true;
        #endregion

        #region Consts
        private const int EM_GETPARAFORMAT = 1085;
        private const int EM_SETPARAFORMAT = 1095;
        private const int EM_SETTYPOGRAPHYOPTIONS = 1226;
        private const int WM_SETREDRAW = 11;
        private const int TO_ADVANCEDTYPOGRAPHY = 1;
        private const int PFM_ALIGNMENT = 8;
        private const int SCF_SELECTION = 1;

        private const UInt32 CFM_BOLD = 0x00000001;
        private const UInt32 CFM_ITALIC = 0x00000002;
        private const UInt32 CFM_UNDERLINE = 0x00000004;
        private const UInt32 CFM_SIZE = 0x80000000;
        private const UInt32 CFM_FACE = 0x20000000;
        private const UInt32 CFE_BOLD = 0x00000001;
        private const UInt32 CFE_ITALIC = 0x00000002;
        private const UInt32 CFE_UNDERLINE = 0x00000004;
        #endregion

        #region Methods.Imports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            ref PARAFORMAT lp);
        #endregion

        #region Methods
        private static bool TryLoadLibrary()
        {
            if (!failLoadModule && moduleHandle == IntPtr.Zero)
            {
                try
                {
                    moduleHandle = LoadLibrary("msftedit.dll");
                }
                catch
                {
                    failLoadModule = true;
                }
            }
            return moduleHandle != IntPtr.Zero;
        }

        private void CheckClassName()
        {
            if (!needCheckClassName) return;

            try
            {
                needCheckClassName = false;
                Rtf = string.Empty;
                Rtf = null;
            }
            catch (Win32Exception)
            {
                StiBaseOptions.ExtendedRichTextLibraryClassName = "RichEdit20W";
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var parameters = base.CreateParams;

                if (StiBaseOptions.ForceLoadExtendedRichTextLibrary.GetValueOrDefault(true) && TryLoadLibrary())
                    parameters.ClassName = StiBaseOptions.ExtendedRichTextLibraryClassName;

                return parameters;
            }
        }

        public void BeginUpdate()
        {
            SendMessage(this.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            SendMessage(this.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
        }

        public bool SetSelectionFont(string face)
        {
            var cf = new CHARFORMATSTRUCT();
            cf.cbSize = Marshal.SizeOf(cf);
            cf.szFaceName = new char[32];
            cf.dwMask = CFM_FACE;
            face.CopyTo(0, cf.szFaceName, 0, Math.Min(31, face.Length));

            var lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
            Marshal.StructureToPtr(cf, lParam, false);

#if NETSTANDARD
            return true;
#else
            var res = Win32.SendMessage(Handle, (int)Win32.Msg.EM_SETCHARFORMAT, SCF_SELECTION, lParam);
            return res == 0;
#endif
        }

        public bool SetSelectionSize(int size)
        {
            var cf = new CHARFORMATSTRUCT();
            cf.cbSize = Marshal.SizeOf(cf);
            cf.dwMask = CFM_SIZE;
            cf.yHeight = size * 20;

            var lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
            Marshal.StructureToPtr(cf, lParam, false);

#if NETSTANDARD
            return true;
#else
            var res = Win32.SendMessage(Handle, (int)Win32.Msg.EM_SETCHARFORMAT, SCF_SELECTION, lParam);
            return res == 0;
#endif
        }

        public bool SetSelectionBold(bool bold)
        {
            return SetSelectionStyle(CFM_BOLD, bold ? CFE_BOLD : 0);
        }

        public bool SetSelectionItalic(bool italic)
        {
            return SetSelectionStyle(CFM_ITALIC, italic ? CFE_ITALIC : 0);
        }

        public bool SetSelectionUnderlined(bool underlined)
        {
            return SetSelectionStyle(CFM_UNDERLINE, underlined ? CFE_UNDERLINE : 0);
        }

        private bool SetSelectionStyle(UInt32 mask, UInt32 effect)
        {
            var cf = new CHARFORMATSTRUCT();
            cf.cbSize = Marshal.SizeOf(cf);
            cf.dwMask = mask;
            cf.dwEffects = effect;

            var lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
            Marshal.StructureToPtr(cf, lParam, false);

#if NETSTANDARD
            return true;
#else
            var res = Win32.SendMessage(Handle, (int)Win32.Msg.EM_SETCHARFORMAT, SCF_SELECTION, lParam);
            return res == 0;
#endif
        }
        #endregion

        #region Handlers
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

#if !NETSTANDARD
            // Enable support for justification.
            SendMessage(new HandleRef(this, Handle), EM_SETTYPOGRAPHYOPTIONS, TO_ADVANCEDTYPOGRAPHY, TO_ADVANCEDTYPOGRAPHY);
#endif
        }
        #endregion

        #region Properties
        public new StiRichTextAlignment SelectionAlignment
        {
            get
            {
                var fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);

#if !NETSTANDARD
                SendMessage(new HandleRef(this, Handle), EM_GETPARAFORMAT, SCF_SELECTION, ref fmt);
#endif

                return (fmt.dwMask & PFM_ALIGNMENT) == 0
                    ? StiRichTextAlignment.Left
                    : (StiRichTextAlignment)fmt.wAlignment;
            }
            set
            {
                var fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = PFM_ALIGNMENT;
                fmt.wAlignment = (short)value;

#if !NETSTANDARD
                SendMessage(new HandleRef(this, Handle), EM_SETPARAFORMAT, SCF_SELECTION, ref fmt);
#endif
            }
        }
        #endregion

        public StiRichBoxControl()
        {
            CheckClassName();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
