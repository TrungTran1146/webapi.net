﻿namespace WebAPI.Model
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
