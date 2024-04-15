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
using System.Linq;
using System.Text;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Dashboard;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that used for report names creation.
    /// </summary>
    public sealed class StiNameCreation
	{
		#region Properties
		public static StiNamingRule NamingRule
		{
			get
			{
				return StiOptions.Engine.NamingRule;
			}
			set
			{
				StiOptions.Engine.NamingRule = value;
			}
		}
        #endregion

        #region Methods
        private static string RemoveSpacesFromName(string baseName, bool removeIncorrectSymbols, StiReport report)
        {
            if (!removeIncorrectSymbols)            
                return baseName;
            
            var sb = new StringBuilder(baseName);
            var charIndex = 0;
            var len = baseName.Length;

            for (int pos = 0; pos < len; pos++)
            {
				if (baseName[pos] == ' ')
				{
					sb.Remove(charIndex, 1);

					if (charIndex < sb.Length && char.IsLetter(sb[charIndex]))
						sb[charIndex] = char.ToUpper(sb[charIndex]);
				}
				else
				{
					charIndex++;
				}
            }
            baseName = sb.ToString();

            return StiNameValidator.CorrectName(baseName, report);
        }

        public static string CreateSimpleName(StiReport report, string baseName)
		{
			baseName = RemoveSpacesFromName(baseName, true, report);
			return $"{baseName}{report.IndexName++}";
		}

		/// <summary>
		/// Creates a name from the base name which is correct for the report.
		/// </summary>
		/// <param name="report">Report.</param>
		/// <param name="baseName">The type to form a name.</param>
		public static string CreateName(StiReport report, string baseName)
		{
			return CreateName(report, baseName, true, true);
		}

		public static string CreateName(StiReport report, string baseName, bool addOne, bool removeIncorrectSymbols)
		{
			return CreateName(report, baseName, addOne, removeIncorrectSymbols, false);
		}

		/// <summary>
		/// Creates a name from the base name which is correct for the report.
		/// </summary>
		/// <param name="report">Report.</param>
		/// <param name="baseName">The type to form a name.</param>
		/// <returns></returns>
		public static string CreateName(StiReport report, string baseName, bool addOne, bool removeIncorrectSymbols, bool forceAdvancedNamingRule)
		{			
			baseName = RemoveSpacesFromName(baseName, removeIncorrectSymbols, report);

			if (!forceAdvancedNamingRule)
			{
				if ((!report.IsDesigning) || NamingRule == StiNamingRule.Simple)
					return $"{baseName}{report.IndexName++}";
			}

			var comps = report.GetComponents();
			var counter = 1;
		
			if (comps.Count == 0 && 
				report.DataSources.Count == 0 &&
				report.Dictionary.DataSources.Count == 0 &&
				report.Dictionary.Relations.Count == 0 &&
				report.Dictionary.Variables.Count == 0)
			{
				if (addOne)
					return $"{baseName}1";

				return baseName;
			}

			while (true)
			{
				var testName = baseName + counter;
				var checkName = (!addOne) && counter == 1 ? baseName : testName;

                if (GetObjectWithName(null, report, comps, checkName) == null)				
                    return (!addOne) && counter == 1 ? baseName : testName;                

                counter++;
			}
		}

        public static string CreateResourceName(StiReport report, string baseName)
        {
            baseName = RemoveSpacesFromName(baseName, false, report);

            var counter = 1;
            while (true)
            {
                var testName = counter == 1 ? baseName : baseName + counter;

                if (!IsResourceNameExists(report, testName))
                    return testName;

                counter++;
            }
        }

        public static string CreateConnectionName(StiReport report, string baseName)
        {
            baseName = RemoveSpacesFromName(baseName, false, report);

            var counter = 1;
            while (true)
            {
                var testName = counter == 1 ? baseName : baseName + counter;

                if (!IsConnectionNameExists(report, testName))
                    return testName;

                counter++;
            }
        }

		public static string CreateDataSourcesName(StiReport report, string baseName)
		{
			baseName = RemoveSpacesFromName(baseName, false, report);

			var counter = 1;
			while (true)
			{
				var testName = counter == 1 ? baseName : baseName + counter;

				if (!IsTableDataSourcesExists(report, testName))
					return testName;

				counter++;
			}
		}

		public static string CreateRelationName(StiReport report, StiDataRelation dataRelation, string baseName)
		{
			baseName = RemoveSpacesFromName(baseName, false, report);

			var counter = 1;
			while (true)
			{
				var testName = counter == 1 ? baseName : baseName + counter;

				if (!IsRelationExists(report, dataRelation, testName))
					return testName;

				counter++;
			}
		}

		public static bool IsRelationExists(StiReport report, StiDataRelation dataRelation, string name)
		{
			if (report == null)
				return false;

			name = name.ToLowerInvariant().Trim();

			return report.Dictionary.Relations.ToList()
				.Any(r => r.Name.ToLowerInvariant().Trim() == name && r.ChildSource.Name == dataRelation.ChildSource.Name);
		}

		public static bool IsTableDataSourcesExists(StiReport report, string name)
		{
			if (report == null) 
				return false;

			name = name.ToLowerInvariant().Trim();

			return report.Dictionary.DataSources.ToList()
				.Any(r => r.Name.ToLowerInvariant().Trim() == name);
		}

		public static bool IsResourceNameExists(StiReport report, string name)
        {
            if (report == null) 
				return false;

            name = name.ToLowerInvariant().Trim();
            return report.Dictionary.Resources.ToList()
                .Any(r => r.Name.ToLowerInvariant().Trim() == name);
        }

        public static bool IsConnectionNameExists(StiReport report, string name)
        {
            if (report == null) 
				return false;

            name = name.ToLowerInvariant().Trim();
            return report.Dictionary.Databases.ToList()
                .Any(r => r.Name.ToLowerInvariant().Trim() == name);
        }

        public static string CreateColumnName(StiDataSource dataSource, string baseName)
        {
            baseName = RemoveSpacesFromName(baseName, false, dataSource?.Dictionary?.Report);

            var counter = 1;
            while (true)
            {
                var testName = counter == 1 ? baseName : baseName + counter;

                if (!IsColumnNameExists(dataSource, testName))
                    return testName;

                counter++;
            }
        }

        public static bool IsColumnNameExists(StiDataSource dataSource, string name)
        {
            name = name.ToLowerInvariant().Trim();

            return dataSource.Columns.ToList()
                .Any(r => r.Name.ToLowerInvariant().Trim() == name);
        }

        public static string CreateParameterName(StiSqlSource dataSource, string baseName)
        {
            baseName = RemoveSpacesFromName(baseName, false, dataSource?.Dictionary?.Report);

            var counter = 1;
            while (true)
            {
                var testName = counter == 1 ? baseName : baseName + counter;

                if (!IsParameterNameExists(dataSource, testName))
                    return testName;

                counter++;
            }
        }

        public static bool IsParameterNameExists(StiSqlSource dataSource, string name)
        {
            name = name.ToLowerInvariant().Trim();

            return dataSource.Parameters.ToList()
                .Any(r => r.Name.ToLowerInvariant().Trim() == name);
        }

        /// <summary>
        /// Checks whether the name of the report is correct.
        /// </summary>
        /// <param name="name">Checked name.</param>
        /// <returns>Result of checking.</returns>
        public static bool IsValidName(StiReport report, string name)
		{
			if (string.IsNullOrEmpty(name) || !(char.IsLetter(name[0]) || name[0] == '_'))
				return false;

			for (int pos = 0; pos < name.Length; pos++)
			{
				if (!(char.IsLetterOrDigit(name[pos]) || (name[pos] == '_')))
					return false;
			}
			
			var comps = report.GetComponents();

			foreach (StiComponent comp in comps)
			{
				if (name == comp.Name)
					return false;
			}

			return true;
		}

		public static bool Exists(object checkedObject, StiReport report, string name)
		{
            return GetObjectWithName(checkedObject, report, name) != null;
		}

		public static bool Exists(StiReport report, string name)
		{
			return Exists(null, report, name);
		}
		
		public static bool CheckName(object checkedObject, 
			StiReport report, string name, string messageBoxCaption)
		{
			return CheckName(checkedObject, report, name, messageBoxCaption, true);
		}

		public static bool CheckName(object checkedObject, 
			StiReport report, string name, string messageBoxCaption, bool isValid)
        {
            if (report == null || !report.IsDesigning)            
                return true;
            
            if (Exists(checkedObject, report, name))
            {
                MessageBox.Show(string.Format(StiLocalization.Get("Errors", "NameExists"), name),
                    messageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
				return false;
            }
            else if (isValid && (!IsValidName(report, name)))
            {
                MessageBox.Show(string.Format(StiLocalization.Get("Errors", "IdentifierIsNotValid"), name),
                    messageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            return true;
        }

        public static object GetObjectWithName(object checkedObject, StiReport report, string name)
		{
			if (report == null)
				return null;

			return GetObjectWithName(checkedObject, report, report.GetComponents(), name);
		}

        private static object GetObjectWithName(object checkedObject,
			StiReport report, StiComponentsCollection comps, string name)
        {
            if (report == null) 
				return null;

            #region Check in components names
            for (int index = 0; index < comps.Count; index++)
			{
				var comp = comps[index];
				if (name == comp.Name && checkedObject != comp)
					return comp;
			}
			#endregion				

            #region Check in datasources names
            for (int index = 0; index < report.Dictionary.DataSources.Count; index++)
            {
                var dataSource = report.Dictionary.DataSources[index];
                if (name == dataSource.Name && checkedObject != dataSource) 
					return dataSource;
            }
            #endregion

            #region Check in business objects names
            for (int index = 0; index < report.Dictionary.BusinessObjects.Count; index++)
            {
                var businessObject = report.Dictionary.BusinessObjects[index];
                if (name == businessObject.Name && checkedObject != businessObject) 
					return businessObject;
            }
            #endregion

			#region Check in variables names
			for (int index = 0; index < report.Dictionary.Variables.Count; index++)
			{
				var variable = report.Dictionary.Variables[index];
				if (name == variable.Name && checkedObject != variable)
					return variable;
			}				
			#endregion

			return null;
		}
		
		/// <summary>
		/// Returns a name of the component.
		/// </summary>
		/// <param name="localizedName">Localized component name.</param>
		/// <param name="name">Did not localized component name.</param>
		/// <returns>Component name.</returns>
		public static string GenerateName(StiReport report, string localizedName, string name)
		{
			if (StiOptions.Engine.ForceGenerationLocalizedName)
				return localizedName;

            if (!StiOptions.Engine.ForceGenerationNonLocalizedName && report != null && report.Info.GenerateLocalizedName)
                return localizedName;

            var componentName = name;
			if (componentName.Length > 1 && componentName.Substring(0, 3) == "Sti")			
				componentName = componentName.Substring(3);            

            #region DBS
            if (componentName.EndsWith("Element"))
                componentName = componentName.Substring(0, componentName.Length - "Element".Length);
			#endregion

			if (componentName.EndsWith("UI"))
				componentName = componentName.Substring(0, componentName.Length - "UI".Length);

			return componentName;
		}

		/// <summary>
		/// Returns a name of the component.
		/// </summary>
		/// <param name="localizedName">Localized component name.</param>
		/// <param name="type">Type of component.</param>
		/// <returns>Component name.</returns>
		public static string GenerateName(StiReport report, string localizedName, Type type)
		{
			return GenerateName(report, localizedName, type.Name);
		}

		/// <summary>
		/// Returns a name for the component.
		/// </summary>
		/// <param name="component">Component for which a name is created.</param>
		/// <returns>Component name.</returns>
		public static string GenerateName(StiComponent component)
		{
			return GenerateName(component.Report, component.LocalizedName, component.GetType());
		}

        /// <summary>
        /// Returns a name for the relation.
        /// </summary>
        /// <param name="relation">Relation for which a name is created.</param>
        /// <returns>Relation name.</returns>
        public static string GenerateName(StiDataRelation relation)
        {
            return GenerateName(relation.Dictionary.Report, StiLocalization.Get("PropertyMain", "DataRelation"), relation.GetType());
        }

        /// <summary>
        /// Returns a name for the datasource.
        /// </summary>
        /// <param name="dataSource">Datasource for which a name is created.</param>
        /// <returns>Datasource name.</returns>
        public static string GenerateName(StiDataSource dataSource)
        {
            return GenerateName(dataSource.Dictionary.Report, StiLocalization.Get("PropertyMain", "DataSource"), dataSource.GetType());
        }
		#endregion
	}
}
