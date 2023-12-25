using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SpaceBots;
using Terminal.Gui;
using static System.Environment;

string apiKey = GetEnvironmentVariable("spaceBotsAPIKey");
if (apiKey == null)
{
    return;
}

var client = new HttpClient
{
    BaseAddress = new Uri("https://space-bots.longwelwind.net/v1/")
};
client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
var serializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true ,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
};

var ratelimit = TimeSpan.FromMilliseconds(200);
Stopwatch lastRequest = null;
async Task<T> Request<T>(string url)
{
    if (lastRequest == null)
    {
        lastRequest = new Stopwatch();
    }

    var elapsed = lastRequest.Elapsed;
    if (elapsed < ratelimit)
    {
        Thread.Sleep(ratelimit - elapsed);
    }
    var response =  await client.GetAsync(url);
    if (response.StatusCode == HttpStatusCode.OK)
    {
        var result =  JsonSerializer.Deserialize<T>(response.Content.ReadAsStream(), serializerOptions);
        lastRequest.Restart();
        return result;
    }
    else
    {
        throw new Exception("Got response code " + response.StatusCode);
    }

    return default(T);
}
var me = await Request<Me>(Me.Url);
var resources = await Request<Resource[]>(Resource.Url);
var shipTypes = await Request<ShipType[]>(ShipType.Url);
var myFleets = await Request<Fleet[]>("fleets/my");
var system = await Request<SpaceSystem>($"systems/{myFleets[0].LocationSystemId}");

var uniqueSystems = new Dictionary<string, SpaceSystem>();
uniqueSystems.Add(system.Id, system);

var systemsToCheck = new HashSet<string>(
system.NeighboringSystems.Select(sys => sys.SystemId)
);

while (systemsToCheck.Count > 0)
{
    var systemToCheck = systemsToCheck.First();
    systemsToCheck.Remove(systemToCheck);
    var checkedSystem = await Request<SpaceSystem>($"systems/{systemToCheck}");
    uniqueSystems.TryAdd(checkedSystem.Id, checkedSystem);
    foreach (var sys in checkedSystem.NeighboringSystems)
    {
        systemsToCheck.Add(sys.SystemId);
    }
}

Console.Write(me);


/*
Application.Init();

try
{
    Application.Run(new MyView());
}
finally
{
    Application.Shutdown();
}*/