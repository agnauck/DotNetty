// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Microbench.Buffers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Columns;
    using BenchmarkDotNet.Configs;

    [CategoriesColumn]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [BenchmarkCategory("Memory")]
    [Config(typeof(MultipleRuntimes))]
    public unsafe class MemoryBenchmark
    {
        private byte[] pinedData;
        private void* pinedPointer1;
        //private void* pinedPointer2;
        //private void* pinedPointer3;
        private byte* pinedPointer4;
        private GCHandle pinnedHandle;
        private byte[] unpinedData;

        int total_len = 8192;
        int off1 = 0;
        int off2 = 4096;
        int cpy_len = 4096;

        [GlobalSetup]
        public void GlobalSetup()
        {
            pinedData = new byte[total_len];
            unpinedData = new byte[total_len];
            //pinedPointer2 = Unsafe.AsPointer(ref pinedData[0]);
            //pinedPointer3 = Unsafe.AsPointer(ref MemoryMarshal.GetReference(new ReadOnlySpan<byte>(pinedData)));
            this.pinnedHandle = GCHandle.Alloc(this.pinedData, GCHandleType.Pinned);
            pinedPointer1 = pinnedHandle.AddrOfPinnedObject().ToPointer();
            pinedPointer4 = (byte*)pinedPointer1;
            //Debug.Assert(pinedPointer1 == pinedPointer2);
            //Debug.Assert(pinedPointer1 == pinedPointer3);
        }

        public void GlobalCleanup()
        {
            if (this.pinnedHandle.IsAllocated)
                this.pinnedHandle.Free();
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Buffer_BlockCopy() => Buffer.BlockCopy(unpinedData, off1, unpinedData, off2, cpy_len);

        [Benchmark, BenchmarkCategory("Copy")]
        public void Array_Copy() => Array.Copy(unpinedData, off1, unpinedData, off2, cpy_len);

        [Benchmark, BenchmarkCategory("Copy")]
        public void Array_ConstrainedCopy() => Array.ConstrainedCopy(unpinedData, off1, unpinedData, off2, cpy_len);

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_U2U_Ref() => Unsafe.CopyBlock(ref unpinedData[off2], ref unpinedData[off1], (uint)cpy_len);

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_U2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                Unsafe.CopyBlock(p2, p1, (uint)cpy_len);
            }
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_P2P_Pined()
        {
            Unsafe.CopyBlock(Unsafe.Add<byte>(pinedPointer1, off2), Unsafe.Add<byte>(pinedPointer1, off1), (uint)cpy_len);
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_P2P_Pined2()
        {
            Unsafe.CopyBlock(((IntPtr)pinedPointer1 + off2).ToPointer(), ((IntPtr)pinedPointer1 + off1).ToPointer(), (uint)cpy_len);
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_P2P_Pined_Cast()
        {
            Unsafe.CopyBlock(Unsafe.Add<byte>(pinedPointer4, off2), Unsafe.Add<byte>(pinedPointer4, off1), (uint)cpy_len);
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlockUnaligned_U2U_Ref() => Unsafe.CopyBlockUnaligned(ref unpinedData[off2], ref unpinedData[off1], (uint)cpy_len);

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlockUnaligned_U2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                Unsafe.CopyBlockUnaligned(p2, p1, (uint)cpy_len);
            }
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Buffer_MemoryCopy_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                Buffer.MemoryCopy(p1, p2, cpy_len, cpy_len);
            }
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_P2P()
        {
            new ReadOnlySpan<byte>(Unsafe.Add<byte>(pinedPointer1, off1), cpy_len).CopyTo(new Span<byte>(Unsafe.Add<byte>(pinedPointer1, off2), cpy_len));
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_P2P_Array()
        {
            new ReadOnlySpan<byte>(pinedData, off1, cpy_len).CopyTo(new Span<byte>(pinedData, off2, cpy_len));
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_P2U()
        {
            new ReadOnlySpan<byte>(Unsafe.Add<byte>(pinedPointer1, off1), cpy_len).CopyTo(new Span<byte>(unpinedData, off2, cpy_len));
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_P2U_Array()
        {
            new ReadOnlySpan<byte>(pinedData, off1, cpy_len).CopyTo(new Span<byte>(unpinedData, off2, cpy_len));
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_U2P()
        {
            new ReadOnlySpan<byte>(unpinedData, off1, cpy_len).CopyTo(new Span<byte>(Unsafe.Add<byte>(pinedPointer1, off2), cpy_len));
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_U2U()
        {
            new ReadOnlySpan<byte>(unpinedData, off1, cpy_len).CopyTo(new Span<byte>(unpinedData, off2, cpy_len));
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Span_Copy_U2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                new ReadOnlySpan<byte>(p1, cpy_len).CopyTo(new Span<byte>(p2, cpy_len));
            }
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_U2P_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            {
                Unsafe.CopyBlock(Unsafe.Add<byte>(pinedPointer1, off2), p1, (uint)cpy_len);
            }
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_U2P_Ref()
        {
            Unsafe.CopyBlock(ref Unsafe.AsRef<byte>(Unsafe.Add<byte>(pinedPointer1, off2)), ref unpinedData[off1], (uint)cpy_len);
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_P2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off2])
            {
                Unsafe.CopyBlock(p1, Unsafe.Add<byte>(pinedPointer1, off2), (uint)cpy_len);
            }
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_P2U_Array()
        {
            Unsafe.CopyBlock(ref unpinedData[off2], ref pinedData[off1], (uint)cpy_len);
        }

        [Benchmark, BenchmarkCategory("Copy")]
        public void Unsafe_CopyBlock_P2U_Ref()
        {
            Unsafe.CopyBlock(ref unpinedData[off2], ref Unsafe.AsRef<byte>(Unsafe.Add<byte>(pinedPointer1, off1)), (uint)cpy_len);
        }

        [Benchmark, BenchmarkCategory("Read")]
        public int Unsafe_As() => Unsafe.As<byte, int>(ref unpinedData[off1]);

        [Benchmark, BenchmarkCategory("Read")]
        public int Unsafe_Read() => Unsafe.ReadUnaligned<int>(ref unpinedData[off1]);

        [Benchmark, BenchmarkCategory("Read")]
        public unsafe int Read_Pointer() => *((int*)Unsafe.Add<byte>(pinedPointer1, off1));

        [Benchmark, BenchmarkCategory("Read")]
        public unsafe int Read_Array() => (unpinedData[off1] << 24) | (unpinedData[off1 + 1] << 16) | (unpinedData[off1 + 2] << 8) | (unpinedData[off1 + 3]) ;
    }
}
