namespace Application2.Services
{
    public class DefaultMessageValidator : IMessageValidator
    {
        public static int MaxLength { get; set; } = 100;

        public string Sanitize(string message)
        {
            ThrowForNullString(message);

            if (Validate(message))
                return message;

            return message.Substring(message.Length - MaxLength, MaxLength);
        }

        public bool Validate(string message)
        {
            ThrowForNullString(message);
            return message.Length <= MaxLength;
        }

        private void ThrowForNullString(string message)
        {
            if(message == null) throw new ArgumentNullException("message");
        }
    }
}
