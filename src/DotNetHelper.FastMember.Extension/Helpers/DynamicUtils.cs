using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace DotNetHelper.FastMember.Extension.Helpers
{
    internal static class DynamicUtils
    {
        internal static class BinderWrapper
        {


            public static CallSiteBinder GetMember(string name, Type context)
            {

                return Binder.GetMember(
                    CSharpBinderFlags.None, name, context, new[] {CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)});

            }

            public static CallSiteBinder SetMember(string name, Type context)
            {

                return Binder.SetMember(
                    CSharpBinderFlags.None, name, context, new[]
                                                               {
                                                                   CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                                                   CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
                                                               });

            }
        }

    }

    internal class NoThrowGetBinderMember : GetMemberBinder
    {
        private readonly GetMemberBinder _innerBinder;

        public NoThrowGetBinderMember(GetMemberBinder innerBinder)
            : base(innerBinder.Name, innerBinder.IgnoreCase)
        {
            _innerBinder = innerBinder;
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            var retMetaObject = _innerBinder.Bind(target, ArrayEmpty<DynamicMetaObject>());

            var noThrowVisitor = new NoThrowExpressionVisitor();
            var resultExpression = noThrowVisitor.Visit(retMetaObject.Expression);

            var finalMetaObject = new DynamicMetaObject(resultExpression, retMetaObject.Restrictions);
            return finalMetaObject;
        }

        public static T[] ArrayEmpty<T>()
        {
            var array = Enumerable.Empty<T>() as T[];
            Debug.Assert(array != null);
            // Defensively guard against a version of Linq where Enumerable.Empty<T> doesn't
            // return an array, but throw in debug versions so a better strategy can be
            // used if that ever happens.
#pragma warning disable CA1825 // Avoid zero-length array allocations.
            return array ?? new T[0];
#pragma warning restore CA1825 // Avoid zero-length array allocations.
        }
    }

    internal class NoThrowSetBinderMember : SetMemberBinder
    {
        private readonly SetMemberBinder _innerBinder;

        public NoThrowSetBinderMember(SetMemberBinder innerBinder)
            : base(innerBinder.Name, innerBinder.IgnoreCase)
        {
            _innerBinder = innerBinder;
        }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            var retMetaObject = _innerBinder.Bind(target, new DynamicMetaObject[] { value });

            var noThrowVisitor = new NoThrowExpressionVisitor();
            var resultExpression = noThrowVisitor.Visit(retMetaObject.Expression);

            var finalMetaObject = new DynamicMetaObject(resultExpression, retMetaObject.Restrictions);
            return finalMetaObject;
        }
    }

    internal class NoThrowExpressionVisitor : ExpressionVisitor
    {
        internal static readonly object ErrorResult = new object();

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            // if the result of a test is to throw an error, rewrite to result an error result value
            if (node.IfFalse.NodeType == ExpressionType.Throw)
            {
                return Expression.Condition(node.Test, node.IfTrue, Expression.Constant(ErrorResult));
            }

            return base.VisitConditional(node);
        }
    }
}
