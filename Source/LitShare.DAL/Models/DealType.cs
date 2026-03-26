namespace LitShare.DAL.Models
{
    using NpgsqlTypes;

    public enum DealType
    {
        /// <summary>
        /// The item is available for exchange with another book or item.
        /// Maps to 'exchange' in the database.
        /// </summary>
        [PgName("exchange")]
        Exchange = 1,

        /// <summary>
        /// The item is available as a free donation.
        /// Maps to 'donation' in the database.
        /// </summary>
        [PgName("donation")]
        Donation = 2,
    }
}