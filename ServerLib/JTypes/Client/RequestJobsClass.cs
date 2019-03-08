namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка должностей пользователей
    /// </summary>
    public class RequestJobsClass : RequestBaseClass
    {
        public RequestJobsClass() : base(Enums.Commands.jobs) { }
    }
}
