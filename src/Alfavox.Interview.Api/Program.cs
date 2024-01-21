using Alfavox.Interview.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient();
builder.Services.AddTransient<ISkywalkerService, SkywalkerService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IHttpContextWrapper, HttpContextWrapper>();
builder.Services.AddTransient<IHypermediaService, HypermediaService>();
builder.Services.AddSingleton<ILoggingService, LoggingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndClient", builder =>
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin()
        );
});

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEndClient");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();