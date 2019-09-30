using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DotNetHelper.FastMember.Extension.Models;
using DotNetHelper.FastMember.Extension.Tests.Models;
using NUnit.Framework;

namespace DotNetHelper.FastMember.Extension.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_Setting_List_Property()
        {
            var list = new List<int>() { 1, 2, 3, 4 };
            var employee = new Employee();
            ExtFastMember.SetMemberValue(employee, "ListOfNumbers", list);
            Assert.AreEqual(list, employee.ListOfNumbers);
        }

        [Test]
        public void Test_AccessingPrivateMembers()
        {
            // FAST-MEMBER DOESN'T SUPPORT ACCESSING NON-PUBLIC PROPERTIES
            var members = ExtFastMember.GetMemberWrappers<PrivatePropertiesModel>(true);
            Assert.AreEqual(0, members.Count);
        }



        [Test]
        public void Test_AccessingPublicMembers()
        {
            var members = ExtFastMember.GetMemberWrappers<PublicPropertiesModel>(true);
            Assert.AreEqual(2, members.Count);
        }


        [Test]
        public void Test_SettingNullablePropertiesValue_To_Null()
        {

            var obj = new NullableFields();
            ExtFastMember.SetMemberValue(obj, "Decimal", null);
            ExtFastMember.SetMemberValue(obj, "DateTime", null);
            ExtFastMember.SetMemberValue(obj, "DateTimeOffset", null);
            ExtFastMember.SetMemberValue(obj, "Object", null);

            ExtFastMember.SetMemberValue(obj, "Decimal", DBNull.Value);
            ExtFastMember.SetMemberValue(obj, "DateTime", DBNull.Value);
            ExtFastMember.SetMemberValue(obj, "DateTimeOffset", DBNull.Value);
            ExtFastMember.SetMemberValue(obj, "Object", DBNull.Value);
        }

        [Test]
        public void Test_SettingNullablePropertiesValue()
        {

            var obj = new NullableFields();
            ExtFastMember.SetMemberValue(obj, "Decimal", 2);
            ExtFastMember.SetMemberValue(obj, "DateTime", DateTime.Now);
            ExtFastMember.SetMemberValue(obj, "DateTimeOffset", DateTimeOffset.Now);
            ExtFastMember.SetMemberValue(obj, "Object", "dsf");

        }





        [Test]
        public void Test_GettingMemberValueFromObject()
        {
            var obj = new PublicPropertiesNoAccessor()
            {
                FalseNullableBoolean = false
                ,
                IsPublicProperty = true
                ,
                NullBoolean = null
            };
            var members = ExtFastMember.GetMemberWrappers<PublicPropertiesNoAccessor>(true);
            foreach (var member in members)
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
        public void Test_SettingMemberValueFromObject()
        {
            var obj = new PublicPropertiesNoAccessor()
            {
                FalseNullableBoolean = false
                ,
                IsPublicProperty = true
                ,
                NullBoolean = null
            };
            var members = ExtFastMember.GetMemberWrappers<PublicPropertiesNoAccessor>(true);


            foreach (var member in members)
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







        [Test]
        public void Test_MultiThreadingLookup()
        {
            List<int> ret = new List<int>(50);
            ret.AddRange(Enumerable.Repeat(8, 50));

            Parallel.ForEach(ret, delegate (int i, ParallelLoopState state)
            {
                ExtFastMember.GetMemberWrappers(typeof(PublicPropertiesModel), true);
                ExtFastMember.GetMemberWrappers(typeof(PublicPropertiesNoAccessor), true);
            });
        }



        [Test]
        public void Test_SetMemberValue_Throws_Exception_On_Non_Settable_Members()
        {

            var obj = new GenericModelWithGetOnlyProperties()
            {
            };
            var members = ExtFastMember.GetMemberWrappers<PublicPropertiesNoAccessor>(true);


            foreach (var member in members)
            {
                if (member.Name == "IsPublicProperty")
                {
                    Assert.That(() => member.SetMemberValue(obj, false),
                        Throws.Exception
                            .TypeOf<InvalidOperationException>()
                            .With.InnerException.TypeOf<ArgumentOutOfRangeException>());

                }

            }
        }































        [Test]
        public void Test_AccessingMembersOfDynamicObjects()
        {
            dynamic dyn = new ExpandoObject();
            dyn.PropNumber1 = 1;
            dyn.PropNumber2 = 2;
            var members = ExtFastMember.GetMemberWrappers(dyn);
            Assert.AreEqual(2, members.Count);
        }



        #region  DYNAMICS OBJECT TESTS



        [Test]
        public void Test_GettingMemberValueFromDynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.FalseNullableBoolean = false;
            obj.IsPublicProperty = true;
            obj.NullBoolean = null;

            var members = ExtFastMember.GetMemberWrappers(obj);

            foreach (MemberWrapper member in members)
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
        public void Test_Set_Member_Value_From_Dynamic_Member_Level()
        {
            dynamic obj = new ExpandoObject();
            obj.FalseNullableBoolean = false;
            obj.IsPublicProperty = true;
            obj.NullBoolean = null;
            var members = ExtFastMember.GetMemberWrappers(obj);

            foreach (MemberWrapper member in members)
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


        [Test]
        public void Test_Set_Member_Value_From_Dynamic_Object_Level()
        {
            dynamic obj = new ExpandoObject();
            obj.FalseNullableBoolean = false;
            obj.IsPublicProperty = true;
            obj.NullBoolean = null;
            var members = ExtFastMember.GetMemberWrappers(obj);

            ExtFastMember.SetMemberValue(obj, "FalseNullableBoolean", null);
            ExtFastMember.SetMemberValue(obj, "IsPublicProperty", false);
            ExtFastMember.SetMemberValue(obj, "NullBoolean", true);
            foreach (MemberWrapper member in members)
            {
                if (member.Name == "FalseNullableBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), null);
                }
                if (member.Name == "IsPublicProperty")
                {
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Name == "NullBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), true);
                }
            }
        }


        [Test]
        public void Test_Set_Member_Value_From_Dynamic_Object_Level_When_Member_Doesnt_Exist()
        {
            dynamic obj = new ExpandoObject();

            var members = ExtFastMember.GetMemberWrappers(obj);

            ExtFastMember.SetMemberValue(obj, "FalseNullableBoolean", null);
            ExtFastMember.SetMemberValue(obj, "IsPublicProperty", false);
            ExtFastMember.SetMemberValue(obj, "NullBoolean", true);
            foreach (MemberWrapper member in members)
            {
                if (member.Name == "FalseNullableBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), null);
                }
                if (member.Name == "IsPublicProperty")
                {
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Name == "NullBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), true);
                }
            }
        }


        [Test]
        public void Test_Set_Member_Value_From_Dynamic_Object_Level_With_Property_Not_Existing()
        {
            dynamic obj = new ExpandoObject();
            ExtFastMember.SetMemberValue(obj, "FalseNullableBoolean", null);
            ExtFastMember.SetMemberValue(obj, "IsPublicProperty", false);
            ExtFastMember.SetMemberValue(obj, "NullBoolean", true);
            var members = ExtFastMember.GetMemberWrappers(obj);
            foreach (MemberWrapper member in members)
            {
                if (member.Name == "FalseNullableBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), null);
                }
                if (member.Name == "IsPublicProperty")
                {
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Name == "NullBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), true);
                }
            }
        }
        #endregion


        #region  ANONYMOUS OBJECT TESTS



        [Test]
        public void Test_GettingMemberValueFrom_Anonymous_Object()
        {
            var obj = new
            {
                FalseNullableBoolean = false
                ,
                IsPublicProperty = true
            };
            var members = ExtFastMember.GetMemberWrappers(obj.GetType(), true);

            foreach (MemberWrapper member in members)
            {
                if (member.Name == "FalseNullableBoolean")
                {
                    Assert.AreEqual(member.GetValue(obj), false);
                }
                if (member.Name == "IsPublicProperty")
                {
                    Assert.AreEqual(member.GetValue(obj), true);
                }
            }
        }

        //[Test]
        //public void Test_Set_Member_Value_From_Anonymous_Member_Level()
        //{
        //    var obj = new
        //    {
        //        FalseNullableBoolean = false
        //        ,
        //        IsPublicProperty = true
        //        ,
        //        StringMe = "sdfsdf"
        //    };
        //    var members = ExtFastMember.GetMemberWrappers(obj.GetType(), true);

        //    foreach (MemberWrapper member in members)
        //    {
        //        if (member.Name == "StringMe")
        //        {
        //            member.SetMemberValue(obj, null);
        //            Assert.AreEqual(member.GetValue(obj), null);
        //        }
        //        if (member.Name == "IsPublicProperty")
        //        {
        //            member.SetMemberValue(obj, false);
        //            Assert.AreEqual(member.GetValue(obj), false);
        //        }
        //    }
        //}


        //[Test]
        //public void Test_Set_Member_Value_From_Anonymous_Object_Level()
        //{
        //    var obj = new
        //    {
        //        FalseNullableBoolean = false
        //        ,IsPublicProperty = true
        //    };


        //    ExtFastMember.SetMemberValue(obj, "FalseNullableBoolean", null);
        //    ExtFastMember.SetMemberValue(obj, "IsPublicProperty", false);
        //    ExtFastMember.SetMemberValue(obj, "NullBoolean", true);
        //    var members = ExtFastMember.GetMemberWrappers(obj.GetType(), true);
        //    foreach (MemberWrapper member in members)
        //    {
        //        if (member.Name == "FalseNullableBoolean")
        //        {
        //            Assert.AreEqual(member.GetValue(obj), null);
        //        }
        //        if (member.Name == "IsPublicProperty")
        //        {
        //            Assert.AreEqual(member.GetValue(obj), false);
        //        }
        //    }
        //}



        #endregion




        [Test]
        public void Test_Setting_String_Value_To_Other_Types()
        {
            var obj = new StringValueModel() { };
            var members = ExtFastMember.GetMemberWrappers<StringValueModel>(true);


            foreach (var member in members)
            {
                if (member.Name == "DateTimeOffset")
                {
                    member.SetMemberValue(obj, "2017-05-30");
                    Assert.AreEqual(member.GetValue(obj), DateTimeOffset.Parse("2017-05-30"));
                }
                if (member.Name == "Guid")
                {
                    member.SetMemberValue(obj, "a19ed8e6-c455-4164-afac-d4043095a4ee");
                    Assert.AreEqual(member.GetValue(obj), Guid.Parse("a19ed8e6-c455-4164-afac-d4043095a4ee"));
                }
                if (member.Name == "TimeSpan")
                {
                    member.SetMemberValue(obj, "01:00:00");
                    Assert.AreEqual(member.GetValue(obj), TimeSpan.Parse("01:00:00"));
                }

            }
        }

    }
}