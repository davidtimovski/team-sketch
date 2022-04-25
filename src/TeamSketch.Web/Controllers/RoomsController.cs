﻿using Microsoft.AspNetCore.Mvc;
using TeamSketch.Common.ApiModels;
using TeamSketch.Web.Persistence;

namespace TeamSketch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRepository _repository;

        public RoomsController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{room}/validate-join/{user}")]
        public async Task<IActionResult> ValidateJoin(string room, string user)
        {
            if (string.IsNullOrEmpty(room) || string.IsNullOrEmpty(user))
            {
                return BadRequest();
            }

            var exists = await _repository.RoomExistsAsync(room);
            if (!exists)
            {
                return Ok(new JoinRoomValidationResult { RoomExists = false });
            }

            var usersInRoom = await _repository.GetActiveUsersInRoomAsync(room);
            if (usersInRoom.Count > 4)
            {
                return Ok(new JoinRoomValidationResult { RoomIsFull = true });
            }

            if (usersInRoom.Contains(user))
            {
                return Ok(new JoinRoomValidationResult { NicknameIsTaken = true });
            }

            return Ok(new JoinRoomValidationResult());
        }
    }
}