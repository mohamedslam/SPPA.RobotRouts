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
using System.Linq;

namespace Stimulsoft.Blockly.StiBlocks.Objects
{
    public class StiGetComponent : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var name = this.Fields.Get("NAME").ToString();

            return GetComponentByName(context, name);
        } 

        protected StiComponent GetComponentByName(Context context, string name)
        {
            if (context.Report.IsRendered)
            {
                var component = context.Report.RenderedPages.GetComponentByName(name);

                if (component != null)
                    return component;


                var dashboards = context.Report.Pages.ToList().Where(p => p.IsDashboard && p.Enabled).ToList();

                foreach (StiPage dash in dashboards)
                {
                    if (dash.Name == name)
                        return dash;

                    component = dash.Components.GetComponentByName(name, dash);

                    if (component != null)
                        return component;

                };
            }

            else
            {
                return context.Report.GetComponentByName(name);
            }

            return null;
        }
        #endregion
    }
}
