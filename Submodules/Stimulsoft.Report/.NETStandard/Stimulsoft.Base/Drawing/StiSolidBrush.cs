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

using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Class describes SolidBrush.
	/// </summary>
	[RefreshProperties(RefreshProperties.All)]
	[JsonObject]
	public class StiSolidBrush : StiBrush
	{
		#region Properties
	    /// <summary>
		/// Gets or sets the color of this StiSolidBrush object.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the color of this StiSolidBrush object.")]
		[RefreshProperties(RefreshProperties.All)]
		public Color Color { get; set; }

		private bool ShouldSerializeColor()
		{
			return Color != Color.Transparent;
		}
		#endregion

		#region StiBrush.Override
		public override StiBrushIdent Ident => StiBrushIdent.Solid;
	    #endregion

        #region IEquatable
        protected bool Equals(StiSolidBrush other)
        {
            return Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            return obj != null && (this == obj || obj.GetType() == GetType() && Equals((StiSolidBrush) obj));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (defaultHashCode * 397) ^ Color.GetHashCode();
            }
        }
        private static int defaultHashCode = "StiSolidBrush".GetHashCode();
        #endregion

		#region Methods
        public void LoadValuesFromJson(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;
                }
            }
        }
		#endregion

		/// <summary>
		/// Creates a new instance of the StiSolidBrush class.
		/// </summary>
		public StiSolidBrush()
		{
			Color = Color.Transparent;
		}

		/// <summary>
		/// Creates a new instance of the StiSolidBrush class.
		/// </summary>
		/// <param name="color">The color of this StiSolidBrush object.</param>
		public StiSolidBrush(Color color)
		{
			this.Color = color;
		}

	    /// <summary>
	    /// Creates a new instance of the StiSolidBrush class.
	    /// </summary>
	    /// <param name="color">A string representation of the color for that StiSolidBrush object.</param>
	    public StiSolidBrush(string color)
	    {
	        this.Color = StiColor.Get(color);
	    }
    }
}
