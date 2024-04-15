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
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Design;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
#else

#endif

namespace Stimulsoft.Base.Serializing
{
	/// <summary>
	/// Class contains methods of serialization and deserialization.
	/// </summary>
	public class StiSerializing
	{
		#region Fields
		/// <summary>
		/// Converter that is used for convertation of objects into the line and back.
		/// </summary>
		private StiObjectStringConverter converter;

		/// <summary>
		/// Hashtable that is used for check acceleration whether this is a content.
		/// </summary>
		private Hashtable contentHashtable = new Hashtable();

		/// <summary>
		/// Collection for serialization/deserialization.
		/// </summary>
		public StiGraphs Graphs;

		private int ItemIndex = 1;

        /// <summary>
        /// A collection which used for conversation of the name of property to correct name of property.
        /// </summary>
        private Hashtable propertyNameHashtable = new Hashtable();
		
		/// <summary>
		/// Collection of references for delayed serialization/deserialization.
		/// </summary>
		public StiReferenceCollection References;
		#endregion

		#region Methods
		/// <summary>
		/// Adds the string representation of type for type conversation.
		/// </summary>
		public static void AddSourceTypeToDestinationType(string typeDestination, string typeSource)
		{
			SourceTypeToDestinationType[typeSource] = typeDestination;
		}

		private static string ConvertSourceTypeToDestinationType(string sourceType)
		{
			var destinationType = SourceTypeToDestinationType[sourceType] as string;
			return destinationType == null ? sourceType : destinationType;
		}

		/// <summary>
		/// Adds the type and its string substitution.
		/// </summary>
		public static void AddStringType(Type type, string str)
		{
			StringToType[str] = type;
			TypeToString[type] = str;
		}

		/// <summary>
		/// Adds the name of the property and its string substitution.
		/// </summary>
		public void AddStringProperty(string property, string str)
		{
			StringToProperty[str] = property;
			PropertyToString[property] = str;
		}

		private Type GetType(string typeStr)
		{
			return StringToType[ConvertSourceTypeToDestinationType(typeStr)] as Type;
		}

		private object GetObjectFromType(string typeStr)
		{
		    var type = GetType(typeStr);
		    var obj = type == null 
		        ? StiActivator.CreateObject(ConvertSourceTypeToDestinationType(typeStr)) 
		        : StiActivator.CreateObject(type);

		    if (obj == null && typeStr == "Stimulsoft.Dashboard.Components.StiDashboard")
		    {
		        try
		        {
		            var assembly = StiAssemblyFinder.GetAssembly("Stimulsoft.Dashboard");
		            if (assembly != null)
		            {
		                obj = StiActivator.CreateObject(ConvertSourceTypeToDestinationType("Stimulsoft.Dashboard.Components.StiDashboard"));
		                if (obj != null)
		                    return obj;
                    }
		        }
		        catch
		        {
		            throw new StiDashboardAssemblyIsNotFoundException();
		        }
		        throw new StiDashboardAssemblyIsNotFoundException();
            }

		    return obj;

        }

		/// <summary>
		/// Clears the hashtable of strings-types.
		/// </summary>
		public static void ClearStringType()
		{
			StringToType.Clear();
			TypeToString.Clear();
		}

		/// <summary>
		/// Clears the hashtable of strings-properties.
		/// </summary>
		public void ClearPropertyString()
		{
			StringToProperty.Clear();
			PropertyToString.Clear();
		}

		/// <summary>
		/// Returns the string substitution for a property name.
		/// </summary>
		public string GetStringFromProperty(string propertyName)
		{
			var str = PropertyToString[propertyName] as string;
			return str ?? propertyName;
		}

		/// <summary>
		/// Returns the property name from string-substitution.
		/// </summary>
		public string GetPropertyFromString(string str)
		{
			var propertyName = StringToProperty[str] as string;
			return propertyName ?? str;
		}

		/// <summary>
		/// Sets delayed references for serialization.
		/// </summary>
		public void SetReferenceSerializing()
		{
			foreach (StiReference reference in References)
			{
				if (reference.PropInfo.Value != null)
				{
					var code = Graphs[reference.PropInfo.Value];
					if (code == -1)
					{
						reference.PropInfo.IsReference = false;
						reference.PropInfo.Value = null;
					}
				
					else
					    reference.PropInfo.ReferenceCode = code;
				}
			}
		}
		
		/// <summary>
		/// Sets delayed references for serialization.
		/// </summary>
		public void SetReferenceDeserializing()
		{
			var index = 0;
			foreach (StiReference reference in References)
			{
				var val = Graphs[reference.PropInfo.ReferenceCode];
				if (val != null)SetProperty(reference.PropertyInfo, reference.Object, val);
				index++;
			}
		}
		
		/// <summary>
		/// Returns the type of elements in the array or collection.
		/// </summary>
		/// <param name="array">Array or collection.</param>
		/// <returns>Element type.</returns>
		public static Type GetTypeOfArrayElement(object array)
		{
			if (array != null)
			{
				var type = array.GetType();
				if (type.GetElementType() != null)return  type.GetElementType();
				var methods = type.GetMethods();

			    foreach (var methodInfo in methods)
			    {
			        if (methodInfo.Name == "get_Item")
			            return methodInfo.ReturnType;
			    }

			    var typeName = array.GetType().FullName;
				return StiTypeFinder.GetType(typeName.Substring(0, typeName.Length - 2));
			}
			return typeof(object);
		}

