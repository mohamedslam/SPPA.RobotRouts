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

using Stimulsoft.Report.Components;


namespace Stimulsoft.Report.Engine
{
	public class StiHierarchicalBandV1Builder : StiDataBandV1Builder
	{
		#region Methods.Helpers
		protected override void ProcessRenderedContainer(StiDataBand masterDataBand, StiContainer container)
		{
			base.ProcessRenderedContainer(masterDataBand, container);

			StiHierarchicalBand masterBand = masterDataBand as StiHierarchicalBand;

            #region Data Source
            if (container != null && !masterBand.IsDataSourceEmpty && (!masterDataBand.DataSource.IsEmpty))
			{
				int level = masterDataBand.DataSource.GetLevel();
				if (level > 0)
				{
					double dist = level * masterBand.Report.Unit.ConvertFromHInches(masterBand.Indent);
					foreach (StiComponent comp in container.Components)
					{
						if (comp.Locked)continue;
						comp.Left += dist;
					}
				}
            }
            #endregion

            #region Business Object
            if (container != null && !masterBand.IsBusinessObjectEmpty && (!masterDataBand.BusinessObject.IsEmpty))
            {
                int level = masterDataBand.BusinessObject.GetLevel();
                if (level > 0)
                {
                    double dist = level * masterBand.Report.Unit.ConvertFromHInches(masterBand.Indent);
                    foreach (StiComponent comp in container.Components)
                    {
                        if (comp.Locked) continue;
                        comp.Left += dist;
                    }
                }
            }
            #endregion
        }
		#endregion
	}
}
