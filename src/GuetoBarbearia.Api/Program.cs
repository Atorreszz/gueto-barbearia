using GuetoBarbearia.Api;
using GuetoBarbearia.Api.Data;
using GuetoBarbearia.Api.models;
using GuetoBarbearia.Api.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<GuetoDbContext>(ConnectionString);
builder.Services.AddScoped<IServicos,ServicosDataBaseService>();
builder.Services.AddScoped<IAgendamento, AgendamentosDataBaseService>();
builder.Services.AddHostedService<LimpezaAgendamentosBackgroundService>();
var app = builder.Build();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






app.Run();