		/// <summary>
		/// Returns the value to default for property. 
		/// If the value is not assigned by default then null returns.
		/// </summary>
		/// <param name="prop">Descriptor.</param>
		/// <returns>Default.</returns>
		public object GetDefaultValue(MemberDescriptor prop)
		{
			var defaultValue = prop.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
			if (defaultValue != null)
				return defaultValue.Value;

			var defaultStringValue = prop.Attributes[typeof(StiDefaultStringValueAttribute)] as StiDefaultStringValueAttribute;
			if (defaultStringValue != null)
				return defaultStringValue.Value;

			return null;
		}
		
		/// <summary>
		/// Returns true if the object is marked as a content.
		/// </summary>
		/// <param name="obj">Object for check.</param>
		/// <returns>Result of check.</returns>
		public bool IsContent(object obj)
		{
            if (obj == null) return false;
			var type = obj.GetType();

		    var result = contentHashtable[type];
		    if (result != null)
		        return (bool) result;

		    var attrs = type.GetCustomAttributes(typeof(StiSerializableAttribute), true);
			if (attrs.Length > 0 && ((StiSerializableAttribute)attrs[0]).Visibility == StiSerializationVisibility.Content)
			{
				contentHashtable.Add(type, true);
				return true;
			}
			contentHashtable.Add(type, false);
			return false;
		}

	
		/// <summary>
		/// Serilize an object into the type List into the list.
		/// </summary>
		/// <param name="props">List for record.</param>
		/// <param name="list">Serialized object.</param>
		/// <param name="serializeType">Serialization type.</param>
		/// <returns>Number of serialized objects.</returns>
		private int SerializeList(StiPropertyInfoCollection props, object list, StiSerializeTypes serializeType)
		{
			if (list == null) return 0;
		
			var count = 0;
			
			foreach (var item in (IList)list)
			{
				if (item == null) continue;
				InvokeSerializing();
				
				if (item.GetType().IsPrimitive || item is string || IsContent(item))
				{
					props.Add(new StiPropertyInfo("value", item, null, false, false, false, false, null));
				}				
				else
				{
					int graphCode = Graphs[item];

					#region Object earlier does not serialized
					if (graphCode == -1)
					{
                        Graphs.Add(item);

						StiPropertyInfo itemProp;

						if (CheckSerializable && item is IStiSerializable && !(item is IStiNonSerialized) && 
							(!IgnoreSerializableForContainer ||
                            IgnoreSerializableForContainer && item.GetType().ToString().IndexOf("Container", StringComparison.InvariantCulture) == -1))
						{
							itemProp = new StiPropertyInfo($"item{count}", item, null, false, true, false, false, item.GetType().ToString());
							itemProp.IsSerializable = true;
						}
						else
						{
                            #region IStiSerializableCustomControl
                            if (item is IStiSerializableCustomControl &&
                                ((IStiSerializableCustomControl)item).Control != null)
                            {
                                var itemControl = ((IStiSerializableCustomControl)item).Control as Control;
                                Graphs.Add(itemControl);

                                var nameControl = $"Item{ItemIndex++}";

                                var piControl = itemControl.GetType().GetProperty("Name");
                                if (piControl != null)
                                    nameControl = (string)piControl.GetValue(itemControl, null);

                                var itemControlProp = new StiPropertyInfo(nameControl, itemControl, null, false, true, false, false, itemControl.GetType().ToString());
                                itemControlProp.Properties.AddRange(SerializeControl(itemControl, serializeType));

                                itemControlProp.ReferenceCode = Graphs[itemControl];
                                props.Add(itemControlProp);
                            }
                            #endregion

							var name = $"Item{ItemIndex++}";

							var pi = item.GetType().GetProperty("Name");
							if (pi != null)
							    name = (string)pi.GetValue(item, null);

							itemProp = new StiPropertyInfo(name, item, null, false, true, false, false, item.GetType().ToString());
							itemProp.Properties.AddRange(SerializeObject(item, serializeType));
                        }
						itemProp.ReferenceCode = Graphs[item];
						props.Add(itemProp);
					}
					#endregion

					#region Object earlier serialized - serializing reference
					else
					{
						var name = $"Item{ItemIndex++}";

						var itemProp = new StiPropertyInfo(name, item, null, false, true, true, false, item.GetType().ToString());
						itemProp.ReferenceCode = graphCode;
						props.Add(itemProp);
					}
					#endregion
				}
				count++;
			}
			
			return count;
		}

        public StiPropertyInfoCollection SerializeControl(Control obj, StiSerializeTypes serializeType)
        {
            var propList = new StiPropertyInfoCollection();

            Attribute[] attr =
            {
                new SerializableAttribute(),
                new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)
            };
            var properties = TypeDescriptor.GetProperties(obj, attr);
            foreach (PropertyDescriptor p in properties)
            {
                if (!p.ShouldSerializeValue(obj))continue;

                var defaultValue = GetDefaultValue(p);
                var isDefaultPresent = p.Attributes[typeof(DefaultValueAttribute)] != null;

                var propInfo = new StiPropertyInfo(p.Name, p.GetValue(obj), defaultValue, isDefaultPresent, false, false, false, p.PropertyType.ToString());
                propList.Add(propInfo);
            }

            return propList;
        }

