using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Simulation.Database;
using Simulation.Common.Enums;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(typeof(TelemetryFields), new EnumSerializer<TelemetryFields>(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(typeof(UAVTypes), new EnumSerializer<UAVTypes>(MongoDB.Bson.BsonType.String));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddOpenApi();
builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();