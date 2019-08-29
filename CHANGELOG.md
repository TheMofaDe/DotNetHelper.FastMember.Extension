# Changelog
All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),


## [1.0.20] - 2019-08-29

### Bug Fix
- Fix bug in SetMemberValue that was throwing invalid cast exception when setting a value that is Guid Or TimeSpan Type to a type of object
 or dynamic
~~~csharp
 public static void SetMemberValue<T>(T poco, string propertyName, object value)
~~~  



[1.0.20]: https://github.com/TheMofaDe/DotNetHelper.Serialization.Abstractions/releases/tag/v1.0.20

