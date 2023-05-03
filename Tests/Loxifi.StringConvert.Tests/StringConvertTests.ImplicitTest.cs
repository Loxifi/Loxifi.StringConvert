namespace Loxifi.StringConvert.Tests
{
	public partial class StringConvertTests
	{
		class ImplicitTest
		{
			public string Value { get; set; }

			public static implicit operator ImplicitTest(string s)
			{
				return new ImplicitTest()
				{
					Value = s
				};
			}
		}
	}
}