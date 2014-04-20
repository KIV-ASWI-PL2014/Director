using System;

namespace Director
{
	/// <summary>
	/// Header class support class for feature reference.
	/// </summary>
	public class Header
	{
		/// <summary>
		/// Header name.
		/// </summary>
		/// <value>The name.</value>
		public String Name { get; set; }

		/// <summary>
		/// Header value.
		/// </summary>
		public String Value { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Header ()
		{
		}

		/// <summary>
		/// Overloaded constructor for creating header from parent.
		/// </summary>
		/// <param name="_parent">parent header</param>
		public Header(Header _parent)
		{
			this.Name = _parent.Name;
			this.Value = _parent.Value;
		}
	}
}

