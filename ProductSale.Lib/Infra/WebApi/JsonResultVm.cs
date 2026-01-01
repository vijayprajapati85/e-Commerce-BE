using System.Net;

namespace ProductSale.Lib.Infra.WebApi
{
    public class JsonResultVm<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Title { get; set; }
        public int TotalCount { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? ErrorReference { get; set; }
        public T? Data { get; set; }
        public static JsonResultVm<T> SuccessResponse(string title, T data, int totalCount = 0)
        {
            return new JsonResultVm<T>
            {
                Title = title,
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Successful",
                Data = data,
                TotalCount = totalCount
            };
        }

        public static JsonResultVm<T> FailResponse(string title, string message, T? data = default(T))
        {
            return new JsonResultVm<T>
            {
                Title = title,
                Success = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message,
                Data = data
            };
        }

    }
}
