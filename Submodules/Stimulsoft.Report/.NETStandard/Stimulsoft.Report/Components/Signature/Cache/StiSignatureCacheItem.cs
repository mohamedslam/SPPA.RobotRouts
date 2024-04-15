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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.SignatureFonts;
using System;
using System.Drawing;
using System.IO;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    public sealed class StiSignatureCacheItem
    {
        public StiSignatureCacheItem()
        {

        }

        public StiSignatureCacheItem(StiElectronicSignature signature)
        {
            this.Name = Guid.NewGuid().ToString().Replace("-", "");

            this.SignatureType = signature.Mode;
            if (signature.Mode == StiSignatureMode.Type)
            {
                this.SelectStyleFullName = signature.Type.FullName;
                this.SelectStyleInitials = signature.Type.Initials;
                this.SelectStyleStyle = signature.Type.Style;
                this.SelectStyleCustomFont = signature.Type.CustomFont;
            }
            else
            {
                this.SignatureImageBytes = signature.Draw.Image;
                if (this.SignatureImageBytes != null)
                {
                    this.SignatureImageBytesHashCode = this.SignatureImageBytes.GetHashCode();
                }

                this.ImageImage = signature.Image.Image;
                if (this.ImageImage != null)
                    ImageImageHashCode = this.ImageImage.GetHashCode();

                this.ImageAspectRatio = signature.Image.AspectRatio;
                this.ImageHorAlignment = signature.Image.HorAlignment;
                this.ImageVertAlignment = signature.Image.VertAlignment;
                this.ImageStretch = signature.Image.Stretch;

                this.TextText = signature.Text.Text;
                this.TextHorAlignment = signature.Text.HorAlignment;
                this.TextVertAlignment = signature.Text.VertAlignment;
                this.TextFont = signature.Text.Font;
                this.TextColor = signature.Text.Color;
            }
        }

        #region Properties
        public string Name { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public StiSignatureMode SignatureType { get; set; }

        public byte[] SignatureImageBytes { get; set; }

        public int SignatureImageBytesHashCode { get; set; }


        public byte[] ImageImage { get; set; }
        public int ImageImageHashCode { get; set; }

        public bool ImageAspectRatio { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StiHorAlignment ImageHorAlignment { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StiVertAlignment ImageVertAlignment { get; set; }

        public bool ImageStretch { get; set; }


        public string TextText { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StiTextHorAlignment TextHorAlignment { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StiVertAlignment TextVertAlignment { get; set; }

        public Font TextFont { get; set; }

        public Color TextColor { get; set; }


        public string SelectStyleFullName { get; set; }

        public string SelectStyleInitials { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StiSignatureStyle SelectStyleStyle { get; set; }

        public string SelectStyleCustomFont { get; set; }
        #endregion

        #region Methods
        internal bool AlreadyExists(StiSignatureCacheItem item)
        {
            if (this.SignatureType == item.SignatureType &&
                this.SignatureImageBytesHashCode == item.SignatureImageBytesHashCode &&
                this.ImageImageHashCode == item.ImageImageHashCode &&
                this.ImageAspectRatio == item.ImageAspectRatio &&
                this.ImageHorAlignment == item.ImageHorAlignment &&
                this.ImageVertAlignment == item.ImageVertAlignment &&
                this.ImageStretch == item.ImageStretch &&
                this.TextText == item.TextText &&
                this.TextHorAlignment == item.TextHorAlignment &&
                this.TextVertAlignment == item.TextVertAlignment &&
                this.TextFont == item.TextFont &&
                this.TextColor == item.TextColor &&
                this.SelectStyleFullName == item.SelectStyleFullName &&
                this.SelectStyleInitials == item.SelectStyleInitials &&
                this.SelectStyleStyle == item.SelectStyleStyle &&
                this.SelectStyleCustomFont == item.SelectStyleCustomFont)
                return true;

            return false;
        }

        public void Save()
        {
            try
            {
                var path = StiSignatureCache.GetFolderPath();
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                path = Path.Combine(path, $"{Name}.json");

                StiFileUtils.ProcessReadOnly(path);
                File.WriteAllText(path, json);
            }
            catch
            {

            }
        }

        public void Delete()
        {
            try
            {
                var path = StiSignatureCache.GetFolderPath();
                if (string.IsNullOrEmpty(path)) return;

                path = Path.Combine(path, $"{Name}.json");
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {

            }
        }
        #endregion
    }
}