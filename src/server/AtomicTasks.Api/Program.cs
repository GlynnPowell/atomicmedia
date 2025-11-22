using AtomicTasks.Application.Tasks;
using AtomicTasks.Infrastructure;
using AtomicTasks.Infrastructure.Tasks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=AtomicTasks.db";

builder.Services.AddDbContext<AtomicTasksDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Task endpoints will be mapped here in the next step.

app.Run();
