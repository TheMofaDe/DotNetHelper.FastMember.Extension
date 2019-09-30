using System;

namespace DotNetHelper.FastMember.Extension.Tests.Models
{
    public class PublicPropertiesModel
    {
        public bool IsPublicProperty { get; set; } = true;
        public bool? NullBoolean { get; set; } = null;
    }

    public class StringValueModel
    {
        public Guid Guid { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }

    public class GenericModelWithGetOnlyProperties
    {
        public bool IsPublicProperty { get; } = true;
        public bool? NullBoolean { get; } = null;
    }

    public class PublicPropertiesNoAccessor
    {
        public bool IsPublicProperty = true;
        public bool? FalseNullableBoolean = false;
        public bool? NullBoolean = null;
    }


    public class NullableFields
    {
        public decimal? Decimal { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTimeOffset? DateTimeOffset { get; set; }
        public object Object { get; set; }
    }

}
