namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка объектов
    /// </summary>
    public class RequestObjectsClass : RequestBaseRequestClass
    {
        public RequestObjectsClass() : base(Enums.Commands.objects) { }
    }
}
