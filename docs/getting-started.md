# Getting started / 快速入门

## 中文说明

### 1. 安装 NuGet 包

独立项目中使用：

```bash
dotnet add package Yzl.Extensions.Http.OpenFeign
```

当前 demo 在仓库内引用：

```xml
<PackageReference Include="Yzl.Extensions.Http.OpenFeign" Version="0.1.16" />
```

### 2. 注册 OpenFeign

```csharp
using Yzl.Extensions.Http.OpenFeign;
using Yzl.Extensions.Http.OpenFeign.Serializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFeignStarter(builder.Configuration, options =>
{
    options.SerializerType = typeof(SystemTextJsonFeignSerializer);
});
```

### 3. 声明 Feign Client

```csharp
[FeignClient(name: "demo-api", url: "{DemoApi:BaseUrl}", timeout: 5000)]
public interface IDemoFeignClient
{
    [Get("/api/users/{id}")]
    Task<UserDto> GetById([PathVariable("id")] long id);
}
```

`url` 支持 `{config:key}` 形式，会从配置读取值：

```json
{
  "DemoApi": {
    "BaseUrl": "http://localhost:17007"
  }
}
```

### 4. 注入并调用

```csharp
app.MapGet("/demo", async (IDemoFeignClient client) =>
{
    return await client.GetById(1);
});
```

## English

### 1. Install the NuGet package

For a standalone application:

```bash
dotnet add package Yzl.Extensions.Http.OpenFeign
```

Inside this repository, the demo uses :

```xml
<PackageReference Include="Yzl.Extensions.Http.OpenFeign" Version="0.1.16" />
```

### 2. Register OpenFeign

```csharp
using Yzl.Extensions.Http.OpenFeign;
using Yzl.Extensions.Http.OpenFeign.Serializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFeignStarter(builder.Configuration, options =>
{
    options.SerializerType = typeof(SystemTextJsonFeignSerializer);
});
```

### 3. Declare a Feign client

```csharp
[FeignClient(name: "demo-api", url: "{DemoApi:BaseUrl}", timeout: 5000)]
public interface IDemoFeignClient
{
    [Get("/api/users/{id}")]
    Task<UserDto> GetById([PathVariable("id")] long id);
}
```

The `url` property supports `{config:key}` placeholders resolved from configuration:

```json
{
  "DemoApi": {
    "BaseUrl": "http://localhost:17007"
  }
}
```

### 4. Inject and call it

```csharp
app.MapGet("/demo", async (IDemoFeignClient client) =>
{
    return await client.GetById(1);
});
```
