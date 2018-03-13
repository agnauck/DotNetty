// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Microbench
{
    using System;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;
    using BenchmarkDotNet.Toolchains.CsProj;
    using BenchmarkDotNet.Toolchains.DotNetCli;
    using DotNetty.Microbench.Allocators;
    using DotNetty.Microbench.Buffers;
    using DotNetty.Microbench.Concurrency;


    class Program
    {
        static readonly Type[] BenchmarkTypes =
        {
            //typeof(PooledByteBufferAllocatorBenchmark),
            //typeof(UnpooledByteBufferAllocatorBenchmark),
            //typeof(ByteBufferBenchmark),
            //typeof(UnpooledByteBufferBenchmark),
            //typeof(PooledByteBufferBenchmark),
            //typeof(FastThreadLocalBenchmark),
            //typeof(SingleThreadEventExecutorBenchmark)
            typeof(ByteBufferEqualsBenchmark),
            typeof(PooledHeapByteBuffer4Benchmark),
            typeof(PooledUnsafeDirectByteBuffer4Benchmark),
            typeof(PooledUnsafeDirectByteBufferEx4Benchmark),
            typeof(UnpooledHeapByteBuffer4Benchmark),
            typeof(UnpooledUnsafeDirectByteBuffer4Benchmark),
            typeof(UnpooledUnsafeDirectByteBufferEx4Benchmark),
        };

        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(BenchmarkTypes);

            if (args == null || args.Length == 0)
            {
                switcher.RunAll();
            }
            else
            {
                switcher.Run(args);
            }
        }
    }
}
