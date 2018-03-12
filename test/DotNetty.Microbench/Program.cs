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
            typeof(UnpooledByteBufferBenchmark),
            typeof(PooledByteBufferBenchmark),
            //typeof(FastThreadLocalBenchmark),
            //typeof(SingleThreadEventExecutorBenchmark)
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

    public class MultipleRuntimes : ManualConfig
    {
        public MultipleRuntimes()
        {
            Add(Job.ShortRun.With(Platform.X64).With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp11)); // .NET Core 1.1
            Add(Job.ShortRun.With(Platform.X64).With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp21)); // .NET Core 2.1

            Add(Job.ShortRun.With(Platform.X64).With(Runtime.Clr).With(CsProjClassicNetToolchain.Net46).WithIsBaseline(true)); // NET 4.6
            Add(Job.ShortRun.With(Platform.X64).With(Runtime.Mono).With(CsProjClassicNetToolchain.Net46)); // Mono
        }
    }
}
