namespace LeaveANoteServerProject.DTO_s.StatsDto_s
{   /// <summary>
    /// Represents the request data for retrieving reports distribution.
    /// </summary>
    public class RegisteredUserDataDtoReq
    {
        /// <summary>
        /// Gets or sets the year for which to retrieve registered users' data.
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Gets or sets the role for which to retrieve registered users' data.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
