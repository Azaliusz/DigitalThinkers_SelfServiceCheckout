using SelfServiceCheckout.Configurations;
using SelfServiceCheckout.Data;
using SelfServiceCheckout.Repositories.Abstractions;
using SelfServiceCheckout.Repositories.Implementations;
using SelfServiceCheckout.Services.Abstractions;
using SelfServiceCheckout.Services.Implementations;

namespace SelfServiceCheckout
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            if (string.Equals(Environment.GetEnvironmentVariable(DataBaseValues.DATABASE), DataBaseValues.SQLite))
            {
                builder.Services.AddScoped<SelfServiceCheckoutDbContext, SelfServiceCheckoutSQLiteDbContext>();
            }
            else
            {
                builder.Services.AddScoped<SelfServiceCheckoutDbContext, SelfServiceCheckoutInMemoryDbContext>();
            }

            builder.Services.AddScoped<IMoneyDenominationRepository, MoneyDenominationRepository>();
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddScoped<ICheckoutService, CheckoutService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.Configure<MoneyOptions>(builder.Configuration.GetSection(nameof(MoneyOptions)));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}