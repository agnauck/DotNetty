using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleApp1
{
    public unsafe class Program
    {
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
            for (int i = 0; i < 100; ++i)
            {
                a();
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Thread.Sleep(100);

            var sw = Stopwatch.StartNew();
            try
            {
                for (; count >= 0; count--)
                {
                    a();
                }
                Console.WriteLine($"{name} success: {sw.ElapsedMilliseconds}ms used.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{name} failed: {sw.ElapsedMilliseconds}ms used, {e}");
            }

        }

        private void Run()
        {
            try
            {
                GlobalSetup();
#if false
#endif
                Test(Buffer_BlockCopy, nameof(Buffer_BlockCopy), loop_copy);
                Test(Buffer_MemoryCopy_Fixed, nameof(Buffer_MemoryCopy_Fixed), loop_copy);
                Test(Array_Copy, nameof(Array_Copy), loop_copy);
                Test(Array_ConstrainedCopy, nameof(Array_ConstrainedCopy), loop_copy);

                Test(Unsafe_CopyBlock_U2U_Ref, nameof(Unsafe_CopyBlock_U2U_Ref), loop_copy);
                Test(Unsafe_CopyBlock_U2U_Fixed, nameof(Unsafe_CopyBlock_U2U_Fixed), loop_copy);
                Test(Unsafe_CopyBlockUnaligned_U2U_Ref, nameof(Unsafe_CopyBlockUnaligned_U2U_Ref), loop_copy);
                Test(Unsafe_CopyBlockUnaligned_U2U_Fixed, nameof(Unsafe_CopyBlockUnaligned_U2U_Fixed), loop_copy);

                Test(Unsafe_CopyBlock_P2P_Pined, nameof(Unsafe_CopyBlock_P2P_Pined), loop_copy);
                Test(Unsafe_CopyBlock_P2P_Pined2, nameof(Unsafe_CopyBlock_P2P_Pined2), loop_copy);
                Test(Unsafe_CopyBlock_P2P_Pined_Cast, nameof(Unsafe_CopyBlock_P2P_Pined_Cast), loop_copy);

                Test(Span_Copy_P2P, nameof(Span_Copy_P2P), loop_copy);
                Test(Span_Copy_P2P_Array, nameof(Span_Copy_P2P_Array), loop_copy);
                Test(Span_Copy_P2U, nameof(Span_Copy_P2U), loop_copy);
                Test(Span_Copy_P2U_Array, nameof(Span_Copy_P2U_Array), loop_copy);
                Test(Span_Copy_U2P, nameof(Span_Copy_U2P), loop_copy);
                Test(Span_Copy_U2U, nameof(Span_Copy_U2U), loop_copy);
                Test(Span_Copy_U2U_Fixed, nameof(Span_Copy_U2U_Fixed), loop_copy);

                Test(Unsafe_CopyBlock_P2U_Fixed, nameof(Unsafe_CopyBlock_P2U_Fixed), loop_copy);
                Test(Unsafe_CopyBlock_P2U_Ref, nameof(Unsafe_CopyBlock_P2U_Ref), loop_copy);
                Test(Unsafe_CopyBlock_P2U_Array, nameof(Unsafe_CopyBlock_P2U_Array), loop_copy);
                Test(Unsafe_CopyBlock_U2P_Fixed, nameof(Unsafe_CopyBlock_U2P_Fixed), loop_copy);
                Test(Unsafe_CopyBlock_U2P_Ref, nameof(Unsafe_CopyBlock_U2P_Ref), loop_copy);

                Test(Unsafe_As, nameof(Unsafe_As), 1000000000);
                Test(Unsafe_Read, nameof(Unsafe_Read), 1000000000);
                Test(Read_Pointer, nameof(Read_Pointer), 1000000000);
                Test(Read_Direct, nameof(Read_Direct), 1000000000);
            }
            finally
            {
                GlobalCleanup();
            }
        }

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
