using NUnit.Framework;
using Ston.Serializers;
using System.Collections.Generic;

namespace Ston.Tests
{
    public class Tests
    {
        [Test]
        public void DeserializationTest()
        {
            var settings = new StonSettings
            (
                options: StonSettings.Options.IgnoreCase,
                converters: new List<IStonConverter>
                {
                    new UrlStonConverter(),
                },
                encoding: System.Text.Encoding.UTF8
            );

            var ston = new StonSerializer();
            var task = ston.DeserializeFileAsync("Examples/test-file.ston", settings);
            task.Wait();
            if (task.IsFaulted)
                throw task.Exception;

            var testSton = task.Result;

            var testObj = testSton.Populate<TestObject>(settings);
            AreEqual(testObj.bool_value, testSton[nameof(testObj.bool_value)]);
            AreEqual(testObj.sbyte_value_func(), testSton["sbyte_value"]);
            AreEqual(testObj.byte_value, testSton[nameof(testObj.byte_value)]);
            AreEqual(testObj.int16_value_func(), testSton["int16_value"]);
            AreEqual(testObj.int32_value, testSton[nameof(testObj.int32_value)]);
            AreEqual(testObj.int64_value_func(), testSton["int64_value"]);
            AreEqual(testObj.uint16_value, testSton[nameof(testObj.uint16_value)]);
            AreEqual(testObj.uint32_value_func(), testSton["uint32_value"]);
            AreEqual(testObj.uint64_value, testSton[nameof(testObj.uint64_value)]);
            AreEqual(testObj.float_value_func(), testSton["float_value"]);
            AreEqual(testObj.double_value, testSton[nameof(testObj.double_value)]);
            AreEqual(testObj.decimal_value_func(), testSton["decimal_value"]);
            AreEqual(testObj.string_value, testSton[nameof(testObj.string_value)]);
            AreEqual(testObj.multi_string_value, testSton[nameof(testObj.multi_string_value)]);

            var innerObj = testObj.inner_object;
            var innerSton = testSton[nameof(testObj.inner_object)] as StonObject;
            AreEqual(innerObj.bool_value, innerSton[nameof(innerObj.bool_value)]);
            AreEqual(innerObj.int_value, innerSton[nameof(innerObj.int_value)]);
            AreEqual(innerObj.float_value, innerSton[nameof(innerObj.float_value)]);

            var internalObj = innerObj.internal_object;
            var internalSton = innerSton[nameof(innerObj.internal_object)] as StonObject;
            AreEqual(internalObj.int32_value, internalSton[nameof(internalObj.int32_value)]);

            var externalObj = testObj.external_object;
            var externalSton = testSton[nameof(testObj.external_object)] as StonObject;
            AreEqual(externalObj.int16_value, externalSton[nameof(externalObj.int16_value)]);
            AreEqual(externalObj.int32_value, externalSton[nameof(externalObj.int32_value)]);
            AreEqual(externalObj.int64_value, externalSton[nameof(externalObj.int64_value)]);
        }

        private static void AreEqual(object actual, IStonValue expected)
        {
            if (expected is StonValue expectedValue)
            {
                Assert.AreEqual(actual, expectedValue.GetValue());
            }
        }

        public class TestObject
        {
            public bool bool_value;

            private sbyte sbyte_value;
            public sbyte sbyte_value_func() => sbyte_value;

            public byte byte_value;

            private short int16_value;
            public short int16_value_func() => int16_value;

            public int int32_value;

            private long int64_value;
            public long int64_value_func() => int64_value;

            public ushort uint16_value;

            private uint uint32_value;
            public uint uint32_value_func() => uint32_value;

            public ulong uint64_value;

            private float float_value;
            public float float_value_func() => float_value;

            public double double_value;

            private decimal decimal_value;
            public decimal decimal_value_func() => decimal_value;

            public string string_value;
            public string multi_string_value;

            public int[] int32_array;

            public InnerObject inner_object;
            public ExternalObject external_object;

            public MagicEnum magic_enum_as_str;
            public MagicEnum magic_enum_as_int;

            public enum MagicEnum
            {
                One,
                Two,
                Three = 3,
                Five = 5,
            }

            public class InnerObject
            {
                public int int_value;
                public float float_value;
                public bool bool_value;
                public InternalObject internal_object;

                public class InternalObject
                {
                    public int int32_value;
                }
            }

            public class ExternalObject
            {
                public short int16_value;
                public int int32_value;
                public long int64_value;
                public InnerValue inner_value;
                public string[] str_array;

                public class InnerValue
                {
                    public short int16_value;
                    public int int32_value;
                    public long int64_value;
                    public ArrayElement[] obj_array;

                    public class ArrayElement
                    {
                        public bool bool_value;
                        public decimal dec_value;

                        public ArrayElement()
                        {
                        }
                    }
                }
            }
        }
    }
}