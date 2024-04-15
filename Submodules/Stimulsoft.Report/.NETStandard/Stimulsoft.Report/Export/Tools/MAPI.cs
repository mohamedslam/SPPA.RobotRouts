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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Security;
using System.Diagnostics;

namespace Stimulsoft.Report.Export
{
    [SuppressUnmanagedCodeSecurity]
    public static class MAPI
    {
        #region Methods
        public static void SendEMail(string subject, string body, string filePath)
        {
            SendEMail(string.Empty, subject, body, filePath);
        }

        public static void SendEMail(string recipient, string subject, string body, string filePath)
        {
            try
            {
                var handle = IntPtr.Zero;
                var session = IntPtr.Zero;

                if (Logon(handle, ref session))
                {
                    var message = CreateMessage(recipient, subject, body, filePath);
                    MAPISendMail(session, handle, message, MAPI_DIALOG, 0);
                    Logoff(handle, ref session);
                    DisposeMessage(message);
                }
                else
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo("Fixmapi.exe"));
                        }
                        catch { }
                    }

                    int result;
                    try
                    {
                        var message = CreateMessage(recipient, subject, body, filePath);
                        result = MAPISendMail(IntPtr.Zero, IntPtr.Zero, message, MAPI_DIALOG | MapiLogonUI, 0);
                        Debug.WriteLine(result);
                    }
                    catch
                    {
                        result = 1;
                    }

                    if (result > 1)
                    {
                        #region Check for Outlook
#if !BLAZOR
                        try
                        {
                            var mailClient = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", "");
                            if (mailClient == null)
                                mailClient = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", "");

                            if (mailClient != null)
                            {
                                var st = mailClient.ToString().ToLowerInvariant();
                                if (!string.IsNullOrEmpty(st) && st.IndexOf("outlook", StringComparison.Ordinal) != -1)
                                    filePath = null; //outlook 2007 and above throw exception if "attachment" command present
                            }
                        }
                        catch
                        {
                        }
#endif
                        #endregion

                        #region Try to ShellExec with Mailto
                        var command = new StringBuilder("mailto:");
                        if (!string.IsNullOrEmpty(recipient))
                            command.Append(recipient);

                        if (!string.IsNullOrEmpty(subject) || !string.IsNullOrEmpty(body) || !string.IsNullOrEmpty(filePath))
                            command.Append("?");

                        if (!string.IsNullOrEmpty(subject))
                            command.Append("subject=" + StiExportUtils.StringToUrl(subject));

                        if (!string.IsNullOrEmpty(body))
                            command.Append("&body=" + StiExportUtils.StringToUrl(body));

                        if (!string.IsNullOrEmpty(filePath))
                            command.Append($"&attachment=\"{StiExportUtils.StringToUrl(filePath)}\"");

                        try
                        {
                            Base.StiProcess.Start($"\"{command}\"");
                        }
                        catch
                        {
                        }
                        #endregion
                    }
                }
            }
            finally
            {
                const int MCW_EW = 0x8001F;
                const int EM_INVALID = 0x10;

                _controlfp(MCW_EW, EM_INVALID);
            }
        }
        #endregion

        #region Methods.Helpers
        private static MapiMessage CreateMessage(string recipient, string subject, string body, string filePath)
        {
            var message = new MapiMessage
            {
                subject = subject,
                noteText = body,
                fileCount = 1,
                files = GetFileDesc(filePath)
            };

            if (!string.IsNullOrEmpty(recipient))
            {
                message.recipCount = 1;
                message.recips = GetRecipients(recipient);
            }

            return message;
        }

        private static IntPtr GetRecipients(string recipient)
        {
            if (string.IsNullOrEmpty(recipient))
                return IntPtr.Zero;

            var size = Marshal.SizeOf(typeof(MapiRecipDesc));
            var intPtr = Marshal.AllocHGlobal(size);

            var mapiRecipDesc = new MapiRecipDesc
            {
                recipClass = (int)HowTo.MAPI_TO,
                name = recipient
            };

            var ptr = (int)intPtr;
            Marshal.StructureToPtr(mapiRecipDesc, (IntPtr)ptr, false);

            return intPtr;
        }

        private static void DisposeMessage(MapiMessage msg)
        {
            if (msg.files == IntPtr.Zero) return;

            Marshal.DestroyStructure(msg.files, typeof(MapiFileDesc));
            Marshal.FreeHGlobal(msg.files);
        }

        private static IntPtr GetFileDesc(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return IntPtr.Zero;

            var mapiType = typeof(MapiFileDesc);
            var size = Marshal.SizeOf(mapiType);

            var mapiPtr = Marshal.AllocHGlobal(size);
            var mapiDesc = new MapiFileDesc
            {
                position = -1,
                fileName = Path.GetFileName(filePath),
                pathName = filePath
            };

            Marshal.StructureToPtr(mapiDesc, mapiPtr, false);
            return mapiPtr;
        }

        private static void Logoff(IntPtr hwnd, ref IntPtr session)
        {
            if (session == IntPtr.Zero) return;

            MAPILogoff(session, hwnd, 0, 0);
            session = IntPtr.Zero;
        }

        private static bool Logon(IntPtr hwnd, ref IntPtr session)
        {
            if (Environment.Is64BitOperatingSystem)
                return false;

            var error = MAPILogon(hwnd, null, null, 0, 0, ref session);
            if (error != 0)
                error = MAPILogon(hwnd, null, null, MapiLogonUI, 0, ref session);

            return error == 0;
        }
        #endregion

        #region Methods.Imports
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _controlfp(int n, int mask);

        [DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
        private static extern int MAPILogoff(IntPtr session, IntPtr hwnd, int flags, int reserved);

        [DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
        private static extern int MAPILogon(IntPtr hwnd, string profileName, string password, int flags, int reserved, ref IntPtr session);

        [DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
        private static extern int MAPISendMail(IntPtr session, IntPtr uiParam, MapiMessage message, int flags, int reserved);
        #endregion

        #region Consts
        private const int MAPI_DIALOG = 8;
        private const int MapiLogonUI = 1;
        #endregion
    }
}
