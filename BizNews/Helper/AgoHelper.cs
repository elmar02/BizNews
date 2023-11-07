namespace BizNews.Helper
{
    public static class AgoHelper
    {
        public static string AgoConvertor(DateTime date)
        {
            TimeSpan timeAgo = DateTime.Now - date;

            if (timeAgo.TotalSeconds < 60)
            {
                int seconds = (int)timeAgo.TotalSeconds;
                return seconds == 1 ? "1 second ago" : $"{seconds} seconds ago";
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                int minutes = (int)timeAgo.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else if (timeAgo.TotalHours < 24)
            {
                int hours = (int)timeAgo.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeAgo.TotalDays < 30)
            {
                int days = (int)timeAgo.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else if (timeAgo.TotalDays < 365)
            {
                int months = (int)(timeAgo.TotalDays / 30);
                return months == 1 ? "1 month ago" : $"{months} months ago";
            }
            else
            {
                int years = (int)(timeAgo.TotalDays / 365);
                return years == 1 ? "1 year ago" : $"{years} years ago";
            }
        }
    }
}
