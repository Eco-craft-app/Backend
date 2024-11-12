using Carter;
using HackHeroes.Recycle.ExceptionHandlers;
using HackHeroes.Recycle.Extensions;
using Modules.Projects.Extensions;
using Modules.Users.Extensions;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<RecycleExceptionHandler>();
builder.Services.AddProjectsServices(builder.Configuration);
builder.Services.AddOtlp(builder.Configuration);
builder.Services.AddUsersModuleServices(builder.Configuration);
builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());
builder.Services.AddCors(options =>
{
    options.AddPolicy("WithFrontend",
        builders =>
        {
            builders.WithOrigins(builder.Configuration.GetValue<string>("FrontendUrl")!)    
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}
app.UseExceptionHandler(_ => { });
app.UseCors("WithFrontend");
app.MapCarter();
app.UseHttpsRedirection();

app.Run();
