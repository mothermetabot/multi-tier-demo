namespace Application1.Structs
{
    public readonly struct Constant
    {
#if DEBUG
        public const string REAL_TIME_SERVICE_URI = "http://localhost:5229/main";
#else
        public const string REAL_TIME_SERVICE_URI = "http://localhost:5000/main";
#endif
    }
}
