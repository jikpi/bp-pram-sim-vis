namespace PRAM_lib.Code.CustomExceptions
{
    /// <summary>
    /// A custom exception that handles runtime errors, which may occur due to user error. Normal exceptions are not handled, to show debug info.
    /// </summary>
    internal class LocalException : Exception
    {
        public LocalException(string message) : base(message)
        {
        }
    }
}
