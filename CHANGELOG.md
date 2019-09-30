# Changelog
All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),


## [1.0.26] - 2019-09-30

### Bug Fix
- Switch ObjectMapper to use ExtFastmember for mapping values 
- Fix Bugs in SetMemberValue & ObjectMapper where mapping non-string type to string properties failed due to no iconvertible provided. These senarios will now map
using ToString() method

## [1.0.24] - 2019-09-22

### Bug Fix
- Fix bug in SetMemberValue where a string that is a timespan couldn't be set to a property that has a value type of TimeSpan due to failed conversion

## [1.0.22] - 2019-09-21

### Bug Fix
- Fix bug in SetMemberValue where a string that is a guid couldn't be set to a property that has a value type of Guid due to failed conversion

## [1.0.20] - 2019-08-29

### Bug Fix
- Fix bug in SetMemberValue that was throwing invalid cast exception when setting a value that is Guid Or TimeSpan Type to a type of object
 or dynamic
~~~csharp
 public static void SetMemberValue<T>(T poco, string propertyName, object value)
~~~  



[1.0.20]: https://github.com/TheMofaDe/DotNetHelper.Serialization.Abstractions/releases/tag/v1.0.20

