﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastGateway.Core;

namespace Benchmark.FastGateway;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser, MaxColumn]

public class CheckIpInIp
{
    // 帮我提供一百个ip ip范围 ip段的案例
    private static string Ips;

    private static string IpRanges;

    private static string IpSegments;

    [GlobalSetup]
    public void Setup()
    {
        Ips = $"192.168.2.10";
        IpRanges = $"192.168.2.1-192.168.2.100";
        IpSegments = $"192.168.2.1/24";
    }

    [Benchmark]
    public void UnsafeCheckIpInIpRange()
    {
        for (var i = 0; i < 10000; i++)
        {
            IpHelper.UnsafeCheckIpInIpRange(Ips, IpRanges);
        }
    }

    [Benchmark]
    public void CheckIpInIpRange()
    {
        for (var i = 0; i < 10000; i++)
        {
            IpHelper.CheckIpInIpRange(Ips, IpRanges);
        }
    }

}