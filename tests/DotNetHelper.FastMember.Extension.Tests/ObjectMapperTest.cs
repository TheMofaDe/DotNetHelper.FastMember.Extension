using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DotNetHelper.FastMember.Extension.Tests
{



    public class StringModel
    {
        public string DateTime { get; set; }
        public string Guid { get; set; }
        public string TimeSpan { get; set; }
        public string Number { get; set; }
    }
    public class TypeModel
    {
        public DateTime DateTime { get; set; }
        public Guid Guid { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public int Number { get; set; }
    }

    [TestFixture]
    public class ObjectMapperTest
    {
        

        //public TypeModel B { get; private set; } = new TypeModel() { };

        [SetUp]
        public void Setup()
        {
            //B = new TypeModel()
            //{
            //    TimeSpan = DateTime.Today.TimeOfDay,
            //    DateTime = DateTime.Today,
            //    Guid = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E"),
            //    Number = 1
            //};
          
        }


        [Test]
        public void Test_Map_Only_Will_Only_Map_Single_Property()
        {
               var value = "1";
                var obj = new {Number = value , TimeSpan = TimeSpan.FromHours(1)};
                var B = new TypeModel();
                ObjectMapper.MapOnly(obj, B, m => m.Number, false, StringComparison.OrdinalIgnoreCase);
                Assert.AreEqual(B.TimeSpan,default(TimeSpan));
                Assert.AreEqual(B.DateTime, default(DateTime));
                Assert.AreEqual(B.Guid, default(Guid));
                Assert.AreEqual(value,B.Number.ToString());
        }


        [Test]
        public void Test_Map_String_To_DateTime()
        {
             var value = DateTime.Now.ToString();
              var obj = new { DateTime = value };
              var B = new TypeModel();
            ObjectMapper.MapOnly(obj, B, m => m.DateTime, false, StringComparison.OrdinalIgnoreCase);
              Assert.AreEqual(value, B.DateTime.ToString()); ;

        }

        [Test]
        public void Test_Map_String_To_TimeSpan()
        {
            var value = DateTime.Now.TimeOfDay.ToString();
            var obj = new { TimeSpan = value };
            var B = new TypeModel();
            ObjectMapper.MapOnly(obj, B, m => m.TimeSpan, false, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(obj.TimeSpan, B.TimeSpan.ToString());
        }

        [Test]
        public void Test_Map_String_To_Guid()
        {
            var value = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E").ToString();
            var obj = new { Guid = value };
            var B = new TypeModel();
            ObjectMapper.MapOnly(obj, B, m => m.Guid, false, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(value, B.Guid.ToString());
        }



        [Test]
        public void Test_Map_DateTime_To_String()
        {
            var value = DateTime.Now;
            var obj = new { DateTime = value };
            var B = new StringModel();
            ObjectMapper.MapOnly(obj, B, m => m.DateTime, false, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(value.ToString(), B.DateTime); ;

        }

        [Test]
        public void Test_Map_TimeSpan_To_String()
        {
            var value = DateTime.Now.TimeOfDay;
            var obj = new { TimeSpan = value };
            var B = new StringModel();
            ObjectMapper.MapOnly(obj, B, m => m.TimeSpan, false, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(value.ToString(), B.TimeSpan);
        }

        [Test]
        public void Test_Map_Guid_To_String()
        {
            var value = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E");
            var obj = new { Guid = value };
            var B = new StringModel();
            ObjectMapper.MapOnly(obj, B, m => m.Guid, false, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(value.ToString(), B.Guid);
        }



        [Test]
        public void Test_Map_Properties()
        {
            var obj = new { Number = 1, TimeSpan = TimeSpan.FromHours(1), DateTime = DateTime.Now,  Guid = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E") };
            var B = new TypeModel();
            ObjectMapper.MapProperties(obj, B, false, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(B.TimeSpan, obj.TimeSpan);
            Assert.AreEqual(B.DateTime, obj.DateTime);
            Assert.AreEqual(B.Guid, obj.Guid);
            Assert.AreEqual(B.Number, obj.Number);
        }



    }
}
