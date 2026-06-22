using System;
using Microsoft.Extensions.DependencyInjection;
using Yzl.Extensions.Http.OpenFeign;
using Yzl.Extensions.Http.OpenFeign.Serializer;

namespace OpenFeign.Net48.Samples.Client
{
    /// <summary>
    /// OpenFeign.Net48 静态 Service Locator 配置
    ///
    /// 适用于 .NET Framework 4.8 项目（如 WebForms、MVC、WinForms），
    /// 在 Global.asax 或应用启动时调用 FeignConfig.Register()，
    /// 然后通过 FeignConfig.GetFeignClient<T>获取 Feign 客户端。
    ///
    /// 使用方式（Global.asax.cs）：
    /// <code>
    /// protected void Application_Start()
    /// {
    ///     FeignConfig.Register();
    /// }
    /// </code>
    ///
    /// 使用方式（任意代码位置）：
    /// <code>
    /// var client = FeignConfig.GetFeignClient<IDemoFeignClient>();
    /// var user = await client.GetById(1);
    /// </code>
    /// </summary>
    public static class FeignConfig
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 注册 OpenFeign 服务。
        /// 在应用启动时调用一次（如 Global.asax Application_Start）。
        /// </summary>
        public static void Register()
        {
            try
            {
                var services = new ServiceCollection();

                // 注册 OpenFeign 客户端
                // 注意：FeignClient 接口中的 url 建议使用硬编码地址或读取配置文件，
                //       本示例使用硬编码 http://localhost:17007
                services.AddFeignStarter(options =>
                {
                    // 全局默认超时（毫秒）
                    options.Default.Timeout = 5000;

                    // 重试策略
                    options.Default.Retry.Enabled = true;
                    options.Default.Retry.MaxAttempts = 3;
                    options.Default.Retry.DelayMs = 500;

                    // 熔断器
                    options.Default.CircuitBreaker.Enabled = true;
                    options.Default.CircuitBreaker.MinimumThroughput = 5;
                    options.Default.CircuitBreaker.BreakSeconds = 10;

                    // 连接池
                    options.HttpClient.Pool.MaxConnections = 20;
                    options.HttpClient.Pool.ConnectionLifetime = TimeSpan.FromMinutes(5);
                    options.HttpClient.Pool.IdleTimeout = TimeSpan.FromMinutes(2);

                    // BasicAuth（可选，按需开启）
                    // options.BasicAuth.Enabled = true;
                    // options.BasicAuth.UserName = "admin";
                    // options.BasicAuth.Password = "123456";

                    // 序列化器
                    options.SerializerType = typeof(SystemTextJsonFeignSerializer);
                });

                ServiceProvider = services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
                // 实际项目中建议使用日志记录
                // Logger.Error(msg);
            }
        }

        /// <summary>
        /// 获取 Feign 客户端实例
        /// </summary>
        /// <typeparam name="T">Feign 客户端接口类型</typeparam>
        /// <returns>Feign 客户端代理实例</returns>
        public static T GetFeignClient<T>() where T : class
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("FeignConfig has not been initialized. Call FeignConfig.Register() first.");
            }
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
