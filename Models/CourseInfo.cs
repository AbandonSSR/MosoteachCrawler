//更新时间：2022年5月18日

using System.Collections.Generic;

namespace MosoteachCrawler
{
    public class Creater
    {
        public string? full_avatar_url { get; set; }
        public string? full_name { get; set; }
        public int? teacher_score { get; set; }
        public string? avatar_url { get; set; }
    }

    public class Course
    {
        public string? create_time { get; set; }
        public string? name { get; set; }
        public int? display_order { get; set; }
        public string? id { get; set; }
    }

    public class Clazz
    {
        public string? name { get; set; }
        public string? id { get; set; }
    }

    public class Data
    {
        public int? display_order { get; set; }
        public string? public_flag { get; set; }
        public string? full_cover_url { get; set; }
        public string? from_mqp { get; set; }
        public string? create_time { get; set; }
        public Creater? creater { get; set; }
        public Course? course { get; set; }
        public string? start_time { get; set; }
        public string? id { get; set; }
        public string? end_time { get; set; }
        public Clazz? clazz { get; set; }
        public string? status { get; set; }
        public string? cover_url { get; set; }
        public string? term_title { get; set; }
        public string? clazz_course_info_url { get; set; }
    }

    public class CourseInfo
    {
        public int? result_code { get; set; }
        public string? result_msg { get; set; }
        public List<Data>? data { get; set; }
    }

}

