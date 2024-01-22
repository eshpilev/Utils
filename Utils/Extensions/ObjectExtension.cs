using System.Collections;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Utils.Extensions
{
	public static class ObjectExtension
	{
		/// <summary> Модификация строковых свойств в объекте рекурсивно </summary>
		/// <para>
		/// Данный метод позволяет рекурсивно перебрать все свойства строкового типа которые есть в объекте <see cref="obj"/>
		/// и применить к ним функцию  <see cref="transformer"/> которая преобразует строку.
		/// Функцию можно применять не ко всем свойствам строкового типа, а по условию <see cref="needTransform"/>
		/// Если условие <see cref="needTransform"/> для объекта составного типа возвращает true,
		/// то это значит что функция-модификатор должна быть применена ко всем вложенным свойствам
		/// </para>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="transformer">Функция-модификатор которая преобразует строку </param>
		/// <param name="needTransform">Условие применения функции-модификатора к свойству, по умолчанию надо обрабатывать все свойства </param>
		public static void TransformStringProperties<T>(this T obj, Func<string, string> transformer, Predicate<PropertyInfo> needTransform = null)
			where T : class, new()
		{
			if (obj == null)
				return;

			if (transformer == null)
				return;

			//Если условие не задано, то свойство должно быть обработано
			needTransform ??= (_) => true;

			var type = obj.GetType();
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
			foreach (var property in properties)
			{
				var value = property.GetValue(obj);
				switch (value)
				{
					case null:
						continue;
					case string stringValue:
						if (needTransform(property))
							property.SetValue(obj, transformer(stringValue));
						break;
					case IList<string> strEnumerable:
						if (needTransform(property))
							for (var i = 0; i < strEnumerable.Count; i++)
								strEnumerable[i] = transformer(strEnumerable[i]);
						break;
					case IList enumerable:
						foreach (var item in enumerable)
							item.TransformStringProperties(transformer, needTransform(property) ? null : needTransform);
						break;
					default:
						value.TransformStringProperties(transformer, needTransform(property) ? null : needTransform);
						break;
				}
			}
		}

		public static string ToJsonString<T>(this T obj)
		{
			if (obj is null)
				return string.Empty;

			return JsonSerializer.Serialize(obj, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
		}
	}
}
