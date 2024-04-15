using System;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Stimulsoft.Report.Design
{
	public class StiGlobalizationItemConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
				if (value is StiGlobalizationItem)
				{
					var item = (StiGlobalizationItem)value;

					var types = new[]
					{
						typeof(string),
						typeof(string)
					};

					var info = typeof(StiGlobalizationItem).GetConstructor(types);
					if (info != null)
					{
						var objs = new object[]	
						{	
							item.PropertyName,
							item.Text
						};
					
						return CreateNewInstanceDescriptor(info, objs);
					}
				}
			}
			else if (destinationType == typeof(string))
			{
				StiGlobalizationItem item = (StiGlobalizationItem)value;
				if (item != null)
				{
				    var ch = culture == null 
				        ? CultureInfo.CurrentCulture.TextInfo.ListSeparator[0] 
				        : culture.TextInfo.ListSeparator[0];

				    return $"{item.PropertyName}{ch}{XmlConvert.EncodeName(item.Text)}";
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}


		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return true; 
			return base.CanConvertFrom(context, sourceType); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) return true;
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType); 
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var text = value as string;

			    var ch = culture == null 
			        ? CultureInfo.CurrentCulture.TextInfo.ListSeparator[0] 
			        : culture.TextInfo.ListSeparator[0];

				var splits = new[]{ ch };
				var words = text.Split(splits);

				return new StiGlobalizationItem(
					words[0],
					XmlConvert.DecodeName(words[1]));
				
				
			}
			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
