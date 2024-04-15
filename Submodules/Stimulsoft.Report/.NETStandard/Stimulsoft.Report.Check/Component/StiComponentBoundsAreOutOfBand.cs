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
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiComponentBoundsAreOutOfBand : StiComponentCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return StiLocalizationExt.Get("CheckComponent", "StiComponentBoundsAreOutOfBandShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiComponentBoundsAreOutOfBandLong"), this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Warning;
            }
        }
        #endregion

        #region Methods
        private bool Check()
        {
            StiBand parentBand = ((StiComponent)Element).Parent as StiBand;

            if (parentBand != null && !(Element is StiContainer))
            {
                Stimulsoft.Base.Drawing.RectangleD compRect = ((StiComponent)Element).GetPaintRectangle(false, false);
                if ((StiComponent)Element is StiHorizontalLinePrimitive) compRect.Height = 0;
                if ((StiComponent)Element is StiVerticalLinePrimitive) compRect.Width = 0;

                Stimulsoft.Base.Drawing.RectangleD bandRect = parentBand.GetPaintRectangle(false, false);

                if ((decimal)compRect.Left < (decimal)bandRect.Left ||
                    (decimal)compRect.Top < (decimal)bandRect.Top ||
                    (decimal)compRect.Right > (decimal)bandRect.Right ||
                    (decimal)compRect.Bottom > (decimal)bandRect.Bottom)
                {
                    return true;
                }
            }

            return false;
        }

        private bool NotAllowToDelete
        {
            get
            {
                return Element is StiComponent && !(Element as StiComponent).AllowDelete;
            }
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {
                bool failed = Check();

                if (failed)
                {
                    StiComponentBoundsAreOutOfBand check = new StiComponentBoundsAreOutOfBand();
                    check.Element = obj;
                    //check.Actions.Add(new StiMoveComponentToPageAreaAction());
                    //check.Actions.Add(new StiMoveComponentToPrintablePageAreaAction());
                    if (!NotAllowToDelete) check.Actions.Add(new StiDeleteComponentAction());
                    return check;
                }
                else return null;
            }
            finally
            {
                this.Element = null;
            }
        }
        #endregion
    }
}