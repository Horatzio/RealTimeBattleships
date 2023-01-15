using BattleShipsAPI;
using BattleShipsAPI.GameSession;
using Microsoft.AspNetCore.Mvc;

const string CorsPolicy = "BattleShipsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBattleshipServices();
builder.Services.AddCors(opts =>
	opts.AddPolicy(CorsPolicy,
	policy => policy.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod()
));

var app = builder.Build();

app.MapPost("/login-user",
	(string username, UserManager userManager) => userManager.CreateOrGetIdAsync(username));

app.MapGet("/get-session",
	(string playerId, GameManager gameManager) => gameManager.GetSessionAsync(playerId));
app.MapPost("/start-game",
	(string playerId, GameManager gameManager) => gameManager.StartGame(playerId));
app.MapPost("/submit-player-positions",
	(string playerId, string sessionId, [FromBody] ShipPositions[] playerPositions, GameManager gameManager) =>
	gameManager.SubmitPlayerPositions(playerId, sessionId, playerPositions));
app.MapPost("/player-shot",
	(string playerId, string sessionId, [FromBody] PlayerShotRequest request, GameManager gameManager) => gameManager.PlayerShot(playerId, sessionId, request.Position));
app.MapPost("/end-session",
	(string sessionId, GameManager gameManager) => gameManager.EndSession(sessionId));


app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.Run();


