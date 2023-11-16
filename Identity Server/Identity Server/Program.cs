using Identity_Server.Extensions;
using Identity_Server.Interfaces;
using Identity_Server.Services;
using Serilog;
using Serilog.Events;


#region Configure logging(serilog)

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Debug)
    .CreateLogger();

#endregion

Log.Information("Loging Configured Successfully");



var builder = WebApplication.CreateBuilder();


builder.Services.AddControllers();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddAuthorization();






builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();