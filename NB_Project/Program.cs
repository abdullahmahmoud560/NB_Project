using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NB_Project.ApplicationDbContext;
using NB_Project.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DB>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:secretKey"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };

});
builder.Services.AddScoped<Token>();

var app = builder.Build();

// ✅ 1. UseHttpsRedirection (يفضل يكون قبل أي شيء)
app.UseHttpsRedirection();

// ✅ 2. UseStaticFiles (مطلوب لـ Swagger وملفات JS/CSS إن وجدت)
app.UseStaticFiles();

// ✅ 3. UseRouting (مهم قبل CORS وAuthentication)
app.UseRouting();

// ✅ 4. UseCors (بعد UseRouting)
app.UseCors(options =>
{
    options.AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials()
           .SetIsOriginAllowed(_ => true);
});

// ✅ 5. UseAuthentication / UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ 6. Swagger (بعد CORS و Auth عادي، لكن يفضل يكون بعد UseRouting)
app.UseSwagger();
app.UseSwaggerUI();

// ✅ 7. MapHub و MapControllers داخل UseEndpoints أو بعدها مباشرة
app.MapHub<ChatHub>("/chathub");
app.MapControllers();

// ✅ 8. Run
app.Run();
