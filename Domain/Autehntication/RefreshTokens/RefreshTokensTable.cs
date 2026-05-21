using BuildHub.DataEngine.Tables.Base;
using BuildHub.Domain.Autehntication.RefreshTokens.Entities;

namespace BuildHub.Domain.Autehntication.RefreshTokens
{
	public sealed class RefreshTokensTable : BaseTable<RefreshTokenEntity>
	{
		public RefreshTokensTable()
			: base(DataEngine.DatabaseConnection.DatabaseSource.Users)
		{

		}
	}
}
