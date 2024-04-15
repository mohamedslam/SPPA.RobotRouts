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
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report
{
	/// <summary>
	/// The class describes the style condition.
	/// </summary>
	[RefreshProperties(RefreshProperties.All)]
    [TypeConverter(typeof(Stimulsoft.Report.Design.StiStyleConditionConverter))]
    public class StiStyleCondition :
        ICloneable, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiStyleCondition
            jObject.AddPropertyEnum("Type", Type, StiStyleConditionType.Placement);
            jObject.AddPropertyEnum("OperationPlacement", OperationPlacement, StiStyleConditionOperation.EqualTo);
            jObject.AddPropertyEnum("OperationPlacementNestedLevel", OperationPlacementNestedLevel, StiStyleConditionOperation.EqualTo);
            jObject.AddPropertyEnum("OperationComponentType", OperationComponentType, StiStyleConditionOperation.EqualTo);
            jObject.AddPropertyEnum("OperationLocation", OperationLocation, StiStyleConditionOperation.EqualTo);
            jObject.AddPropertyEnum("OperationComponentName", OperationComponentName, StiStyleConditionOperation.EqualTo);
            jObject.AddPropertyEnum("Placement", Placement, StiStyleComponentPlacement.None);
            jObject.AddPropertyInt("PlacementNestedLevel", PlacementNestedLevel, 1);
            jObject.AddPropertyEnum("ComponentType", ComponentType, StiStyleComponentType.Text);
            jObject.AddPropertyEnum("Location", Location, StiStyleLocation.None);
            jObject.AddPropertyStringNullOrEmpty("ComponentName", ComponentName);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch(property.Name)
                {
                    case "Type":
                        this.Type = property.DeserializeEnum<StiStyleConditionType>();
                        break;

                    case "OperationPlacement":
                        this.OperationPlacement = property.DeserializeEnum<StiStyleConditionOperation>();
                        break;

                    case "OperationPlacementNestedLevel":
                        this.OperationPlacementNestedLevel = property.DeserializeEnum<StiStyleConditionOperation>();
                        break;

                    case "OperationComponentType":
                        this.OperationComponentType = property.DeserializeEnum<StiStyleConditionOperation>();
                        break;

                    case "OperationLocation":
                        this.OperationLocation = property.DeserializeEnum<StiStyleConditionOperation>();
                        break;

                    case "OperationComponentName":
                        this.OperationComponentName = property.DeserializeEnum<StiStyleConditionOperation>();
                        break;

                    case "Placement":
                        this.Placement = property.DeserializeEnum<StiStyleComponentPlacement>();
                        break;

                    case "PlacementNestedLevel":
                        this.PlacementNestedLevel = property.DeserializeInt();
                        break;

                    case "ComponentType":
                        this.ComponentType = property.DeserializeEnum<StiStyleComponentType>();
                        break;

                    case "Location":
                        this.Location = property.DeserializeEnum<StiStyleLocation>();
                        break;

                    case "ComponentName":
                        this.ComponentName = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets type of this style condition.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleConditionType.Placement)]
        public StiStyleConditionType Type { get; set; } = StiStyleConditionType.Placement;

        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component placements.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleConditionOperation.EqualTo)]
        public StiStyleConditionOperation OperationPlacement { get; set; } = StiStyleConditionOperation.EqualTo;

        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component nested level.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleConditionOperation.EqualTo)]
        public StiStyleConditionOperation OperationPlacementNestedLevel { get; set; } = StiStyleConditionOperation.EqualTo;

        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component types.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleConditionOperation.EqualTo)]
        public StiStyleConditionOperation OperationComponentType { get; set; } = StiStyleConditionOperation.EqualTo;
        
        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component locations.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleConditionOperation.EqualTo)]
        public StiStyleConditionOperation OperationLocation { get; set; } = StiStyleConditionOperation.EqualTo;
        
        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component names.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleConditionOperation.EqualTo)]
        public StiStyleConditionOperation OperationComponentName { get; set; } = StiStyleConditionOperation.EqualTo;
        
        /// <summary>
        /// Gets or sets type of bands on which component can be placed.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleComponentPlacement.None)]
        public StiStyleComponentPlacement Placement { get; set; } = StiStyleComponentPlacement.None;

        /// <summary>
        /// Gets or sets value which indicates nested level of specified component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1)]
        public int PlacementNestedLevel { get; set; } = 1;
        
        /// <summary>
        /// Gets or sets component type which can be detected by style condition.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleComponentType.Text)]
        public StiStyleComponentType ComponentType { get; set; } = StiStyleComponentType.Text;

        /// <summary>
        /// Gets or sets variant of component location on parent component area.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStyleLocation.None)]
        public StiStyleLocation Location { get; set; } = StiStyleLocation.None;
        
        /// <summary>
        /// Gets or sets component name or part of component name.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        public string ComponentName { get; set; } = "";
        #endregion

        #region Methods
        internal void FromElements(StiStyleConditionElement[] elements)
        {
            this.Type = 0;

            foreach (var element in elements)
            {
                #region StiStyleConditionComponentNameElement
                var componentNameElement = element as StiStyleConditionComponentNameElement;
                if (componentNameElement != null)
                {
                    this.Type |= StiStyleConditionType.ComponentName;
                    this.ComponentName = componentNameElement.ComponentName;
                    this.OperationComponentName = componentNameElement.OperationComponentName;
                }
                #endregion

                #region StiStyleConditionComponentTypeElement
                var componentTypeElement = element as StiStyleConditionComponentTypeElement;
                if (componentTypeElement != null)
                {
                    this.Type |= StiStyleConditionType.ComponentType;
                    this.ComponentType = componentTypeElement.ComponentType;
                    this.OperationComponentType = componentTypeElement.OperationComponentType;
                }
                #endregion

                #region StiStyleConditionPlacementElement
                var placementElement = element as StiStyleConditionPlacementElement;
                if (placementElement != null)
                {
                    this.Type |= StiStyleConditionType.Placement;
                    this.Placement = placementElement.Placement;
                    this.OperationPlacement = placementElement.OperationPlacement;
                }
                #endregion

                #region StiStyleConditionPlacementNestedLevelElement
                var placementNestedLevel = element as StiStyleConditionPlacementNestedLevelElement;
                if (placementNestedLevel != null)
                {
                    this.Type |= StiStyleConditionType.PlacementNestedLevel;
                    this.PlacementNestedLevel = placementNestedLevel.PlacementNestedLevel;
                    this.OperationPlacementNestedLevel = placementNestedLevel.OperationPlacementNestedLevel;
                }
                #endregion

                #region StiStyleConditionLocationElement
                var locationElement = element as StiStyleConditionLocationElement;
                if (locationElement != null)
                {
                    this.Type |= StiStyleConditionType.Location;
                    this.Location = locationElement.Location;
                    this.OperationLocation = locationElement.OperationLocation;
                }
                #endregion
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new object of the type StiStyleCondition.
        /// </summary>
        public StiStyleCondition()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyleCondition.
        /// </summary>
        public StiStyleCondition(StiStyleConditionElement[] elements)
        {
            this.FromElements(elements);
        }

        /// <summary>
        /// Creates a new object of the type StiStyleCondition.
        /// </summary>
        public StiStyleCondition(StiStyleConditionType type,
            StiStyleConditionOperation operationPlacement, StiStyleConditionOperation operationPlacementNestedLevel, StiStyleConditionOperation operationComponentType, 
            StiStyleConditionOperation operationLocation, StiStyleConditionOperation operationComponentName,
            StiStyleComponentPlacement placement, int placementNestedLevel, StiStyleComponentType componentType, StiStyleLocation location, string componentName)
        {
            this.Type = type;
            this.OperationPlacement = operationPlacement;
            this.OperationPlacementNestedLevel = operationPlacementNestedLevel;
            this.OperationComponentType = operationComponentType;
            this.OperationLocation = operationLocation;
            this.OperationComponentName = operationComponentName;
            this.Placement = placement;
            this.PlacementNestedLevel = placementNestedLevel;
            this.ComponentType = componentType;
            this.Location = location;
            this.ComponentName = componentName;
        }
        #endregion
    }
}
