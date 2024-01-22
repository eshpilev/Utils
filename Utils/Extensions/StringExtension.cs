namespace Utils.Extensions
{
	public static class StringExtension
	{
		public static bool IsNullOrEmpty(this string value) =>
			string.IsNullOrEmpty(value);
		public static bool IsNullOrWhiteSpace(this string value) =>
			string.IsNullOrWhiteSpace(value);		

		/// <summary>
		/// Конвертирует в Enum, в случае неудачи возвращает значение по умолчанию.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="defaultValue">Значение Enum по умолчанию </param>
		/// <returns></returns>
		public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
		{
			if (!string.IsNullOrWhiteSpace(value) &&
				Enum.TryParse<TEnum>(value, true, out var result) &&
				Enum.IsDefined(typeof(TEnum), result))
			{
				return result;
			}
			return defaultValue;
		}
	}
}
