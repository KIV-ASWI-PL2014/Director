using System;

namespace Xwt.Backends
{
	public interface IMultiLineTextEntryBackend: IWidgetBackend
	{
		string Text { get; set; }
		bool ReadOnly { get; set; }
	}
}

