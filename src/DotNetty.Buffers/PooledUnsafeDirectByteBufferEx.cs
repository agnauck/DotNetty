// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Buffers
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetty.Common;
    using DotNetty.Common.Internal;

    sealed unsafe class PooledUnsafeDirectByteBufferEx : PooledByteBuffer<byte[]>
    {
        static readonly ThreadLocalPool<PooledUnsafeDirectByteBufferEx> Recycler = new ThreadLocalPool<PooledUnsafeDirectByteBufferEx>(handle => new PooledUnsafeDirectByteBufferEx(handle, 0));

        byte* memoryAddress;

        internal static PooledUnsafeDirectByteBufferEx NewInstance(int maxCapacity)
        {
            PooledUnsafeDirectByteBufferEx buf = Recycler.Take();
            buf.Reuse(maxCapacity);
            return buf;
        }

        PooledUnsafeDirectByteBufferEx(ThreadLocalPool.Handle recyclerHandle, int maxCapacity)
            : base(recyclerHandle, maxCapacity)
        {
        }

        internal override void Init(PoolChunk<byte[]> chunk, long handle, int offset, int length, int maxLength,
            PoolThreadCache<byte[]> cache)
        {
            base.Init(chunk, handle, offset, length, maxLength, cache);
            this.InitMemoryAddress();
        }

        internal override void InitUnpooled(PoolChunk<byte[]> chunk, int length)
        {
            base.InitUnpooled(chunk, length);
            this.InitMemoryAddress();
        }

        void InitMemoryAddress()
        {
            //It's safe to get an unfixed pointer here since the main buffer is pinned in DirectArena.MemoryChunk.ctor
            this.memoryAddress = (byte*)Unsafe.AsPointer(ref this.Memory.AsRef(this.Offset));
        }

        public override bool IsDirect => true;

        protected internal override byte _GetByte(int index) => UnsafeByteBufferUtil.GetByte(ref this.Addr(index));

        protected internal override short _GetShort(int index) => UnsafeByteBufferUtil.GetShort(ref this.Addr(index));

        protected internal override short _GetShortLE(int index) => UnsafeByteBufferUtil.GetShortLE(ref this.Addr(index));

        protected internal override int _GetUnsignedMedium(int index) => UnsafeByteBufferUtil.GetUnsignedMedium(ref this.Addr(index));

        protected internal override int _GetUnsignedMediumLE(int index) => UnsafeByteBufferUtil.GetUnsignedMediumLE(ref this.Addr(index));

        protected internal override int _GetInt(int index) => UnsafeByteBufferUtil.GetInt(ref this.Addr(index));

        protected internal override int _GetIntLE(int index) => UnsafeByteBufferUtil.GetIntLE(ref this.Addr(index));

        protected internal override long _GetLong(int index) => UnsafeByteBufferUtil.GetLong(ref this.Addr(index));

        protected internal override long _GetLongLE(int index) => UnsafeByteBufferUtil.GetLongLE(ref this.Addr(index));

        public override IByteBuffer GetBytes(int index, IByteBuffer destination, int dstIndex, int length)
        {
            this.CheckIndex(index, length);
            UnsafeByteBufferUtil.GetBytes(this, ref this.Addr(index), index, destination, dstIndex, length);
            return this;
        }

        public override IByteBuffer GetBytes(int index, byte[] destination, int dstIndex, int length)
        {
            this.CheckIndex(index, length);
            UnsafeByteBufferUtil.GetBytes(this, ref this.Addr(index), index, destination, dstIndex, length);
            return this;
        }

        public override IByteBuffer GetBytes(int index, Stream destination, int length)
        {
            UnsafeByteBufferUtil.GetBytes(this, ref this.Addr(index), destination, length);
            return this;
        }

        protected internal override void _SetByte(int index, int value) => UnsafeByteBufferUtil.SetByte(ref this.Addr(index), value);

        protected internal override void _SetShort(int index, int value) => UnsafeByteBufferUtil.SetShort(ref this.Addr(index), value);

        protected internal override void _SetShortLE(int index, int value) => UnsafeByteBufferUtil.SetShortLE(ref this.Addr(index), value);

        protected internal override void _SetMedium(int index, int value) => UnsafeByteBufferUtil.SetMedium(ref this.Addr(index), value);

        protected internal override void _SetMediumLE(int index, int value) => UnsafeByteBufferUtil.SetMediumLE(ref this.Addr(index), value);

        protected internal override void _SetInt(int index, int value) => UnsafeByteBufferUtil.SetInt(ref this.Addr(index), value);

        protected internal override void _SetIntLE(int index, int value) => UnsafeByteBufferUtil.SetIntLE(ref this.Addr(index), value);

        protected internal override void _SetLong(int index, long value) => UnsafeByteBufferUtil.SetLong(ref this.Addr(index), value);

        protected internal override void _SetLongLE(int index, long value) => UnsafeByteBufferUtil.SetLongLE(ref this.Addr(index), value);

        public override IByteBuffer SetBytes(int index, IByteBuffer src, int srcIndex, int length)
        {
            this.CheckIndex(index, length);
            UnsafeByteBufferUtil.SetBytes(this, ref this.Addr(index), index, src, srcIndex, length);
            return this;
        }

        public override IByteBuffer SetBytes(int index, byte[] src, int srcIndex, int length)
        {
            this.CheckIndex(index, length);
            UnsafeByteBufferUtil.SetBytes(this, ref this.Addr(index), ref src.AsRef(srcIndex), length);
            return this;
        }

        public override Task<int> SetBytesAsync(int index, Stream src, int length, CancellationToken cancellationToken)
        {
            this.CheckIndex(index, length);
            int read = UnsafeByteBufferUtil.SetBytes(this, ref this.Addr(index), src, length);
            return Task.FromResult(read);
        }

        public override IByteBuffer Copy(int index, int length)
        {
            this.CheckIndex(index, length);
            return UnsafeByteBufferUtil.Copy(this, ref this.Addr(index), index, length);
        }

        public override int IoBufferCount => 1;

        public override ArraySegment<byte> GetIoBuffer(int index, int length)
        {
            this.CheckIndex(index, length);
            index = this.Idx(index);
            return new ArraySegment<byte>(this.Memory, index, length);
        }

        public override ArraySegment<byte>[] GetIoBuffers(int index, int length) => new[] { this.GetIoBuffer(index, length) };

        public override bool HasArray => true;

        public override byte[] Array
        {
            get
            {
                this.EnsureAccessible();
                return this.Memory;
            }
        }

        public override int ArrayOffset => this.Offset;

        public override bool HasMemoryAddress => true;

        public override ref byte GetPinnableMemoryAddress()
        {
            this.EnsureAccessible();
            return ref this.Memory.AsRef(this.Offset);
        }

        public override IntPtr AddressOfPinnedMemory()
        {
            return new IntPtr(this.memoryAddress);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref byte Addr(int index) => ref this.Memory.AsRef(this.Offset + index);

        public override IByteBuffer SetZero(int index, int length)
        {
            this.CheckIndex(index, length);
            UnsafeByteBufferUtil.SetZero(ref this.Addr(index), length);
            return this;
        }

        public override IByteBuffer WriteZero(int length)
        {
            this.EnsureWritable(length);
            int wIndex = this.WriterIndex;
            UnsafeByteBufferUtil.SetZero(ref this.Addr(wIndex), length);
            this.SetWriterIndex(wIndex + length);
            return this;
        }
    }
}
