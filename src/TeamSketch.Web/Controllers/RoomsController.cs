using Microsoft.AspNetCore.Mvc;
using TeamSketch.Common.ApiModels;
using TeamSketch.Web.Persistence;

namespace TeamSketch.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RoomsController : ControllerBase
{
    private readonly IRepository _repository;

    public RoomsController(IRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{room}/validate-join/{nickname}")]
    public async Task<IActionResult> ValidateJoin(string room, string nickname)
    {
        if (string.IsNullOrEmpty(room) || string.IsNullOrEmpty(nickname))
        {
            return BadRequest();
        }

        var exists = await _repository.RoomExistsAsync(room);
        if (!exists)
        {
            return Ok(new JoinRoomValidationResult { RoomExists = false });
        }

        var participantsInRoom = await _repository.GetActiveParticipantsInRoomAsync(room);
        if (participantsInRoom.Count > 4)
        {
            return Ok(new JoinRoomValidationResult { RoomExists = true, RoomIsFull = true });
        }

        if (participantsInRoom.Contains(nickname))
        {
            return Ok(new JoinRoomValidationResult { RoomExists = true, RoomIsFull = false, NicknameIsTaken = true });
        }

        return Ok(new JoinRoomValidationResult { RoomExists = true });
    }

    [HttpGet("{room}/participants")]
    public async Task<IActionResult> GetParticipantsInRoom(string room)
    {
        if (string.IsNullOrEmpty(room))
        {
            return BadRequest();
        }

        var participantsInRoom = await _repository.GetActiveParticipantsInRoomAsync(room);
        return Ok(participantsInRoom);
    }
}
