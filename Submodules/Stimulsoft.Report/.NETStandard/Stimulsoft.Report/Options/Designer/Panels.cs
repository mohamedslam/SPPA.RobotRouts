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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
            public static class Panels
            {
                public static class Dictionary
                {
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForVariable { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForDataSource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForDataColumn { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForDataRelation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForDataParameter { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForDataConnection { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateForResource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForVariable { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForResource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForDataSource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForBusinessObject { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForDataColumn { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForDataRelation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForDataParameter { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForDataConnection { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesForDataTransformation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForVariable { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForResource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForDataSource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForBusinessObject { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForDataColumn { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForDataRelation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForDataParameter { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForDataConnection { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditForDataTransformation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForVariable { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForResource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForDataSource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForBusinessObject { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForDataColumn { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForDataRelation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForDataParameter { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForDataConnection { get; set; } = true;
                    
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteForDataTransformation { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowUseAliases { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowCreateFieldOnDoubleClick { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowCreateLabel { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowActionsButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowNewButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowSaveResource { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowUpButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDownButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowSortItemsButton { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictXmlMergeMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictXmlExportMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictXmlImportMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictSaveMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictMergeMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictOpenMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDictNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowConnectionNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataTransformationNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowRelationNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataSourceNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBusinessObjectNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowColumnNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataParameterNewMenuItem { get; set; } = true;

                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool ShowDataSourcesNewMenuItem { get; set; }

                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool ShowRelationsImportMenuItem { get; set; }

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataMonitorMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowSynchronizeMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowViewDataMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPropertiesMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDuplicateMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowCategoryNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowVariableNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowResourceNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEditMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowMakeThisRelationActive { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDeleteMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowContextMenu { get; set; } = true;

                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool ShowMarkUsedMenuItem { get; set; }

                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool ShowRemoveUnusedMenuItem { get; set; }

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowCalcColumnNewMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowExpandAllMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowCollapseAllMenuItem { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEmptyDataSourcesCategory { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEmptyBusinessObjectsCategory { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEmptyVariablesCategory { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowEmptyResourcesCategory { get; set; } = true;
                }
            }
		}
    }
}