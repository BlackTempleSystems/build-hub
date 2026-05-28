namespace UnitTests.DataEngineTests.Entities
{
	using BuildHub.DataEngine.Entities;
	using BuildHub.DataEngine.Exceptions.Entities;
	using UnitTests.DataEngineTests.Tables;

	[TestClass]
	public sealed class EntityDataMapperMetadataTests
	{
		[TestMethod]
		public void Get_Column_Info_Should_Return_Configured_Size()
		{
			var columnInfo = new ColumnInfo("DISPLAY_NAME", 128);

			Assert.AreEqual("DISPLAY_NAME", columnInfo.ColumnName);
			Assert.AreEqual(128, columnInfo.Size);
		}

		[TestMethod]
		public void Get_Column_Info_Should_Throw_For_Unmapped_Property()
		{
			var propertyInfo = typeof(EntityWithUnmappedProperty).GetProperty(nameof(EntityWithUnmappedProperty.UnmappedName))!;

			Assert.Throws<MissingColumnDescriptionException>(() => EntityDataMapper.GetColumnInfo(propertyInfo));
		}

		[TestMethod]
		public void Get_Table_Name_Should_Throw_When_Attribute_Is_Missing()
		{
			Assert.Throws<MissingTableNameException>(() => EntityDataMapper.GetTableName<EntityWithoutTableName>());
		}

		[TestMethod]
		public void Get_Primary_Key_Mapping_Data_Should_Throw_When_Primary_Key_Is_Missing()
		{
			Assert.Throws<System.Data.MissingPrimaryKeyException>(() => EntityDataMapper.GetPrimaryKeyMappingData<EntityWithoutPrimaryKey>());
		}

		[TestMethod]
		public void Has_Identity_And_Primary_Key_Should_Read_Base_Entity_Attributes()
		{
			var idProperty = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!;
			var guidProperty = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Guid))!;

			Assert.IsTrue(EntityDataMapper.HasIdentityColumn(idProperty));
			Assert.IsTrue(EntityDataMapper.HasPrimaryKeyColumn(idProperty));
			Assert.IsFalse(EntityDataMapper.HasIdentityColumn(guidProperty));
			Assert.IsFalse(EntityDataMapper.HasPrimaryKeyColumn(guidProperty));
		}

		[TestMethod]
		public void Get_Column_Value_Should_Read_Property_Value_From_Entity()
		{
			var entity = new IntegrationTestEntity()
			{
				Name = "Metadata Test"
			};

			var propertyInfo = typeof(IntegrationTestEntity).GetProperty(nameof(IntegrationTestEntity.Name))!;

			Assert.AreEqual("Metadata Test", EntityDataMapper.GetColumnValue(entity, propertyInfo));
		}

		[TestMethod]
		public void Column_Mapping_Data_Should_Expose_Column_And_Property_Metadata()
		{
			var propertyInfo = typeof(IntegrationTestEntity).GetProperty(nameof(IntegrationTestEntity.Name))!;
			var columnInfo = new ColumnInfo("NAME");

			var mappingData = new ColumnMappingData(columnInfo, propertyInfo);

			Assert.AreSame(columnInfo, mappingData.ColumnInfo);
			Assert.AreSame(propertyInfo, mappingData.PropertyInfo);
		}

		private sealed class EntityWithoutTableName : IEntity
		{
			[PrimaryKey]
			[ColumnInfo("ID")]
			public int Id { get; set; }

			[ColumnInfo("GUID")]
			public Guid Guid { get; set; }
		}

		[TableName("NO_PRIMARY_KEY_ENTITY")]
		private sealed class EntityWithoutPrimaryKey : IEntity
		{
			[ColumnInfo("ID")]
			public int Id { get; set; }

			[ColumnInfo("GUID")]
			public Guid Guid { get; set; }
		}

		[TableName("UNMAPPED_ENTITY")]
		private sealed class EntityWithUnmappedProperty : IEntity
		{
			[PrimaryKey]
			[ColumnInfo("ID")]
			public int Id { get; set; }

			[ColumnInfo("GUID")]
			public Guid Guid { get; set; }

			public string UnmappedName { get; set; } = string.Empty;
		}
	}
}
