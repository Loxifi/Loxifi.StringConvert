namespace Loxifi.StringConvert.Tests
{
	[TestClass]
	public partial class StringConvertTests
	{
		[TestMethod]
		public void TestBoolInt()
		{
			string s = "1";
			bool b = s.Convert<bool>();
			Assert.IsTrue(b);
		}

		[TestMethod]
		public void TestBoolIntFalse()
		{
			string s = "0";
			bool b = s.Convert<bool>();
			Assert.IsFalse(b);
		}

		[TestMethod]
		public void TestBoolString()
		{
			string s = "true";
			bool b = s.Convert<bool>();
			Assert.IsTrue(b);
		}

		[TestMethod]
		public void TestBoolStringFalse()
		{
			string s = "false";
			bool b = s.Convert<bool>();
			Assert.IsFalse(b);
		}

		[TestMethod]
		public void TestExplicit()
		{
			string s = "123";
			ExplicitTest explicitTest = s.Convert<ExplicitTest>();
			Assert.AreEqual("123", explicitTest.Value);
		}

		[TestMethod]
		public void TestImplicit()
		{
			string s = "123";
			ImplicitTest explicitTest = s.Convert<ImplicitTest>();
			Assert.AreEqual("123", explicitTest.Value);
		}

		[TestMethod]
		public void TestNullableIntNull()
		{
			string s = "";
			int? i = s.Convert<int?>();
			Assert.IsFalse(i.HasValue);
		}

		[TestMethod]
		public void TestNullableIntValue()
		{
			string s = "1";
			int? i = s.Convert<int?>();
			Assert.AreEqual(1, i);
		}

		[TestMethod]
		public void TestString()
		{
			string s = "XYZ";
			string i = s.Convert<string>()!;
			Assert.AreEqual("XYZ", i);
		}

		[TestMethod]
		public void TestStringNull()
		{
			string s = null;
			string i = s.Convert<string>()!;
			Assert.IsNull(i);
		}
	}
}