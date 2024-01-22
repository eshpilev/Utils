using Utils.Attributes;
using Utils.Helpers;
using Utils.Tests.Helpers;

namespace Utils.Tests
{
	[TestClass]
	public class CleanerPhoneNumberTests
	{
		[TestMethod]
		public void Clean()
		{
			var company = new CompanyInfo
			{
				Inn = "1234567890",
				Ogrn = "12345678901234",
				Name = "OOO ������ +7(495)001-01-01",
				AddInfo = "���������� ��� +79099991234",
				Founders = new[] { "������ 8(909)987-65-43", "������" },
				Director = new DirectorInfo
				{
					Fio = "�������",
					AddInfo = "+7-925-123-45-67"
				},
				Customers = new CustomerInfo[]
				{
					new()
					{
						Inn = "1111111111",
						Name = "������ 1"
					},
					new()
					{
						Inn = "2222222222",
						Name = "������ 890998765432",
						AddInfo = "89099876543"
					}
				}				
			};

			new CleanerPhoneNumber().Clean(company);

			TestsHelper.JsonAssert(company, "cleanerPhone.json");
		}
	}

	public class CompanyInfo
	{
		public string Inn { get; set; }
		public string Ogrn { get; set; }
		public string Name { get; set; }
		[NeedCleaningPhone]
		public string AddInfo { get; set; }
		[NeedCleaningPhone]
		public string[] Founders { get; set; }
		[NeedCleaningPhone]
		public DirectorInfo Director { get; set; }
		public CustomerInfo[] Customers { get; set; }
	}

	public class DirectorInfo
	{		
		public string Fio { get; set; }
		public string AddInfo { get; set; }
	}

	public class CustomerInfo
	{
		public string Inn { get; set; }	
		public string Name { get; set; }
		[NeedCleaningPhone]
		public string AddInfo { get; set; }
	}
}