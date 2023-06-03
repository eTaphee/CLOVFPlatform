using System;

namespace CLOVFPlatform.Server.Controllers
{
	public class CLOVFPlatformException : Exception
	{
		public int StatusCode { get; set; }
		public string? Detail { get; set; }

		public CLOVFPlatformException(int statusCode, string message, string? detail = null) : base(message)
		{
			this.StatusCode = statusCode;
			this.Detail = detail;
		}
	}
}

