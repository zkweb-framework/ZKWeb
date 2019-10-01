using System.Collections.Generic;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions
{
    [Tests]
#pragma warning disable S101 // Types should be named in camel case
    class IDictionaryExtensionsTest
    {
#pragma warning restore S101 // Types should be named in camel case
        public void GetOrDefault()
        {
            var dict = new Dictionary<string, string>() { { "exist", "1" } };
            Assert.Equals(dict.GetOrDefault("exist"), "1");
            Assert.Equals(dict.GetOrDefault("notexist"), null);
            Assert.Equals(dict.GetOrDefault("notexist", "default"), "default");
            Assert.Equals(dict.GetOrDefault(null, "default"), "default");
            Assert.Equals(dict.GetOrDefault("", "default"), "default");
        }

        public void GetOrCreate()
        {
            var dict = new Dictionary<string, string>() { { "exist", "1" } };
            Assert.Equals(dict.GetOrCreate("exist", () => "create"), "1");
            Assert.Equals(dict.GetOrCreate("notexist", () => "create"), "create");
            Assert.Equals(dict["notexist"], "create");
        }

#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
        enum TestEnum
        {
#pragma warning restore S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
            Zero = 0,
            One = 1
        }

        public void GetOrDefaultT()
        {
            var dict = new Dictionary<string, object>() { { "exist", 1 } };
            Assert.Equals(dict.GetOrDefault<int>("exist"), 1);
            Assert.Equals(dict.GetOrDefault<int>("notexist"), 0);
            Assert.Equals(dict.GetOrDefault("notexist", 100), 100);
            Assert.Equals(dict.GetOrDefault<TestEnum>("exist"), TestEnum.One);
            Assert.Equals(dict.GetOrDefault<TestEnum>("notexist"), TestEnum.Zero);
        }
    }
}
