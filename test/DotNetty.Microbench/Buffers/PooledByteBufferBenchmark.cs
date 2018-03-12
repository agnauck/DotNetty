// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Microbench.Buffers
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;
    using DotNetty.Buffers;
    using DotNetty.Common;

    [ClrJob, CoreJob]
    [BenchmarkCategory("ByteBuffer")]
    public class PooledByteBufferBenchmark
    {
        static PooledByteBufferBenchmark()
        {
            ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Disabled;
        }

        AbstractByteBuffer unsafeBufferEx;
        AbstractByteBuffer unsafeBuffer;
        AbstractByteBuffer buffer;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.unsafeBufferEx = (AbstractByteBuffer)PooledByteBufferAllocator.Default.NewDirectBufferEx(65536, AbstractByteBufferAllocator.DefaultMaxCapacity);
            this.unsafeBuffer = (AbstractByteBuffer)PooledByteBufferAllocator.Default.DirectBuffer(65536);
            this.buffer = (AbstractByteBuffer)PooledByteBufferAllocator.Default.HeapBuffer(65536);
            this.buffer.WriteLong(1L);
            this.unsafeBufferEx.SetWriterIndex(65536);
            this.unsafeBuffer.SetWriterIndex(65536);
            this.buffer.SetWriterIndex(65536);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            this.unsafeBuffer.Release();
            this.buffer.Release();
        }

        [Benchmark]
        public void CheckIndexUnsafeEx() => this.unsafeBufferEx.CheckIndex(0, 8);

        [Benchmark]
        public void CheckIndexUnsafe() => this.unsafeBuffer.CheckIndex(0, 8);

        [Benchmark]
        public void CheckIndex() => this.buffer.CheckIndex(0, 8);

        [Benchmark]
        public byte GetByteUnsafeEx() => this.unsafeBufferEx.GetByte(0);

        [Benchmark]
        public byte GetByteUnsafe() => this.unsafeBuffer.GetByte(0);

        [Benchmark]
        public byte GetByte() => this.buffer.GetByte(0);

        [Benchmark]
        public short GetShortUnsafe() => this.unsafeBufferEx.GetShort(0);

        [Benchmark]
        public short GetShortUnsafeEx() => this.unsafeBuffer.GetShort(0);

        [Benchmark]
        public short GetShort() => this.buffer.GetShort(0);

        [Benchmark]
        public int GetMediumUnsafe() => this.unsafeBufferEx.GetMedium(0);

        [Benchmark]
        public int GetMediumUnsafeEx() => this.unsafeBuffer.GetMedium(0);

        [Benchmark]
        public int GetMedium() => this.buffer.GetMedium(0);

        [Benchmark]
        public int GetIntUnsafe() => this.unsafeBufferEx.GetInt(0);

        [Benchmark]
        public int GetIntUnsafeEx() => this.unsafeBuffer.GetInt(0);

        [Benchmark]
        public int GetInt() => this.buffer.GetInt(0);

        [Benchmark]
        public long GetLongUnsafeEx() => this.unsafeBufferEx.GetLong(0);

        [Benchmark]
        public long GetLongUnsafe() => this.unsafeBuffer.GetLong(0);

        [Benchmark]
        public long GetLong() => this.buffer.GetLong(0);

        [Benchmark]
        public bool EqualsUnsafeEx() => ByteBufferUtil.Equals(this.unsafeBufferEx, this.unsafeBufferEx);

        [Benchmark]
        public bool EqualsUnsafe() => ByteBufferUtil.Equals(this.unsafeBuffer, this.unsafeBuffer);

        [Benchmark]
        public bool Equals() => ByteBufferUtil.Equals(this.buffer, this.buffer);
    }
}
