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

namespace Stimulsoft.Report
{
    public interface IStiProgressInformation
    {
        #region Methods
        void Start(string title);

		void Start(string title, int progressMaximum);

		void SetProgressBar(int value, int maximum);

		void Show();

		void Hide();

		void HideProgressBar();

		void ShowProgressBar();

		void Update(string value);

	    void Update(string value, int progressValue);

	    void Close();

		void SetAllowClose(bool allows);
        #endregion

        #region Properties
        bool IsBreaked { get; set; }

        bool IsMarquee { get; set; }

        bool AllowUseDoEvents { get; set; }

		bool IsVisible { get; } 
        #endregion
    }
}
