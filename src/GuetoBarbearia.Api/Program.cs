using GuetoBarbearia.Api;
using GuetoBarbearia.Api.Data;
using GuetoBarbearia.Api.models;
using GuetoBarbearia.Api.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<GuetoDbContext>(ConnectionString);
builder.Services.AddScoped<IServicos, ServicosDataBaseService>();
builder.Services.AddScoped<IAgendamento, AgendamentosDataBaseService>();
builder.Services.AddHostedService<LimpezaAgendamentosBackgroundService>();
var JwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(JwtConfig["Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtConfig["Issuer"],
        ValidAudience = JwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
        
    };
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
