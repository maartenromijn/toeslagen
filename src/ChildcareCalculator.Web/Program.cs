using ChildcareCalculator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // For development, use SQLite
    // For production, this would be configured via environment variables
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=childcare_calculator.db";
    options.UseSqlite(connectionString);
    
    // Enable sensitive data logging for development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Add application services
builder.Services.AddScoped<ChildcareCalculator.Application.Services.RoundingService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.IncomeCalculationService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.LeaveCalculationService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.MortgageCalculationService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.HomeCalculationService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.AssessmentIncomeService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.ChildcareCostService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.ChildcareBenefitService>();
builder.Services.AddScoped<ChildcareCalculator.Application.Services.JsonImportExportService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
