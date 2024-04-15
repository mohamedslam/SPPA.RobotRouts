#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Drawing
{
    public struct CharacterRange
    {
        public int First { get; set; }

        public int Length { get; set; }

        //public bool Equals(object obj)
        //{
        //    if (!(obj is CharacterRange))
        //        return false;

        //    CharacterRange cr = (CharacterRange)obj;
        //    return this == cr;
        //}

        //public int GetHashCode()
        //{
        //    return (First ^ Length);
        //}

        //public static bool operator ==(CharacterRange cr1, CharacterRange cr2)
        //{
        //    return ((cr1.First == cr2.First) && (cr1.Length == cr2.Length));
        //}

        //public static bool operator !=(CharacterRange cr1, CharacterRange cr2)
        //{
        //    return ((cr1.First != cr2.First) || (cr1.Length != cr2.Length));
        //}

        public CharacterRange(int first, int length)
        {
            First = first;
            Length = length;
        }

        public static implicit operator System.Drawing.CharacterRange(CharacterRange range)
        {
            var netRange = new System.Drawing.CharacterRange();
            netRange.First = range.First;
            netRange.Length = range.Length;

            return netRange;
        }

        public static implicit operator CharacterRange(System.Drawing.CharacterRange netRange)
        {
            var range = new CharacterRange();
            range.First = netRange.First;
            range.Length = netRange.Length;

            return range;
        }
    }
}