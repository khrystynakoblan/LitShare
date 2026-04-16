namespace LitShare.DAL.Models
{
    using NpgsqlTypes;

    public enum RequestStatus
    {
        /// <summary>
        /// Request is waiting for a response. Maps to 'pending' in the database.
        /// </summary>
        [PgName("pending")]
        Pending,

        /// <summary>
        /// Request has been accepted by the owner. Maps to 'accepted' in the database.
        /// </summary>
        [PgName("accepted")]
        Accepted,

        /// <summary>
        /// Request has been rejected by the owner. Maps to 'rejected' in the database.
        /// </summary>
        [PgName("rejected")]
        Rejected
    }
}