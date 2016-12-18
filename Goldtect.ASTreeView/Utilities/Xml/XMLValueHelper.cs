using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Goldtect.Utilities.Xml
{
	public static class XmlValueHelper<T>
	{
		public static T GetValue( XmlNode node )
		{
			return GetValue( node, ".", null, default( T ), Thread.CurrentThread.CurrentCulture );
		}

		public static T GetValue( XmlNode node, string xPath )
		{
			return GetValue( node, xPath, null, default( T ), Thread.CurrentThread.CurrentCulture );
		}

		public static T GetValue( XmlNode node, string xPath, T defaultValue )
		{
			return GetValue( node, xPath, null, defaultValue, Thread.CurrentThread.CurrentCulture );
		}

		public static T GetValue( XmlNode node, string xPath, XmlNamespaceManager nsManager, T defaultValue )
		{
			return GetValue( node, xPath, nsManager, defaultValue, Thread.CurrentThread.CurrentCulture );
		}

		public static T GetValue( XmlNode node, string xPath, CultureInfo culture )
		{
			return GetValue( node, xPath, null, default( T ), culture );
		}

		public static T GetValue( XmlNode node, string xPath, CultureInfo culture, T defaultValue )
		{
			return GetValue( node, xPath, null, defaultValue, culture );
		}

		public static T GetValue( XmlNode node, string xPath, XmlNamespaceManager nsManager, T defaultValue, CultureInfo culture )
		{
			XmlNode item;
			if( nsManager == null )
				item = node.SelectSingleNode( xPath );
			else
				item = node.SelectSingleNode( xPath, nsManager );
			if( item == null )
			{
				return defaultValue;
			}
			else if( item.NodeType == XmlNodeType.Attribute )
			{
				return ConvertValue( item.Value, defaultValue, culture );
			}
			else
			{
				return ConvertValue( item.InnerText, defaultValue, culture );
			}
		}

		public static T ConvertValue( string value )
		{
			return ConvertValue( value, default( T ) );
		}

		public static T ConvertValue( string value, T defaultValue )
		{
			return ConvertValue( value, defaultValue, Thread.CurrentThread.CurrentCulture );
		}

		public static T ConvertValue( string value, T defaultValue, CultureInfo culture )
		{
			if( typeof( T ) == typeof( string ) )
			{
				return (T)(object)value;
			}
			TypeConverter converter = TypeDescriptor.GetConverter( typeof( T ) );
			bool valid = converter.IsValid( value );
			if( valid )
			{
				try
				{
					return (T)converter.ConvertFromString( null, culture, value );
				}
				catch
				{
					valid = false;
				}
			}
			if( !valid )
			{
				if( typeof( T ) == typeof( bool ) )
				{
					//If the conversion failed and the target type is boolean check if the value is an integer
					int tmpValue = 0;
					if( XmlValueHelper<int>.ConvertValue( value, culture, ref tmpValue ) )
					{
						return (T)(object)( tmpValue != 0 );
					}
				}
			}
			return defaultValue;
		}

		public static bool ConvertValue( string value, CultureInfo culture, ref T returnValue )
		{
			if( typeof( T ) == typeof( string ) )
			{
				returnValue = (T)(object)value;
				return true;
			}
			TypeConverter converter = TypeDescriptor.GetConverter( typeof( T ) );
			if( converter.IsValid( value ) )
			{
				try
				{
					returnValue = (T)converter.ConvertFromString( null, culture, value );
					return true;
				}
				catch { }
			}
			return false;
		}
	}
}
