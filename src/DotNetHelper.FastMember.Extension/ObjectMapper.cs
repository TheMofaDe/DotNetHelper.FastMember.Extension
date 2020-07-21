using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DotNetHelper.FastMember.Extension.Comparer;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Helpers;
using DotNetHelper.FastMember.Extension.Models;
namespace DotNetHelper.FastMember.Extension
{
    public static class ObjectMapper
    {
        /// <summary>
        /// return a dictionary of matching members between to class objects
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="exactTypeOnly"></param>
        /// <param name="comparer"></param>
        /// <param name="formatProviders"></param>
        /// <returns></returns>
        private static Tuple<Dictionary<MemberWrapper, MemberWrapper>> GetMatchingMembers<T1, T2>(bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> formatProviders = null) where T2 : class where T1 : class
        {
            var members1 = ExtFastMember.GetMemberWrappers<T1>(true);
            var members2 = ExtFastMember.GetMemberWrappers<T2>(true);


            var propertyMapping = new Dictionary<MemberWrapper, MemberWrapper>();
            members2.ForEach(delegate (MemberWrapper m2)
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
            return new Tuple<Dictionary<MemberWrapper, MemberWrapper>>(propertyMapping);
        }

        public static T2 Map<T1, T2>(T1 original, T2 copyCat, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture) where T1 : class where T2 : class
        {

            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer);
            var sameKids = tuple.Item1;

            sameKids.ForEach(delegate (KeyValuePair<MemberWrapper, MemberWrapper> pair)
            {
                pair.Value.SetMemberValue(copyCat, pair.Key.GetValue(original));
            });

            return copyCat;
        }
        public static T2 MapDontThrow<T1, T2>(T1 original, T2 copyCat, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null) where T1 : class where T2 : class
        {
            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer, beforeMappinFormatProviders);
            var sameKids = tuple.Item1;
            sameKids.ForEach(delegate (KeyValuePair<MemberWrapper, MemberWrapper> pair)
            {
                try
                {
                    pair.Value.SetMemberValue(copyCat, pair.Key.GetValue(original));
                }
                catch (Exception)
                {
                }
            });

            return copyCat;
        }


        public static T2 MapExcept<T1, T2>(T1 original, T2 copyCat, Expression<Func<T1, object>> excludeProperties = null, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null) where T1 : class where T2 : class
        {
            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer, beforeMappinFormatProviders);
            var sameKids = tuple.Item1;
            var list = excludeProperties.GetPropertyNamesFromExpression();
            var temp = sameKids.AsList();
            temp.RemoveAll(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer)));

            temp.ForEach(delegate (KeyValuePair<MemberWrapper, MemberWrapper> pair)
            {
                pair.Value.SetMemberValue(copyCat, pair.Key.GetValue(original));
            });

            return copyCat;
        }

        public static T2 MapExceptDontThrow<T1, T2>(T1 original, T2 copyCat, Expression<Func<T1, object>> excludeProperties = null, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null) where T1 : class where T2 : class
        {
            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer, beforeMappinFormatProviders);
            var sameKids = tuple.Item1;
            var list = excludeProperties.GetPropertyNamesFromExpression();
            var temp = sameKids.AsList();
            temp.RemoveAll(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer)));
            temp.ForEach(delegate (KeyValuePair<MemberWrapper, MemberWrapper> pair)
            {
                try
                {
                    pair.Value.SetMemberValue(copyCat, pair.Key.GetValue(original));
                }
                catch (Exception)
                {
                    //  throw new PropertyMapException($"Failed To Map Property {pair.Key.Name} : {pair.Key.Type.FullName} --> {pair.Value.Name} : {pair.Value.Type.FullName}",error);
                }
            });

            return copyCat;
        }

        public static T2 MapOnly<T1, T2>(T1 original, T2 copyCat, Expression<Func<T1, object>> includeProperties, bool exactTypeOnly = false, StringComparison comparer = StringComparison.CurrentCulture, IDictionary<Type, IFormatProvider> beforeMappinFormatProviders = null) where T1 : class where T2 : class
        {
            var tuple = GetMatchingMembers<T1, T2>(exactTypeOnly, comparer, beforeMappinFormatProviders);
            var list = includeProperties.GetPropertyNamesFromExpression();
            var sameKids = tuple.Item1.Where(m => list.Contains(m.Value.Name, new EqualityComparerString(comparer))).AsList();
            sameKids.ForEach(delegate (KeyValuePair<MemberWrapper, MemberWrapper> pair)
            {
                pair.Value.SetMemberValue(copyCat, pair.Key.GetValue(original));
            });

            return copyCat;
        }

    }
}
