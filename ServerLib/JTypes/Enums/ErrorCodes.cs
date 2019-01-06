namespace ServerLib.JTypes.Enums
{
    /// <summary>
    /// Коды ошибок
    /// </summary>
    public enum ErrorCodes
    {
        NoError, FatalError, DatabaseError, NotAuthenticated, IncorrectToken, IncorrectLoginOrPassword, SessionNotFound, NonTextMessage, NotJSONObject
    }
}
