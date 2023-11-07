namespace BizNews.Helper
{
    public static class SeoHelper
    {
        public static string ConverToSeo(this string url) {
            string azLetters = "çəğıöşü ";
            string replaceLetters = "cegiosu-";
            string result = string.Empty;
            foreach (char s in url)
            {
                int index = azLetters.IndexOf(char.ToLower(s));
                result += index != -1 ? replaceLetters[index]: char.IsLetterOrDigit(s)? char.ToLower(s) : "";
            }
            return result;
        }
    }
}
