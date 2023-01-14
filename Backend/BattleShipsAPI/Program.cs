using BattleShipsAPI;
using BattleShipsAPI.GameSession;
using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Mvc;

const string CorsPolicy = "BattleShipsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DataStore>((_) => BattleShipsDataStoreFactory.Create());
builder.Services.AddSingleton<UserManager>();
builder.Services.AddSingleton<IShipFactory, DefaultShipFactory>();
builder.Services.AddSingleton<GameManager>();
builder.Services.AddCors(opts => 
    opts.AddPolicy(CorsPolicy, 
    policy => policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
));

var app = builder.Build();

app.MapPost("/login-user", 
    (string username, UserManager userManager) => userManager.CreateOrGetId(username));
app.MapGet("/get-name",
    (string playerId, UserManager userManager) => userManager.GetUsername(playerId));


app.MapGet("/get-session",
    (string playerId, GameManager gameManager) => gameManager.GetSession(playerId));
app.MapPost("/start-game", 
    (string playerId, GameManager gameManager) => gameManager.StartGame(playerId));
app.MapPost("/submit-player-positions",
    (string playerId, string sessionId, [FromBody] ShipPositions[] playerPositions, GameManager gameManager) =>
    gameManager.SubmitPlayerPositions(playerId, sessionId, playerPositions));
app.MapPost("/player-shot",
    (string playerId, string sessionId, [FromBody] PlayerShotRequest request, GameManager gameManager) => gameManager.PlayerShot(playerId, sessionId, request.Position));

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.Run();


