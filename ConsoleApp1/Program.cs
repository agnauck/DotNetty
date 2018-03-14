using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleApp1
{
    public unsafe class Program
    {
        static Program()
        {
            Environment.SetEnvironmentVariable("io.netty.noPreferDirect", "true");
            DotNetty.Common.ResourceLeakDetector.Level = DotNetty.Common.ResourceLeakDetector.DetectionLevel.Disabled;
        }

        private byte[] pinedData = new byte[8192];
        private void* pinedPointer1;
        private void* pinedPointer2;
        private void* pinedPointer3;
        private byte* pinedPointer4;
        private GCHandle pinnedHandle;
        private byte[] unpinedData = new byte[8192];

        public void GlobalSetup()
        {
            pinedPointer2 = Unsafe.AsPointer(ref pinedData[0]);
            pinedPointer3 = Unsafe.AsPointer(ref MemoryMarshal.GetReference(new ReadOnlySpan<byte>(pinedData)));
            this.pinnedHandle = GCHandle.Alloc(this.pinedData, GCHandleType.Pinned);
            pinedPointer1 = pinnedHandle.AddrOfPinnedObject().ToPointer();
            pinedPointer4 = (byte*)pinedPointer1;
            Debug.Assert(pinedPointer1 == pinedPointer2);
            Debug.Assert(pinedPointer1 == pinedPointer3);

#if !NETCOREAPP1_1
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
#endif
        }

        public void GlobalCleanup()
        {
            if (this.pinnedHandle.IsAllocated)
                this.pinnedHandle.Free();
        }

        static void Main(string[] args)
        {
            new Program().Run();
            Console.ReadLine();
        }
        int off1 = 0;
        int off2 = 4096;
        int len = 4096;
        int loop_copy = 10000000;

        public void Buffer_BlockCopy() => Buffer.BlockCopy(unpinedData, off1, unpinedData, off2, len);
        public void Array_Copy() => Array.Copy(unpinedData, off1, unpinedData, off2, len);
        public void Array_ConstrainedCopy() => Array.ConstrainedCopy(unpinedData, off1, unpinedData, off2, len);
        public void Unsafe_CopyBlock_U2U_Ref() => Unsafe.CopyBlock(ref unpinedData[off2], ref unpinedData[off1], (uint)len);
        public void Unsafe_CopyBlock_U2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                Unsafe.CopyBlock(p2, p1, (uint)len);
            }
        }
        public void Unsafe_CopyBlock_P2P_Pined()
        {
            Unsafe.CopyBlock(Unsafe.Add<byte>(pinedPointer1, off2), Unsafe.Add<byte>(pinedPointer1, off1), (uint)len);
        }
        public void Unsafe_CopyBlock_P2P_Pined2()
        {
            Unsafe.CopyBlock(((IntPtr)pinedPointer1 + off2).ToPointer(), ((IntPtr)pinedPointer1 + off1).ToPointer(), (uint)len);
        }
        public void Unsafe_CopyBlock_P2P_Pined_Cast()
        {
            Unsafe.CopyBlock(Unsafe.Add<byte>(pinedPointer4, off2), Unsafe.Add<byte>(pinedPointer4, off1), (uint)len);
        }
        public void Unsafe_CopyBlockUnaligned_U2U_Ref() => Unsafe.CopyBlockUnaligned(ref unpinedData[off2], ref unpinedData[off1], (uint)len);
        public void Unsafe_CopyBlockUnaligned_U2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                Unsafe.CopyBlockUnaligned(p2, p1, (uint)len);
            }
        }
        public void Buffer_MemoryCopy_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                Buffer.MemoryCopy(p1, p2, len, len);
            }
        }
        public void Span_Copy_P2P()
        {
            new ReadOnlySpan<byte>(Unsafe.Add<byte>(pinedPointer1, off1), len).CopyTo(new Span<byte>(Unsafe.Add<byte>(pinedPointer1, off2), len));
        }
        public void Span_Copy_P2P_Array()
        {
            new ReadOnlySpan<byte>(pinedData, off1, len).CopyTo(new Span<byte>(pinedData, off2, len));
        }
        public void Span_Copy_P2U()
        {
            new ReadOnlySpan<byte>(Unsafe.Add<byte>(pinedPointer1, off1), len).CopyTo(new Span<byte>(unpinedData, off2, len));
        }
        public void Span_Copy_P2U_Array()
        {
            new ReadOnlySpan<byte>(pinedData, off1, len).CopyTo(new Span<byte>(unpinedData, off2, len));
        }
        public void Span_Copy_U2P()
        {
            new ReadOnlySpan<byte>(unpinedData, off1, len).CopyTo(new Span<byte>(Unsafe.Add<byte>(pinedPointer1, off2), len));
        }
        public void Span_Copy_U2U()
        {
            new ReadOnlySpan<byte>(unpinedData, off1, len).CopyTo(new Span<byte>(unpinedData, off2, len));
        }
        public void Span_Copy_U2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            fixed (byte* p2 = &unpinedData[off2])
            {
                new ReadOnlySpan<byte>(p1, len).CopyTo(new Span<byte>(p2, len));
            }
        }
        public void Unsafe_CopyBlock_U2P_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off1])
            {
                Unsafe.CopyBlock(Unsafe.Add<byte>(pinedPointer1, off2), p1, (uint)len);
            }
        }
        public void Unsafe_CopyBlock_U2P_Ref()
        {
            Unsafe.CopyBlock(ref Unsafe.AsRef<byte>(Unsafe.Add<byte>(pinedPointer1, off2)), ref unpinedData[off1], (uint)len);
        }
        public void Unsafe_CopyBlock_P2U_Fixed()
        {
            fixed (byte* p1 = &unpinedData[off2])
            {
                Unsafe.CopyBlock(p1, Unsafe.Add<byte>(pinedPointer1, off2), (uint)len);
            }
        }
        public void Unsafe_CopyBlock_P2U_Array()
        {
            Unsafe.CopyBlock(ref unpinedData[off2], ref pinedData[off1], (uint)len);
        }
        public void Unsafe_CopyBlock_P2U_Ref()
        {
            Unsafe.CopyBlock(ref unpinedData[off2], ref Unsafe.AsRef<byte>(Unsafe.Add<byte>(pinedPointer1, off1)), (uint)len);
        }
        int a = 0;
        public void Unsafe_As() => a = Unsafe.As<byte, int>(ref unpinedData[off1]);
        public void Unsafe_Read() => a = Unsafe.ReadUnaligned<int>(ref unpinedData[off1]);
        public unsafe void Read_Pointer() => a = *((byte*)Unsafe.Add<byte>(pinedPointer1, off1));
        public unsafe void Read_Direct() => a = unpinedData[off1];

        private void Test(Action a, string name, int count = 10000000)
        {
            Action empty = () => { };
            for (int i = 0; i < 100; ++i)
            {
                a();
                empty();
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Thread.Sleep(100);

            long total = 0, baseline = 0;
            var sw = Stopwatch.StartNew();
            try
            {
                for (int i = count; i >= 0; i--)
                {
                    a();
                }
                total = sw.ElapsedMilliseconds;
                Thread.Sleep(100);

                sw.Restart();
                for (int i = count; i >= 0; i--)
                {
                    empty();
                }
                baseline = sw.ElapsedMilliseconds;
                Thread.Sleep(100);
                Console.WriteLine($"{name} success: {total - baseline}/{total}ms used in method.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{name} failed: {sw.ElapsedMilliseconds + total}ms used, {e}");
            }

        }

        private void Run()
        {
            try
            {
                GlobalSetup();

                //Test(Buffer_BlockCopy, nameof(Buffer_BlockCopy), loop_copy);
                //Test(Buffer_MemoryCopy_Fixed, nameof(Buffer_MemoryCopy_Fixed), loop_copy);
                //Test(Array_Copy, nameof(Array_Copy), loop_copy);
                //Test(Array_ConstrainedCopy, nameof(Array_ConstrainedCopy), loop_copy);

                //Test(Unsafe_CopyBlock_U2U_Ref, nameof(Unsafe_CopyBlock_U2U_Ref), loop_copy);
                //Test(Unsafe_CopyBlock_U2U_Fixed, nameof(Unsafe_CopyBlock_U2U_Fixed), loop_copy);
                //Test(Unsafe_CopyBlockUnaligned_U2U_Ref, nameof(Unsafe_CopyBlockUnaligned_U2U_Ref), loop_copy);
                //Test(Unsafe_CopyBlockUnaligned_U2U_Fixed, nameof(Unsafe_CopyBlockUnaligned_U2U_Fixed), loop_copy);

                //Test(Unsafe_CopyBlock_P2P_Pined, nameof(Unsafe_CopyBlock_P2P_Pined), loop_copy);
                //Test(Unsafe_CopyBlock_P2P_Pined2, nameof(Unsafe_CopyBlock_P2P_Pined2), loop_copy);
                //Test(Unsafe_CopyBlock_P2P_Pined_Cast, nameof(Unsafe_CopyBlock_P2P_Pined_Cast), loop_copy);

                //Test(Span_Copy_P2P, nameof(Span_Copy_P2P), loop_copy);
                //Test(Span_Copy_P2P_Array, nameof(Span_Copy_P2P_Array), loop_copy);
                //Test(Span_Copy_P2U, nameof(Span_Copy_P2U), loop_copy);
                //Test(Span_Copy_P2U_Array, nameof(Span_Copy_P2U_Array), loop_copy);
                //Test(Span_Copy_U2P, nameof(Span_Copy_U2P), loop_copy);
                //Test(Span_Copy_U2U, nameof(Span_Copy_U2U), loop_copy);
                //Test(Span_Copy_U2U_Fixed, nameof(Span_Copy_U2U_Fixed), loop_copy);

                //Test(Unsafe_CopyBlock_P2U_Fixed, nameof(Unsafe_CopyBlock_P2U_Fixed), loop_copy);
                //Test(Unsafe_CopyBlock_P2U_Ref, nameof(Unsafe_CopyBlock_P2U_Ref), loop_copy);
                //Test(Unsafe_CopyBlock_P2U_Array, nameof(Unsafe_CopyBlock_P2U_Array), loop_copy);
                //Test(Unsafe_CopyBlock_U2P_Fixed, nameof(Unsafe_CopyBlock_U2P_Fixed), loop_copy);
                //Test(Unsafe_CopyBlock_U2P_Ref, nameof(Unsafe_CopyBlock_U2P_Ref), loop_copy);

                //Test(Unsafe_As, nameof(Unsafe_As), 1000000000);
                //Test(Unsafe_Read, nameof(Unsafe_Read), 1000000000);
                //Test(Read_Pointer, nameof(Read_Pointer), 1000000000);
                //Test(Read_Direct, nameof(Read_Direct), 1000000000);

                int r = 0, w = 0, c = 65536;
//Base:
//GetInt success: 7429ms used.
//CheckIndex success: 14156ms used.
//CheckIndex_buf2 success: 15816ms used.
//Capacity success: 3282ms used.
//CheckIndex0 success: 12445ms used.
//EnsureAccessible success: 3324ms used.
//EnsureWritable success: 9283ms used.
//EnsureWritable2 success: 8080ms used.
//CheckReadableBytes success: 6809ms used.

//Post
                var buf2 = (DotNetty.Buffers.AbstractByteBuffer)DotNetty.Buffers.Unpooled.WrappedBuffer(new byte[256]);
                var buf = (DotNetty.Buffers.AbstractByteBuffer)DotNetty.Buffers.ByteBufferUtil.DefaultAllocator.Buffer(256);
                buf.SetWriterIndex(128);
                buf2.SetWriterIndex(128);
                var loop_index = 100_000_000_0;
                bool a = false;
                //Test(() =>
                //{
                //    a = DotNetty.Common.Internal.MathUtil.IsOutOfBounds(r, 4, c);
                //}, "IsOutOfBounds1", loop_index);
                //Test(() =>
                //{
                //    a = DotNetty.Common.Internal.MathUtil.IsOutOfBounds2(r, 4, c);
                //}, "IsOutOfBounds2", loop_index);
                //Test(() =>
                //{
                //    a = DotNetty.Common.Internal.MathUtil.IsOutOfBounds3(r, 4, c);
                //}, "IsOutOfBounds3", loop_index);
                //Test(() =>
                //{
                //    buf.CheckIndex(r);
                //}, "CheckIndex", loop_index);
                ////Test(() =>
                ////{
                ////    buf.CheckIndex2(r);
                ////}, "CheckIndex2", loop_index);
                //Test(() =>
                //{
                //    buf.CheckIndex(r, 4);
                //}, "CheckIndex4", loop_index);
                //Test(() =>
                //{
                //    buf2.CheckIndex(r, 4);
                //}, "CheckIndex_buf2", loop_index);
                //Test(() =>
                //{
                //    buf.GetInt(r);
                //}, "GetInt", loop_index);
                //Test(() =>
                //{
                //    a = buf.Capacity > 0;
                //}, "Capacity", loop_index);
                //Test(() =>
                //{
                //    a = buf.ReferenceCount > 0;
                //}, "ReferenceCount", loop_index);
                //Test(() =>
                //{
                //    buf.CheckIndex0(r, 4);
                //}, "CheckIndex0", loop_index);
                //Test(() =>
                //{
                //    buf.EnsureAccessible();
                //}, "EnsureAccessible", loop_index);
                //Test(() =>
                //{
                //    buf.EnsureWritable(4);
                //}, "EnsureWritable", loop_index);
                Test(() =>
                {
                    buf.EnsureWritable0(4);
                }, "EnsureWritable0", loop_index);
                //Test(() =>
                //{
                //    buf.EnsureWritable(4, false);
                //}, "EnsureWritable2", loop_index);
                Test(() =>
                {
                    buf.writerIndex = 128;
                    buf.WriteInt(4);
                }, "WriteInt", loop_index);
                //Test(() =>
                //{
                //    buf.CheckReadableBytes(4);
                //}, "CheckReadableBytes", loop_index);
                Console.WriteLine(a);
            }
            finally
            {
                GlobalCleanup();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOutOfBounds2(int index, int length, int capacity) =>
            (index | length | (index + length) | (capacity - (index + length))) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOutOfBounds3(int index, int length, int capacity) =>
            index < 0 || length < 0 || index + length < 0 || (index + length) > capacity;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool IsOutOfBounds4(int index, int length, int capacity) =>
            (index | length | (index + length) | (capacity - (index + length))) < 0;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool IsOutOfBounds5(int index, int length, int capacity) =>
            index < 0 || length < 0 || index + length < 0 || (index + length) > capacity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOutOfBounds6(int index, int length, int capacity) =>
            (index | length | unchecked(index + length) | (capacity - (index + length))) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOutOfBounds7(int index, int length, int capacity) =>
            (index | length | (index + length) | (capacity - (index + length))) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOutOfBounds8(int index, int length, int capacity) =>
            index < 0 || (index | length | unchecked(index + length) | (capacity - (index + length))) < 0;

        private class TestTimer : IDisposable
        {
            Stopwatch sw;
            string name;
            public TestTimer(string name)
            {
                this.name = name;
                this.sw = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                Console.WriteLine($"{name}: {sw.ElapsedMilliseconds} used.");
            }
        }
    }
}
