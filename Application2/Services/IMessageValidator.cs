namespace Application2.Services
{
    public interface IMessageValidator
    {
        bool Validate(string message);

        string Sanitize(string message);
    }
}
