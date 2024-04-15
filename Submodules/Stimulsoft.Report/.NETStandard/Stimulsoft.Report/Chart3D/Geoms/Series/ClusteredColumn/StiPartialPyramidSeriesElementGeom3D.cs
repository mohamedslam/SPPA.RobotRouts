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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    internal class StiPartialPyramidSeriesElementGeom3D :
        StiSeriesElementGeom3D,
        IStiBorderColor,
        IStiColor,
        IStiDrawSidesGeom3D
    {
        #region Properties
        public Color Color { get; set; }

        public Color BorderColor { get; set; }

        public StiRectangle3D Rect3D { get; }

        public StiRectangle3D PartialRect3D { get; }

        public double[] BackFace => new double[] { 4, 5, 6, 7 };

        public double[] LeftFace => new double[] { 0, 4, 5, 1 };

        public double[] BottomFace => new double[] { 0, 3, 7, 4 };

        public double[] TopFace => new double[] { 1, 2, 6, 5 };

        public double[] RightFace => new double[] { 2, 3, 7, 6 };

        public double[] FrontFace => new double[] { 0, 1, 2, 3 };

        private bool drawLeft = true;
        public bool DrawLeft
        {
            get
            {
                return drawLeft;
            }
            set
            {
                drawLeft = value;
                BuildFaces();
                BuildColorFaces();
            }
        }

        private bool drawBack = true;
        public bool DrawBack
        {
            get
            {
                return drawBack;
            }
            set
            {
                drawBack = value;
                BuildFaces();
                BuildColorFaces();
            }
        }

        private bool drawTop = true;
        public bool DrawTop { 
            get
            {
                return drawTop;
            }
            set
            {
                drawTop = value;
                BuildFaces();
                BuildColorFaces();
            }
        }

        private bool drawBottom = true;
        public bool DrawBottom
        {
            get
            {
                return drawBottom;
            }
            set
            {
                drawBottom = value;
                BuildFaces();
                BuildColorFaces();
            }
        }
        #endregion

        #region Methods
        private void SetVertexes(StiRectangle3D r, StiRectangle3D pr)
        {
            var deltaBottom = r.Height/(pr.Y - r.Y);
            var deltaTop = r.Height/(pr.Top - r.Y);

            var deltaXTop = pr.Length / deltaTop / 2;
            var deltaZTop = r.Width / deltaTop / 2;

            var deltaXBot = pr.Length / deltaBottom / 2;
            var deltaZBot = r.Width / deltaBottom / 2;

            this.Vertexes = new StiMatrix(new double[,]
               {
                /*0*/{r.X + deltaXBot, pr.Y, r.Z + r.Width - deltaZBot, 1 },
                /*1*/{r.X + deltaXTop, pr.Y + pr.Height, r.Z + r.Width - deltaZTop, 1 },
                /*2*/{r.X + r.Length - deltaXTop, pr.Y + pr.Height, r.Z + r.Width - deltaZTop, 1 },
                /*3*/{r.X + r.Length - deltaXBot, pr.Y, r.Z + r.Width - deltaZBot, 1 },
                /*4*/{r.X + deltaXBot, pr.Y, r.Z + deltaZBot, 1 },
                /*5*/{r.X + deltaXTop, pr.Y + pr.Height, r.Z + deltaZTop, 1 },
                /*6*/{r.X + r.Length - deltaXTop, pr.Y + pr.Height, r.Z + deltaZTop, 1 },
                /*7*/{r.X + r.Length- deltaXBot, pr.Y, r.Z + deltaZBot, 1 }
               });
        }

        public override void DrawBorder(StiContext context, StiMatrix vertices)
        {
            var color = GetBorderColor();

            if (color == null) return;

            DrawFaceBorder(context, vertices, this.FrontFace, color.GetValueOrDefault());
            DrawFaceBorder(context, vertices, this.RightFace, color.GetValueOrDefault());
        }

        public void BuildFaces()
        {
            var listFaces = new List<double[]>()
            {
                RightFace,
                FrontFace
            };
            
            if (DrawTop)
                listFaces.Insert(0, TopFace);

            if (DrawBottom)
                listFaces.Insert(0, BottomFace);

            if (DrawBack)
                listFaces.Insert(0, BackFace);

            if (DrawLeft)
                listFaces.Insert(0, LeftFace);

            this.Faces = listFaces;
        }

        public void BuildColorFaces()
        {
            var colorsFaces = new List<Color>()
            {
                StiColorUtils.ChangeDarkness(Color, 0.4f),
                Color
            };
            
            if (DrawTop)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            if (DrawBottom)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            if (DrawBack)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            if (DrawLeft)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            this.ColorsFaces = colorsFaces.ToArray();
        }
        #endregion

        public StiPartialPyramidSeriesElementGeom3D(StiRectangle3D rect3D, StiRectangle3D partialRect3D, double value, int index, IStiSeries series, Color color, Color borderColor, StiRender3D render) :
            base(render, value, index, series, color)
        {
            this.Rect3D = rect3D;
            this.PartialRect3D = partialRect3D;

            SetVertexes(rect3D, partialRect3D);

            this.Color = color;
            this.BorderColor = borderColor;

            this.BuildFaces();
            this.BuildColorFaces();
        }
    }
}
