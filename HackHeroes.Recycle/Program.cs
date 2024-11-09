using Carter;
using HackHeroes.Recycle.Extensions;
using Modules.Projects.Extensions;
using Modules.Users.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectsServices(builder.Configuration);
builder.Services.AddUsersModuleServices(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("WithFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")    
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

app.UseCors("WithFrontend");
app.MapCarter();
app.UseHttpsRedirection();

app.Run();
