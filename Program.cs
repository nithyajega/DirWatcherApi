using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DirWatcherApi.Services;
using DirWatcherApi.Data;
using Microsoft.EntityFrameworkCore;
using DirWatcherApi.Models;
using Microsoft.Extensions.DependencyInjection;
using DirWatcherApi.Controllers;

internal class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //builder.Services.AddHostedService<DirWatcherBackgroundService>();
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DirWatcherConnectionString")));
        builder.Services.AddSingleton<FileWatcherService>();
        
        var app = builder.Build();
        var fileWatcherService = app.Services.GetRequiredService<FileWatcherService>();
        fileWatcherService.StartScheduledMonitoring();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
        

    }


}
