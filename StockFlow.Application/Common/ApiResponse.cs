namespace StockFlow.Application.Common
{
	public class ApiResponse<T>
	{
		public T? Value { get; set; }
		public string Message { get; set; } = string.Empty;
		public bool Succeeded { get; set; }

		public static ApiResponse<T> Success(T value, string message = "")
		{
			return new ApiResponse<T>()
			{
				Value = value,
				Succeeded = true,
				Message = message
			};
		}

		public static ApiResponse<T> Fail(string message = "")
		{
			return new ApiResponse<T>()
			{
				Value = default,
				Succeeded = false,
				Message = message
			};
		}
	}
}
