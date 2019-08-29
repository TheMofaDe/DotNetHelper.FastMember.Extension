using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using DotNetHelper.FastMember.Extension.Extension;

namespace DotNetHelper.FastMember.Extension.Helpers
{

    public static class New<T>
    {
        public static readonly Func<T> Instance = Creator();
        private static Func<T> Creator()
        {
            var t = typeof(T);
            try
            {
                if (t == typeof(string))
                    return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

                if (t.HasDefaultConstructor())
                    return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

                // Create an instance of the SomeType class that is defined in a non system assembly 
                // TODO :: MAYBE ENABLE FOR ONLY NET FRAMEWORK
                //    var oh = Activator.CreateInstanceFrom(Assembly.GetEntryAssembly().CodeBase, typeof(T).FullName);
                // Call an instance method defined by the SomeType type using this object.
                //      return Expression.Lambda<Func<T>>(Expression.Constant(oh.Unwrap())).Compile();

                var c = typeof(T).GetTypeInfo().DeclaredConstructors.Single(ci => ci.GetParameters().Length == 0);
                if (Type.EmptyTypes != null) return (Func<T>)c.Invoke(Type.EmptyTypes);

                return Activator.CreateInstance<Func<T>>();
            }
            catch (Exception)
            {
                return () => (T)FormatterServices.GetUninitializedObject(t);
            }
        }
    }


}
