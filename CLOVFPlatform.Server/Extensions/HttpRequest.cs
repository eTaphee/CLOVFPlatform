using System;

namespace CLOVFPlatform.Server.Extensions
{
	public static class HttpRequestExtensions
	{
		public static string getRequestUrl(this HttpRequest request)
		{
            return $"{request.Scheme}://{request.Host}{request.Path}";
        }
	}
}

