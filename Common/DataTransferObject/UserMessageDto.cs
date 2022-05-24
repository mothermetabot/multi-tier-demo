using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataTransferObject
{
    public class UserMessageDto
    {
        /// <summary>
        /// The Id of the user sending the message.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The content of the message sent.
        /// </summary>
        public string? Content { get; set; }
    }
}
