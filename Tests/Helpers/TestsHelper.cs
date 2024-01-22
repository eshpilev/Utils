using System.Text.RegularExpressions;
using Utils.Extensions;

namespace Utils.Tests.Helpers
{
	public class TestsHelper
	{
		public static string GetEnvironmentPath(string fileName) =>	@$"Environment/Result/{fileName}";

		private static readonly Regex JsonOneLine = new("(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", RegexOptions.Compiled);

		public static void JsonAssert<T>(T obj, string expectedFile)
		{
			var jsonExpected = File.ReadAllText(GetEnvironmentPath(expectedFile));
			jsonExpected = JsonOneLine.Replace(jsonExpected, "$1");

			var jsonActual = obj.ToJsonString(); 
			Assert.AreEqual(jsonExpected, jsonActual);		
		}
	}
}
