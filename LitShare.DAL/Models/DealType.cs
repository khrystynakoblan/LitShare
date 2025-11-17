using NpgsqlTypes;

namespace LitShare.DAL.Models
{
    public enum DealType
    {
        [PgName("exchange")]
        Exchange,

        [PgName("donation")]
        Donation
    }
}