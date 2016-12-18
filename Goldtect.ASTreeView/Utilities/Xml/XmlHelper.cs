using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Web;
using System.Xml;

namespace Goldtect.Utilities.Xml
{
	public class XmlHelper
	{
		public static XmlDocument CreateDocument( )
		{
			// Create the basic document.
			XmlDocument doc = new XmlDocument();
			XmlNode docDeclaration = doc.CreateXmlDeclaration( "1.0", "UTF-8", null );
			doc.AppendChild( docDeclaration );

			return doc;
		}

		public static XmlNode AddElement( XmlNode parent, string tagName, string textContent )
		{
			XmlNode node = parent.OwnerDocument.CreateElement( tagName );
			parent.AppendChild( node );

			if( textContent != null )
			{
				XmlNode content = parent.OwnerDocument.CreateTextNode( textContent );
				node.AppendChild( content );
			}

			return node;
		}

		public static XmlNode AddAttribute( XmlNode parent, string attributeName, string textContent )
		{
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( attributeName );
			attribute.Value = textContent;
			parent.Attributes.Append( attribute );

			return attribute;
		}

		public static string GetFormattedXmlString( XmlDocument doc )
		{
			return GetFormattedXmlString( doc, false );
		}

		public static string GetFormattedXmlString( XmlDocument doc, bool needHtmlEncoding )
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter( sw );
			writer.Formatting = Formatting.Indented;

			doc.WriteTo( writer );

			if( needHtmlEncoding )
				return HttpUtility.HtmlEncode( sw.ToString() );
			else
				return sw.ToString();
		}
	}
}
