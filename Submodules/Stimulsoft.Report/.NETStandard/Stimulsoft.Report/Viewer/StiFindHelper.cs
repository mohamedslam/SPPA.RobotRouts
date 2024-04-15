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

using System;
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using System.Text.RegularExpressions;
using System.Globalization;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Viewer
{
    /// <summary>
	/// This class helps in search components in report
	/// </summary>
	public class StiFindHelper
	{
		#region Fields
		private string prevValue = "";
		private bool prevMatchWholeWord = true;
		private bool prevMatchCase = true;		
		#endregion

		#region Properties
	    public List<StiComponent> FindedComponents { get; private set; } = new List<StiComponent>();

	    public Hashtable HashOfFindedPages { get; private set; } = new Hashtable();

	    public int CurrentComponent { get; private set; }

	    public bool FindActivated { get; private set; }

	    public bool AllowLoop { get; set; } = true;

	    public bool AlwaysFindAgain { get; set; }

	    public bool IsBof { get; set; }

	    public bool IsEof { get; set; }
	    #endregion

		#region Events
		public event StiProgressChangedEventHandler ProgressChanged;

		public void InvokeProgressChanged(int value, int maximum)
		{
            this.ProgressChanged?.Invoke(this, new StiProgressChangedEventArgs(value, maximum));
        }
		#endregion

		#region Methods
        public void ResetFind()
        {
            this.prevValue = null;
        }

		public void CloseFind(StiReport report)
		{
            prevValue = null;
            
            if (report != null)
                ClearFind(report.RenderedPages);
		}

        public void ClearFind(StiReport report)
        {
            if (report != null)
                ClearFind(report.RenderedPages);
        }

        public void ClearFind(StiPagesCollection pages)
        {
            if (pages != null && pages.Count > 0)
            {
                foreach (StiPage page in pages)
                {
                    var comps = page.GetComponents();
                    foreach (StiComponent comp in comps)
                    {
                        comp.HighlightState = StiHighlightState.Hide;
                    }
                }
            }

            if (FindedComponents != null)
            {
                FindedComponents.Clear();
                FindedComponents = null;
            }

            if (HashOfFindedPages != null)
            {
                HashOfFindedPages.Clear();
                HashOfFindedPages = null;
            }
            FindActivated = false;
        }

        public void StartFind(string value, StiReport report, bool matchCase, bool matchWholeWord)
        {
            if (report != null)
                StartFind(value, report.RenderedPages, matchCase, matchWholeWord);
        }

		public void StartFind(string value, StiPagesCollection pages, bool matchCase, bool matchWholeWord)
		{
            IsBof = false;
            IsEof = false;

			if (pages != null && pages.Count > 0)
			{				
				//If conditions of search is changed 
				if (value != prevValue || 
					matchCase != prevMatchCase || 
					matchWholeWord != prevMatchWholeWord || 
                    AlwaysFindAgain)
				{
                    RichTextBox richForConvert = null;

                    var regex = new Regex(@"\s+"); 

					//Store conditions of search
					prevValue = value;
					prevMatchWholeWord = matchWholeWord;
					prevMatchCase = matchCase;

					//Match case property
					if (!matchCase) value = value.ToUpper(CultureInfo.InvariantCulture);
                    value = regex.Replace(value, " ");

					//Close previous find results
                    ClearFind(pages);

					//Init progress bar
                    InvokeProgressChanged(0, pages.Count);
			
					int index = 0;

                    #region Foreach all pages in report
                    foreach (StiPage page in pages)
					{
                        bool pageAdded = false;
                        InvokeProgressChanged(index++, pages.Count);

						//Foreach all components in one page
						StiComponentsCollection comps = page.GetComponents();
						foreach (StiComponent comp in comps)
						{
							IStiText text = comp as IStiText;
							//Process only text components
                            if (text != null && text.Text != null && text.GetTextInternal() != null)
							{
                                string s = text.GetTextInternal();

                                #region Process StiRichText
                                if (comp is StiRichText)
                                {
                                    if (richForConvert == null)
                                        richForConvert = new Controls.StiRichTextBox(false);
                                    richForConvert.Rtf = ((StiRichText)comp).RtfText;
                                    s = richForConvert.Text;
                                }
                                #endregion

                                var textComp = comp as StiText;
                                if (textComp != null && textComp.AllowHtmlTags)
                                {
                                    s = StiTextHelper.HtmlToPlainText(text.Text);
                                }

                                if (!matchCase) s = s.ToUpper(CultureInfo.InvariantCulture);
                                s = regex.Replace(s, " ");

                                #region Match WholeWord
                                //Match WholeWord
								if (matchWholeWord)
								{
									#region Start
                                    int startIndex = s.IndexOf(value, StringComparison.InvariantCulture);
									if (startIndex == -1)continue;

									if (startIndex > 0 && char.IsLetterOrDigit(s, startIndex - 1))continue;
									#endregion

									#region End
									int endIndex = startIndex + value.Length;
									if (endIndex < s.Length)
									{
										if (char.IsLetterOrDigit(s, endIndex))continue;
									}
									#endregion

									if (FindedComponents == null)
										FindedComponents = new List <StiComponent>();

									FindedComponents.Add(comp);

                                    if (!pageAdded)
                                    {
                                        if (HashOfFindedPages == null)
                                            HashOfFindedPages = new Hashtable();

                                        HashOfFindedPages[page] = page;
                                        pageAdded = true;
                                    }
                                }
                                #endregion

                                else 
								{
                                    if (s.IndexOf(value, StringComparison.InvariantCulture) >= 0)
									{
										if (FindedComponents == null)
                                            FindedComponents = new List<StiComponent>();

										FindedComponents.Add(comp);

                                        if (!pageAdded)
                                        {
                                            if (HashOfFindedPages == null)
                                                HashOfFindedPages = new Hashtable();

                                            HashOfFindedPages[page] = page;
                                            pageAdded = true;
                                        }
									}
								}
							}
						}
                    }
                    #endregion

                    #region Change hightlight condition of selected components
					if (FindedComponents != null)
					{
						foreach (StiComponent comp in FindedComponents)
						{
							comp.HighlightState = StiHighlightState.Show;
						}
                    }
                    #endregion

                    #region Mark first of selected components
                    if (FindedComponents != null && FindedComponents.Count > 0)
                    {
                        FindedComponents[0].HighlightState = StiHighlightState.Active;
                    }
                    else
                    {
                        IsEof = true;
                    }
                    #endregion

                    CurrentComponent = 0;

                    if (richForConvert != null)
                    {
                        richForConvert.Dispose();
                        richForConvert = null;
                    }
				}
				else
				{
                    if (FindedComponents != null && FindedComponents.Count > 0)
                    {
                        FindedComponents[CurrentComponent].HighlightState = StiHighlightState.Show;
                        CurrentComponent++;

                        if (CurrentComponent >= FindedComponents.Count)
                        {
                            if (AllowLoop)
                            {
                                CurrentComponent = 0;
                            }
                            else
                            {
                                CurrentComponent--;
                                IsEof = true;
                            }
                        }

                        FindedComponents[CurrentComponent].HighlightState = StiHighlightState.Active;
                    }
                    else IsEof = true;
				}

                FindActivated = true;
			}
		}

        public void StartFindPrevious(string value, StiReport report, bool matchCase, bool matchWholeWord)
        {
            if (report != null)
                StartFindPrevious(value, report.RenderedPages, matchCase, matchWholeWord);
        }

        public void StartFindPrevious(string value, StiPagesCollection pages, bool matchCase, bool matchWholeWord)
        {
            IsBof = false;
            IsEof = false;

            if (pages != null && pages.Count > 0)
            {
                //If conditions of search is changed
                if (value != prevValue ||
                    matchCase != prevMatchCase ||
                    matchWholeWord != prevMatchWholeWord ||
                    AlwaysFindAgain)
                {
                    //Store conditions of search
                    prevValue = value;
                    prevMatchWholeWord = matchWholeWord;
                    prevMatchCase = matchCase;

                    //Match case property
                    if (!matchCase) value = value.ToUpper(CultureInfo.InvariantCulture);

                    //Close previous find results
                    ClearFind(pages);

                    //Init progress bar
                    InvokeProgressChanged(0, pages.Count);

                    int index = 0;
                    //Foreach all pages in report
                    foreach (StiPage page in pages)
                    {
                        bool pageAdded = false;
                        InvokeProgressChanged(index++, pages.Count);

                        //Foreach all components in one page
                        StiComponentsCollection comps = page.GetComponents();
                        foreach (StiComponent comp in comps)
                        {
                            IStiText text = comp as IStiText;
                            //Process only text components
                            if (text != null && text.Text != null && text.GetTextInternal() != null)
                            {
                                string s = text.GetTextInternal();
                                if (!matchCase) s = s.ToUpper(CultureInfo.InvariantCulture);

                                //Match WholeWord
                                if (matchWholeWord)
                                {
                                    #region Start
                                    int startIndex = s.IndexOf(value, StringComparison.InvariantCulture);
                                    if (startIndex == -1) continue;

                                    if (startIndex > 0 && char.IsLetterOrDigit(s, startIndex - 1)) continue;
                                    #endregion

                                    #region End
                                    int endIndex = startIndex + value.Length;
                                    if (endIndex < s.Length)
                                    {
                                        if (char.IsLetterOrDigit(s, endIndex)) continue;
                                    }
                                    #endregion

                                    if (FindedComponents == null)
                                        FindedComponents = new List<StiComponent>();

                                    FindedComponents.Add(comp);

                                    if (!pageAdded)
                                    {
                                        if (HashOfFindedPages == null)
                                            HashOfFindedPages = new Hashtable();

                                        HashOfFindedPages[page] = page;
                                        pageAdded = true;
                                    }
                                }
                                else
                                {
                                    if (s.IndexOf(value, StringComparison.InvariantCulture) >= 0)
                                    {
                                        if (FindedComponents == null)
                                            FindedComponents = new List<StiComponent>();

                                        FindedComponents.Add(comp);

                                        if (!pageAdded)
                                        {
                                            if (HashOfFindedPages == null)
                                                HashOfFindedPages = new Hashtable();

                                            HashOfFindedPages[page] = page;
                                            pageAdded = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Change hightlight condition of selected components
                    if (FindedComponents != null)
                    {
                        foreach (StiComponent comp in FindedComponents)
                        {
                            comp.HighlightState = StiHighlightState.Show;
                        }
                    }

                    //Mark first of selected components
                    if (FindedComponents != null && FindedComponents.Count > 0) FindedComponents[0].HighlightState = StiHighlightState.Active;
                    else IsBof = true;

                    CurrentComponent = 0;
                }
                else
                {
                    if (FindedComponents != null && FindedComponents.Count > 0)
                    {
                        FindedComponents[CurrentComponent].HighlightState = StiHighlightState.Show;
                        CurrentComponent--;
                        if (CurrentComponent == -1)
                        {
                            if (AllowLoop) CurrentComponent = FindedComponents.Count - 1;
                            else
                            {
                                CurrentComponent = 0;
                                IsBof = true;
                            }
                        }
                        FindedComponents[CurrentComponent].HighlightState = StiHighlightState.Active;
                    }
                    else IsBof = true;
                }
                FindActivated = true;
            }
        }
		#endregion
	}
}