using NUnit.Framework;
using Storm.Attributes;
using Storm.Serializers;
using System;
using System.Collections.Generic;

namespace Storm.Tests
{
    public class Tests
    {
        [Test]
        public void SerializationTest()
        {
            var settings = StormSettings.Default();

            var random = new Random();
            var original = new TestObject
            {
                bool_value = true,
                uint16_value = (ushort)random.Next(short.MinValue, short.MaxValue),
                int32_value = random.Next(int.MinValue, int.MaxValue),
                uint64_value = (ulong)random.Next(int.MinValue, int.MaxValue),
                int32_array = new int[]
                {
                    123, 13215, 5, 1345, 151, 24, 5, 12, 34123, 4, 1234
                },
                null_string_value = null,
                single_string_value = "sadassjghadsfklahgjklahsd",
                multi_string_value = "ad ad ad a d\nasd asd askfaj dsf \n asd sad ",
                magic_enum_as_int = TestObject.MagicEnum.Three,
                magic_enum_as_str = TestObject.MagicEnum.Five,
            };

            var serializer = new StormSerializer();
            var serializeTask = serializer.SerializeAsync(original, settings);
            serializeTask.Wait();
            if (serializeTask.IsFaulted)
                throw serializeTask.Exception;

            var storm = serializeTask.Result;

            var deserializeTask = serializer.DeserializeAsync<TestObject>(storm, settings);
            deserializeTask.Wait();
            if (deserializeTask.IsFaulted)
                throw deserializeTask.Exception;

            var deserialized = deserializeTask.Result;
            Assert.NotNull(deserialized);
            Assert.AreEqual(original.bool_value, deserialized.bool_value);
            Assert.AreEqual(original.sbyte_value_func(), deserialized.sbyte_value_func());
            Assert.AreEqual(original.byte_value, deserialized.byte_value);
            Assert.AreEqual(original.int16_value_func(), deserialized.int16_value_func());
            Assert.AreEqual(original.int32_value, deserialized.int32_value);
            Assert.AreEqual(original.int64_value_func(), deserialized.int64_value_func());
            Assert.AreEqual(original.uint16_value, deserialized.uint16_value);
            Assert.AreEqual(original.uint32_value_func(), deserialized.uint32_value_func());
            Assert.AreEqual(original.uint64_value, deserialized.uint64_value);
            Assert.AreEqual(original.float_value_func(), deserialized.float_value_func());
            Assert.AreEqual(original.double_value, deserialized.double_value);
            Assert.AreEqual(original.decimal_value_func(), deserialized.decimal_value_func());
            Assert.AreEqual(original.int32_array, deserialized.int32_array);
            Assert.AreEqual(original.single_string_value, deserialized.single_string_value);
            Assert.AreEqual(original.multi_string_value, deserialized.multi_string_value);
            Assert.AreEqual(original.magic_enum_as_str, original.magic_enum_as_str);
            Assert.AreEqual(original.magic_enum_as_int, original.magic_enum_as_int);
        }

        [Test]
        public void DeserializationTest()
        {
            var settings = new StormSettings
            (
                options:            StormSettings.Options.IgnoreCase,
                converters:         new List<IStormConverter>
                                    {
                                        new UrlStormConverter(),
                                    },
                encoding:           System.Text.Encoding.UTF8,
                defaultEnumFormat:  StormEnumFormat.String,
                intentSize:         2
            );

            var serializer = new StormSerializer();
            var task = serializer.DeserializeFileAsync("Examples/test-file.storm", settings);
            task.Wait();
            if (task.IsFaulted)
                throw task.Exception;

            var testStorm = task.Result;

            var testObj = testStorm.Populate<TestObject>(settings);
            AreEqual(testObj.bool_value, testStorm[nameof(testObj.bool_value)]);
            AreEqual(testObj.sbyte_value_func(), testStorm["sbyte_value"]);
            AreEqual(testObj.byte_value, testStorm[nameof(testObj.byte_value)]);
            AreEqual(testObj.int16_value_func(), testStorm["int16_value"]);
            AreEqual(testObj.int32_value, testStorm[nameof(testObj.int32_value)]);
            AreEqual(testObj.int64_value_func(), testStorm["int64_value"]);
            AreEqual(testObj.uint16_value, testStorm[nameof(testObj.uint16_value)]);
            AreEqual(testObj.uint32_value_func(), testStorm["uint32_value"]);
            AreEqual(testObj.uint64_value, testStorm[nameof(testObj.uint64_value)]);
            AreEqual(testObj.float_value_func(), testStorm["float_value"]);
            AreEqual(testObj.double_value, testStorm[nameof(testObj.double_value)]);
            AreEqual(testObj.decimal_value_func(), testStorm["decimal_value"]);
            AreEqual(testObj.single_string_value, testStorm[nameof(testObj.single_string_value)]);
            AreEqual(testObj.multi_string_value, testStorm[nameof(testObj.multi_string_value)]);
            Assert.AreEqual(testObj.int_to_ignore, 0);

            var innerObj = testObj.inner_object;
            var innerStorm = testStorm[nameof(testObj.inner_object)] as StormObject;
            AreEqual(innerObj.bool_value, innerStorm[nameof(innerObj.bool_value)]);
            AreEqual(innerObj.int_value, innerStorm[nameof(innerObj.int_value)]);
            AreEqual(innerObj.float_value, innerStorm[nameof(innerObj.float_value)]);

            var internalObj = innerObj.internal_object;
            var internalStorm = innerStorm[nameof(innerObj.internal_object)] as StormObject;
            AreEqual(internalObj.int32_value, internalStorm[nameof(internalObj.int32_value)]);

            var externalObj = testObj.external_object;
            var externalStorm = testStorm[nameof(testObj.external_object)] as StormObject;
            AreEqual(externalObj.int16_value, externalStorm[nameof(externalObj.int16_value)]);
            AreEqual(externalObj.int32_value, externalStorm[nameof(externalObj.int32_value)]);
            AreEqual(externalObj.int64_value, externalStorm[nameof(externalObj.int64_value)]);
        }

        private static void AreEqual(object actual, IStormValue expected)
        {
            if (expected is StormValue expectedValue)
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

            public string null_string_value;
            public string single_string_value;
            public string multi_string_value;

            public int[] int32_array;

            public InnerObject inner_object;
            public ExternalObject external_object;

            [StormEnum(format = StormEnumFormat.String)]
            public MagicEnum magic_enum_as_str;
            [StormEnum(format = StormEnumFormat.Value)]
            public MagicEnum magic_enum_as_int;

            [StormIgnore]
            public int int_to_ignore;

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