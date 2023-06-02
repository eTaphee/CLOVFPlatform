using System;
using Microsoft.EntityFrameworkCore;

namespace CLOVFPlatform.Server.Models
{
    /// <summary>
    /// 페이징 리스트
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class PaginatedList<T> : List<T>
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
        /// 총 페이지 개수
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 총 항목 개수
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 이전 페이지 여부
        /// </summary>
        // public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// 다음 페이지 여부
        /// </summary>
        // public bool HasNextPage => Page < PageCount;

        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, int count, int page, int pageSize)
        {
            TotalCount = count;
            PageCount = (int)Math.Ceiling(count / (double)pageSize);
            Page = page;
            PageSize = pageSize;

            AddRange(items);
        }

        /// <summary>
        /// 페이징 링크 반환
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<PaginatedLink> GetPaginatedLinks(string path)
        {
            var links = new List<PaginatedLink>();

            links.Add(new PaginatedLink(path, 1, PageSize, PageCount) { Rel = "first", Method = "GET" });
            links.Add(new PaginatedLink(path, (((Page - 1) < 1) ? 1 : Page - 1), PageSize, PageCount) { Rel = "prev", Method = "GET" });
            links.Add(new PaginatedLink(path, (((Page + 1) > PageCount) ? PageCount : Page + 1), PageSize, PageCount) { Rel = "next", Method = "GET" });
            links.Add(new PaginatedLink(path, PageCount, PageSize, PageCount) { Rel = "last", Method = "GET" });

            return links;
        }

        /// <summary>
        /// 페이징된 목록 반환
        /// </summary>
        /// <param name="source">페이징 할 원본 리스트</param>
        /// <param name="page">현재 페이지 위EmployeeDTO</param>
        /// <param name="pageSize">페이지 항목 개ㅅ</param>
        /// <returns></returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int page, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, page, pageSize);
        }
    }
}

