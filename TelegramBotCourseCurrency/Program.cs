using TelegramBotCourseCurrency;
using TelegramBotCourseCurrency.Service;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(p => p.ListenLocalhost(7001));
// Add services to the container.
builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddTransient<ITelegramBotService, TelegramBotService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.Services.GetRequiredService<TelegramBot>().StartRecieveBot().Wait();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();