//更新时间：2022年5月18日

namespace MosoteachCrawler
{
    public class User
    {
        public string? userId { get; set; }
        public string? fullName { get; set; }
        public string? nickName { get; set; }
        public string? studentNo { get; set; }
        public string? fullAvatarUrl { get; set; }
        public string? userType { get; set; }
    }

    public class UserInfo
    {
        public bool? status { get; set; }
        public string? token { get; set; }
        public User? user { get; set; }
    }
}
