using TeamSketch.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ActionHub>("/actionHub");
app.Run();
