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

namespace Stimulsoft.Base.Design
{
	public static class StiEditors
	{
		public const string Base = "Stimulsoft.Base.Drawing.Design.";
		public const string Report = "Stimulsoft.Report.Design.Components.";

        public const string Bool = Base + "StiBoolEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
		public const string Enum = Base + "StiEnumEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
		public const string ExpressionBool = Base + "StiExpressionBoolEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
		public const string ExpressionBrush = Base + "StiExpressionBrushEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
		public const string ExpressionColor = Base + "StiExpressionColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
		public const string ExpressionEnum = Base + "StiExpressionEnumEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
        public const string Font = Report + "StiFontEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
        public const string FontName = Base + "StiFontNameEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
        public const string StringCollection = Report + "StiStringCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
        public const string PenStyle = Base + "StiPenStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo;
		
	}
}
