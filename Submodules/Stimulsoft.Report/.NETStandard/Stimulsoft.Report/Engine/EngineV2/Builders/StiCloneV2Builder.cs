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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiCloneV2Builder : StiContainerV2Builder
	{
		public override StiComponent InternalRender(StiComponent masterComp)
		{			
			var masterContainer = masterComp as StiClone;
			var selectedContainer = masterContainer.Container;

			if (IsParentClonation(masterContainer))
				return null;

			while (selectedContainer is StiClone)
			{
				selectedContainer = ((StiClone)selectedContainer).Container;
			}

			if (selectedContainer == null)
			    return base.InternalRender(masterContainer);

			var container = selectedContainer.Render() as StiContainer;
			container.Border = masterContainer.Border.Clone() as StiBorder;
			container.Brush = masterContainer.Brush.Clone() as StiBrush;
			container.ClientRectangle = masterContainer.ClientRectangle;

			return container;
		}

		private bool IsParentClonation(StiClone clone)
        {
			if (clone.Container == null)
				return false;

			var parent = clone.Parent;

			while (parent != null && parent != clone.Page)
            {
				if (parent == clone.Container)
					return true;

				parent = parent.Parent;
            }
			return false;
        }
	}
}
