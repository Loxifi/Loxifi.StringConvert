namespace Loxifi.StringConvert.Tests
{
	public partial class StringConvertTests
	{
		class ExplicitTest
		{
			public string Value { get; set; }

			public static explicit operator ExplicitTest(string s)
			{
				return new ExplicitTest()
				{
					Value = s
				};
			}
		}
	}
}