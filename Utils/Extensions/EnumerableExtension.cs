namespace Utils.Extensions
{
	public static class EnumerableExtension
	{
		/// <summary>
		/// Позволяет выполнить действие для каждого элемента из коллекции
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)			
				action(item);			
		}
	}
}
