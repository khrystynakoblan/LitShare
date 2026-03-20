namespace LitShare.DAL.Models
{
    using NpgsqlTypes;

    public enum RoleType
    {
        /// <summary>
        /// Standard application user role. Maps to 'user' in the database.
        /// </summary>
        [PgName("user")]
        User,

        /// <summary>
        /// Administrative user role with elevated privileges. Maps to 'admin' in the database.
        /// </summary>
        [PgName("admin")]
        Admin,
    }
}