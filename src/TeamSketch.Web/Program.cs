using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpOverrides;
using TeamSketch.Web.Config;
using TeamSketch.Web.Hubs;
using TeamSketch.Web.Persistence;
using TeamSketch.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddOptions<DatabaseSettings>().Bind(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IRandomRoomQueue, RandomRoomQueue>();
builder.Services.AddSingleton<ILiveLocationsService, LiveLocationsService>();
builder.Services.AddTransient<IRepository, Repository>();

builder.WebHost.UseUrls("http://localhost:5150");

var app = builder.Build();

app.UseRouting()
   .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    }).UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

app.MapHub<ActionHub>("/actionHub", options =>
{
    options.Transports = HttpTransportType.WebSockets;
});

app.Lifetime.ApplicationStarted.Register(async () => {
    var repository = app.Services.GetService<IRepository>();
    await repository.DisconnectAllAsync();
});

app.Run();
