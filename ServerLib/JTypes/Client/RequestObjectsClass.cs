namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка объектов
    /// </summary>
    public class RequestObjectsClass : RequestBaseClass
    {
        public RequestObjectsClass() : base(Enums.Commands.objects) { }
    }
}
