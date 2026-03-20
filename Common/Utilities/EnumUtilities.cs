using System.ComponentModel;

namespace BuildHub.Common.Utilities
{
    /// <summary>
    /// Provides utility methods for working with enumerations, including retrieving descriptions and values.
    /// </summary>
    /// <remarks>This static class contains methods that facilitate the retrieval of description attributes
    /// from enumeration values and the enumeration values themselves. It is designed to simplify common operations
    /// related to enumerations in .NET applications.</remarks>
    public static class EnumUtilities
    {
        /// <summary>
		/// Retrieves a description attribute from an enumeration
		/// </summary>
		/// <typeparam name="TEnumType">Type parameter for enumerations</typeparam>
		/// <param name="enumeration">Value of the enum</param>
		/// <returns>string</returns>
		public static string GetEnumDescription<TEnumType>(Enum enumeration)
            where TEnumType : Enum
        {
            DescriptionAttribute? descriptionAttribute = enumeration.GetType()?.GetField(enumeration.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;

            return descriptionAttribute?.Description ?? string.Empty;
        }

        /// <summary>
        /// Retrieves all values of the specified enumeration type.
        /// </summary>
        /// <typeparam name="TEnumType">The enumeration type whose values are to be retrieved. This type must be an enumeration.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all values of the specified enumeration type.</returns>
        public static IEnumerable<TEnumType> GetEnumValues<TEnumType>() => Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>();

    }
}
