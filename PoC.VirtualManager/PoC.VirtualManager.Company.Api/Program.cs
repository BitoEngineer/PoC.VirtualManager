using PoC.VirtualManager.Company.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mongoDbConnectionString = Environment.GetEnvironmentVariable("VirtualManager_MongoDb_ConnectionString") ?? throw new Exception("VirtualManager_MongoDb_ConnectionString env variable must be set");
builder.Services.AddTeamsRespositories(mongoDbConnectionString);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();