		/// <summary>
		/// Serilizes an object into the list.
		/// </summary>
		/// <param name="obj">Object for serialization.</param>
		/// <param name="serializeType">Serialization type.</param>
		/// <returns>List of serialized objects.</returns>
		public StiPropertyInfoCollection SerializeObject(object obj, StiSerializeTypes serializeType) 
		{			
			if (IsContent(obj))
			{
			    return new StiPropertyInfoCollection
			    {
			        new StiPropertyInfo(string.Empty, obj, null, false, false, false, false, null)
			    };
			}

			if (obj == null) return null;
			if (Graphs[obj] == -1)Graphs.Add(obj);

			var propList = new StiPropertyInfoCollection();
            
			var props = TypeDescriptor.GetProperties(obj);
			if (props.Count == 0) return null;

			if (SortProperties)props = props.Sort();
			var count = props.Count;

			for (var index = 0; index < count; index++)
			{
				InvokeSerializing();
				var prop = props[index];	
								
				var serializableAttribute = prop.Attributes[typeof(StiSerializableAttribute)] as StiSerializableAttribute;
				if (serializableAttribute != null)
				{
					var contSer = false;
					if ((serializeType & StiSerializeTypes.SerializeToCode) > 0 && 
						(serializableAttribute.SerializeType & StiSerializeTypes.SerializeToCode) > 0)contSer = true;

					else if ((serializeType & StiSerializeTypes.SerializeToDesigner) > 0 && 
						(serializableAttribute.SerializeType & StiSerializeTypes.SerializeToDesigner) > 0)contSer = true;

					else if ((serializeType & StiSerializeTypes.SerializeToSaveLoad) > 0 && 
						(serializableAttribute.SerializeType & StiSerializeTypes.SerializeToSaveLoad) > 0)contSer = true;

					else if ((serializeType & StiSerializeTypes.SerializeToDocument) > 0 && 
						(serializableAttribute.SerializeType & StiSerializeTypes.SerializeToDocument) > 0)contSer = true;

					if (!contSer)continue;							

					//If serialized
					if (serializableAttribute.Visibility != StiSerializationVisibility.None && prop.Attributes[typeof(StiNonSerializedAttribute)] == null)
					{
						var visibility = serializableAttribute.Visibility;
						var defaultValue = GetDefaultValue(prop);
					    var isDefaultPresent = prop.Attributes[typeof(DefaultValueAttribute)] != null ||
							prop.Attributes[typeof(StiDefaultStringValueAttribute)] != null;

                        var valueObject = prop.GetValue(obj);
						StiPropertyInfo propInfo = null;
						
						#region Value is null
						if (valueObject == null)
						{
							propInfo = new StiPropertyInfo(
								prop.Name, null, defaultValue, isDefaultPresent, true, false, false, prop.PropertyType.ToString());
						}
						#endregion

						#region Is Reference
						else if (visibility == StiSerializationVisibility.Reference)
						{
							propInfo = new StiPropertyInfo(
								prop.Name, valueObject, defaultValue, isDefaultPresent, true, true, false, prop.PropertyType.ToString());
							References.Add(propInfo);
						}
						#endregion

						#region Is Simple types
						else if (visibility == StiSerializationVisibility.Content)
						{
							var skip = false;
							if ((serializeType & StiSerializeTypes.SerializeToCode) > 0)
							{
								if (valueObject is StiStyleBrush && defaultValue as string == "StyleBrush")
									skip = true;

								else if (valueObject is StiDefaultBrush && defaultValue as string == "DefaultBrush")
									skip = true;
							}

							if (!skip && !IsDefaultValue(valueObject, null))
							{
								propInfo = new StiPropertyInfo(
									prop.Name, valueObject, defaultValue, isDefaultPresent, false, false, false, prop.PropertyType.ToString());
							}
						}
						#endregion

						#region Is List
						else if (visibility == StiSerializationVisibility.List)
						{
							if (valueObject is IList)
							{
								propInfo = new StiPropertyInfo(
									prop.Name, valueObject, defaultValue, isDefaultPresent, false, false, true, prop.PropertyType.ToString());
								propInfo.Count = SerializeList(propInfo.Properties, valueObject, serializeType);
							}
							else throw new Exception($"{valueObject} is not list");
						}
						#endregion

						#region Is Class
						else if (visibility == StiSerializationVisibility.Class)
						{
							if (valueObject.GetType().IsClass)
							{
								#region Stimulsoft Reports specified condition!!
								var allow = prop.Name == "Interaction" && serializeType == StiSerializeTypes.SerializeToCode;
								#endregion

								if (!(IsDefaultValue(valueObject, prop) && (!allow)))
								{
									#region Check object on subject of ignore references
									var attrs = (StiReferenceIgnoreAttribute[])prop.PropertyType.GetCustomAttributes(typeof(StiReferenceIgnoreAttribute), false);
									#endregion

									var graphCode = Graphs[valueObject];

                                    #region Stimulsoft Reports specified condition!!
                                    //TextFormat property ?????? ????????????? ? ??? ??? value ???, ??????? ?? ???? ?????? ????????? ?? reference,
                                    //? ??? ?????????? TextFormat ???? ?????? ????????????? ??????.
                                    //????? Graphs.Add() ?? ????????? ?????????? ?????? ? ???-??????? ? ?????????????? ?? ??????? ????? reference,
                                    //??????? ????????? ??? ???? ???????? ? ?????? StiCodeDomSerializator ??? ????????? ??????
                                    //??? ????????? ?????????? ? ?????????? reference
								    if (prop.Name == "TextFormat" && serializeType == StiSerializeTypes.SerializeToCode)
								        graphCode = -1;
								    #endregion

                                    #region If object earlier does not serialized or it serialized without Graphs
                                    if (graphCode == -1 || attrs.Length > 0)
									{
										Graphs.Add(valueObject);
										var refCode = Graphs[valueObject];

										if (CheckSerializable && valueObject is IStiSerializable && (!(valueObject is IStiNonSerialized)))
										{
											propInfo = new StiPropertyInfo(
												prop.Name, valueObject, defaultValue, isDefaultPresent, true, false, false, prop.PropertyType.ToString());
											propInfo.IsSerializable = true;
										}
										else
										{
											propInfo = new StiPropertyInfo(
												prop.Name, valueObject, defaultValue, isDefaultPresent, true, false, false, prop.PropertyType.ToString());

											var properties = SerializeObject(valueObject, serializeType);
											if (properties != null)
											    propInfo.Properties.AddRange(properties);
										}
										propInfo.ReferenceCode = refCode;
									}
									#endregion

									#region Object earlier serialized - serialize reference
									else
									{
										propInfo = new StiPropertyInfo(
											prop.Name, valueObject, defaultValue, isDefaultPresent, true, true, false, prop.PropertyType.ToString());
										propInfo.ReferenceCode = graphCode;
									}
									#endregion
								}
									
							}
							else throw new Exception($"{valueObject} is not class");
						}
						#endregion

                        #region Is Control
                        else if (visibility == StiSerializationVisibility.Control)
                        {
                            var graphCode = Graphs[valueObject];
                            propInfo = new StiPropertyInfo(
                                prop.Name, valueObject, defaultValue, isDefaultPresent, true, true, false, prop.PropertyType.ToString());
                            propInfo.ReferenceCode = graphCode;
                        }
                        #endregion

                        if (propInfo != null)
                            propList.Add(propInfo);
					}
				}
			}
			return propList;
		}

