using BattleShipsAPI;
using BattleShipsAPI.Hubs;
using JsonFlatFileDataStore;

const string CorsPolicy = "BattleShipsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<DataStore>((_) => DataStoreFactory.Create());
builder.Services.AddSingleton<UserManager>();
builder.Services.AddSingleton<GameManager>();
builder.Services.AddCors(opts => 
    opts.AddPolicy(CorsPolicy, 
    policy => policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
));

var app = builder.Build();

app.MapHub<BattleShipsHub>("/battleships-hub");

app.MapPost("/login-user", 
    (string username, UserManager userManager) => userManager.CreateOrGetId(username));
app.MapGet("/get-name",
    (string playerId, UserManager userManager) => userManager.GetUsername(playerId));

app.MapGet("/get-session", 
    (string playerId, GameManager gameManager) => gameManager.GetSession(playerId));
app.MapGet("/available-players",
    (GameManager gameManager) => gameManager.GetAvailablePlayers());

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.Run();


