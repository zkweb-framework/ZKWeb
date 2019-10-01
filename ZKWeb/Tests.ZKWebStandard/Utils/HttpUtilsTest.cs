using System.Collections.Generic;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Tests.Utils
{
    [Tests]
    class HttpUtilsTest
    {
        public void UrlEncode()
        {
            Assert.Equals(HttpUtils.UrlEncode("'&^%=abc"), "%27%26%5E%25%3Dabc");
        }

        public void UrlDecode()
        {
            Assert.Equals(HttpUtils.UrlDecode("%27%26%5E%25%3Dabc"), "'&^%=abc");
        }

        public void HtmlEncode()
        {
            Assert.Equals(HttpUtils.HtmlEncode("asd'\"<>"), "asd&#39;&quot;&lt;&gt;");
        }

        public void HtmlDecode()
        {
            Assert.Equals(HttpUtils.HtmlDecode("asd&#39;&quot;&lt;&gt;"), "asd'\"<>");
        }

        public void SplitPathAndQuery()
        {
            string path;
            string query;

            HttpUtils.SplitPathAndQuery("test", out path, out query);
            Assert.Equals(path, "test");
            Assert.Equals(query, "");

            HttpUtils.SplitPathAndQuery("test?a=1&b=2", out path, out query);
            Assert.Equals(path, "test");
            Assert.Equals(query, "?a=1&b=2");
        }

        public void ParseQueryString()
        {
            var result = HttpUtils.ParseQueryString("a=1&b=2&key=%27%26%5E%25%3Dabc");
            Assert.Equals(result.Count, 3);
            Assert.Equals(result["a"].Count, 1);
            Assert.Equals(result["a"][0], "1");
            Assert.Equals(result["b"].Count, 1);
            Assert.Equals(result["b"][0], "2");
            Assert.Equals(result["key"].Count, 1);
            Assert.Equals(result["key"][0], "'&^%=abc");

            result = HttpUtils.ParseQueryString("?name=some&name=other");
            Assert.Equals(result.Count, 1);
            Assert.Equals(result["name"].Count, 2);
            Assert.Equals(result["name"][0], "some");
            Assert.Equals(result["name"][1], "other");
        }

        public void BuildQueryString()
        {
            var query = new Dictionary<string, IList<string>>();
            query["name"] = new[] { "john", "harold" };
            query["age"] = new[] { "50&51" };
            var result = HttpUtils.BuildQueryString(query);
            Assert.IsTrueWith(
                (result == "name=john&name=harold&age=50%2651" ||
                result == "age=50%2651&name=john&name=harold"), result);
        }
    }
}
