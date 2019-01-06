namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка должностей пользователей
    /// </summary>
    public class JobsClass : BaseRequestClass
    {
        public JobsClass()
        {
            Command = Enums.Commands.jobs;
        }
    }
}
