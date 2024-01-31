using System.Reflection;
using System.Text.RegularExpressions;
using BizArk.Core.Extensions.AttributeExt;
using Utils.Attributes;
using Utils.Extensions;

namespace Utils.Helpers
{
    /// <summary> Удаление номера телефона в строковых свойствах объекта </summary>
    public class CleanerPhoneNumber
	{
		public IEnumerable<string> Regions { get; set; } = new[] { "RU" };
		public CleanerPhoneNumber() { }
		public CleanerPhoneNumber(IEnumerable<string> regions) => Regions = regions;

		public void Clean<T>(T data) where T : class =>
			((object)data).TransformStringProperties(RemovePhoneNumber, NeedRemove);		

		private static bool NeedRemove(PropertyInfo prop) =>
			prop.GetAttribute<NeedCleaningPhoneAttribute>(false) != null;

		public string RemovePhoneNumber(string value)
		{
			var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
			foreach (var region in Regions)
			{
				phoneNumberUtil.FindNumbers(value, region)
					.Where(x => Regex.IsMatch(x.RawString, "^(\\+{1})?[\\d\\s\\(\\)-]+$"))
					.ForEach(x => value = value.Replace(x.RawString, ""));
			}
			value = Regex.Replace(value.Trim(), "\\s+", " ");
			return string.IsNullOrEmpty(value) ? null : value;
		}
	}	
}