var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:17007");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/", () => "Yzl.Extensions.Http.OpenFeign Samples API");
app.MapControllers();

app.Run();
