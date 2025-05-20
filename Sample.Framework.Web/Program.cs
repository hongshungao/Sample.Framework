using Microsoft.EntityFrameworkCore;
using Sample.Framework.Core.Data;
using Sample.Framework.Generator;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// 配置Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Db/sample_framework.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 配置代码生成器
var templateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..\\Sample.Framework.Generator\\Templates");
var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..\\Generated");
builder.Services.AddSingleton(new CodeGenerator(templateDirectory, outputDirectory));

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // 确保数据库已创建
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// 配置前端静态文件
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "" // Serve files from the root URL path, e.g., /index.html
});
app.MapFallbackToFile("index.html"); // Will serve ClientApp/dist/index.html due to the above UseStaticFiles config

app.Run();