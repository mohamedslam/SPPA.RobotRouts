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

namespace Stimulsoft.Base.Drawing
{ 	

	public static class StiTextRotationHelper
	{
		public static void CalculateAngle(StiTextDockMode dockMode, ref float angle, ref StiRotationMode rotationMode)
		{
			switch (dockMode)
			{
				case StiTextDockMode.Top:
					if (angle == 0f)rotationMode = StiRotationMode.CenterBottom;
					else if (angle >= 0)rotationMode = StiRotationMode.LeftCenter;
					else if (angle <= -0)rotationMode = StiRotationMode.RightCenter;
					break;

				case StiTextDockMode.Bottom:
					if (angle == 0f)rotationMode = StiRotationMode.CenterTop;
					else if (angle >= 0)rotationMode = StiRotationMode.RightCenter;
					else if (angle <= -0)rotationMode = StiRotationMode.LeftCenter;
					break;

				case StiTextDockMode.Left:
					rotationMode = StiRotationMode.RightCenter;
					break;

				case StiTextDockMode.Right:
					rotationMode = StiRotationMode.LeftCenter;
					break;
			}
		}
	}
}
