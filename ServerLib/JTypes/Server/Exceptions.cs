namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Исключения
    /// </summary>
    public static class Exceptions
    {
        public static readonly object ErrorNonTextMessage = new
        {
            state = "error",
            code = 2,
            description = "Received non-text message"
        };

        public static readonly object ErrorSessionNotFound = new
        {
            state = "error",
            code = 3,
            description = "Session not found"
        };

        public static readonly object ErrorNotAuthenticated = new
        {
            state = "error",
            code = 4,
            description = "User is not authenticated"
        };

        public static readonly object ErrorNotJSONObject = new
        {
            state = "error",
            code = 5,
            description = "Not a json object"
        };

        public static readonly object ErrorUnknownCommand = new
        {
            state = "error",
            code = 6,
            description = "Unknown command"
        };

        public static readonly object ErrorIncorrectLoginOrPassword = new
        {
            state = "error",
            code = 7,
            description = "Incorrect login or password"
        };

        public static readonly object ErrorIncorrectToken = new
        {
            state = "error",
            code = 8,
            description = "Incorrect token"
        };
    }
}
