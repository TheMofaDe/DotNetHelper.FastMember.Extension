

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DotNetHelper.FastMember.Extension.Helpers
{

    internal static class OSHelper
    {
        public static bool IsRunningOnIOS { get; }
        static OSHelper()
        {
#if NETSTANDARD
            IsRunningOnIOS = File.Exists(@"/System/Library/CoreServices/SystemVersion.plist") || System.Runtime.InteropServices.RuntimeInformation
                                             .IsOSPlatform(OSPlatform.OSX);
#endif
        }
    }
}


