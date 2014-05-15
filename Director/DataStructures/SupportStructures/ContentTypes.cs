using System;
using Director.Formatters;
using Newtonsoft.Json.Linq;

namespace Director
{
	// Content type enum.
	public enum ContentType : int
	{
		JSON = 0, XML = 1, PLAIN = 2
	}

	public static class ContentTypeUtils
	{
		/// <summary>
		/// To string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="t">T.</param>
		public static String ToString(ContentType t)
		{
			if (t == ContentType.JSON) {
				return "json";
			} else if (t == ContentType.XML) {
				return "xml";
			} else {
				return "plain";
			}
		}

		/// <summary>
		/// From stirng.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="s">S.</param>
		public static ContentType FromString(String s)
		{
			if (s == "json") {
				return ContentType.JSON;
			} else if (s == "xml") {
				return ContentType.XML;
			} else {
				return ContentType.PLAIN;
			}
		}

		/// <summary>
		/// Try to guess content type!
		/// </summary>
		/// <returns>The content type.</returns>
		/// <param name="s">S.</param>
		public static ContentType GuessContentType(String s) {
			if (s == null || s.Trim ().Length == 0)
				return ContentType.PLAIN;

			// Try parse Json
			try {
				JObject.Parse (s);
				return ContentType.JSON;
			} catch {

			}
				
			return ContentType.PLAIN;
		}
	}
}