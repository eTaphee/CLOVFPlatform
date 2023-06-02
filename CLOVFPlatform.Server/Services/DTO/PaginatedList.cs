using System;
using CLOVFPlatform.Server.Models;

namespace CLOVFPlatform.Server.Services.DTO
{
    /// <summary>
    /// 페이징 리스트
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class PaginatedList<T>
    {
        /// <summary>
        /// 현재 페이지 위치
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 페이지 항목 개수
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 현재 항목 개수
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 다음 페이지 항목 개수
        /// </summary>
        public int NextCount
        {
            get
            {
                return (Page < PageCount) ? (TotalCount - (PageSize * Page)) : 0;
            }
        }

        /// <summary>
        /// 현재 페이지 위치 까지 항목 개수
        /// </summary>
        public int UpToPageCount
        {
            get
            {
                return (Page < PageCount) ? Page * PageSize : TotalCount;
            }
        }

        /// <summary>
        /// 총 페이지 개수
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 총 항목 개수
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 링크
        /// </summary>
        public IEnumerable<PaginatedLink> Links { get; set; }

        /// <summary>
        /// 항목 목록
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        public PaginatedList()
		{
		}
	}
}

