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

    public class MultipleRuntimes : ManualConfig
    {
        public MultipleRuntimes()
        {
            //Job cfg = Job.Dry.WithEvaluateOverhead(false);
            Job cfg = Job.Default;
            Add(cfg.UnfreezeCopy().With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp11).WithId("netcoreapp1.1")); // .NET Core 1.1
            Add(cfg.UnfreezeCopy().With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp21).WithId("netcoreapp2.1")); // .NET Core 2.1

            Add(cfg.UnfreezeCopy().With(Runtime.Clr).With(CsProjClassicNetToolchain.Net46).WithIsBaseline(true).WithId("net46")); // NET 4.6
            Add(cfg.UnfreezeCopy().With(Runtime.Mono).WithId("mono")); // Mono
        }
    }
}
