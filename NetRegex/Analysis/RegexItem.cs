using System;

namespace NetRegex.Analysis
{
	public abstract class RegexItem
	{
		public RegexItem()
		{
		}

		public void Parse(string expression)
		{

		}

		public abstract string ToString(int indent);
	}
}
