using DailyGeneration.Api.Extensions;
using DailyGeneration.Api.Services;
using DailyGeneration.Application.Services;
using DailyGeneration.Domain.Entities;
using DailyGeneration.Infrastructure.EntityFrameworkCore;
using DailyGeneration.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//EntityFramework Core��������
builder.Services.AddDbContextFactory<TodoDbContext>(option =>
{
    option.UseSqlServer("server=.;database=Todo;user id=laiwang;password=123123;TrustServerCertificate=true;");
    option.UseOpenIddict();
});

//Identity��������
builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<TodoDbContext>()
                .AddDefaultTokenProviders();

builder.Services.OpenIddictConfigureServices();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    // ���õ�¼�͵ǳ�·��
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
});

//����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173").AllowAnyHeader();
    });
});

builder.Services.AddScoped<TodoRepository>();
builder.Services.AddScoped<TodoApplication>();
builder.Services.AddScoped<AuthorizationApplication>();

var app = builder.Build();

//����Ա�û���ɫ��ʼ��
await IdentityExtension.SeedAdminUserAsync(app.Services.CreateScope().ServiceProvider, app.Configuration);

app.UseCors("AllowOrigins");

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
