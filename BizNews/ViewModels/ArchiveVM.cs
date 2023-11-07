using BizNews.DTOs;
using BizNews.Models;

namespace BizNews.ViewModels
{
    public class ArchiveVM
    {
        public PageDTO PageDTO { get; set; }
        public List<Article> Articles { get; set; }
        public List<Article> TrandingNews { get; set; }
        public Advert Advert { get; set; }
        public List<Tag> Tags { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public List<int> Dates { get; set; }
    }
}
