namespace TeamSketch.Common.ApiModels;

public readonly record struct JoinRoomValidationResult(bool RoomExists = true, bool RoomIsFull = false, bool NicknameIsTaken = false);