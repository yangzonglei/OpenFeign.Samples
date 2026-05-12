# Configuration guide / 配置指南

## appsettings.json

```json
{
  "DemoApi": {
    "BaseUrl": "http://localhost:17007"
  },
  "spring": {
    "feign": {
      "default": {
        "timeout": 5000,
        "retry": {
          "enabled": true,
          "maxAttempts": 2,
          "delayMs": 200
        },
        "hedging": {
          "enabled": false,
          "maxAttempts": 2,
          "delayMs": 100
        },
        "circuitBreaker": {
          "enabled": false,
          "failureRatio": 0.5,
          "samplingSeconds": 30,
          "minimumThroughput": 10,
          "breakSeconds": 10
        }
      },
      "httpClient": {
        "pooledConnectionLifetimeSeconds": 300,
        "pooledConnectionIdleTimeoutSeconds": 120,
        "maxConnectionsPerServer": 50
      },
      "basicAuth": {
        "enabled": false,
        "username": "demo-user",
        "password": "demo-password"
      }
    }
  }
}
```

中文：`spring:feign:default` 是全局默认配置；方法上的 `timeout` 会覆盖客户端默认 timeout。

English: `spring:feign:default` contains global defaults. A method-level `timeout` overrides the client default timeout.

## 代码配置 / Configure in code

```csharp
builder.Services.AddFeignStarter(builder.Configuration, options =>
{
    options.SerializerType = typeof(SystemTextJsonFeignSerializer);
});
```

中文：`SerializerType` 可以指定序列化器类型。示例使用 `SystemTextJsonFeignSerializer`。

English: `SerializerType` selects the serializer implementation. The demo uses `SystemTextJsonFeignSerializer`.

## URL 配置占位符 / URL placeholders

```csharp
[FeignClient(name: "demo-api", url: "{DemoApi:BaseUrl}")]
public interface IDemoFeignClient
{
}
```

中文：`{DemoApi:BaseUrl}` 会从配置中读取 `DemoApi:BaseUrl`。

English: `{DemoApi:BaseUrl}` is resolved from configuration key `DemoApi:BaseUrl`.

## Timeout / 超时

```csharp
[FeignClient(name: "demo-api", url: "{DemoApi:BaseUrl}", timeout: 5000)]
public interface IDemoFeignClient
{
    [Get("/api/timeout", timeout: 1000)]
    string TimeoutWithFallback();
}
```

中文：客户端级 timeout 是默认值，方法级 timeout 优先级更高。

English: The client timeout is the default. The method-level timeout has higher priority.

## Retry / 重试

中文：retry 适合幂等请求，例如 GET。对 POST/PUT/PATCH 等非幂等请求开启重试前应评估业务影响。

English: Retry is best suited for idempotent requests such as GET. Evaluate business impact before enabling it for POST/PUT/PATCH.

## Hedging / 对冲请求

中文：hedging 会并发或延迟发起额外请求以降低尾延迟，通常只适合幂等读请求。

English: Hedging sends additional requests to reduce tail latency and is usually appropriate only for idempotent reads.

## Circuit breaker / 熔断

中文：熔断器用于在下游持续失败时快速失败，避免请求堆积。

English: The circuit breaker fails fast when the downstream service keeps failing, preventing request buildup.

## Basic Auth

中文：启用 Basic Auth 后，OpenFeign 会为请求添加基础认证信息。

English: When Basic Auth is enabled, OpenFeign adds the basic authentication header to requests.

```json
{
  "spring": {
    "feign": {
      "basicAuth": {
        "enabled": true,
        "username": "demo-user",
        "password": "demo-password"
      }
    }
  }
}
```
