using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace Goldtect
{
	public class ASUpdatePanel : UpdatePanel
	{
		private static readonly Regex REGEX_CLIENTSCRIPTS = new Regex(
		"<script\\s((?<aname>[-\\w]+)=[\"'](?<avalue>.*?)[\"']\\s?)*\\s*>(?<script>.*?)</script>",
		RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled |
		RegexOptions.ExplicitCapture );
		private bool m_RegisterInlineClientScripts = true;

		/// <summary>
		/// If the updatepanel shall parse and append inline scripts, default true
		/// </summary>
		public bool RegisterInlineClientScripts
		{
			get
			{
				return this.m_RegisterInlineClientScripts;
			}
			set
			{
				this.m_RegisterInlineClientScripts = value;
			}
		}

		#region improvement
		private readonly StringBuilder CombinedScripts = new StringBuilder();

		protected virtual string RegisterAndRemoveScripts( string htmlsource )
		{
			if (this.ContentTemplate != null && htmlsource.IndexOf("<script" ) != -1)
			{
				MatchCollection matches = REGEX_CLIENTSCRIPTS.Matches(htmlsource);
				for (int i = 0; i < matches.Count; i++)
				{
					string script = matches[i].Groups["script"].Value;
					string scriptSrc = "";
					CaptureCollection aname = matches[i].Groups["aname"].Captures;
					CaptureCollection avalue = matches[i].Groups["avalue"].Captures;
					for (int u = 0; u < aname.Count; u++)
					{
						if (aname[u].Value.IndexOf("src", StringComparison.CurrentCultureIgnoreCase) == 0)
						{
							scriptSrc = avalue[u].Value;
							break;
						}
					}

					if (scriptSrc.Length > 0)
						ScriptManager.RegisterClientScriptInclude(this, this.GetType(), scriptSrc.GetHashCode().ToString(), scriptSrc);
					else
						this.CombinedScripts.Append(script);

					htmlsource = htmlsource.Replace(matches[i].Value, "");
				}
			}
			return htmlsource;
		}

		private bool _rendered;
		protected override void RenderChildren( HtmlTextWriter writer )
		{
			ScriptManager sm = ScriptManager.GetCurrent( Page );
			if( sm != null && sm.IsInAsyncPostBack )
			{
				if( this._rendered )
					return;

				using( HtmlTextWriter writer2 = new HtmlTextWriter( new StringWriter( CultureInfo.CurrentCulture ) ) )
				{
					base.RenderChildren( writer2 );
					string output = writer2.InnerWriter.ToString();

					// remove scripts:
					if( this.RegisterInlineClientScripts )
					{
						int lengthBefore = output.Length;
						// modify output
						output = RegisterAndRemoveScripts( output );
						// register client block
						string combined = CombinedScripts.ToString();
						if( combined.Length > 0 )
							ScriptManager.RegisterClientScriptBlock( this, this.GetType(), combined.GetHashCode().ToString(), combined, true );
						int lengthAfter = output.Length;
						int difference = lengthBefore - lengthAfter;
						// replace content only if there are some gains:
						if( difference > 0 )
							output = ReplaceRenderedOutputContentSize( output, difference );
					}

					writer.Write( output );
				}
			}
			else
			{
				base.RenderChildren( writer );
			}
			this._rendered = true;
		}

		private static string ReplaceRenderedOutputContentSize( string output, int difference )
		{
			string[] split = output.Split( '|' );
			int size = int.Parse( split[0] );
			split[0] = ( size - difference ).ToString();
			return string.Join( "|", split );
		}

		#endregion

		#region original
		/*
		protected virtual string AppendInlineClientScripts( string htmlsource )
		{
			if( this.ContentTemplate != null && htmlsource.IndexOf(
				"<script", StringComparison.CurrentCultureIgnoreCase ) > -1 )
			{
				MatchCollection matches = REGEX_CLIENTSCRIPTS.Matches( htmlsource );
				if( matches.Count > 0 )
				{
					for( int i = 0; i < matches.Count; i++ )
					{
						string script = matches[i].Groups["script"].Value;
						string scriptID = script.GetHashCode().ToString();
						string scriptSrc = "";

						CaptureCollection aname = matches[i].Groups["aname"].Captures;
						CaptureCollection avalue = matches[i].Groups["avalue"].Captures;
						for( int u = 0; u < aname.Count; u++ )
						{
							if( aname[u].Value.IndexOf( "src",
								StringComparison.CurrentCultureIgnoreCase ) == 0 )
							{
								scriptSrc = avalue[u].Value;
								break;
							}
						}

						if( scriptSrc.Length > 0 )
						{
							ScriptManager.RegisterClientScriptInclude( this,
								this.GetType(), scriptID, scriptSrc );
						}
						else
						{
							ScriptManager.RegisterClientScriptBlock( this, this.GetType(),
								scriptID, script, true );
						}

						htmlsource = htmlsource.Replace( matches[i].Value, "" );
					}

				}
			}
			return htmlsource;
		}
		

		protected override void RenderChildren( HtmlTextWriter writer )
		{
			ScriptManager sm = ScriptManager.GetCurrent( Page );
			if( this.RegisterInlineClientScripts && sm != null && sm.IsInAsyncPostBack )
			{
				using( HtmlTextWriter htmlwriter = new HtmlTextWriter( new StringWriter() ) )
				{
					base.RenderChildren( htmlwriter );

					string html;
					int outputSize;

					//Get the actual rendering and size
					html = htmlwriter.InnerWriter.ToString();
					outputSize = html.Length;

					//Append inlinescripts and fetch the new markup and size
					html = this.AppendInlineClientScripts( html );
					outputSize -= html.Length;

					//Replace ContentSize if there are any gains
					if( outputSize > 0 )
					{
						html = this.SetOutputContentSize( html, outputSize );
					}

					writer.Write( html );
				}
			}
			else
			{
				base.RenderChildren( writer );
			}
		}

		private string SetOutputContentSize( string html, int difference )
		{
			string[] split = html.Split( '|' );
			int size = int.Parse( split[0] );
			split[0] = ( size - difference ).ToString();
			return string.Join( "|", split );
		}*/
		#endregion
	}
}