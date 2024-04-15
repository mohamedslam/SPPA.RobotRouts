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

using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiPageInfoV2 : StiComponentInfo
	{
		internal StiComponentsCollection Overlays;

		/// <summary>
		/// Индекс страницы с которой началось построение контейнеров на странице.
		/// </summary>
		public int IndexOfStartRenderedPages = -1;

		/// <summary>
		/// Используется для указания MasterComponent при построении SubPage's.
		/// </summary>
		public StiDataBand MasterDataBand;

		/// <summary>
		/// Если равно true, то ReportTitleBand's уже постоены и больше их для этой страницы строить не нужно.
		/// </summary>
		public bool IsReportTitlesRendered;

		/// <summary>
		/// Количество отрендеренных страниц. 
		/// </summary>
		public int RenderedCount;

		/// <summary>
		/// Содержит позицию верхней печати после вывода статических бэндов. Используется 
		/// для определения верхней позиции вывода CrossTab's, а также для вывода кросс-примитивов.
		/// </summary>
		public double PositionFromTop;

		/// <summary>
		/// Содержит позицию нижней печати после вывода статических бэндов. Используется 
		/// для определения нижней позиции вывода CrossTab's, а также для вывода кросс-примитивов.
		/// </summary>
		public double PositionFromBottom;
	}
}
