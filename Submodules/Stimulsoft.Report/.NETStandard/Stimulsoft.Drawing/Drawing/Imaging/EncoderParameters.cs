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

namespace Stimulsoft.Drawing.Imaging
{
    public sealed class EncoderParameters : IDisposable
    {
        private EncoderParameter[] parameters;

        public EncoderParameters()
        {
            parameters = new EncoderParameter[1];
        }

        public EncoderParameters(int count)
        {
            parameters = new EncoderParameter[count];
        }

        public EncoderParameter[] Param
        {
            get
            {
                return parameters;
            }

            set
            {
                parameters = value;
            }
        }

        public void Dispose()
        {
        }

        public static implicit operator System.Drawing.Imaging.EncoderParameters(EncoderParameters parameters)
        {
            var netParameters = new System.Drawing.Imaging.EncoderParameters(parameters.parameters.Length);

            for (var index = 0; index < parameters.parameters.Length; index++)
            {
                netParameters.Param[index] = parameters.parameters[index];
            }

            return netParameters;
        }
    }
}