using FoolGame.Core;
using Microsoft.AspNetCore.Mvc;

namespace FoolGame.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController
{
    private readonly Handlers.GameHandler _gameHandler;

    public GameController(Handlers.GameHandler gameHandler)
    {
        _gameHandler = gameHandler;
    }

    [HttpPost("new")]
    public Task<Guid> NewGame(int playersCount) => _gameHandler.create(playersCount);

    [HttpGet("list")]
    public async Task<List<Handlers.GameStatusResult>> ListGames() => (await _gameHandler.list()).ToList();
    
    [HttpPost("join")]
    public Task<Guid> JoinGame(Guid gameId) => _gameHandler.join(gameId);

    [HttpPost("start")]
    public Task StartGame(Guid gameId) => _gameHandler.start(gameId);

    [HttpPost("play")]
    public Task<Handlers.GameStateResult> Play(Guid token, int cardIndex) => _gameHandler.play(token, cardIndex);

    [HttpGet("state")]
    public Task<Handlers.GameStateResult> StateGame(Guid token) => _gameHandler.state(token);

    [HttpPost("beat")]
    public Task<Handlers.GameStateResult> Beat(Guid token, int handCardIndex, int tableCardIndex) =>
        _gameHandler.beat(token, handCardIndex, tableCardIndex);

    [HttpPost("take")]
    public Task<Handlers.GameStateResult> Take(Guid token) => _gameHandler.take(token);

    [HttpPost("end_round")]
    public Task EndRound(Guid gameId) => _gameHandler.end_round(gameId);
}