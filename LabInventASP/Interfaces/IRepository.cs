namespace LabInventASP.Interfaces
{
    public interface IRepository<T>
    {
        public string Save(T device);
        public T Load(string id);
        public List<T> LoadAll();
    }
}
