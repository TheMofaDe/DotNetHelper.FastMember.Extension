using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetHelper.FastMember.Extension.Tests
{
    public class PrivatePropertiesModel
    {
        private bool IsPrivate { get; set; } = true;
        private bool? IsPrivateNullable { get; set; } = null;
    }
}
