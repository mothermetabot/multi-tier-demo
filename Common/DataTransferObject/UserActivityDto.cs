using Common.Enumerations;

namespace Common.DataTransferObject
{
    public class UserActivityDto
    {
        /// <summary>
        /// The type of connection this object represents.
        /// </summary>
        public UserActivityType Type { get; set; }

        /// <summary>
        /// The user this activity notification originated from.
        /// </summary>
        public UserDto? User { get; set; }
    }
}
