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
	public class XMLValidationHelper
	{
		private string lastXMLValidationError = string.Empty;
		private int lastXMLValidationErrorsCount = 0;

		public string LastXMLValidationError
		{
			get
			{
				return this.lastXMLValidationError;
			}
		}

		public int LastXMLValidationErrorsCount
		{
			get
			{
				return this.lastXMLValidationErrorsCount;
			}
		}

		public bool ValidateXML( string xmlData, String schemaPath )
		{
			XmlReader schemaReader = XmlReader.Create( schemaPath );

			return ValidateXML( xmlData, schemaReader );
		}

		public bool ValidateXML( XmlDocument data, String schemaPath )
		{
			return ValidateXML( data.OuterXml, schemaPath );
		}

		public bool ValidateXML( XmlDocument data, XmlReader schemaReader )
		{
			return ValidateXML( data.OuterXml, schemaReader );
		}


		public bool ValidateXML( string xmlData, XmlReader schemaReader )
		{
			try
			{
				this.lastXMLValidationError = string.Empty;
				this.lastXMLValidationErrorsCount = 0;

				XmlReaderSettings settings = new XmlReaderSettings();
				settings.ValidationType = ValidationType.Schema;
				settings.Schemas.Add( null, schemaReader );
				settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
				settings.ValidationEventHandler += new ValidationEventHandler( settings_ValidationEventHandler );

				StringReader xmlStream = new StringReader( xmlData );
				XmlReader reader = XmlReader.Create( xmlStream, settings );
				while( reader.Read() ) ;

				return this.lastXMLValidationErrorsCount == 0;
			}
			catch( Exception error )
			{
				// XML Validation failed
				return false;
			}
		}



		void settings_ValidationEventHandler( object sender, ValidationEventArgs e )
		{
			this.lastXMLValidationError += ( e.Message + "\r\n" );
			this.lastXMLValidationErrorsCount++;
		}
	}
}
