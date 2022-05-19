using System.Collections.Generic;

namespace MosoteachCrawler
{
    public class Resource
    {
        public List<string> Name;
        public List<string> Url;

        public Resource()
        {
            Name = new List<string>();
            Url = new List<string>();
        }
    }
}
