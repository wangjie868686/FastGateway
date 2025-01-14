﻿namespace FastGateway.BackgroundServices;

public sealed class RenewSslBackgroundService(IServiceProvider serviceProvider, IMemoryCache memoryCache)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("证书自动续期启动成功，基于Let's Encrypt 的免费证书");


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var freeSql = scope.ServiceProvider.GetRequiredService<IFreeSql>();

                var certs = await freeSql.Select<Cert>()
                    .ToListAsync(stoppingToken);

                // 如果证书过期或者快过期，就续期 7 天内过期的证书
                foreach (var certItem in certs.Where(x => x.NotAfter == null || x.NotAfter < DateTime.Now.AddDays(7)))
                {
                    try
                    {
                        // 申请证书，使用Let's Encrypt，需要先注册账户
                        var context = await CertService.RegisterWithLetsEncrypt(certItem.Email);

                        // 申请证书
                        await CertService.ApplyForCert(memoryCache, context, certItem);

                        await freeSql.Update<Cert>()
                            .Set(x => x.RenewStats, certItem.RenewStats)
                            .Set(x => x.RenewTime, certItem.RenewTime)
                            .Set(x => x.NotAfter, certItem.NotAfter)
                            .Set(x => x.Expired, certItem.Expired)
                            .Set(x => x.Certs, certItem.Certs)
                            .Where(x => x.Id == certItem.Id)
                            .ExecuteAffrowsAsync(stoppingToken);

                        // 成功以后需要刷新证书列表
                        await CertService.LoadCerts(freeSql);
                    }
                    catch (Exception e)
                    {
                        certItem.RenewStats = RenewStats.Fail;
                        certItem.RenewTime = DateTime.Now;
                        certItem.NotAfter = DateTime.Now.AddDays(-1);
                        certItem.Expired = true;
                        certItem.ClearCerts();

                        await freeSql.Update<Cert>()
                            .Set(x => x.RenewStats, certItem.RenewStats)
                            .Set(x => x.RenewTime, certItem.RenewTime)
                            .Set(x => x.NotAfter, certItem.NotAfter)
                            .Set(x => x.Expired, certItem.Expired)
                            .Where(x => x.Id == certItem.Id)
                            .ExecuteAffrowsAsync(stoppingToken);

                        Console.WriteLine($"域名：{string.Join(';', certItem.Domains)} 证书续期失败：" + e.Message);
                    }
                }

                // 等待12小时
                await Task.Delay(1000 * 60 * 60 * 12, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}