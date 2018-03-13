// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Microbench.Buffers
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Columns;
    using DotNetty.Buffers;
    using DotNetty.Common;

    [CategoriesColumn]
    [BenchmarkCategory("Equals")]
    [Config(typeof(MultipleRuntimes))]
    public class ByteBufferEqualsBenchmark
    {
        static ByteBufferEqualsBenchmark()
        {
            ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Disabled;
        }

        const int off = 4;
        const int len = 4096;
        protected AbstractByteBuffer buffer1;
        protected AbstractByteBuffer buffer2;

        [GlobalSetup]
        public void GlobalSetup()
        {
            //Don't assert they use the same buffer
            this.buffer1 = (AbstractByteBuffer)Unpooled.WrappedBuffer(new byte[len + off], 0, len + off);
            this.buffer2 = (AbstractByteBuffer)Unpooled.WrappedBuffer(new byte[len + off], 0, len + off);
            this.buffer1.SetWriterIndex(len + off);
            this.buffer2.SetWriterIndex(len + off);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            this.buffer1.Release();
            this.buffer2.Release();
        }

        [Benchmark]
        public bool EqualsWithSpan() => ByteBufferUtil.Equals32(this.buffer1, off, this.buffer2, off, len);

        [Benchmark]
        public bool EqualsWithArray() => ByteBufferUtil.Equals3(this.buffer1, off, this.buffer2, off, len);

        [Benchmark]
        public bool Equals_Get() => ByteBufferUtil.Equals2(this.buffer1, off, this.buffer2, off, len);

        [Benchmark(Baseline = true)]
        public bool Equals_Old() => ByteBufferUtil.Equals1(this.buffer1, off, this.buffer2, off, len);
    }
}
