# Yzl.Extensions.Http.OpenFeign Samples

## 中文说明

这是 `Yzl.Extensions.Http.OpenFeign` 的完整示例项目，演示如何用声明式接口发起 HTTP 请求。示例项目位于 `OpenFeign.Samples` 目录，可作为独立 GitHub 仓库维护。

本 demo 覆盖：

- `GET`、`POST`、`PUT`、`PATCH`、`DELETE`、`HEAD`、`OPTIONS`、`TRACE`
- `[PathVariable]`、`[RequestParam]`、`[QueryMap]`、`[RequestHeader]`、`[RequestBody]`
- DTO、`string`、`byte[]`、`Stream`、`HttpContent` 请求体
- `RawFormat = true` 和 `RawFormat = false`
- 自定义 `IFeignResponseResolver`
- 全局请求头 `IFeignRequestHeaderProvider`
- fallback、timeout、retry、hedging、circuit breaker 配置
- SSE：`IAsyncEnumerable<T>` 和 `ISseStream<T>`

当前库暂不支持 form-urlencoded、multipart/form-data 文件上传、cookie 参数绑定。源码中存在对应枚举占位，但没有策略实现，因此文档和示例不会把它们写成已支持功能。

## English

This is a complete demo for `Yzl.Extensions.Http.OpenFeign`, showing how to make HTTP requests through declarative .NET interfaces. The samples live under `OpenFeign.Samples` and can be maintained as a standalone GitHub repository.

The demo covers:

- `GET`, `POST`, `PUT`, `PATCH`, `DELETE`, `HEAD`, `OPTIONS`, `TRACE`
- `[PathVariable]`, `[RequestParam]`, `[QueryMap]`, `[RequestHeader]`, `[RequestBody]`
- DTO, `string`, `byte[]`, `Stream`, and `HttpContent` request bodies
- `RawFormat = true` and `RawFormat = false`
- Custom `IFeignResponseResolver`
- Global headers via `IFeignRequestHeaderProvider`
- Fallback, timeout, retry, hedging, and circuit breaker configuration
- SSE with `IAsyncEnumerable<T>` and `ISseStream<T>`

Form-url-encoded requests, multipart/file upload, and cookie parameter binding are not implemented by the current library version.

## 快速开始 / Quick start

```bash
dotnet build OpenFeign.Samples/OpenFeign.Samples.sln

dotnet run --project OpenFeign.Samples/src/OpenFeign.Samples.Api/OpenFeign.Samples.Api.csproj

dotnet run --project OpenFeign.Samples/src/OpenFeign.Samples.Client/OpenFeign.Samples.Client.csproj
```

默认端口 / Default ports:

- API: `http://localhost:17007`
- Client: `http://localhost:17008`

访问 demo 入口 / Try the demo endpoints:

- `http://localhost:17008/demo/basic`
- `http://localhost:17008/demo/methods`
- `http://localhost:17008/demo/body`
- `http://localhost:17008/demo/advanced`
- `http://localhost:17008/demo/sse`

## 文档 / Docs

- [Getting started / 快速入门](docs/getting-started.md)
- [Attributes reference / 特性参考](docs/attributes-reference.md)
- [Configuration guide / 配置指南](docs/configuration-guide.md)
- [Advanced features / 高级功能](docs/advanced-features.md)

## 项目结构 / Project structure

```text
src/OpenFeign.Samples.Api      被调用的 ASP.NET Core API / provider API
src/OpenFeign.Samples.Client   OpenFeign 调用方 / Feign consumer
docs                        中英双语文档 / bilingual docs
```

## 作为独立仓库发布 / Publishing as a standalone repo

当前示例通过 `ProjectReference` 引用本仓库源码，便于本地开发验证。拆成独立仓库时，把引用改为 NuGet 包即可：

The demo currently references the local source project for easier verification inside this repository. When moving it to a standalone repo, replace the project reference with the NuGet package:

```xml
<ItemGroup>
  <PackageReference Include="Yzl.Extensions.Http.OpenFeign" Version="x.y.z" />
</ItemGroup>
```
