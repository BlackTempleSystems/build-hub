namespace UnitTests.CommonTests.Utilities
{
	using BuildHub.Common.Utilities;
	using System.ComponentModel;

	[TestClass]
	public sealed class UtilitiesReflectionTests
	{
		private enum TestEnumeration
		{
			First,
			Second
		}

		[TestMethod]
		public void FormatDateTime_Should_Use_Default_Format()
		{
			var dateTime = new DateTime(2026, 5, 28, 14, 30, 15, 123);

			Assert.AreEqual("2026-05-28 14:30:15.123", Utilities.FormatDateTime(dateTime));
		}

		[TestMethod]
		public void FormatDateTime_Should_Use_Custom_Format()
		{
			var dateTime = new DateTime(2026, 5, 28);

			Assert.AreEqual("20260528", Utilities.FormatDateTime(dateTime, "yyyyMMdd"));
		}

		[TestMethod]
		public void GetTypeName_Should_Return_Type_Name()
		{
			Assert.AreEqual(nameof(SampleModel), Utilities.GetTypeName(typeof(SampleModel)));
		}

		[TestMethod]
		public void GetObjectProperties_Should_Return_Public_And_Non_Public_Instance_Properties()
		{
			var propertyNames = Utilities.GetObjectProperties<SampleModel>()
				.Select(property => property.Name)
				.ToArray();

			CollectionAssert.Contains(propertyNames, nameof(SampleModel.Name));
			CollectionAssert.Contains(propertyNames, nameof(SampleModel.Count));
			CollectionAssert.Contains(propertyNames, "Hidden");
		}

		[TestMethod]
		public void GetPropertyValue_Should_Return_Current_Property_Value()
		{
			var model = new SampleModel
			{
				Name = "BuildHub"
			};

			var propertyInfo = typeof(SampleModel).GetProperty(nameof(SampleModel.Name))!;

			Assert.AreEqual("BuildHub", Utilities.GetPropertyValue(model, propertyInfo));
		}

		[TestMethod]
		public void GetPropertyInfo_Should_Handle_Value_Type_Property_Expressions()
		{
			var propertyInfo = Utilities.GetPropertyInfo<SampleModel>(model => model.Count);

			Assert.AreEqual(nameof(SampleModel.Count), propertyInfo.Name);
		}

		[TestMethod]
		public void GetMemberExpression_Should_Throw_For_Invalid_Expression()
		{
			Assert.Throws<ArgumentException>(() => Utilities.GetMemberExpression<SampleModel>(model => model.Name.ToUpper()));
		}

		[TestMethod]
		public void GetEnumValues_Should_Return_All_Enum_Values()
		{
			var values = EnumUtilities.GetEnumValues<TestEnumeration>().ToArray();

			CollectionAssert.AreEqual(new[] { TestEnumeration.First, TestEnumeration.Second }, values);
		}

		private sealed class SampleModel
		{
			private string Hidden { get; set; } = "hidden";

			public string Name { get; set; } = string.Empty;

			[Description("Count")]
			public int Count { get; set; }
		}
	}
}
