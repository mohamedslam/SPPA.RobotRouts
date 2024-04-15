#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Json.Linq
{
    public static class JsonExtensions
    {
        #region Методы для сохранения Отчета в JSON
        public static void RemoveProperty(this JObject jObject, string propertyName)
        {
            foreach (JProperty token in jObject.ChildrenTokens)
            {
                if (token.Name == propertyName)
                {
                    jObject.ChildrenTokens.Remove(token);
                    return;
                }
            }
        }

        public static void AddPropertyInt(this JObject jObject, string propertyName, int value, int defaultValue = 0)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyIntNoDefaultValue(this JObject jObject, string propertyName, int value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFloat(this JObject jObject, string propertyName, float value, float defaultValue = 0f)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFloatNullable(this JObject jObject, string propertyName, float? value, float? defaultValue)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyDouble(this JObject jObject, string propertyName, double value, double defaultValue = 0d)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyShort(this JObject jObject, string propertyName, short value, short defaultValue = 0)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyDouble(this JObject jObject, string propertyName, double value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyDecimal(this JObject jObject, string propertyName, decimal value, decimal defaultValue)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyDecimal(this JObject jObject, string propertyName, decimal value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyIntArray(this JObject jObject, string propertyName, int[] array)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.IntArray(array);
            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyJObject(this JObject jObject, string propertyName, JObject value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyStringArray(this JObject jObject, string propertyName, string[] array)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.StringArray(array);
            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyBoolArray(this JObject jObject, string propertyName, bool[] array, bool ignoreDefaultValues = false)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.BoolArray(array, ignoreDefaultValues);
            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyListString(this JObject jObject, string propertyName, List<string> array)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.ListString(array);
            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyPoint(this JObject jObject, string propertyName, Point point)
        {
            //Removes property if it was saved previously
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.Point(point);
            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyPointF(this JObject jObject, string propertyName, PointF point)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.PointF(point);
            if (value == null || value.Count == 0)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyIdent(this JObject jObject, string propertyName, string value)
        {
            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyBool(this JObject jObject, string propertyName, bool value, bool defaultValue = false, bool ignoreDefaultValues = false)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == defaultValue && !ignoreDefaultValues)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Записываем Ticks у заданной даты
        public static void AddPropertyDateTime(this JObject jObject, string propertyName, DateTime value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            string jsonMsDate = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            }).Replace("\"", string.Empty).Replace(@"\", string.Empty);

            jObject.Add(new JProperty(propertyName, jsonMsDate));
        }


        // Записываем Ticks у заданной даты
        public static void AddPropertyTimeSpan(this JObject jObject, string propertyName, TimeSpan value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            jObject.Add(new JProperty(propertyName, value.Ticks));
        }

        // Добавляет заданное Enum значение, если оно не равно defaultValue(значения не должны быть равны null)
        public static void AddPropertyEnum(this JObject jObject, string propertyName, Enum value, Enum defaultValue)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value.Equals(defaultValue))
                return;

            jObject.Add(new JProperty(propertyName, value.ToString()));
        }

        // Добавляет заданное Enum значение
        public static void AddPropertyEnum(this JObject jObject, string propertyName, Enum value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value != null)
                jObject.Add(new JProperty(propertyName, value.ToString()));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyString(this JObject jObject, string propertyName, string value, string defaultValue)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (value == null || value.Equals(defaultValue))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyString(this JObject jObject, string propertyName, string value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyRectangleD(this JObject jObject, string propertyName, RectangleD rect)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.RectangleD(rect);
            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertySizeD(this JObject jObject, string propertyName, SizeD size, string defaultValue = "0,0")
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.SizeD(size);
            if (value == null || value.Equals(defaultValue))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertySize(this JObject jObject, string propertyName, Size size)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.Size(size);
            if (value == null)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyShadow(this JObject jObject, string propertyName, StiSimpleShadow shadow)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.JShadow(shadow);
            if (value == null)
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyCornerRadius(this JObject jObject, string propertyName, StiCornerRadius cornerRadius)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = cornerRadius.ToString();
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyStringNullOrEmpty(this JObject jObject, string propertyName, string value)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyCap(this JObject jObject, string propertyName, StiCap cap)
        {
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.JCap(cap);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyByteArray(this JObject jObject, string propertyName, byte[] buffer)
        {
            RemoveProperty(jObject, propertyName);

            var value = global::System.Convert.ToBase64String(buffer);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyImage(this JObject jObject, string propertyName, Image image)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiImageConverter.ImageToString(image);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }
        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyBrush(this JObject jObject, string propertyName, StiBrush brush)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.JBrush(brush);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyBorder(this JObject jObject, string propertyName, StiSimpleBorder border)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            if (border == null)
                return;

            var value = StiJsonReportObjectHelper.Serialize.JBorder(border);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyBorder(this JObject jObject, string propertyName, StiBorder border)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.JBorder(border);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyColor(this JObject jObject, string propertyName, Color color, string htmlDefaultColor)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.JColor(color, ColorTranslator.FromHtml(htmlDefaultColor));
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyArrayColor(this JObject jObject, string propertyName, Color[] colors)
        {
            var res = new JObject();

            for (int index = 0; index < colors.Length; index++)
            {
                var color = colors[index];
                string colorStr = StiJsonReportObjectHelper.Serialize.JColor(color);
                res.AddPropertyString(index.ToString(), colorStr);
            }

            jObject.AddPropertyJObject(propertyName, res);
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyColor(this JObject jObject, string propertyName, Color color, Color defaultColor)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.JColor(color, defaultColor);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        #region Font
        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontArial8(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial8(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFontMicrosoftSansSerif8(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontMicrosoftSansSerif8(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFontTahoma8(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontTahoma8(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFontTahoma12Bold(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontTahoma12Bold(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFontArial8BoldPixel(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial8BoldPixel(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontArial7(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial7(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontArial10(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial10(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontArial13(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial13(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontArial11(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial11(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontArial12Bold(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial12Bold(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFontSegoeUI12Bold(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontSegoeUI12Bold(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        public static void AddPropertyFontArial14Bold(this JObject jObject, string propertyName, Font font)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.FontArial14Bold(font);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }

        // Добавляет заданное значение, если оно не IsNullOrEmpty 
        public static void AddPropertyFont(this JObject jObject, string propertyName, Font font, string defaultFamily,
            float defaultEmSize)
        {
            // Ищем и удаляем свойство, если оно было уже добавлено ранее
            RemoveProperty(jObject, propertyName);

            var value = StiJsonReportObjectHelper.Serialize.Font(font, defaultFamily, defaultEmSize);
            if (string.IsNullOrEmpty(value))
                return;

            jObject.Add(new JProperty(propertyName, value));
        }
        #endregion

        #endregion
    }
}
