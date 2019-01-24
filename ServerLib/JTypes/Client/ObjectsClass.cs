namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка объектов
    /// </summary>
    public class ObjectsClass : BaseRequestClass
    {
        public ObjectsClass() : base(Enums.Commands.objects) { }
    }
}
