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

using Microsoft.Win32;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Stimulsoft.Base
{
    public class StiCID : StiObject
    {
        #region Consts
        private const string Key = "fjk2dpfko5epefko4prmk";
        private const string Undefined = "Undefined";
        private const string Prefix = "PL534950";
        #endregion

        #region Properties
        public string MachineAddress { get; set; }

        public string MachineName { get; set; }

        public string MachineUserName { get; set; }

        public string MachineGuid { get; set; }
        #endregion

        #region Methods
        internal string GetEncrypt()
        {
            return $"{Prefix}{StiEncryption.Encrypt(this.SaveToString(), Key)}";
        }

        public static string GetDefault()
        {
            return Prefix + StiEncryption.Encrypt(GetDeveloperCID().SaveToString(), Key);
        }

        private static StiCID GetDeveloperCID()
        {
            return new StiCID(
                GetCurrentMachineName(),
                GetCurrentMachineAddress(),
                GetCurrentMachineUserName(),
                GetCurrentMachineGuid());
        }

        private static string GetCurrentMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return Undefined;
            }
        }

        private static string GetCurrentMachineGuid()
        {
            try
            {
#if !BLAZOR
                var x64Result = string.Empty;
                var x86Result = string.Empty;

                var keyBaseX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                var keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
                var keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);

                var resultObjX64 = keyX64.GetValue("MachineGuid", "default");
                var resultObjX86 = keyX86.GetValue("MachineGuid", "default");

                keyX64.Close();
                keyX86.Close();

                keyBaseX64.Close();
                keyBaseX86.Close();

                keyX64.Dispose();
                keyX86.Dispose();

                keyBaseX64.Dispose();
                keyBaseX86.Dispose();

                keyX64 = null;
                keyX86 = null;
                keyBaseX64 = null;
                keyBaseX86 = null;

                if (resultObjX64 != null && resultObjX64.ToString() != "default")                
                    return resultObjX64.ToString();                

                if (resultObjX86 != null && resultObjX86.ToString() != "default")                
                    return resultObjX86.ToString();
#endif
            }
            catch (Exception)
            {
                return Undefined;
            }

            return Undefined;
        }

        private static string GetCurrentMachineAddress()
        {
            try
            {
                var networks = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up);

                if (networks == null)
                    return Undefined;

                var network = networks.FirstOrDefault(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet);

                if (network == null)
                    network = networks.FirstOrDefault(n => n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);

                if (network == null)
                    return Undefined;
                
                return string.Join("-", Regex.Matches(network.GetPhysicalAddress().ToString(), @"\w{2}").Cast<Match>());
            }
            catch
            {
                return Undefined;
            }
        }

        private static string GetCurrentMachineUserName()
        {
            try
            {
                return Environment.UserName;
            }
            catch
            {
                return Undefined;
            }
        }

        public static bool IsCID(string cid)
        {
            return cid != null && cid.StartsWith(Prefix);
        }
        #endregion

        private StiCID()
        {
        }

        public StiCID(string cid)
        {
            if (!IsCID(cid))return;

            cid = cid.Substring(Prefix.Length);

            this.LoadFromString(StiEncryption.Decrypt(cid, Key));
        }

        public StiCID(string machineName, string machineAddress, string machineUserName)
        {
            this.MachineName = machineName;
            this.MachineAddress = machineAddress;
            this.MachineUserName = machineUserName;
        }

        public StiCID(string machineName, string machineAddress, string machineUserName, string machineGuid)
        {
            this.MachineName = machineName;
            this.MachineAddress = machineAddress;
            this.MachineUserName = machineUserName;
            this.MachineGuid = machineGuid;
        }
    }
}