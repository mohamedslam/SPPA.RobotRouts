#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Helpers;
using System;

namespace Stimulsoft.Base.Licenses
{
    public abstract class StiLicenseObject
    {
        #region Properties
#if BLAZOR
        protected virtual string EncryptKey => "ieICWsADhNJQBsVv63j/tA==";
#else
        protected virtual string EncryptKey => "rXtkxg№g3o9P@9BfNt#vu";
#endif
        #endregion

        #region Methods.SaveLoad
        protected internal void LoadFromString(string str)
        {
            StiJsonHelper.LoadFromJsonString(str, this);
        }

        protected internal string SaveToString()
        {
            return StiJsonHelper.SaveToJsonString(this);
        }

        protected internal void LoadFromBytes(byte[] bytes)
        {
            var str = StiBytesToStringConverter.ConvertBytesToString(bytes);
            this.LoadFromString(str);
        }

        protected internal byte[] SaveToBytes()
        {
            var str = SaveToString();
            return StiBytesToStringConverter.ConvertStringToBytes(str);
        }

        protected internal void DecryptFromBytes(byte[] bytes)
        {
            var decryptedBytes = StiCryptHelper.Decrypt(bytes, EncryptKey);
            var str = StiBytesToStringConverter.ConvertBytesToString(decryptedBytes);
            this.LoadFromString(str);
        }

        protected internal byte[] EncryptToBytes()
        {
            var bytes = SaveToBytes();
            return StiCryptHelper.Encrypt(bytes, EncryptKey);
        }

        protected internal string EncryptToString()
        {
            var bytes = EncryptToBytes();
            return Convert.ToBase64String(bytes);
        }

        protected internal void DecryptFromString(string str)
        {
            var bytes = Convert.FromBase64String(str);
            DecryptFromBytes(bytes);
        }
        #endregion
    }
}