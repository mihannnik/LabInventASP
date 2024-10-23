namespace FileParser.Interfaces
{
    public interface IRabbitMQService
    {
        public void SendMessage(object obj);
        public void SendMessage(string message);
    }
}
