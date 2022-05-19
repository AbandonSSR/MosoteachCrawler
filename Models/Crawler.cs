using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;

using System.Collections.Generic;

namespace MosoteachCrawler
{
    public class Crawler
    {
        private static HttpClient httpClient = new HttpClient(new HttpClientHandler() { UseCookies = true });

        /// <summary>
        /// 获取 Cookie
        /// </summary>
        /// <returns>账号密码错误返回 False ，成功返回 True</returns>
        public static async Task<bool> GetUserInfo()
        {
            var url = "https://coreapi-proxy.mosoteach.cn/index.php/passports/account-login";
            var json = "{\"account\":\"" + Administrator.Account + "\",\"password\":\"" + Administrator.Password + "\"}";
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;

            Administrator.UserInfo = JsonSerializer.Deserialize<UserInfo>(result);

            if (Administrator.UserInfo != null && Administrator.UserInfo.status == true)
            {
                url = $"https://www.mosoteach.cn/web/index.php?c=passport&m=save_proxy_token&proxy_token={Administrator.UserInfo.token}&remember_me=N";
                response = await httpClient.GetAsync(url);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取用户课程信息
        /// </summary>
        /// <returns>成功返回 True，未知错误返回 False</returns>
        public static async Task<bool> GetUserCourse()
        {
            string url = "https://www.mosoteach.cn/web/index.php?c=clazzcourse&m=my_joined";

            var response = await httpClient.PostAsync(url, null);
            string result = response.Content.ReadAsStringAsync().Result;

            Administrator.UserCourse = JsonSerializer.Deserialize<CourseInfo>(result);

            if (Administrator.UserCourse != null && Administrator.UserCourse.result_msg == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取指定课程的资源
        /// </summary>
        /// <param name="course">待查询课程名称</param>
        /// <param name="resource">存储获取到的资源</param>
        /// <returns>成功返回 True，未知错误返回 False</returns>
        public static async Task<bool> GetCourseRes(DataTable resource)
        {
            string url = string.Empty;
            for (int i = 0; i < Administrator.UserCourse.data.Count; i++)
            {
                if (Administrator.UserCourse.data[i].course.name == Administrator.SelectCourseName)
                {
                    url = string.Format("https://www.mosoteach.cn/web/index.php?c=res&m=index&clazz_course_id={0}", Administrator.UserCourse.data[i].id);
                    break;
                }
            }

            var response = await httpClient.GetAsync(url);
            string result = response.Content.ReadAsStringAsync().Result;

            //简化Html
            result = result.Remove(result.IndexOf("<!-- 交互式教材二维码 -->"));
            result = result.Substring(result.IndexOf("res-list-box"), result.Length - result.IndexOf("res-list-box"));
            //获取包含每一个资源的字符串
            string[] separator = { "res-type manual-order" };
            string[] keywordArr = result.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            //正则表达式
            string regexUrl = @"data-href=""[\S]+""";
            string ragexName = @"title=(.*?)<";
            //匹配并存储数据
            for (int i = 0; i < keywordArr.Length - 1; i++)
            {
                Match match = Regex.Match(keywordArr[i], regexUrl);
                string data_href = match.Value;
                data_href = data_href.Substring(11, data_href.Length - 12);
                match = Regex.Match(keywordArr[i], ragexName);
                string name = match.Value;
                name = name.Substring(name.IndexOf('>') + 1, name.Length - name.IndexOf('>') - 2);

                DataRow row = resource.NewRow();
                row["select"] = false;
                row["name"] = name;
                row["url"] = data_href;
                resource.Rows.Add(row);
            }

            return true;
        }

        /// <summary>
        /// 下载选择的资源
        /// </summary>
        /// <returns>成功返回 True，未知错误返回 False</returns>
        public static async Task<bool> DownloadRes()
        {
            try
            {
                Directory.CreateDirectory(DownloadInfo.Path);
                for (int i = 0; i < Administrator.Resource.Rows.Count; i++)
                {
                    if (Administrator.Resource.Rows[i][0].Equals(false))
                        continue;

                    byte[] data = await httpClient.GetByteArrayAsync(Administrator.Resource.Rows[i][2].ToString());

                    string filePath = @$"{DownloadInfo.Path}\{Administrator.Resource.Rows[i][1]}";
                    File.WriteAllBytes(filePath, data);

                    Administrator.Resource.Rows[i][0] = false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
