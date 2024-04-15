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

using Stimulsoft.Blockly.Blocks;
using Stimulsoft.Blockly.Model;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Blockly.StiBlocks.Report
{
    public class StiAllComponentsFrom : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var name = this.Fields.Get("NAME").ToString();

            var compContainer = GetComponent(context, name);

            if (compContainer is StiPage report)
            {
                return report.GetComponents().ToList();
            }
            else if (compContainer is StiContainer container)
            {
                return container.GetComponents().ToList();
            }
            return null;
        }

        private StiComponent GetComponent(Context context, string name)
        {
            StiComponent compContainer = null;

            if (context.Report.IsRendered)
            {
                foreach (StiPage page in context.Report.RenderedPages)
                {
                    if (page.Name == name)
                        return page;
                }

                compContainer = context.Report.RenderedPages.GetComponentByName(name);
            }
            else
            {
                compContainer = context.Report.GetComponentByName(name);
            }

            return compContainer;
        } 
        #endregion
    }
}