
using congestion.calculator;
using CongestionTaxCalculator.Api.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionTaxCalculator.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddScoped<ICalculationHandler, CalculationHandler>(ServiceProvider =>
                new CalculationHandler(
                    freeHourPrice: builder.Configuration.GetSection("FreeHourPrice").Get<int>()
                    , normalHourPrice: builder.Configuration.GetSection("NormalHourPrice").Get<int>()
                    , rushHourPrice: builder.Configuration.GetSection("RushHourPrice").Get<int>()
                    , extraRushHourPrice: builder.Configuration.GetSection("ExtraRushHourPrice").Get<int>()
                    , freeVehicles: builder.Configuration.GetSection("FreeVehicles").Get<int[]>()
                    , freeMonths: builder.Configuration.GetSection("FreeMonths").Get<int[]>()
                    , freeHourRanges: builder.Configuration.GetSection("FreeHourRanges").Get<string[]>()
                    , normalHourRanges: builder.Configuration.GetSection("NormalHourRanges").Get<string[]>()
                    , rushHourRanges: builder.Configuration.GetSection("RushHourRanges").Get<string[]>()
                    , extraRushHourRanges: builder.Configuration.GetSection("ExtraRushHourRanges").Get<string[]>()
                    , hasExtraRushHour: builder.Configuration.GetSection("HasExtraRushHour").Get<bool>()
                    , isFreeForDaysBeforeHolidays: builder.Configuration.GetSection("IsFreeForDaysBeforeHolidays").Get<bool>()
                    , payOnceInAnHour: builder.Configuration.GetSection("PayOnceInAnHour").Get<bool>()
                    , holidays: builder.Configuration.GetSection("Holidays").Get<string[]>()
                    ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
