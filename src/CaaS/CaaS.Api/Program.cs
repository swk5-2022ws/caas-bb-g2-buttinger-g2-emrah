using Caas.Core.Common.Ado;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Interfaces.Services;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using System.Configuration;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
IConfiguration _configuration = configurationBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCartRepository, ProductCartRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IDiscountActionRepository, DiscountActionRepository>();
builder.Services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
builder.Services.AddScoped<IDiscountCartRepository, DiscountCartRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICouponLogic, CouponLogic>();
builder.Services.AddScoped<IShopLogic, ShopLogic>();
builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddScoped<IDiscountActionLogic, DiscountActionLogic>();
builder.Services.AddScoped<IDiscountLogic, DiscountLogic>();
builder.Services.AddScoped<IDiscountRuleLogic, DiscountRuleLogic>();
builder.Services.AddScoped<IOrderLogic, OrderLogic>();
builder.Services.AddScoped<IAdoTemplate, AdoTemplate>();
builder.Services.AddScoped<ICleanupService, CleanupService>();
builder.Services.AddAutoMapper(typeof(Program));


builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/jobs");

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var jobService = services.GetRequiredService<ICleanupService>();

    RecurringJob.AddOrUpdate("Remove old carts", () => jobService.RemoveNotUsedCartsAsync(), Cron.Minutely);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
