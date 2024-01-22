using Utils.Helpers;

namespace Utils.Tests
{
	[TestClass]
	public class StringBuilderLimitedTests
	{
		public static readonly string Delimiter = Environment.NewLine;

		[TestMethod]
		public void MoreLimit()
		{
			var errors = new StringBuilderLimited(3);
			errors.AppendLine("1");
			errors.AppendLine("2");
			errors.AppendLine("3");
			errors.AppendLine("4");
			errors.AppendLine("5");

			var result = errors.ToString();
			Assert.AreEqual(result, $"ВНИМАНИЕ! Список сообщений переполнен! Отображено 3 из 5:{Delimiter}" +
			                        $"1. 1{Delimiter}2. 2{Delimiter}3. 3{Delimiter}");
		}

		[TestMethod]
		public void LessLimit()
		{
			var errors = new StringBuilderLimited(5);
			errors.AppendLine("1");
			errors.AppendLine("2");
			errors.AppendLine("3");

			var result = errors.ToString();
			Assert.AreEqual(result, $"1. 1{Delimiter}2. 2{Delimiter}3. 3{Delimiter}");
		}
	}
}