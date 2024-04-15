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

using System;
using System.Xml.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Databases;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Base
{
    /// <summary>
    /// This is a base class for all Stimulsoft Server object.
    /// </summary>
    public abstract class StiObject : ICloneable
    {
        #region ICloneable
        public virtual object Clone()
        {
            return base.MemberwiseClone();
        }
        #endregion

        #region Methods.SaveLoad
        /// <summary>
        /// Resets properties to default values.
        /// </summary>
        protected virtual void ResetProperties()
        {
            StiPropertyHelper.Reset(this);
        }

        /// <summary>
        /// Loads element from string.
        /// </summary>
        /// <param name="str">String representation which contain item description.</param>
        public virtual void LoadFromString(string str)
        {
            ResetProperties();
            JsonConvert.PopulateObject(str, this);
        }

        /// <summary>
        /// Loads element from byte array.
        /// </summary>
        public virtual void LoadFromBytes(byte[] bytes)
        {
            var str = StiPacker.UnpackToString(bytes);
            if (str == null) return;

            LoadFromString(str);
        }

        /// <summary>
        /// Saves element to string.
        /// </summary>
        /// <returns>String representation which contains schema.</returns>
        public virtual string SaveToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, StiJsonHelper.DefaultSerializerSettings);
        }

        /// <summary>
        /// Saves element to byte array.
        /// </summary>
        /// <returns>String representation which contain item description.</returns>
        public virtual byte[] SaveToBytes(bool allowPacking = true)
        {
            return StiPacker.PackToBytes(SaveToString(), allowPacking);
        }

        /// <summary>
        /// Saves element to XDocument object.
        /// </summary>
        /// <returns>XElement object which contains xml description of saved schema.</returns>
        public virtual XDocument SaveToXml()
        {
            return JsonConvert.DeserializeXNode(this.SaveToString());
        }
        #endregion
    }
}
