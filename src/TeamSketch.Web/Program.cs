using Microsoft.AspNetCore.Http.Connections;
using TeamSketch.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddMessagePackProtocol();

builder.WebHost.UseUrls("http://localhost:5150");

var app = builder.Build();

app.MapHub<ActionHub>("/actionHub", options =>
{
    options.Transports = HttpTransportType.WebSockets;
});

app.Run();
