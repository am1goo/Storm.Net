using NUnit.Framework;
using Storm.Attributes;
using Storm.Converters;
using Storm.Serializers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
                byte_value = (byte)random.Next(0, byte.MaxValue),
                uint16_value = (ushort)random.Next(short.MinValue, short.MaxValue),
                int32_value = random.Next(int.MinValue, int.MaxValue),
                uint64_value = (ulong)random.Next(int.MinValue, int.MaxValue),
                double_value = random.NextDouble(),
                int32_array = new int[]
                {
                    123, 13215, 5, 1345, 151, 24, 5, 12, 34123, 4, 1234
                },
                uint64_list = new List<ulong>()
                {
                    1234, 3210, 2134, 21341, 34123, 8943891268932
                },
                int32_linked_list = new LinkedList<int>(new int[]
                {
                    10, 20, 30, 40, 50, 60, 70, 80, 90 
                }),
                byte_hash_set = new HashSet<byte>()
                {
                    1, 2, 3, 4, 5, 6, 7, 128, 255
                },
                double_sorted_set = new SortedSet<double>(new double[]
                {
                    21354231, 2135, 123, 1234, 2346, 67543, 42321,4, 6, 4653456543,
                }),
                ushort_queue = new Queue<ushort>(new ushort[]
                {
                    123, 2351, 43, 23412, 424, 6543, 23451
                }),
                decimal_concurrent_queue = new ConcurrentQueue<decimal>(new decimal[]
                {
                    9, 8, 7, 6, 5, 4, 3, 2, 1, 0
                }),
                null_string_value = null,
                single_string_value = "sadassjghadsfklahgjklahsd",
                multi_string_value = "ad ad ad a d\nasd asd askfaj dsf \n asd sad ",

                inner_object = new TestObject.InnerObject
                {
                    bool_value = true,
                    int_value = random.Next(int.MinValue, int.MaxValue),
                    float_value = (float)random.NextDouble(),
                    internal_object = new TestObject.InnerObject.InternalObject
                    {
                        int32_value = random.Next(int.MinValue, int.MaxValue)
                    },
                },
                external_object = new TestObject.ExternalObject
                {
                    int16_value = (short)random.Next(0, short.MaxValue),
                    int32_value = random.Next(0, int.MaxValue),
                    int64_value = (long)random.Next(0, int.MaxValue),
                    inner_value = new TestObject.ExternalObject.InnerValue
                    {
                         int16_value = (short)random.Next(short.MinValue, short.MaxValue),
                         int32_value = random.Next(int.MinValue, int.MaxValue),
                         int64_value = random.Next(int.MinValue, int.MaxValue),
                         obj_array = new TestObject.ExternalObject.InnerValue.ArrayElement[]
                         {
                             new TestObject.ExternalObject.InnerValue.ArrayElement
                             {
                                 bool_value = random.Next(0, 2) == 1,
                                 dec_value = random.Next(int.MinValue, int.MaxValue),
                             },
                             new TestObject.ExternalObject.InnerValue.ArrayElement
                             {
                                 bool_value = random.Next(0, 2) == 1,
                                 dec_value = random.Next(int.MinValue, int.MaxValue),
                             },
                             new TestObject.ExternalObject.InnerValue.ArrayElement
                             {
                                 bool_value = random.Next(0, 2) == 1,
                                 dec_value = random.Next(int.MinValue, int.MaxValue),
                             },
                         }
                    }
                },

                magic_enum_as_int = TestObject.MagicEnum.Three,
                magic_enum_as_str = TestObject.MagicEnum.Five,
            };
            original.sbyte_value_set((sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue));
            original.int16_value_set((short)random.Next(short.MinValue, short.MaxValue));
            original.int64_value_set(random.Next(0, int.MaxValue));
            original.float_value_set((float)random.NextDouble());
            original.decimal_value_set(random.Next(0, int.MaxValue));

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
            Assert.AreEqual(original, deserialized);
        }

        [Test]
        public void DeserializationTest()
        {
            var settings = new StormSettings
            {
                options     = StormSettings.Options.IgnoreCase,
                converters  = new List<IStormConverter>
                {
                    new UrlStormConverter(),
                },
            };

            var serializer = new StormSerializer();
            var task = serializer.DeserializeFileAsync("Examples/test-file.storm", settings);
            task.Wait();
            if (task.IsFaulted)
                throw task.Exception;

            var testStorm = task.Result;

            var ctx = new StormContext(serializer, settings, null);
            var testObj = testStorm.Populate<TestObject>(ctx);
            AreEqual(testObj.bool_value, testStorm[nameof(testObj.bool_value)]);
            AreEqual(testObj.sbyte_value_get(), testStorm["sbyte_value"]);
            AreEqual(testObj.byte_value, testStorm[nameof(testObj.byte_value)]);
            AreEqual(testObj.int16_value_get(), testStorm["int16_value"]);
            AreEqual(testObj.int32_value, testStorm[nameof(testObj.int32_value)]);
            AreEqual(testObj.int64_value_get(), testStorm["int64_value"]);
            AreEqual(testObj.uint16_value, testStorm[nameof(testObj.uint16_value)]);
            AreEqual(testObj.uint32_value_get(), testStorm["uint32_value"]);
            AreEqual(testObj.uint64_value, testStorm[nameof(testObj.uint64_value)]);
            AreEqual(testObj.float_value_get(), testStorm["float_value"]);
            AreEqual(testObj.double_value, testStorm[nameof(testObj.double_value)]);
            AreEqual(testObj.decimal_value_get(), testStorm["decimal_value"]);
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

            [StormInclude]
            private sbyte sbyte_value { get; set; }
            public sbyte sbyte_value_get() => sbyte_value;
            public void sbyte_value_set(sbyte value) => sbyte_value = value;

            public byte byte_value { get; set; }

            [StormInclude]
            private short int16_value;
            public short int16_value_get() => int16_value;
            public void int16_value_set(short value) => int16_value = value;

            public int int32_value;

            [StormInclude]
            private long int64_value;
            public long int64_value_get() => int64_value;
            public void int64_value_set(long value) => int64_value = value;

            public ushort uint16_value;

            [StormInclude]
            private uint uint32_value;
            public uint uint32_value_get() => uint32_value;
            public void uint32_value_set(uint value) => uint32_value = value;

            public ulong uint64_value;

            [StormInclude]
            private float float_value;
            public float float_value_get() => float_value;
            public void float_value_set(float value) => float_value = value;

            public double double_value;

            [StormInclude]
            private decimal decimal_value;
            public decimal decimal_value_get() => decimal_value;
            public void decimal_value_set(decimal value) => decimal_value = value;

            public string null_string_value;
            public string single_string_value;
            public string multi_string_value;

            public int[] int32_array;
            public List<ulong> uint64_list;
            public LinkedList<int> int32_linked_list;
            public HashSet<byte> byte_hash_set;
            public SortedSet<double> double_sorted_set;
            public Queue<ushort> ushort_queue;
            public ConcurrentQueue<decimal> decimal_concurrent_queue;

            public uint[] null_array;
            public List<long> null_list;
            public LinkedList<uint> null_linked_list;
            public HashSet<sbyte> null_hash_set;
            public SortedSet<byte> null_sorted_set;
            public Queue<short> null_queue;
            public ConcurrentQueue<double> null_concurrent_queue;

            public InnerObject inner_object;
            public ExternalObject external_object;

            [StormEnum(format = StormEnumFormat.String)]
            public MagicEnum magic_enum_as_str;
            [StormEnum(format = StormEnumFormat.Value)]
            public MagicEnum magic_enum_as_int;

            [StormIgnore]
            public int int_to_ignore;

            public override bool Equals(object obj)
            {
                return obj is TestObject other &&
                       bool_value == other.bool_value &&
                       sbyte_value == other.sbyte_value &&
                       byte_value == other.byte_value &&
                       int16_value == other.int16_value &&
                       int32_value == other.int32_value &&
                       int64_value == other.int64_value &&
                       uint16_value == other.uint16_value &&
                       uint32_value == other.uint32_value &&
                       uint64_value == other.uint64_value &&
                       float_value == other.float_value &&
                       double_value == other.double_value &&
                       decimal_value == other.decimal_value &&
                       null_string_value == other.null_string_value &&
                       single_string_value == other.single_string_value &&
                       multi_string_value == other.multi_string_value &&
                       (int32_array == other.int32_array || Enumerable.SequenceEqual(int32_array, other.int32_array)) &&
                       (uint64_list == other.uint64_list || Enumerable.SequenceEqual(uint64_list, other.uint64_list)) &&
                       (int32_linked_list == other.int32_linked_list || Enumerable.SequenceEqual(int32_linked_list, other.int32_linked_list)) &&
                       (byte_hash_set == other.byte_hash_set || Enumerable.SequenceEqual(byte_hash_set, other.byte_hash_set)) &&
                       (double_sorted_set == other.double_sorted_set || Enumerable.SequenceEqual(double_sorted_set, other.double_sorted_set)) &&
                       (ushort_queue == other.ushort_queue || Enumerable.SequenceEqual(ushort_queue, other.ushort_queue)) &&
                       (decimal_concurrent_queue == other.decimal_concurrent_queue || Enumerable.SequenceEqual(decimal_concurrent_queue, other.decimal_concurrent_queue)) &&
                       (null_array == other.null_array || Enumerable.SequenceEqual(null_array, other.null_array)) &&
                       (null_list == other.null_list || Enumerable.SequenceEqual(null_list, other.null_list)) &&
                       (null_linked_list == other.null_linked_list || Enumerable.SequenceEqual(null_linked_list, other.null_linked_list)) &&
                       (null_hash_set == other.null_hash_set || Enumerable.SequenceEqual(null_hash_set, other.null_hash_set)) &&
                       (null_sorted_set == other.null_sorted_set || Enumerable.SequenceEqual(null_sorted_set, other.null_sorted_set)) &&
                       (null_queue == other.null_queue || Enumerable.SequenceEqual(null_queue, other.null_queue)) &&
                       (null_concurrent_queue == other.null_concurrent_queue || Enumerable.SequenceEqual(null_concurrent_queue, other.null_concurrent_queue)) &&
                       EqualityComparer<InnerObject>.Default.Equals(inner_object, other.inner_object) &&
                       EqualityComparer<ExternalObject>.Default.Equals(external_object, other.external_object) &&
                       magic_enum_as_str == other.magic_enum_as_str &&
                       magic_enum_as_int == other.magic_enum_as_int &&
                       int_to_ignore == other.int_to_ignore;
            }

            public override int GetHashCode()
            {
                HashCode hash = new HashCode();
                hash.Add(bool_value);
                hash.Add(sbyte_value);
                hash.Add(byte_value);
                hash.Add(int16_value);
                hash.Add(int32_value);
                hash.Add(int64_value);
                hash.Add(uint16_value);
                hash.Add(uint32_value);
                hash.Add(uint64_value);
                hash.Add(float_value);
                hash.Add(double_value);
                hash.Add(decimal_value);
                hash.Add(null_string_value);
                hash.Add(single_string_value);
                hash.Add(multi_string_value);
                hash.Add(int32_array);
                hash.Add(uint64_list);
                hash.Add(int32_linked_list);
                hash.Add(byte_hash_set);
                hash.Add(double_sorted_set);
                hash.Add(ushort_queue);
                hash.Add(decimal_concurrent_queue);
                hash.Add(null_array);
                hash.Add(null_list);
                hash.Add(null_linked_list);
                hash.Add(null_hash_set);
                hash.Add(null_sorted_set);
                hash.Add(null_queue);
                hash.Add(null_concurrent_queue);
                hash.Add(inner_object);
                hash.Add(external_object);
                hash.Add(magic_enum_as_str);
                hash.Add(magic_enum_as_int);
                hash.Add(int_to_ignore);
                return hash.ToHashCode();
            }

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

                public override bool Equals(object obj)
                {
                    return obj is InnerObject @object &&
                           int_value == @object.int_value &&
                           float_value == @object.float_value &&
                           bool_value == @object.bool_value &&
                           EqualityComparer<InternalObject>.Default.Equals(internal_object, @object.internal_object);
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(int_value, float_value, bool_value, internal_object);
                }

                public class InternalObject
                {
                    public int int32_value;

                    public override bool Equals(object obj)
                    {
                        return obj is InternalObject @object &&
                               int32_value == @object.int32_value;
                    }

                    public override int GetHashCode()
                    {
                        return HashCode.Combine(int32_value);
                    }
                }
            }

            public class ExternalObject
            {
                public short int16_value;
                public int int32_value;
                public long int64_value;
                public InnerValue inner_value;
                public string[] str_array;

                public override bool Equals(object obj)
                {
                    return obj is ExternalObject other &&
                           int16_value == other.int16_value &&
                           int32_value == other.int32_value &&
                           int64_value == other.int64_value &&
                           Equals(inner_value, other.inner_value) &&
                           (str_array == other.str_array || Enumerable.SequenceEqual(str_array, other.str_array));
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(int16_value, int32_value, int64_value, inner_value, str_array);
                }

                public class InnerValue
                {
                    public short int16_value;
                    public int int32_value;
                    public long int64_value;
                    public ArrayElement[] obj_array;

                    public override bool Equals(object obj)
                    {
                        return obj is InnerValue other &&
                               int16_value == other.int16_value &&
                               int32_value == other.int32_value &&
                               int64_value == other.int64_value &&
                               (obj_array == other.obj_array || Enumerable.SequenceEqual(obj_array, other.obj_array));
                    }

                    public override int GetHashCode()
                    {
                        return HashCode.Combine(int16_value, int32_value, int64_value, obj_array);
                    }

                    public class ArrayElement
                    {
                        public bool bool_value;
                        public decimal dec_value;

                        public ArrayElement()
                        {

                        }

                        public override bool Equals(object obj)
                        {
                            return obj is ArrayElement element &&
                                   bool_value == element.bool_value &&
                                   dec_value == element.dec_value;
                        }

                        public override int GetHashCode()
                        {
                            return HashCode.Combine(bool_value, dec_value);
                        }
                    }
                }
            }
        }
    }
}