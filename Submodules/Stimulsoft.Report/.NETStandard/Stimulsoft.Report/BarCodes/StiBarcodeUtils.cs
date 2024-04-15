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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    public class StiBarcodeUtils
    {
        #region GaloisField
        public sealed class GaloisField
        {
            public static GaloisField Aztec_Data_12 = new GaloisField(0x1069, 4096, 1); // x^12 + x^6 + x^5 + x^3 + 1
            public static GaloisField Aztec_Data_10 = new GaloisField(0x409, 1024, 1); // x^10 + x^3 + 1
            public static GaloisField Aztec_Data_8 = new GaloisField(0x012D, 256, 1); // x^8 + x^5 + x^3 + x^2 + 1
            public static GaloisField Aztec_Data_6 = new GaloisField(0x43, 64, 1); // x^6 + x + 1
            public static GaloisField Aztec_Param = new GaloisField(0x13, 16, 1); // x^4 + x + 1
            public static GaloisField QRCode_256 = new GaloisField(0x011D, 256, 0); // x^8 + x^4 + x^3 + x^2 + 1
            public static GaloisField Maxicode_64 = Aztec_Data_6;
            public static GaloisField DataMatrix_256 = Aztec_Data_8;

            private readonly int[] expTable;
            private readonly int[] logTable;
            private readonly GaloisFieldPolynomial zero;
            private readonly GaloisFieldPolynomial one;
            private readonly int size;
            private readonly int generatorBase;

            public GaloisField(int primitive, int size, int genBase)
            {
                this.size = size;
                this.generatorBase = genBase;

                expTable = new int[size];
                logTable = new int[size];
                int x = 1;
                for (int i = 0; i < size; i++)
                {
                    expTable[i] = x;
                    x <<= 1;
                    if (x >= size)
                    {
                        x ^= primitive;
                        x &= size - 1;
                    }
                }
                for (int i = 0; i < size - 1; i++)
                {
                    logTable[expTable[i]] = i;
                }
                zero = new GaloisFieldPolynomial(this, new int[] { 0 });
                one = new GaloisFieldPolynomial(this, new int[] { 1 });
            }

            internal GaloisFieldPolynomial GetZero() => zero;

            internal GaloisFieldPolynomial GetOne() => one;

            internal int GetGeneratorBase() => generatorBase;

            internal GaloisFieldPolynomial BuildMonomial(int degree, int coefficient)
            {
                if (degree < 0)
                {
                    throw new ArgumentException();
                }
                if (coefficient == 0)
                {
                    return zero;
                }
                int[] coefficients = new int[degree + 1];
                coefficients[0] = coefficient;
                return new GaloisFieldPolynomial(this, coefficients);
            }

            internal static int AddOrSubtract(int a, int b)
            {
                return a ^ b;
            }

            internal int Exp(int a)
            {
                return expTable[a];
            }

            internal int Inverse(int a)
            {
                if (a == 0)
                {
                    throw new ArithmeticException();
                }
                return expTable[size - logTable[a] - 1];
            }

            internal int Multiply(int a, int b)
            {
                if (a == 0 || b == 0)
                {
                    return 0;
                }
                return expTable[(logTable[a] + logTable[b]) % (size - 1)];
            }
        }
        #endregion

        #region GaloisFieldPolynomial
        internal sealed class GaloisFieldPolynomial
        {
            private readonly GaloisField field;
            private readonly int[] coefficients;

            internal GaloisFieldPolynomial(GaloisField field, int[] coefficients)
            {
                if (coefficients == null || coefficients.Length == 0)
                {
                    throw new ArgumentException();
                }
                this.field = field;
                int coefficientsLength = coefficients.Length;
                if (coefficientsLength > 1 && coefficients[0] == 0)
                {
                    int firstNonZero = 1;
                    while (firstNonZero < coefficientsLength && coefficients[firstNonZero] == 0)
                    {
                        firstNonZero++;
                    }
                    if (firstNonZero == coefficientsLength)
                    {
                        this.coefficients = new int[] { 0 };
                    }
                    else
                    {
                        this.coefficients = new int[coefficientsLength - firstNonZero];
                        Array.Copy(coefficients,
                            firstNonZero,
                            this.coefficients,
                            0,
                            this.coefficients.Length);
                    }
                }
                else
                {
                    this.coefficients = coefficients;
                }
            }

            internal int[] GetCoefficients() => coefficients;

            internal int GetDegree() => coefficients.Length - 1;

            internal bool IsZero => coefficients[0] == 0;

            internal int GetCoefficient(int degree)
            {
                return coefficients[coefficients.Length - 1 - degree];
            }

            //internal int EvaluateAt(int a)
            //{
            //    int result = 0;
            //    if (a == 0)
            //    {
            //        return GetCoefficient(0);
            //    }
            //    if (a == 1)
            //    {
            //        foreach (var coefficient in coefficients)
            //        {
            //            result = GaloisField.AddOrSubtract(result, coefficient);
            //        }
            //        return result;
            //    }
            //    result = coefficients[0];
            //    int size = coefficients.Length;
            //    for (int i = 1; i < size; i++)
            //    {
            //        result = GaloisField.AddOrSubtract(field.Multiply(a, result), coefficients[i]);
            //    }
            //    return result;
            //}

            internal GaloisFieldPolynomial AddOrSubtract(GaloisFieldPolynomial other)
            {
                if (!field.Equals(other.field))
                {
                    throw new ArgumentException("GaloisFieldPolynomial do not have same GaloisField field");
                }
                if (IsZero)
                {
                    return other;
                }
                if (other.IsZero)
                {
                    return this;
                }

                var smallerCoefficients = this.coefficients;
                var largerCoefficients = other.coefficients;
                if (smallerCoefficients.Length > largerCoefficients.Length)
                {
                    var temp = smallerCoefficients;
                    smallerCoefficients = largerCoefficients;
                    largerCoefficients = temp;
                }
                var sumDiff = new int[largerCoefficients.Length];
                int lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;
                Array.Copy(largerCoefficients, 0, sumDiff, 0, lengthDiff);

                for (int i = lengthDiff; i < largerCoefficients.Length; i++)
                {
                    sumDiff[i] = GaloisField.AddOrSubtract(smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
                }

                return new GaloisFieldPolynomial(field, sumDiff);
            }

            internal GaloisFieldPolynomial Multiply(GaloisFieldPolynomial other)
            {
                if (!field.Equals(other.field))
                {
                    throw new ArgumentException("GaloisFieldPolynomial do not have same GaloisField field");
                }
                if (IsZero || other.IsZero)
                {
                    return field.GetZero();
                }
                int[] aCoefficients = this.coefficients;
                int aLength = aCoefficients.Length;
                int[] bCoefficients = other.coefficients;
                int bLength = bCoefficients.Length;
                int[] product = new int[aLength + bLength - 1];
                for (int i = 0; i < aLength; i++)
                {
                    int aCoeff = aCoefficients[i];
                    for (int j = 0; j < bLength; j++)
                    {
                        product[i + j] = GaloisField.AddOrSubtract(product[i + j],
                            field.Multiply(aCoeff, bCoefficients[j]));
                    }
                }
                return new GaloisFieldPolynomial(field, product);
            }

            //internal GaloisFieldPolynomial Multiply(int scalar)
            //{
            //    if (scalar == 0)
            //    {
            //        return field.GetZero();
            //    }
            //    if (scalar == 1)
            //    {
            //        return this;
            //    }
            //    int size = coefficients.Length;
            //    int[] product = new int[size];
            //    for (int i = 0; i < size; i++)
            //    {
            //        product[i] = field.Multiply(coefficients[i], scalar);
            //    }
            //    return new GaloisFieldPolynomial(field, product);
            //}

            internal GaloisFieldPolynomial MultiplyByMonomial(int degree, int coefficient)
            {
                if (degree < 0)
                {
                    throw new ArgumentException();
                }
                if (coefficient == 0)
                {
                    return field.GetZero();
                }
                int size = coefficients.Length;
                int[] product = new int[size + degree];
                for (int i = 0; i < size; i++)
                {
                    product[i] = field.Multiply(coefficients[i], coefficient);
                }
                return new GaloisFieldPolynomial(field, product);
            }

            internal GaloisFieldPolynomial[] Divide(GaloisFieldPolynomial other)
            {
                if (!field.Equals(other.field))
                {
                    throw new ArgumentException("GaloisFieldPolynomial do not have same GaloisField field");
                }
                if (other.IsZero)
                {
                    throw new ArgumentException("Divide by zero");
                }

                GaloisFieldPolynomial quotient = field.GetZero();
                GaloisFieldPolynomial remainder = this;

                int denominatorLeadingTerm = other.GetCoefficient(other.GetDegree());
                int inverseDenominatorLeadingTerm = field.Inverse(denominatorLeadingTerm);

                while (remainder.GetDegree() >= other.GetDegree() && !remainder.IsZero)
                {
                    int degreeDifference = remainder.GetDegree() - other.GetDegree();
                    int scale = field.Multiply(remainder.GetCoefficient(remainder.GetDegree()), inverseDenominatorLeadingTerm);
                    GaloisFieldPolynomial term = other.MultiplyByMonomial(degreeDifference, scale);
                    GaloisFieldPolynomial iterationQuotient = field.BuildMonomial(degreeDifference, scale);
                    quotient = quotient.AddOrSubtract(iterationQuotient);
                    remainder = remainder.AddOrSubtract(term);
                }

                return new GaloisFieldPolynomial[] { quotient, remainder };
            }

        }
        #endregion

        #region ReedSolomonEncoder
        public sealed class ReedSolomonEncoder
        {
            private readonly GaloisField field;
            private readonly IList<GaloisFieldPolynomial> cachedGenerators;

            public ReedSolomonEncoder(GaloisField field)
            {
                this.field = field;
                this.cachedGenerators = new List<GaloisFieldPolynomial>();
                cachedGenerators.Add(new GaloisFieldPolynomial(field, new int[] { 1 }));
            }

            private GaloisFieldPolynomial BuildGenerator(int degree)
            {
                if (degree >= cachedGenerators.Count)
                {
                    var lastGenerator = cachedGenerators[cachedGenerators.Count - 1];
                    for (int d = cachedGenerators.Count; d <= degree; d++)
                    {
                        var nextGenerator = lastGenerator.Multiply(new GaloisFieldPolynomial(field, new int[] { 1, field.Exp(d - 1 + field.GetGeneratorBase()) }));
                        cachedGenerators.Add(nextGenerator);
                        lastGenerator = nextGenerator;
                    }
                }
                return cachedGenerators[degree];
            }

            public void Encode(int[] toEncode, int ecBytes)
            {
                if (ecBytes == 0)
                {
                    throw new ArgumentException("No ErrorCorrection bytes");
                }
                var dataBytes = toEncode.Length - ecBytes;
                if (dataBytes <= 0)
                {
                    throw new ArgumentException("No data bytes");
                }

                var generator = BuildGenerator(ecBytes);
                var infoCoefficients = new int[dataBytes];
                Array.Copy(toEncode, 0, infoCoefficients, 0, dataBytes);

                var info = new GaloisFieldPolynomial(field, infoCoefficients);
                info = info.MultiplyByMonomial(ecBytes, 1);

                var remainder = info.Divide(generator)[1];
                var coefficients = remainder.GetCoefficients();
                var numZeroCoefficients = ecBytes - coefficients.Length;
                for (var i = 0; i < numZeroCoefficients; i++)
                {
                    toEncode[dataBytes + i] = 0;
                }

                Array.Copy(coefficients, 0, toEncode, dataBytes + numZeroCoefficients, coefficients.Length);
            }
        }
        #endregion

        #region BitMatrix
        public sealed class BitMatrix
        {
            private readonly int width;
            private readonly int height;
            private readonly int rowSize;
            private readonly int[] bits;

            public int Width => width;
            public int Height => height;

            public BitMatrix(int dimension)
               : this(dimension, dimension)
            {
            }

            public BitMatrix(int width, int height)
            {
                if (width < 1 || height < 1)
                {
                    throw new ArgumentException("Both dimensions must be greater than 0");
                }
                this.width = width;
                this.height = height;
                this.rowSize = (width + 31) >> 5;
                bits = new int[rowSize * height];
            }

            public bool this[int x, int y]
            {
                get
                {
                    int offset = y * rowSize + (x >> 5);
                    return (((int)((uint)(bits[offset]) >> (x & 0x1f))) & 1) != 0;
                }
                set
                {
                    if (value)
                    {
                        int offset = y * rowSize + (x >> 5);
                        bits[offset] |= 1 << (x & 0x1f);
                    }
                    else
                    {
                        int offset = y * rowSize + (x / 32);
                        bits[offset] &= ~(1 << (x & 0x1f));
                    }
                }
            }
        }
        #endregion

    }
}
