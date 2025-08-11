using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ChatBotJob.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChatBotJob.Interfaces;
using ChatBotJob.Services;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");



// // DB
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlite("Data Source=chatapp.db"));



// Identity (cookie-based with default UI)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LogoutPath = "/Identity/Account/Logout";
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAuthorizationCore();

// App Services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddSingleton<IStockCommandPublisher, StockCommandPublisher>();
// Background consumer that writes bot messages from RabbitMQ → DB
builder.Services.AddHostedService<ChatQueueConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

async Task EnsureBotUserAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var um = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var bot = await um.FindByNameAsync("bot");
    if (bot is null)
    {
        bot = new IdentityUser { UserName = "bot", Email = "bot@local" };
        // simplest password for test env:
        await um.CreateAsync(bot, "Bot@12345");
    }
}

await EnsureBotUserAsync(app);
    
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
