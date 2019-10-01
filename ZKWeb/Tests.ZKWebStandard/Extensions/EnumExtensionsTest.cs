using System.ComponentModel;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions
{
    [Tests]
    class EnumExtensionsTest
    {
        public void GetDescription()
        {
            Assert.Equals(TestEnum.A.GetDescription(), "TestEnum_A");
            Assert.Equals(TestEnum.B.GetDescription(), "TestEnum_B");
            Assert.Equals(TestEnum.C.GetDescription(), "C");
            Assert.Equals(((TestEnum)100).GetDescription(), "100");
        }

        public void GetAttribute()
        {
            Assert.Equals(TestEnum.A.GetAttribute<DescriptionAttribute>().Description, "TestEnum_A");
            Assert.Equals(TestEnum.B.GetAttribute<DescriptionAttribute>().Description, "TestEnum_B");
            Assert.Equals(TestEnum.C.GetAttribute<DescriptionAttribute>(), null);
        }

#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
        enum TestEnum
        {
#pragma warning restore S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
            [Description("TestEnum_A")]
            A,
            [Description("TestEnum_B")]
            B,
            C
        }
    }
}
