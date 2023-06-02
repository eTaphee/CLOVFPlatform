using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CLOVFPlatform.Server.Models
{
    /// <summary>
    /// 페이징 링크
    /// </summary>
	public class PaginatedLink
	{
        /// <summary>
        /// 링크 관계
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// 메서드
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 링크
        /// </summary>
        public string Link => $"{Path}?page={Page}&pageSize={PageSize}";

        /// <summary>
        /// 요청 경로
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 현재 페이지 위치
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 페이지 항목 개수
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 총 페이지 개수
        /// </summary>
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

