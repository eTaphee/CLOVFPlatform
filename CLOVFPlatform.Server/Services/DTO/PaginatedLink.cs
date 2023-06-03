using Newtonsoft.Json;

namespace CLOVFPlatform.Server.Services.DTO
{
    /// <summary>
    /// 페이징 링크
    /// </summary>
	public class PaginatedLink
	{
        /// <summary>
        /// 링크 관계
        /// </summary>
        [JsonProperty(Order = 0)]
        public string? Rel { get; set; }

        /// <summary>
        /// 메서드
        /// </summary>
        [JsonProperty(Order = 1)]
        public string? Method { get; set; }

        /// <summary>
        /// 링크
        /// </summary>
        [JsonProperty(Order = 2)]
        public string Link => $"{Path}?page={Page}&pageSize={PageSize}";

        /// <summary>
        /// 요청 경로
        /// </summary>
        [JsonIgnore]
        public string? Path { get; set; }

        /// <summary>
        /// 현재 페이지 위치
        /// </summary>
        [JsonIgnore]
        public int Page { get; set; }

        /// <summary>
        /// 페이지 항목 개수
        /// </summary>
        [JsonIgnore]
        public int PageSize { get; set; }

        /// <summary>
        /// 총 페이지 개수
        /// </summary>
        [JsonIgnore]
        public int PageCount { get; set; }

        public PaginatedLink()
		{
		}

        public PaginatedLink(string path, int page, int pageSize, int pageCount)
        {
            Path = path;
            Page = (page < 1) ? 1 : page;
            PageSize = (pageSize < 1) ? 1 : pageSize;
            PageCount = pageCount;
        }

        public override string ToString()
        {
            return $"<{Link}>; rel=\"{Rel}\"";
        }
    }
}