		private static bool IsDefaultValue(object valueObject, PropertyDescriptor prop)
		{
			var defaultValue = valueObject as IStiDefault;
			if (defaultValue == null)
				return false;

			//Special fix for the 'ValueFormat' property for the chart element. 
			//This property has other default value and should be serialized in any case
			if (prop != null && prop.Name == "ValueFormat" && prop.PropertyType != null && prop.PropertyType.Name == "StiFormatService")
				return false;

			return defaultValue.IsDefault;
		}

		/// <summary>
		/// Saves object into XML.
		/// </summary>
		/// <param name="tw">Object to save into XML.</param>
		/// <param name="prop">Serialized objects list.</param>
		private void SerializeProperty(XmlTextWriter tw, StiPropertyInfo prop)
        {
            #region Get correct property name
            var propertyName = propertyNameHashtable[prop.Name] as string;
            if (propertyName == null)
            {
                propertyName = XmlConvert.EncodeName(GetStringFromProperty(prop.Name));
                var sb = new StringBuilder();
                foreach (var c in propertyName)
                {
                    if (char.IsPunctuation(c))
                        sb.Append("_");
                    else
                        sb.Append(c);
                }
                propertyName = sb.ToString();
                propertyNameHashtable[prop.Name] = propertyName;
            }
            #endregion

            if (prop.Value == null)
			{
				if (prop.IsDefaultValueSpecified)
				{
					tw.WriteStartElement(propertyName);
					tw.WriteAttributeString("isNull", "true");
					tw.WriteEndElement();
				}
			}
			else if (prop.IsKey)
			{
				if (prop.IsReference)
				{
					tw.WriteStartElement(propertyName);
					tw.WriteAttributeString("isRef", prop.ReferenceCode.ToString());
					tw.WriteEndElement();
				}
				else
				{
					tw.WriteStartElement(propertyName);
					if (prop.ReferenceCode != -1)
					{
						tw.WriteAttributeString("Ref", prop.ReferenceCode.ToString());
					}
					
					if (prop.Value != null)
					{
						var typeName = TypeToString[prop.Value.GetType()] as string;
						if (typeName == null)
						    typeName = prop.Value.GetType().FullName;

						tw.WriteAttributeString("type", typeName);
					}
					if (prop.IsSerializable)
					{
						tw.WriteAttributeString("isSer", "true");
						var serializable = prop.Value as IStiSerializable;
						serializable.Serialize(converter, tw);
						Graphs.Add(serializable, prop.ReferenceCode);
					}
					else
					{
						tw.WriteAttributeString("isKey", "true");
						SerializeObject(tw, prop.Properties);
					}
					tw.WriteEndElement();					
				}
			}
			else if (prop.IsList)
			{
				tw.WriteStartElement(propertyName);
				tw.WriteAttributeString("isList", "true");
				if (prop.Value != null)
				{
					tw.WriteAttributeString("count", prop.Count.ToString());
				}
				SerializeObject(tw, prop.Properties);
				tw.WriteEndElement();
			}
			else 
			{
				if (prop.Value is Metafile)
				{					
					tw.WriteStartElement(propertyName);					
					tw.WriteString(StiMetafileConverter.MetafileToString(prop.Value as Metafile));
					tw.WriteEndElement();
				}
				else if (prop.Value is Image)
				{
					tw.WriteStartElement(propertyName);
					tw.WriteString(StiImageConverter.ImageToString(prop.Value as Image));
					tw.WriteEndElement();
				}
				else
				{
					var valueString = converter.ObjectToString(prop.Value);
					if (valueString != null)
					{
						var propDefaultValue = prop.DefaultValue as string;
						if (propDefaultValue == null || (propDefaultValue is string && valueString != propDefaultValue))
						{
							tw.WriteStartElement(propertyName);
							tw.WriteString(valueString);
							tw.WriteEndElement();
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves the list serialized objects into XML.
		/// </summary>
		/// <param name="tw">Object to save into XML</param>
		/// <param name="props">Serialized objects list.</param>
		public void SerializeObject(XmlTextWriter tw, StiPropertyInfoCollection props) 
		{
			if (props == null) return;
			foreach (StiPropertyInfo prop in props)
			{
				InvokeSerializing();
			    if (!prop.IsDefaultValueSpecified || !Equals(prop.DefaultValue, prop.Value))
			        SerializeProperty(tw, prop);
			}
		}
		
		/// <summary>
		/// Serialize an object into the list.
		/// </summary>
		/// <param name="obj">Object for serialization.</param>
		/// <param name="serializeType">Serialization type.</param>
		/// <returns>Serialized objects list.</returns>
		public StiPropertyInfoCollection Serialize(object obj, StiSerializeTypes serializeType)
		{			
			ItemIndex = 1;
			Graphs = new StiGraphs();
			References = new StiReferenceCollection();
			var props = SerializeObject(obj, serializeType);
			SetReferenceSerializing();
			return props;
		}

		/// <summary>
		/// Serializes an object into the stream.
		/// </summary>
		/// <param name="obj">Object for serialization.</param>
		/// <param name="stream">Stream in which serialization will be generated.</param>
		/// <param name="application">Application that generates serialization.</param>
		public void Serialize(object obj, Stream stream, string application)
		{	
			Serialize(obj, stream, application, StiSerializeTypes.SerializeToSaveLoad);
		}
		
		/// <summary>
		/// Serializes object into the stream.
		/// </summary>
		/// <param name="obj">Object for serialization.</param>
		/// <param name="stream">Stream in which serialization will be generated.</param>
		/// <param name="application">Application that generates serialization.</param>
		/// <param name="serializeType">Serialization type.</param>
		public void Serialize(object obj, Stream stream, string application, StiSerializeTypes serializeType)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var tw = new XmlTextWriter(stream, Encoding.UTF8);
				tw.Formatting = Formatting.Indented;

				tw.WriteStartDocument(true);
				tw.WriteStartElement("StiSerializer");
				tw.WriteAttributeString("version", StiFileVersions.ReportFile);
                tw.WriteAttributeString("type", "Net");
				tw.WriteAttributeString("application", application);
            
				Graphs = new StiGraphs();
				References = new StiReferenceCollection();
			
				var props = SerializeObject(obj, serializeType);
				SetReferenceSerializing();
				SerializeObject(tw, props);

				tw.WriteEndElement();
				tw.Flush();
			}				
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}
		
		/// <summary>
		/// Seializes object using textWriter.
		/// </summary>
		/// <param name="obj">Object for serialization.</param>
		/// <param name="textWriter">TextWriter in which serialization will be generated.</param>
		/// <param name="application">Application that generates serialization.</param>
		/// <param name="serializeType">Serialization type.</param>
		public void Serialize(object obj, TextWriter textWriter, string application, StiSerializeTypes serializeType)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var tw = new XmlTextWriter(textWriter);

				tw.WriteStartDocument(true);
				tw.WriteStartElement("StiSerializer");
                tw.WriteAttributeString("version", StiFileVersions.ReportFile);
                tw.WriteAttributeString("type", "Net");
				tw.WriteAttributeString("application", application);
			            
				Graphs = new StiGraphs();
				References = new StiReferenceCollection();

				var props = SerializeObject(obj, serializeType);
				SetReferenceSerializing();
				SerializeObject(tw, props);

				tw.WriteEndElement();
				tw.Flush();
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		/// <summary>
		/// Serializes object into the file.
		/// </summary>
		/// <param name="obj">Object for serialization.</param>
		/// <param name="path">File in which serialization will be generated.</param>
		/// <param name="application">Application that generates serialization.</param>
		public void Serialize(object obj, string path, string application)
		{
			StiFileUtils.ProcessReadOnly(path);
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			Serialize(obj, stream, application);
			stream.Flush();
			stream.Close();
		}
	
		/// <summary>
		/// Sets property p in object obj value.
		/// </summary>
		/// <param name="p">Property for set.</param>
		/// <param name="obj">Object in which the property is situated.</param>
		/// <param name="value">Value for setup.</param>
		public void SetProperty(PropertyInfo p, object obj, object value)
		{
			if (p != null && p.CanWrite)
			    p.SetValue(obj, value, null);
		}

		/// <summary>
		/// Deserializes object from the list.
		/// </summary>
		/// <param name="obj">Object for deserialization.</param>
		/// <param name="props">List contains objects.</param>
		private void DeserializeObject(object obj, StiPropertyInfoCollection props) 
		{
			foreach (StiPropertyInfo prop in props)
			{
				InvokeDeserializing();
				PropertyInfo p = null;

				if (obj != null)
			    {
			        var type = obj.GetType();

			        if (prop.Name == "Border" && type.Name == "StiPanelElement")//Special support for StiPanelElement.Border
			            p = type.GetProperties().FirstOrDefault(c => c.Name == "Border" && c.PropertyType == typeof(StiSimpleBorder));

					else if (prop.Name == "Border" && type.Name == "StiPanelUI")//Special support for StiPanelUI.Border
						p = type.GetProperties().FirstOrDefault(c => c.Name == "Border" && c.PropertyType == typeof(StiSimpleBorder));

					else
			            p = type.GetProperty(prop.Name);
			    }

			    #region Property Not Found
				if (p == null && obj != null)
				{
					var e = new StiPropertyNotFoundEventArgs(prop.Name, obj.GetType());
					InvokePropertyNotFound(this, e);

					if (e.PropertyName != prop.Name)
					{
						prop.Name = e.PropertyName;
						p = obj.GetType().GetProperty(prop.Name);
					}
				}
				if (p == null)continue;
				#endregion

			    if (!prop.IsReference && prop.ReferenceCode != -1)
			        Graphs.Add(prop.Value, prop.ReferenceCode);

			    //Is Class
				if (prop.IsKey)
				{	
					SetProperty(p, obj, prop.Value);

					if (p != null)
					    DeserializeObject(p.GetValue(obj, null), prop.Properties);
					else
					    DeserializeObject(null, prop.Properties);
					
					converter.SetProperty(p, obj, prop.Value);
				}
				
				//Is List
				else if (prop.IsList)
				{
					var count = prop.Count;

				    if (p != null)
					{
					    var list = p.GetValue(obj, null) as IList;
						if (list == null)
                        {
							try
							{
								list = StiActivator.CreateObject(p.PropertyType) as IList;
								p.SetValue(obj, list);
							}
                            catch
                            {
                            }
                        }
					    if (list != null)
						{
							var elementType = GetTypeOfArrayElement(list);
							if (list is Array)
							{
								list = Array.CreateInstance(elementType, count);
								SetProperty(p, obj, list);
							}
							else list.Clear();

							var listType = list.GetType().ToString();
										
							var index = 0;
							foreach (StiPropertyInfo property in prop.Properties)
							{
								#region Invoke StiTypeNotFoundEvent
								if (property.Value == null)
								{
									var e = new StiTypeNotFoundEventArgs(property.TypeName);
									InvokeTypeNotFound(this, e);
								    if (e.CreatedObject != null)
								        property.Value = e.CreatedObject;
								}
								#endregion

								#region Create Undefined Types
								if (property.Value == null)
								{
								    if (listType.EndsWithInvariant("StiComponentsCollection"))
								        property.Value = StiActivator.CreateObject("Stimulsoft.Report.Components.StiUndefinedComponent");

								    else if (listType.EndsWithInvariant("StiPagesCollection"))
								        property.Value = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage");

								    else if (listType.EndsWithInvariant("StiDatabaseCollection"))
								        property.Value = StiActivator.CreateObject("Stimulsoft.Report.Dictionary.StiUndefinedDatabase");

								    else if (listType.EndsWithInvariant("StiDataSourcesCollection"))
								        property.Value = StiActivator.CreateObject("Stimulsoft.Report.Dictionary.StiUndefinedDataSource");
								}
								#endregion

								InvokeDeserializing();
							    object item;
							    if (property.IsReference && property.ReferenceCode != -1)
								{
									property.Value = Graphs[property.ReferenceCode];
									item = property.Value;
								}
								else
								{
									if (elementType == typeof(string) || !(property.Value is string))
									{
										item = property.Value;
									}
									else 
									{
										try
										{
											item = converter.StringToObject((string)property.Value, elementType);
										}
										catch
										{
											item = (string)property.Value;
										}
									}
								}

							    if (!property.IsReference && property.ReferenceCode != -1 && property.Value != null)
							        Graphs.Add(property.Value, property.ReferenceCode);

							    if (list is Array)
								{
									object objs = null;
									if (elementType == item.GetType())
									    objs = item;
									else
									    converter.StringToObject((string)item, elementType);

									((Array)list).SetValue(objs, index++);
								}
								else 
								{
									if (item != null)
									{
										list.Add(item);
										if (!property.IsReference && !property.IsSerializable)
										    DeserializeObject(item, property.Properties);
									}
								}
							}
						}
					}
				}
				//Is Reference
				else if (prop.IsReference)
				{

					var val = Graphs[prop.ReferenceCode];
					SetProperty(p, obj, val);
					if (val == null)References.Add(prop, obj, p);
				}
				//Is Null
				else if (prop.Value == null)
				{
					SetProperty(p, obj, null);
				}
				else if (p != null)
				{
					var valueObj = prop.Value;
				    if (prop.Value is string && p.PropertyType == typeof(Color[]) && string.IsNullOrWhiteSpace(prop.Value as string))
				        valueObj = new Color[0];

				    else if (prop.Value is string && p.PropertyType != typeof(object))
				        valueObj = converter.StringToObject((string) prop.Value, p.PropertyType);

				    SetProperty(p, obj, valueObj);
				}
			}
		}

		/// <summary>
		/// Deserilizes object from XML.
		/// </summary>
		/// <param name="tr">Object for to read XML.</param>
		/// <returns>List contains objects.</returns>
		private StiPropertyInfoCollection DeserializeObject(XmlTextReader tr, string parentPropName) 
		{
            var propList = new StiPropertyInfoCollection();
			tr.Read();
			while (!tr.EOF) 
			{
				if (tr.IsStartElement()) 
				{
					InvokeDeserializing();

                    var prop = new StiPropertyInfo(
						XmlConvert.DecodeName(GetPropertyFromString(tr.Name)), null, 
						tr.GetAttribute("isKey") == "true",
						tr.MoveToAttribute("isRef"),
						tr.MoveToAttribute("isList"), null);
				
					var isNull = tr.GetAttribute("isNull") == "true";
					if (isNull)
					{
						prop.Value = null;
					}
					else if (tr.GetAttribute("isSer") != null)
					{
						var s = tr.GetAttribute("Ref");
						if (s != null)prop.ReferenceCode = int.Parse(s);

						var typeName = tr.GetAttribute("type");

						prop.TypeName = typeName;

						prop.Value = GetObjectFromType(typeName);
						prop.IsSerializable = true;
                        var serializable = prop.Value as IStiSerializable;
						serializable.Deserialize(converter, tr);
						Graphs.Add(serializable, prop.ReferenceCode);
					}
					else if (prop.IsReference)
					{
						prop.ReferenceCode = int.Parse(tr.GetAttribute("isRef"));
					}
					else if (prop.IsList)
					{
					    prop.Count = int.Parse(tr.GetAttribute("count"));
					    if (prop.Count > 0 && !tr.IsEmptyElement)
					        prop.Properties = DeserializeObject(tr, prop.Name);
					}
					else if (!prop.IsKey)
					{
						if (tr.GetAttribute("isImage") == "true")
						{
							prop.Value = StiImageConverter.StringToImage(tr.ReadString());
							prop.TypeName = typeof(Image).ToString();
						}
						else if (tr.GetAttribute("isEmfImage") == "true")
						{
							prop.Value = StiMetafileConverter.StringToMetafile(tr.ReadString());
							prop.TypeName = typeof(Image).ToString();
						}
						else
						{
							prop.Value = tr.ReadString();
						}
						
					}
					else if (prop.IsKey)
					{
						var s = tr.GetAttribute("Ref");
						if (s != null)
						    prop.ReferenceCode = int.Parse(s);						
						
						var typeName = tr.GetAttribute("type");
						prop.TypeName = typeName;

						prop.Value = GetObjectFromType(typeName);
					    if (!tr.IsEmptyElement)
					        prop.Properties = DeserializeObject(tr, prop.Name);

					    #region AllowFixOldChartTitle
                        if (AllowFixOldChartTitle)
                        {
                            if ((parentPropName == "YAxis" || parentPropName == "YRightAxis") && prop.Name == "Title")
                            {
                                #region Exists ?
                                var exists = false;
                                foreach (StiPropertyInfo pr in prop.Properties)
                                {
                                    if (pr.Name == "Direction")
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                                #endregion

                                if (!exists)
                                {
                                    var propInfo = prop.Value.GetType().GetProperty("Direction");

                                    if (parentPropName == "YAxis")
                                        propInfo.SetValue(prop.Value, 3, new object[0]);//BottomToTop = 3

                                    if (parentPropName == "YRightAxis")
                                        propInfo.SetValue(prop.Value, 2, new object[0]);//TopToBottom = 2
                                }
                            }
                        }
                        #endregion
                    }
					propList.Add(prop);
				}
				else if (tr.NodeType == XmlNodeType.EndElement)break;
				
				tr.Read();
			}
			return propList;
		}

		/// <summary>
		/// Deserializes object from the stream.
		/// </summary>
		/// <param name="obj">Object for deserialization.</param>
		/// <param name="stream">Stream from which deserialization is generated.</param>
		/// <param name="application">Application that generates deserialization.</param>
		public void Deserialize(object obj, Stream stream, string application)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var tr = new XmlTextReader(stream);
				tr.DtdProcessing = DtdProcessing.Ignore;
				tr.Read();
			
				tr.Read();
				if (tr.IsStartElement())
				{
					if (tr.Name == "StiSerializer")
					{
						Graphs = new StiGraphs();
						References = new StiReferenceCollection();
						Graphs.Add(obj);

					    var app = tr.GetAttribute("application");
						if (app == application)
						{
							DeserializeObject(obj, DeserializeObject(tr, null));
							SetReferenceDeserializing();
						}
					}
				}
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		/// <summary>
		/// Deserializes object using textReader.
		/// </summary>
		/// <param name="obj">Object for deserialization.</param>
		/// <param name="textReader">TextReader from which deserialization will be generated.</param>
		/// <param name="application">Application that generates deserialization.</param>
		public void Deserialize(object obj, TextReader textReader, string application)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var tr = new XmlTextReader(textReader);
				tr.DtdProcessing = DtdProcessing.Ignore;
				tr.Read();

				tr.Read();
				if (tr.IsStartElement())
				{
					if (tr.Name == "StiSerializer")
					{
						Graphs = new StiGraphs();
						References = new StiReferenceCollection();
						Graphs.Add(obj);

					    var app = tr.GetAttribute("application");
                        if (app == application)
                        {
                            DeserializeObject(obj, DeserializeObject(tr, null));
                            SetReferenceDeserializing();
                        }
					}
				}
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		/// <summary>
		/// Deserializes object from the file.
		/// </summary>
		/// <param name="obj">Object for deserialization.</param>
		/// <param name="path">File from which deserialization will be generated.</param>
		/// <param name="application">Application that generates deserialization.</param>
		public void Deserialize(object obj, string path, string application)
		{
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			Deserialize(obj, stream, application);
			stream.Close();
		}

		#endregion
		
		#region Properties.Static
	    /// <summary>
        /// Gets or sets value which indicates that bug with loading old charts (without Direction property) will be fixed during loading of report.
        /// </summary>
        public static bool AllowFixOldChartTitle { get; set; }
        #endregion

	    #region Properties
        /// <summary>
        /// Gets or sets value indicates should properties be sorted alphabetically or not.
        /// </summary>
        public bool SortProperties { get; set; } = true;

	    /// <summary>
		/// Gets or sets value, indicates should objects, when serialization, be checked in their capability or not to realize IStiSerializable.
		/// </summary>
		public bool CheckSerializable { get; set; }

	    /// <summary>
		/// Internal use only.
		/// </summary>
		public bool IgnoreSerializableForContainer { get; set; }
        
	    /// <summary>
		/// Gets or sets table for transformation of a type into the string.
		/// </summary>
		public static Hashtable TypeToString { get; set; } = new Hashtable();
        
	    /// <summary>
		/// Gets or sets table for transformation a string into the type.
		/// </summary>
		public static Hashtable StringToType { get; set; } = new Hashtable();

	    public static Hashtable SourceTypeToDestinationType { get; set; } = new Hashtable();

	    private static Hashtable propertyToString = new Hashtable();
        private static Hashtable propertyToString2 = new Hashtable();
        /// <summary>
		/// Gets or sets table for transformation the property name into the string.
		/// </summary>
		public Hashtable PropertyToString
		{
			get
			{
                if (IsDocument)
                    return propertyToString2;
                else
                    return propertyToString;
			}
			set
			{
                if (IsDocument)
                    propertyToString2 = value;
                else
                    propertyToString = value;
            }
		}


		private static Hashtable stringToProperty = new Hashtable();
        private static Hashtable stringToProperty2 = new Hashtable();
		/// <summary>
		/// Gets or sets table for transformation the string into the property name.
		/// </summary>
		public Hashtable StringToProperty
		{
			get
			{
                if (IsDocument)
                    return stringToProperty2;
                else
				    return stringToProperty;
			}
			set
			{
                if (IsDocument)
                    stringToProperty2 = value;
                else
                    stringToProperty = value;
            }
		}

	    /// <summary>
        /// Gets or sets a value indicates Document mode.
        /// </summary>
        public bool IsDocument { get; set; }
	    #endregion

		#region Events.Static
		#region TypeNotFound
		public static event StiTypeNotFoundEventHandler TypeNotFound;

		internal static void InvokeTypeNotFound(StiSerializing serializing, StiTypeNotFoundEventArgs e)
		{
            TypeNotFound?.Invoke(serializing, e);
        }
		#endregion

		#region PropertyNotFound
		public static event StiPropertyNotFoundEventHandlers PropertyNotFound;

		internal static void InvokePropertyNotFound(StiSerializing serializing, StiPropertyNotFoundEventArgs e)
		{
            PropertyNotFound?.Invoke(serializing, e);
        }
		#endregion

        #region Serializing
        public static event EventHandler GlobalSerializing;

        public static void InvokeGlobalSerializing(object sender, EventArgs e)
        {
            GlobalSerializing?.Invoke(sender, e);
        }
        #endregion

        #region GlobalDeserializing
        public static event EventHandler GlobalDeserializing;

        public static void InvokeGlobalDeserializing(object sender, EventArgs e)
        {
            GlobalDeserializing?.Invoke(sender, e);
        }
        #endregion
		#endregion

		#region Events
		#region Serializing
		/// <summary>
		/// Event occurs when serializing of one element.
		/// </summary>
		public event EventHandler Serializing;

		protected virtual void OnSerializing(EventArgs e)
		{
            InvokeGlobalSerializing(this, e);
		}

		/// <summary>
		/// Raises the Serializing event for this control.
		/// </summary>
		public void InvokeSerializing()
		{
			OnSerializing(EventArgs.Empty);
            this.Serializing?.Invoke(null, EventArgs.Empty);
        }
		#endregion

		#region Deserializing
		/// <summary>
		/// Event occurs when deserializing of one element.
		/// </summary>
		public event EventHandler Deserializing;

		protected virtual void OnDeserializing(EventArgs e)
		{
            InvokeGlobalDeserializing(this, e);
		}

		/// <summary>
		/// Raises the Deserializing event for this control.
		/// </summary>
		public void InvokeDeserializing()
		{
			OnDeserializing(EventArgs.Empty);
            this.Deserializing?.Invoke(null, EventArgs.Empty);
        }
		#endregion
		#endregion

		/// <summary>
		/// Creates a new instance of the StiSerializing class.
		/// </summary>
		public StiSerializing() : this(new StiObjectStringConverter())
		{
		}

		/// <summary>
		/// Creates a new instance of the StiSerializing class.
		/// </summary>
		/// <param name="converter">Converter for transformation of objects into the string and back.</param>
		public StiSerializing(StiObjectStringConverter converter)
		{
			this.converter = converter;
		}
	}
}