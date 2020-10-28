namespace HomeLibApi.Models
{

    public class ApiResponse<T> where T : class
    {
        public T Data { get; set; }
    }
}