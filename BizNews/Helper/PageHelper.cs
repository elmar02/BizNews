using BizNews.DTOs;
using BizNews.Models;

namespace BizNews.Helper
{
    public static class PageHelper
    {
        public static PageDTO CreatePageDTO(this List<Article> articles, int page)
        {
            PageDTO pageDTO = new();
            pageDTO.PageSize = (int)Math.Ceiling((double)articles.Count / pageDTO.MaxArticleCount);
            List<Article> pageArticles;
            if (page == 0 || page > pageDTO.PageSize) page = 1;
            pageArticles = articles.Skip((page - 1) * pageDTO.MaxArticleCount).Take(pageDTO.MaxArticleCount).ToList();
            pageDTO.ActivePage = page;
            articles.Clear();
            articles.AddRange(pageArticles);
            return pageDTO;
        }

    }
}
