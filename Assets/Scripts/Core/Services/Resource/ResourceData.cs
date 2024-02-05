namespace Runner.Core.Services.Resource
{
    /// <summary>
    /// Контейнер для передачи параметров инстанцирования объекта
    /// </summary>
    public struct ResourceData
    {
        public bool DontDestroy;
        public string Name;

        public ResourceData(bool dontDestroy, string name)
        {
            DontDestroy = dontDestroy;
            Name = name;
        }
    }
}