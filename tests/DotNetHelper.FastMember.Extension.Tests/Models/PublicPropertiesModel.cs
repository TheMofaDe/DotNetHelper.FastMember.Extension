using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetHelper.FastMember.Extension.Tests
{
    public class PublicPropertiesModel
    {
        public bool IsPublicProperty { get; set; } = true; 
        public bool? NullBoolean { get; set; } = null;
    }

    public class GenericModelWithGetOnlyProperties
    {
        public bool IsPublicProperty { get; } = true;
        public bool? NullBoolean { get;  } = null;
    }

    public class PublicPropertiesNoAccessor
    {
        public bool IsPublicProperty  = true;
        public bool? FalseNullableBoolean = false;
        public bool? NullBoolean = null;
    }

}
