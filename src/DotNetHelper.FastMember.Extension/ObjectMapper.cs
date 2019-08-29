using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DotNetHelper.FastMember.Extension.Comparer;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Helpers;
using FastMember;

namespace DotNetHelper.FastMember.Extension
{
    public static class ObjectMapper
    {
        private static Tuple<Dictionary<Member, Member>, TypeAccessor, TypeAccessor> GetMatchingMembers<T1, T2>(bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> formatProviders = null)
        {
            var accessor1 = TypeAccessor.Create(typeof(T1), true);
            var accessor2 = TypeAccessor.Create(typeof(T2), true);

            var members1 = accessor1.GetMembers().AsList();
            var members2 = accessor2.GetMembers().AsList();

            var propertyMapping = new Dictionary<Member, Member>();
            members2.ForEach(delegate (Member m2)
            {
                var master = members1.FirstOrDefault(m1 => string.Equals(m2.Name, m1.Name, comparer));
                if (string.IsNullOrEmpty(master?.Name)) return;
                if (exactTypeOnly)
                {
                    if (master.Type == m2.Type)
                        propertyMapping.Add(master, m2);
                }
                else
                {
                    propertyMapping.Add(master, m2);
                }

            });
            return new Tuple<Dictionary<Member, Member>, TypeAccessor, TypeAccessor>(propertyMapping, accessor1, accessor2);
        }




        private static object GetValue<T1>(KeyValuePair<Member, Member> pair, TypeAccessor accessor1, T1 original, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null)
        {

            if (!beforeMappinFormatProviders.IsNullOrEmpty())
            {
#if NETFRAMEWORK
                return Convert.ChangeType(accessor1[original, pair.Key.Name], pair.Value.Type, beforeMappinFormatProviders.GetValueOrDefault(pair.Key.Type));
#else
                return Convert.ChangeType(accessor1[original, pair.Key.Name], pair.Value.Type, beforeMappinFormatProviders.GetValueOrDefaultValue(pair.Key.Type));
#endif
            }
            else
            {
                if (!pair.Value.Type.IsNullable().isNullableT) // System.Convert Dont Handle Nullable<T> see link for reference https://stackoverflow.com/questions/3531318/convert-changetype-fails-on-nullable-types
                {
                    return accessor1[original, pair.Key.Name];
                }
                else
                {
                    return Convert.ChangeType(accessor1[original, pair.Key.Name], pair.Value.Type, null);
                }
            }
        }

        public static T2 MapProperties<T1, T2>(T1 original, T2 copyCat, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null)
        {

            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer);
            var sameKids = tuple.Item1;
            var accessor1 = tuple.Item2;
            var accessor2 = tuple.Item3;
            sameKids.ForEach(delegate (KeyValuePair<Member, Member> pair)
            {
                accessor2[copyCat, pair.Value.Name] = GetValue(pair, accessor1, original, beforeMappinFormatProviders);
            });

            return copyCat;
        }
        public static T2 MapPropertiesDontThrow<T1, T2>(T1 original, T2 copyCat, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null)
        {

            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer);
            var sameKids = tuple.Item1;
            var accessor1 = tuple.Item2;
            var accessor2 = tuple.Item3;
            sameKids.ForEach(delegate (KeyValuePair<Member, Member> pair)
            {
                try
                {
                    accessor2[copyCat, pair.Value.Name] = GetValue(pair, accessor1, original, beforeMappinFormatProviders);
                }
                catch (Exception)
                {
                }
            });

            return copyCat;
        }


        public static T2 MapExcept<T1, T2>(T1 original, T2 copyCat, Expression<Func<T1, object>> excludeProperties = null, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null)
        {
            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer);
            var sameKids = tuple.Item1;
            var list = excludeProperties.GetPropertyNamesFromExpression();
            var temp = sameKids.AsList();
            temp.RemoveAll(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer)));
            var accessor1 = tuple.Item2;
            var accessor2 = tuple.Item3;
            temp.ForEach(delegate (KeyValuePair<Member, Member> pair)
            {
                accessor2[copyCat, pair.Value.Name] = GetValue(pair, accessor1, original, beforeMappinFormatProviders);
            });

            return copyCat;
        }

        public static T2 MapExceptDontThrow<T1, T2>(T1 original, T2 copyCat, Expression<Func<T1, object>> excludeProperties = null, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null)
        {
            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer);
            var sameKids = tuple.Item1;
            var list = excludeProperties.GetPropertyNamesFromExpression();
            var temp = sameKids.AsList();
            temp.RemoveAll(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer)));
            var accessor1 = tuple.Item2;
            var accessor2 = tuple.Item3;
            temp.ForEach(delegate (KeyValuePair<Member, Member> pair)
            {
                try
                {
                    accessor2[copyCat, pair.Value.Name] = GetValue(pair, accessor1, original, beforeMappinFormatProviders);
                }
                catch (Exception)
                {
                    //  throw new PropertyMapException($"Failed To Map Property {pair.Key.Name} : {pair.Key.Type.FullName} --> {pair.Value.Name} : {pair.Value.Type.FullName}",error);
                }
            });

            return copyCat;
        }

        public static T2 MapOnly<T1, T2>(T1 original, T2 copyCat, Expression<Func<T1, object>> includeProperties = null, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null)
        {
            var tuple = GetMatchingMembers<T1, T2>();
            var list = includeProperties.GetPropertyNamesFromExpression();
            var sameKids = tuple.Item1.Where(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer))).AsList();
            var temp = sameKids.AsList();
            temp.RemoveAll(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer)));
            var accessor1 = tuple.Item2;
            var accessor2 = tuple.Item3;
            temp.ForEach(delegate (KeyValuePair<Member, Member> pair)
            {
                accessor2[copyCat, pair.Value.Name] = GetValue(pair, accessor1, original, beforeMappinFormatProviders);
            });

            return copyCat;
        }

    }
}
