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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Localization;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dictionary;
using System.IO;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiHyperlinkProcessor
    {
        #region Methods.Get
        public static byte[] TryGetBytes(StiReport report, string hyperlink, bool firstPositionInDataSource = false, bool allowDataLoading = false)
        {
            try
            {
                return GetBytes(report, hyperlink, firstPositionInDataSource, allowDataLoading);
            }
            catch
            {
                return null;
            }            
        }

        public static byte[] GetBytes(StiReport report, string hyperlink, bool firstPositionInDataSource = false, bool allowDataLoading = false)
        {
            var resourceName = GetResourceNameFromHyperlink(hyperlink);
            if (resourceName != null)
            {
                var resource = GetResource(report, resourceName);
                return resource?.Content;
            }

            var variableName = GetVariableNameFromHyperlink(hyperlink);
            if (variableName != null)
            {
                var variable = GetVariable(report, variableName);
                if (variable != null)
                {
                    if (variable.ValueObject is Image)
                        return StiImageConverter.ImageToBytes(variable.ValueObject as Image, true);

                    if (variable.ValueObject is byte[] && StiImageHelper.IsImage(variable.ValueObject as byte[]))
                        return variable.ValueObject as byte[];

                    if (variable.ValueObject is string && IsHttpOrHttpsHyperlink(variable.ValueObject as string))
                        return StiBytesFromURL.Load(variable.ValueObject as string, report?.CookieContainer, report?.HttpHeadersContainer);

#if !CLOUD
                    if (variable.ValueObject is string && IsFilePath(variable.ValueObject as string))
                        return File.ReadAllBytes(variable.ValueObject as string);
#endif
                }
                return null;
            }

            var dataColumnName = GetDataColumnNameFromHyperlink(hyperlink);
            if (dataColumnName != null)
            {
                if (allowDataLoading)
                {
                    var bytes = GetBytesFromColumnWithLoading(report, dataColumnName);
                    if (bytes != null)
                        return bytes;
                }

                return StiDataColumn.GetDatasFromDataColumn(report.Dictionary, dataColumnName, null, firstPositionInDataSource).FirstOrDefault() as byte[];
            }

            var file = GetFileNameFromHyperlink(hyperlink);
            if (file != null)
                return File.Exists(file) ? File.ReadAllBytes(file) : null;

            return StiDownloadCache.Get(hyperlink);
        }

        private static byte[] GetBytesFromColumnWithLoading(StiReport report, string dataColumnName)
        {
            var dataSource = StiDataColumn.GetDataSourceFromDataColumn(report.Dictionary, dataColumnName);
            if (dataSource == null)
                return null;

            var dataTable = StiDataPicker.Fetch(report, dataSource);
            if (dataTable == null)
                return null;

            if (!dataTable.Columns.Contains(dataColumnName)) 
                return null;

            var obj = dataTable.Rows[0][dataColumnName];
            if (obj is string)
            {
                try
                {
                    obj = global::System.Convert.FromBase64String(obj as string);
                }
                catch { };
            }
            return obj as byte[];
        }

        public static Image GetImage(StiReport report, string hyperlink, int width, int height)
        {
            var resourceName = GetResourceNameFromHyperlink(hyperlink);
            if (resourceName != null)
            {
                var resource = GetResource(report, resourceName);
                return resource?.GetResourceAsImage();
            }

            var variableName = GetVariableNameFromHyperlink(hyperlink);
            if (variableName != null)
            {
                var variable = GetVariable(report, variableName);
                var value = variable?.ValueObject;

                return value is byte[]? StiImageConverter.BytesToImage(value as byte[])
                    : value as Image;
            }

            var dataColumnName = GetDataColumnNameFromHyperlink(hyperlink);
            if (dataColumnName != null)
            {
                var value = StiDataColumn.GetDatasFromDataColumn(report.Dictionary, dataColumnName).FirstOrDefault();

                return value is byte[]? StiImageConverter.BytesToImage(value as byte[])
                    : value as Image;
            }

            var file = GetFileNameFromHyperlink(hyperlink);
            if (file != null)
                return StiImageHelper.FromFile(file, width, height);

            return StiImageFromURL.LoadImage(hyperlink, report?.CookieContainer, report?.HttpHeadersContainer);
        }

        public static string GetString(StiReport report, string hyperlink)
        {
            var resourceName = GetResourceNameFromHyperlink(hyperlink);
            if (resourceName != null)
            {
                var resource = GetResource(report, resourceName);
                return resource != null ? StiBytesToStringConverter.ConvertBytesToString(resource.Content) : null;
            }

            var variableName = GetVariableNameFromHyperlink(hyperlink);
            if (variableName != null)
            {
                var variable = GetVariable(report, variableName);
                return variable?.ValueObject as string;
            }

            var dataColumnName = GetDataColumnNameFromHyperlink(hyperlink);
            if (dataColumnName != null)
                return StiDataColumn.GetDatasFromDataColumn(report.Dictionary, dataColumnName).FirstOrDefault() as string;

            var file = GetFileNameFromHyperlink(hyperlink);
            if (file != null)
                return File.Exists(file) ? File.ReadAllText(file) : null;

            return StiBytesToStringConverter.ConvertBytesToString(StiDownloadCache.Get(hyperlink));
        }

        private static StiResource GetResource(StiReport report, string resourceName)
        {
            if (report == null || string.IsNullOrWhiteSpace(resourceName)) 
                return null;

            resourceName = resourceName.ToLowerInvariant().Trim();

            return report.Dictionary.Resources.ToList()
                .FirstOrDefault(r => r.Name != null && r.Name.ToLowerInvariant().Trim() == resourceName);
        }

        private static StiVariable GetVariable(StiReport report, string variableName)
        {
            if (report == null || string.IsNullOrWhiteSpace(variableName)) 
                return null;

            variableName = variableName.ToLowerInvariant().Trim();

            return report.Dictionary.Variables.ToList()
                .FirstOrDefault(v => v.Name != null && v.Name.ToLowerInvariant().Trim() == variableName);
        }

        private static StiDataColumn GetDataColumn(StiReport report, string dataColumnName)
        {
            if (report == null || string.IsNullOrWhiteSpace(dataColumnName)) 
                return null;

            dataColumnName = dataColumnName.ToLowerInvariant().Trim();

            return StiDataPathFinder.GetColumnFromPath(dataColumnName, report.Dictionary);
        }

        public static string GetServerNameFromHyperlink(string hyperlink)
        {
            return IsServerHyperlink(hyperlink) ? hyperlink.Remove(0, ServerIdent.Length) : null;
        }

        public static string GetResourceNameFromHyperlink(string hyperlink)
        {
            return IsResourceHyperlink(hyperlink) ? hyperlink.Remove(0, ResourceIdent.Length) : null;
        }

        public static string GetVariableNameFromHyperlink(string hyperlink)
        {
            return IsVariableHyperlink(hyperlink) ? hyperlink.Remove(0, VariableIdent.Length) : null;
        }

        public static string GetDataColumnNameFromHyperlink(string hyperlink)
        {
            return IsDataColumnHyperlink(hyperlink) ? hyperlink.Remove(0, DataColumnIdent.Length) : null;
        }

        /// <summary>
        /// Returns real-existing column name from hyperlink. Otherwise its returns null.
        /// </summary>
        /// <returns></returns>
        public static string GetRealDataColumnFromHyperlink(StiReport report, string hyperlink)
        {
            var dataColumnName = GetDataColumnNameFromHyperlink(hyperlink);
            if (string.IsNullOrWhiteSpace(dataColumnName))
                return null;

            var dataSource = StiDataColumn.GetDataSourceFromDataColumn(report.Dictionary, dataColumnName);
            if (dataSource == null)
                return null;

            var columnName = StiDataColumn.GetColumnNameFromDataColumn(report.Dictionary, dataColumnName);
            if (string.IsNullOrWhiteSpace(columnName))
                return null;

            return dataColumnName;
        }

        public static string GetFileNameFromHyperlink(string hyperlink)
        {
            return IsFileHyperlink(hyperlink) ? hyperlink.Remove(0, FileIdent.Length) : null;
        }

        public static bool IsServerHyperlink(string hyperlink)
        {
            return !string.IsNullOrWhiteSpace(hyperlink) && hyperlink.StartsWithInvariantIgnoreCase(ServerIdent);
        }

        public static bool IsResourceHyperlink(string hyperlink)
        {
            return !string.IsNullOrWhiteSpace(hyperlink) && hyperlink.StartsWithInvariantIgnoreCase(ResourceIdent);
        }

        public static bool IsVariableHyperlink(string hyperlink)
        {
            return !string.IsNullOrWhiteSpace(hyperlink) && hyperlink.StartsWithInvariantIgnoreCase(VariableIdent);
        }

        public static bool IsDataColumnHyperlink(string hyperlink)
        {
            return !string.IsNullOrWhiteSpace(hyperlink) && hyperlink.StartsWithInvariantIgnoreCase(DataColumnIdent);
        }

        public static bool IsFileHyperlink(string hyperlink)
        {
            return !string.IsNullOrWhiteSpace(hyperlink) && hyperlink.StartsWithInvariantIgnoreCase(FileIdent);
        }

        public static bool IsFilePath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || path.Length > 260)
                    return false;

                Path.GetFullPath(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool IsHttpOrHttpsHyperlink(string hyperlink)
        {
            return !string.IsNullOrWhiteSpace(hyperlink) && (hyperlink.StartsWithInvariantIgnoreCase(HttpIdent) || hyperlink.StartsWithInvariantIgnoreCase(HttpsIdent));
        }

        public static string CreateResourceName(string name)
        {
            return $"{ResourceIdent}{name}";
        }

        public static string CreateVariableName(string name)
        {
            return $"{VariableIdent}{name}";
        }

        public static string CreateDataColumnName(string name)
        {
            return $"{DataColumnIdent}{name}";
        }

        public static string CreateFileName(string path)
        {
            return $"{FileIdent}{path}";
        }

        public static string HyperlinkToString(string hyperlink)
        {
            if (IsResourceHyperlink(hyperlink))
                return $"{Loc.Get("PropertyMain", "Resource")}: {GetResourceNameFromHyperlink(hyperlink)}";

            if (IsVariableHyperlink(hyperlink))
                return $"{Loc.Get("PropertyMain", "Variable")}: {GetVariableNameFromHyperlink(hyperlink)}";

            if (IsDataColumnHyperlink(hyperlink))
                return $"{Loc.Get("PropertyMain", "DataColumn")}: {GetDataColumnNameFromHyperlink(hyperlink)}";

            if (IsServerHyperlink(hyperlink))
                return $"Server: {GetVariableNameFromHyperlink(hyperlink)}";

            if (IsFileHyperlink(hyperlink))
                return $"{Loc.Get("PropertyMain", "File")}: {GetFileNameFromHyperlink(hyperlink)}";

            return $"{Loc.Get("PropertyMain", "Hyperlink")}: {hyperlink}";
        }
        #endregion

        #region Consts
        public const string ServerIdent = "stimulsoft-server://";
        public const string ResourceIdent = "resource://";
        public const string VariableIdent = "variable://";
        public const string DataColumnIdent = "datacolumn://";
        public const string FileIdent = "file://";
        public const string HttpIdent = "http://";
        public const string HttpsIdent = "https://";
        #endregion
    }
}
