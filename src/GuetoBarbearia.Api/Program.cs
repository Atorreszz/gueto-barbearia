using GuetoBarbearia.Api;
using GuetoBarbearia.Api.Data;
using GuetoBarbearia.Api.models;
using GuetoBarbearia.Api.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

// Carrega variáveis do arquivo .env se existir localmente
CarregarArquivoEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Insira o token JWT no campo abaixo usando o seguinte formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    x.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(ConnectionString))
{
    throw new InvalidOperationException(
        "A string de conexão ('ConnectionStrings:DefaultConnection') não foi configurada! " +
        "Configure no User Secrets ('dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"sua_string\"') " +
        "ou no arquivo .env / variáveis de ambiente.");
}

builder.Services.AddNpgsql<GuetoDbContext>(ConnectionString);
builder.Services.AddScoped<IServicos, ServicosDataBaseService>();
builder.Services.AddScoped<IAgendamento, AgendamentosDataBaseService>();
builder.Services.AddHostedService<LimpezaAgendamentosBackgroundService>();

var JwtConfig = builder.Configuration.GetSection("Jwt");
var jwtKey = JwtConfig["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "A chave JWT ('Jwt:Key') não foi configurada! " +
        "Configure no User Secrets ('dotnet user-secrets set \"Jwt:Key\" \"sua_chave\"') " +
        "ou no arquivo .env / variáveis de ambiente.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

void CarregarArquivoEnv()
{
    var caminhos = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(AppContext.BaseDirectory, ".env"),
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env")
    };

    foreach (var caminho in caminhos)
    {
        if (File.Exists(caminho))
        {
            foreach (var linha in File.ReadAllLines(caminho))
            {
                var trim = linha.Trim();
                if (string.IsNullOrWhiteSpace(trim) || trim.StartsWith("#")) continue;
                var partes = trim.Split('=', 2);
                if (partes.Length == 2)
                {
                    var k = partes[0].Trim();
                    var v = partes[1].Trim().Trim('"').Trim('\'');
                    Environment.SetEnvironmentVariable(k, v);
                    if (k.Contains("__"))
                    {
                        Environment.SetEnvironmentVariable(k.Replace("__", ":"), v);
                    }
                }
            }
            break;
        }
    }
}
