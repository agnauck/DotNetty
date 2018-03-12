// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Buffers.Tests
{
    using Xunit;

    public class UnsafeDirectByteBufferExTest : AbstractByteBufferTests
    {
        protected override IByteBuffer NewBuffer(int length, int maxCapacity)
        {
            IByteBuffer buffer = this.NewDirectBufferEx(length, maxCapacity);
            Assert.Equal(0, buffer.WriterIndex);
            return buffer;
        }

        protected IByteBuffer NewDirectBufferEx(int length, int maxCapacity) =>
            new UnpooledUnsafeDirectByteBufferEx(UnpooledByteBufferAllocator.Default, length, maxCapacity);
    }
}
