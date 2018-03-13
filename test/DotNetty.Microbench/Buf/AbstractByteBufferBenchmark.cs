// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Microbench.Buffers
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Columns;
    using BenchmarkDotNet.Configs;
    using DotNetty.Buffers;
    using DotNetty.Common;

    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [Config(typeof(MultipleRuntimes))]
    public abstract class AbstractByteBufferBenchmark
    {
        static AbstractByteBufferBenchmark()
        {
            ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Disabled;
        }

        protected AbstractByteBuffer buffer;
        protected int offset = 4;
        protected int len = 4;

        protected abstract AbstractByteBuffer Alloc(int capacity);

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.buffer = Alloc(16);
            this.buffer.SetWriterIndex(12);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            this.buffer.Release();
        }

        [Benchmark]
        [BenchmarkCategory("EnsureAccessible")]
        public void EnsureAccessible() => this.buffer.EnsureAccessible();

        [Benchmark]
        [BenchmarkCategory("CheckIndex")]
        public void CheckIndex() => this.buffer.CheckIndex(this.offset, this.len);

        [Benchmark]
        [BenchmarkCategory("GetByte")]
        public byte GetByte() => this.buffer.GetByte(this.offset);

        [Benchmark]
        [BenchmarkCategory("_GetByte")]
        public byte _GetByte() => this.buffer._GetByte(this.offset);

        [Benchmark]
        [BenchmarkCategory("_GetUnsignedMedium")]
        public int _GetUnsignedMedium() => this.buffer._GetUnsignedMedium(this.offset);

        [Benchmark]
        [BenchmarkCategory("_GetUnsignedMediumLE")]
        public int _GetUnsignedMediumLE() => this.buffer._GetUnsignedMediumLE(this.offset);

        [Benchmark]
        [BenchmarkCategory("_GetInt")]
        public int _GetInt() => this.buffer._GetInt(this.offset);

        [Benchmark]
        [BenchmarkCategory("_GetIntLE")]
        public int _GetIntLE() => this.buffer._GetIntLE(this.offset);

        [Benchmark]
        [BenchmarkCategory("EnsureWritable")]
        public void EnsureWritable() => this.buffer.EnsureWritable(this.len);

        [Benchmark]
        [BenchmarkCategory("_SetByte")]
        public void _SetByte() => this.buffer._SetByte(this.offset, 4);

        [Benchmark]
        [BenchmarkCategory("_SetMedium")]
        public void _SetMedium() => this.buffer._SetMedium(this.offset, 4);

        [Benchmark]
        [BenchmarkCategory("_SetMediumLE")]
        public void _SetMediumLE() => this.buffer._SetMediumLE(this.offset, 4);

        [Benchmark]
        [BenchmarkCategory("_SetInt")]
        public void _SetInt() => this.buffer._SetInt(this.offset, 4);

        [Benchmark]
        [BenchmarkCategory("_SetIntLE")]
        public void _SetIntLE() => this.buffer._SetIntLE(this.offset, 4);
    }

    [BenchmarkCategory("PooledByteBuffer")]
    public class PooledHeapByteBuffer4Benchmark : AbstractByteBufferBenchmark
    {
        protected override AbstractByteBuffer Alloc(int capacity) => (AbstractByteBuffer)PooledByteBufferAllocator.Default.Buffer(capacity);
    }

    [BenchmarkCategory("PooledUnsafeDirectByteBuffer")]
    public class PooledUnsafeDirectByteBuffer4Benchmark : AbstractByteBufferBenchmark
    {
        protected override AbstractByteBuffer Alloc(int capacity) => (AbstractByteBuffer)PooledByteBufferAllocator.Default.DirectBuffer(capacity);
    }

    [BenchmarkCategory("PooledUnsafeDirectByteBufferEx")]
    public class PooledUnsafeDirectByteBufferEx4Benchmark : AbstractByteBufferBenchmark
    {
        protected override AbstractByteBuffer Alloc(int capacity) => (AbstractByteBuffer)PooledByteBufferAllocator.Default.NewDirectBufferEx(capacity, int.MaxValue);
    }

    [BenchmarkCategory("UnpooledHeapByteBuffer")]
    public class UnpooledHeapByteBuffer4Benchmark : AbstractByteBufferBenchmark
    {
        protected override AbstractByteBuffer Alloc(int capacity) => new UnpooledHeapByteBuffer(UnpooledByteBufferAllocator.Default, capacity, int.MaxValue);
    }

    [BenchmarkCategory("UnpooledUnsafeDirectByteBuffer")]
    public class UnpooledUnsafeDirectByteBuffer4Benchmark : AbstractByteBufferBenchmark
    {
        protected override AbstractByteBuffer Alloc(int capacity) => new UnpooledUnsafeDirectByteBuffer(UnpooledByteBufferAllocator.Default, capacity, int.MaxValue);
    }

    [BenchmarkCategory("UnpooledUnsafeDirectByteBufferEx")]
    public class UnpooledUnsafeDirectByteBufferEx4Benchmark : AbstractByteBufferBenchmark
    {
        protected override AbstractByteBuffer Alloc(int capacity) => new UnpooledUnsafeDirectByteBufferEx(UnpooledByteBufferAllocator.Default, capacity, int.MaxValue);
    }
}
