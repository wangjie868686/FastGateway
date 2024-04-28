﻿namespace FastGateway.Services;

public static class StatisticRequestService
{
    /// <summary>
    /// 获取今天的统计数据
    /// </summary>
    public static async Task<ResultDto<StatisticRequestCountDto>> GetDayStatisticAsync(IFreeSql freeSql,
        string? serviceId)
    {
        var now = DateTime.Now;
        var today = new DateTime(now.Year, now.Month, now.Day);

        var year = today.Year;
        var month = today.Month;
        var day = today.Day;

        var statistic = await freeSql.Select<StatisticRequestCount>()
            .Where(x => (string.IsNullOrEmpty(serviceId) || x.ServiceId == serviceId) && x.Year == year &&
                        x.Month == month &&
                        x.Day == day)
            .FirstAsync();

        var currentStatistic = StatisticsBackgroundService.GetCurrentRequestCount(serviceId);

        if (statistic == null)
        {
            return ResultDto<StatisticRequestCountDto>.SuccessResult(new StatisticRequestCountDto()
            {
                Error4xxCount = currentStatistic.Error4xxCount,
                Error5xxCount = currentStatistic.Error5xxCount,
                RequestCount = currentStatistic.RequestCount
            });
        }

        return ResultDto<StatisticRequestCountDto>.SuccessResult(new StatisticRequestCountDto
        {
            RequestCount = statistic.RequestCount + currentStatistic.RequestCount,
            Error4xxCount = statistic.Error4xxCount + currentStatistic.Error4xxCount,
            Error5xxCount = statistic.Error5xxCount + currentStatistic.Error5xxCount,
        });
    }

    /// <summary>
    /// 获取数据总数统计
    /// </summary>
    /// <param name="freeSql"></param>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    public static async Task<ResultDto<StatisticRequestCountDto>> GetTotalStatisticAsync(
        IFreeSql freeSql,
        string? serviceId)
    {
        var query = freeSql.Select<StatisticRequestCount>().Where(x =>
            string.IsNullOrEmpty(serviceId) || x.ServiceId == serviceId);

        var currentStatistic = StatisticsBackgroundService.GetCurrentRequestCount(serviceId);

        var requestCount = await query.SumAsync(x => x.RequestCount);

        var error4XxCount = await query.SumAsync(x => x.Error4xxCount);

        var error5XxCount = await query.SumAsync(x => x.Error5xxCount);

        return ResultDto<StatisticRequestCountDto>.SuccessResult(new StatisticRequestCountDto
        {
            RequestCount = (int)(requestCount + currentStatistic.RequestCount),
            Error4xxCount = (int)(error4XxCount + currentStatistic.Error4xxCount),
            Error5xxCount = (int)(error5XxCount + currentStatistic.Error5xxCount),
        });
    }

    /// <summary>
    /// 获取归属地统计
    /// </summary>
    /// <param name="freeSql"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static async Task<List<StatisticLocationDto>> GetStatisticLocationAsync(IFreeSql freeSql, DateTime? now)
    {
        var query = freeSql.Select<StatisticIp>();
        
        if(now != null)
        {
            var year = now.Value.Year;
            var month = now.Value.Month;
            var day = now.Value.Day;

            query = query.Where(x => x.Year == year && x.Month == month && x.Day == day);
        }

        var result = await query
            .GroupBy(e => e.Location)
            .OrderByDescending(e => e.Sum(e.Value.Count))
            .ToListAsync(e => new StatisticLocationDto
            {
                Location = e.Key,
                Count = e.Sum(e.Value.Count)
            });

        // 计算比例
        var total = result.Sum(x => x.Count);
        result.ForEach(x =>
        {
            x.Ratio = Math.Round((x.Count / total) * 100, 2);

            var locations = x.Location?.Split("|", StringSplitOptions.RemoveEmptyEntries);

            if (locations == null || locations.Length == 0)
                return;

            x.Country = locations.FirstOrDefault();

            if (locations.Length == 1)
                return;

            x.Province = locations[1];
        });

        return result;
    }

    /// <summary>
    /// 获取七天的归属地统计
    /// </summary>
    /// <param name="freeSql"></param>
    /// <returns></returns>
    public static async Task<List<DayStatisticLocationCountDto>> GetDayStatisticLocationCountAsync(IFreeSql freeSql)
    {
        var now = DateTime.Now.AddDays(-7);
        var today = new DateTime(now.Year, now.Month, now.Day);

        var year = today.Year;
        var month = today.Month;
        var day = today.Day;

        var result = await freeSql.Select<StatisticIp>()
            .Where(x => x.Year == year &&
                        x.Month == month &&
                        x.Day >= day)
            .GroupBy(e => e.Ip)
            .OrderByDescending(e => e.Sum(e.Value.Count))
            .Take(10)
            .ToListAsync(e => new DayStatisticLocationCountDto
            {
                Ip = e.Key,
                Count = e.Sum(e.Value.Count),
                Location = e.Value.Location
            });

        return result;
    }
}