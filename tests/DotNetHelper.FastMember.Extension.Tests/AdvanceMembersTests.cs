using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DotNetHelper.FastMember.Extension;
using DotNetHelper.FastMember.Extension.Models;
using DotNetHelper.FastMember.Extension.Tests;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_AccessingPrivateMembers()
        {
            // FAST-MEMBER DOESN'T SUPPORT ACCESSING NON-PUBLIC PROPERTIES
            var members = ExtFastMember.GetAdvanceMembers<PrivatePropertiesModel>(true);
            Assert.AreEqual(0,members.Count);
        }



        [Test]
        public void Test_AccessingPublicMembers()
        {
            var members = ExtFastMember.GetAdvanceMembers<PublicPropertiesModel>(true);
            Assert.AreEqual(2, members.Count);
        }






        [Test]
        public void Test_GettingMemberValueFromObject()
        {
            var obj = new PublicPropertiesNoAccessor()
            {
                FalseNullableBoolean = false
                ,IsPublicProperty = true
                ,NullBoolean = null
            };
            var members = ExtFastMember.GetAdvanceMembers<PublicPropertiesNoAccessor>();
            foreach(var member in members)
            {
                if (member.Member.Name == "FalseNullableBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Member.Name == "IsPublicProperty")
                {
                    Assert.AreEqual(member.GetValue(obj), true);
                }
                if (member.Member.Name == "NullBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), null);
                }
            }
        }

        [Test]
        public void Test_SetttingMemberValueFromObject()
        {
            var obj = new PublicPropertiesNoAccessor()
            {
                FalseNullableBoolean = false
                ,
                IsPublicProperty = true
                ,
                NullBoolean = null
            };
            var members = ExtFastMember.GetAdvanceMembers<PublicPropertiesNoAccessor>();
            foreach (var member in members)
            {
                if (member.Member.Name == "FalseNullableBoolean")
                {
                    member.SetMemberValue(obj,null);
                    Assert.AreEqual(member.GetValue(obj), null);
                }
                if (member.Member.Name == "IsPublicProperty")
                {
                    member.SetMemberValue(obj, false);
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Member.Name == "NullBoolean")
                {
                    member.SetMemberValue(obj, true);
                    Assert.AreEqual(member.GetValue(obj),true);
                }
            }
        }



        [Test]
        public void Test_MultiThreadingLookup()
        {
            List<int> ret = new List<int>(50);
            ret.AddRange(Enumerable.Repeat(8, 50));

            Parallel.ForEach(ret, delegate(int i, ParallelLoopState state)
            {
                ExtFastMember.GetAdvanceMembers(typeof(PublicPropertiesModel));
                ExtFastMember.GetAdvanceMembers(typeof(PublicPropertiesNoAccessor));
            });
        }































        [Test]
        public void Test_AccessingMembersOfDynamicObjects()
        {
            dynamic dyn = new ExpandoObject();
            dyn.PropNumber1 = 1;
            dyn.PropNumber2 = 2;
            var members = ExtFastMember.GetDynamicAdvanceMembers(dyn);
            Assert.AreEqual(2, members.Count);
        }





        [Test]
        public void Test_GettingMemberValueFromDynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.FalseNullableBoolean = false;
            obj.IsPublicProperty = true;
            obj.NullBoolean = null;
             
            var members = ExtFastMember.GetDynamicAdvanceMembers(obj);
            
            foreach (DynamicAdvanceMember member in members)
            {
                if (member.Name == "FalseNullableBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Name == "IsPublicProperty")
                {
                    Assert.AreEqual(member.GetValue(obj), true);
                }
                if (member.Name == "NullBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), null);
                }
            }
        }

        [Test]
        public void Test_SetttingMemberValueFromDynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.FalseNullableBoolean = false;
            obj.IsPublicProperty = true;
            obj.NullBoolean = null;
            var members = ExtFastMember.GetDynamicAdvanceMembers(obj);
            foreach (DynamicAdvanceMember member in members)
            {
                if (member.Name == "FalseNullableBoolean")
                {
                    member.SetMemberValue(obj, null);
                    Assert.AreEqual(member.GetValue(obj), null);
                }
                if (member.Name == "IsPublicProperty")
                {
                    member.SetMemberValue(obj, false);
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Name == "NullBoolean")
                {
                    member.SetMemberValue(obj, true);
                    Assert.AreEqual(member.GetValue(obj), true);
                }
            }
        }

    }
}