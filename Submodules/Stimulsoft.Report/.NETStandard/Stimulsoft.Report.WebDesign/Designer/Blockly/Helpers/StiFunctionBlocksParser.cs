#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Report.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web
{
    internal class StiFunctionBlocksParser
    {
        #region Methods
        public static string GetInitFunctionBlocks()
        {
            var blocksJs = string.Empty;
            var functions = StiFunctions.GetFunctions(false);

            var index = 0;

            foreach (var func in functions)
            {
                var functionBlock = new StiBlocklyBlock(func);

                blocksJs += functionBlock.GetJsDefinition();

                index++;
            }

            return blocksJs;
        }

        public static string GetFunctionsGrouppedInCategoriesBlocks()
        {
            var hashtable = StiFunctions.GetFunctionsGrouppedInCategories();
            var categories = new string[hashtable.Keys.Count];
            hashtable.Keys.CopyTo(categories, 0);

            Array.Sort(categories);

            var result = string.Empty;

            foreach (var category in categories)
            {
                var functionList = hashtable[category] as List<StiFunction>;

                var categoryContent = string.Empty;

                foreach (var func in functionList.OrderBy(f => f.FunctionName).ToList())
                {
                    var functionBlock = new StiBlocklyBlock(func);
                    categoryContent += functionBlock.GetXmlToolboxDefinition();
                }

                result += GetCategoryBlock(category.ToString(), "#4e85c3", categoryContent);
            }

            return result;
        }

        private static string GetCategoryBlock(string name, string color, string content)
        {
            return $"<category name=\"{name}\" colour=\"{color}\">{content}</category>";
        }
        #endregion
    }
}