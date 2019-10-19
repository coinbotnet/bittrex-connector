namespace Coinbot.Bittrex.Models
{
    public class ResultWrapper<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public T result { get; set; }
    }
}