namespace AS.Core.Primitives
{
    public sealed class BaseApiResponse<T>
    {
        public BaseApiResponse(T? data)
        {
            this.Data = data;
            this.Success = true;
        }

        public BaseApiResponse(string error)
        {
            this.Success = false;
            this.Message = error;
        }

        public T? Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
