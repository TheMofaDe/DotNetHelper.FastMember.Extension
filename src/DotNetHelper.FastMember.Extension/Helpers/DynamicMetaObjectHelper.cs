using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DotNetHelper.FastMember.Extension.Extension;

namespace DotNetHelper.FastMember.Extension.Helpers
{
    public class DynamicObjectHelper
    {
        private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>> _callSiteGetters =
            new ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>>(CreateCallSiteGetter);

        private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>> _callSiteSetters =
            new ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>>(CreateCallSiteSetter);



        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMetaObjectHelper"/> class.
        /// </summary>
        public DynamicObjectHelper()
        {

        }

        private static CallSite<Func<CallSite, object, object>> CreateCallSiteGetter(string name)
        {
            var getMemberBinder = (GetMemberBinder)DynamicUtils.BinderWrapper.GetMember(name, typeof(DynamicUtils));
            return CallSite<Func<CallSite, object, object>>.Create(new NoThrowGetBinderMember(getMemberBinder));
        }

        private static CallSite<Func<CallSite, object, object, object>> CreateCallSiteSetter(string name)
        {
            var binder = (SetMemberBinder)DynamicUtils.BinderWrapper.SetMember(name, typeof(DynamicUtils));
            return CallSite<Func<CallSite, object, object, object>>.Create(new NoThrowSetBinderMember(binder));
        }


        public List<string> GetDynamicMemberNames(IDynamicMetaObjectProvider dynamicObject) 
        {
            dynamicObject.IsNullThrow(nameof(dynamicObject));
            return dynamicObject.GetMetaObject(Expression.Constant(dynamicObject)).GetDynamicMemberNames().AsList();
        }

        public Dictionary<string,object> GetDynamicMemberNameAndValues(IDynamicMetaObjectProvider dynamicObject)
        {
            dynamicObject.IsNullThrow(nameof(dynamicObject));
            var dictionary = new Dictionary<string,object>();
            var names = dynamicObject.GetMetaObject(Expression.Constant(dynamicObject)).GetDynamicMemberNames();
            foreach (var name in names)
            {
                if(TryGetMember(dynamicObject,name, out var value))
                 dictionary.Add(name,value);
            }
            return dictionary;
        }

        public bool TryGetMember(IDynamicMetaObjectProvider dynamicProvider, string name, out object value)
        {
            dynamicProvider.IsNullThrow(nameof(dynamicProvider));
            var callSite = _callSiteGetters.Get(name);

            var result = callSite.Target(callSite, dynamicProvider);

            if (!ReferenceEquals(result, NoThrowExpressionVisitor.ErrorResult))
            {
                value = result;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
        {
            dynamicProvider.IsNullThrow(nameof(dynamicProvider));

            var callSite = _callSiteSetters.Get(name);

            var result = callSite.Target(callSite, dynamicProvider, value);

            return !ReferenceEquals(result, NoThrowExpressionVisitor.ErrorResult);
        }
    }

}
