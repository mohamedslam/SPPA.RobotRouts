#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.Report.Export
{
    public sealed class StiPdfStructure
    {
        #region Subclasses
        public class StiPdfObjInfo
        {
            public int Ref = -1;
            internal StiPdfStructure Info = null;

            public bool IsUsed
            {
                get
                {
                    return !(Ref == -1);
                }
            }

            public void AddRef()
            {
                Info.AddRef(this);
            }

            public override string ToString()
            {
                if (Ref == -1) return "-";
                return string.Format("ref = {0}", Ref);
            }

        }

        public class StiPdfContentObjInfo : StiPdfObjInfo
        {
            public StiPdfObjInfo Content;
        }

        public class StiPdfXObjectObjInfo : StiPdfObjInfo
        {
            public StiPdfObjInfo Mask;
        }

        public class StiPdfFontObjInfo : StiPdfObjInfo
        {
            public StiPdfObjInfo DescendantFont;
            public StiPdfObjInfo ToUnicode;
            public StiPdfObjInfo CIDSet;
            public StiPdfObjInfo Encoding;
            public StiPdfObjInfo FontDescriptor;
            public StiPdfObjInfo FontFile2;
        }

        public class StiPdfOutlinesObjInfo : StiPdfObjInfo
        {
            public List<StiPdfObjInfo> Items;
        }

        public class StiPdfPatternsObjInfo : StiPdfObjInfo
        {
            public StiPdfObjInfo Resources;
            public StiPdfObjInfo First;
            public List<StiPdfObjInfo> HatchItems;
            public List<StiPdfObjInfo> ShadingItems;
            public List<StiPdfObjInfo> ShadingFunctionItems;
        }

        public class StiPdfAnnotObjInfo : StiPdfObjInfo
        {
            public StiPdfObjInfo AP;
            public List<StiPdfObjInfo> AA;
        }
        public class StiPdfAcroFormObjInfo : StiPdfObjInfo
        {
            public List<StiPdfAnnotObjInfo> Annots;
            public List<List<StiPdfAnnotObjInfo>> CheckBoxes;
            public List<StiPdfAnnotObjInfo> UnsignedSignatures;
            public List<StiPdfAnnotObjInfo> Signatures;
            public List<StiPdfAnnotObjInfo> Tooltips;
            public List<StiPdfFontObjInfo> AnnotFontItems;
        }
        #endregion

        #region Structure
        public StiPdfObjInfo Root;          //1
        public StiPdfObjInfo Info;          //2
        public StiPdfObjInfo ColorSpace;    //3
        public StiPdfObjInfo Pages;         //4
        public StiPdfObjInfo StructTreeRoot;    //5
        public StiPdfObjInfo OptionalContentGroup;  //6

        public List<StiPdfContentObjInfo> PageList;

        public List<StiPdfXObjectObjInfo> XObjectList;

        public List<StiPdfFontObjInfo> FontList;

        public StiPdfOutlinesObjInfo Outlines;
        public StiPdfPatternsObjInfo Patterns;

        public List<StiPdfObjInfo> LinkList;

        public StiPdfObjInfo Encode;
        public StiPdfObjInfo ExtGState;

        public StiPdfAcroFormObjInfo AcroForm;        
        
        public StiPdfObjInfo Metadata;
        public StiPdfObjInfo DestOutputProfile;
        public StiPdfObjInfo OutputIntents;

        public StiPdfContentObjInfo EmbeddedJS;

        public List<StiPdfContentObjInfo> EmbeddedFilesList;

        #endregion

        #region Fields
        private int objectsCounter = 0;
        private ArrayList objects = null;   //temp
        #endregion

        #region Methods
        public void AddRef(StiPdfObjInfo info)
        {
            objectsCounter++;
            info.Ref = objectsCounter;
        }

        public StiPdfObjInfo CreateObject(bool addRef = false)
        {
            var obj = new StiPdfObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            return obj;
        }

        public StiPdfContentObjInfo CreateContentObject(bool addRef = false)
        {
            var obj = new StiPdfContentObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            obj.Content = CreateObject(addRef);
            return obj;
        }

        public StiPdfXObjectObjInfo CreateXObject(bool addRef = false, bool haveMask = false)
        {
            var obj = new StiPdfXObjectObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            obj.Mask = CreateObject(addRef && haveMask);
            return obj;
        }

        public StiPdfFontObjInfo CreateFontObject(bool addRef = false, bool useUnicodeMode = true, bool standardPdfFonts = false, bool embeddedFonts = true, bool annotFont = false)
        {
            var obj = new StiPdfFontObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            obj.DescendantFont = CreateObject(addRef && useUnicodeMode);
            obj.ToUnicode = CreateObject(addRef && useUnicodeMode);
            obj.CIDSet = CreateObject(addRef && useUnicodeMode);
            obj.Encoding = CreateObject(addRef && !useUnicodeMode && !annotFont);
            obj.FontDescriptor = CreateObject(addRef && !standardPdfFonts);
            obj.FontFile2 = CreateObject(addRef && embeddedFonts);
            return obj;
        }

        public StiPdfOutlinesObjInfo CreateOutlinesObject(bool addRef = false)
        {
            var obj = new StiPdfOutlinesObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            obj.Items = new List<StiPdfObjInfo>();
            return obj;
        }

        public StiPdfPatternsObjInfo CreatePatternsObject(bool addRef = false)
        {
            var obj = new StiPdfPatternsObjInfo();
            obj.Info = this;
            obj.Resources = CreateObject(addRef);
            obj.First = CreateObject(addRef);
            obj.HatchItems = new List<StiPdfObjInfo>();
            obj.ShadingItems = new List<StiPdfObjInfo>();
            obj.ShadingFunctionItems = new List<StiPdfObjInfo>();
            return obj;
        }

        public StiPdfAcroFormObjInfo CreateAcroFormObject(bool addRef = false)
        {
            var obj = new StiPdfAcroFormObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            obj.Annots = new List<StiPdfAnnotObjInfo>();
            obj.CheckBoxes = new List<List<StiPdfAnnotObjInfo>>();
            obj.UnsignedSignatures = new List<StiPdfAnnotObjInfo>();
            obj.Signatures = new List<StiPdfAnnotObjInfo>();
            obj.Tooltips = new List<StiPdfAnnotObjInfo>();
            obj.AnnotFontItems = new List<StiPdfFontObjInfo>();
            return obj;
        }

        public StiPdfAnnotObjInfo CreateAnnotObject(bool addRef = false, bool createAP = false, int numberAA = 0)
        {
            var obj = new StiPdfAnnotObjInfo();
            obj.Info = this;
            if (addRef) AddRef(obj);
            obj.AP = CreateObject(addRef && createAP);
            obj.AA = new List<StiPdfObjInfo>();
            for (int index = 0; index < numberAA; index++)
            {
                obj.AA.Add(CreateObject(addRef));
            }
            return obj;
        }
        #endregion

        public StiPdfStructure()
        {
            objectsCounter = 0;
            objects = new ArrayList();  //temp

            Root = CreateObject(true);
            Info = CreateObject(true);
            ColorSpace = CreateObject(true);
            Pages = CreateObject(true);
            StructTreeRoot = CreateObject(true);
            OptionalContentGroup = CreateObject(true);

            PageList = new List<StiPdfContentObjInfo>();
            XObjectList = new List<StiPdfXObjectObjInfo>();
            FontList = new List<StiPdfFontObjInfo>();
            LinkList = new List<StiPdfObjInfo>();
            EmbeddedFilesList = new List<StiPdfContentObjInfo>();
        
        }
    }
}
