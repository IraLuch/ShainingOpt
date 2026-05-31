namespace ShainingOpt.Helpers
{
    public static class PaginationHelpers
    {
        public static (int TotalPages, int Start, int End) CalculatePagination(int totalItems, int pageSize, int pageNumber)
        {
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var start = Math.Max(1, pageNumber - 2);
            var end = Math.Min(pageNumber + 2, totalPages);
            return (totalPages, start, end);
        }
    }
}
