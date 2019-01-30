namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка должностей пользователей
    /// </summary>
    public class RequestJobsClass : RequestBaseRequestClass
    {
        public RequestJobsClass() : base(Enums.Commands.jobs) { }
    }
}
