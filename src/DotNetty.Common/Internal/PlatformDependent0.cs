// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Common.Internal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    static class PlatformDependent0
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe bool ByteArrayEquals(byte* bytes1, int startPos1, byte* bytes2, int startPos2, int length)
        {
            if (length <= 0)
            {
                return true;
            }

            byte* baseOffset1 = bytes1 + startPos1;
            byte* baseOffset2 = bytes2 + startPos2;
            int remainingBytes = length & 7;
            byte* end = baseOffset1 + remainingBytes;
            for (byte* i = baseOffset1 - 8 + length, j = baseOffset2 - 8 + length; i >= end; i -= 8, j -= 8)
            {
                if (Unsafe.ReadUnaligned<long>(i) != Unsafe.ReadUnaligned<long>(j))
                {
                    return false;
                }
            }

            if (remainingBytes >= 4)
            {
                remainingBytes -= 4;
                if (Unsafe.ReadUnaligned<int>(baseOffset1 + remainingBytes) != Unsafe.ReadUnaligned<int>(baseOffset2 + remainingBytes))
                {
                    return false;
                }
            }
            if (remainingBytes >= 2)
            {
                return Unsafe.ReadUnaligned<short>(baseOffset1) == Unsafe.ReadUnaligned<short>(baseOffset2)
                    && (remainingBytes == 2 || *(bytes1 + startPos1 + 2) == *(bytes2 + startPos2 + 2));
            }
            return *baseOffset1 == *baseOffset2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool ByteArrayEquals(byte[] bytes1, int startPos1, byte[] bytes2, int startPos2, int length) =>
            new ReadOnlySpan<byte>(bytes1, startPos1, length).SequenceEqual(new ReadOnlySpan<byte>(bytes2, startPos2, length));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ByteArrayCompareTo(byte[] bytes1, int startPos1, int len1, byte[] bytes2, int startPos2, int len2) =>
            new ReadOnlySpan<byte>(bytes1, startPos1, len1).SequenceCompareTo(new ReadOnlySpan<byte>(bytes2, startPos2, len2));
    }
}
