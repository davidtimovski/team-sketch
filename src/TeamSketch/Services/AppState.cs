﻿using TeamSketch.Models;

namespace TeamSketch.Services;

public interface IAppState
{
    string Nickname { get; set; }
    string Room { get; set; }
    public BrushSettings BrushSettings { get; }
}

public sealed class AppState : IAppState
{
    public string Nickname { get; set; }
    public string Room { get; set; }
    public BrushSettings BrushSettings { get; } = new("avares://TeamSketch/Assets/Images/Cursors");
}
