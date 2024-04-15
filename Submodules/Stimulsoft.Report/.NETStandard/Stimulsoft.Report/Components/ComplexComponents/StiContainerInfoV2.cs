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
using System.ComponentModel;
using System.Data;

namespace Stimulsoft.Report.Components
{
    public class StiContainerInfoV2 : StiComponentInfo
	{
        /// <summary>
        /// Позиция в DataBand на момент генерации этого контейнера.
        /// </summary>
        internal int DataBandPosition { get; set; } = -1;

        /// <summary>
        /// Строка источника данных на момент генерации этого контейнера.
        /// </summary>
        internal DataRow DataSourceRow { get; set; }

        /// <summary>
        /// Текущее значение бизнес-объекта на момент генерации этого контейнера.
        /// </summary>
        internal object BusinessObjectCurrent { get; set; }

        /// <summary>
        /// Если true, то этот контейнер сгенерирован автоматически (PrintOnAllPages).
        /// </summary>
        internal bool IsAutoRendered { get; set; }

        /// <summary>
        /// Если свойство установлено в true, то свойство ResetPageNumber для контейнера будет проигнорировано.
        /// </summary>
        internal bool IgnoreResetPageNumber { get; set; }

        /// <summary>
        /// Если true, то этот контейнер выводится в колонкаих на DataBand.
        /// </summary>
        internal bool IsColumns { get; set; }

        /// <summary>
        /// Индекс колонки присваеваеися только тем контейнерам, которые леэат на колонках страницы. Он используется для определения 
        /// контейнера в который сейчас необходимо выводить данные. Индекс помогает выводить корректно данные в ситуации когда на колонках страницы 
        /// лежат контейнеры в которых находятся DataBand.
        /// </summary>
        internal int RenderStep { get; set; } = -1;

	    [Browsable(false)]
		public int SetSegmentPerWidth { get; set; } = -1;

	    /// <summary>
	    /// Band, который сгенерировал этот контейнер.
	    /// </summary>
        internal StiBand ParentBand { get; set; }
	}
}
