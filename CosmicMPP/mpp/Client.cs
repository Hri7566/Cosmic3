using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

namespace CosmicMPP.mpp;

public class Client
{
    private readonly Uri _uri;
    private readonly string? _token;
    private ClientWebSocket _ws;
    private bool _canConnect;
    
    public Client(Uri uri, string? token)
    {
        _uri = uri;
        _token = token;
        _ws = new ClientWebSocket();
        
        _bindEventListeners();
    }

    public bool IsConnected()
    {
        return _ws.State == WebSocketState.Open;
    }

    public bool IsConnecting()
    {
        return _ws.State == WebSocketState.Connecting;
    }

    public async Task Start()
    {
        try
        {
            _canConnect = true;
            await Connect().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Stop()
    {
        _canConnect = false;
        _ws.Abort();
        _ws.Dispose();
        _ws = new ClientWebSocket();
    }

    private async Task Connect()
    {
        try
        {
            if (!_canConnect || IsConnected() || IsConnecting()) return;

            await _ws.ConnectAsync(_uri, CancellationToken.None).ConfigureAwait(false);
            var buffer = new byte[1024 * 1024];

            var hiMsgArray = new JArray();
            var hiMsg = new JObject
            {
                ["m"] = "hi",
                ["token"] = _token
            };
            
            hiMsgArray.Add(hiMsg);
            await SendArray(hiMsgArray).ConfigureAwait(false);
            
            while (!_ws.CloseStatus.HasValue)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                
                try
                {
                    var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var msgs = JArray.Parse(data);

                    foreach (var msg in msgs)
                    {
                        var messageType = msg["m"]?.ToObject<string>();
                        if (messageType == null) continue;
                        Emit(messageType, (JObject)msg);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to parse message: " + e.Message);
                }
            }
            
            Stop();
            await Start().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Stop();
            await Start().ConfigureAwait(false);
        }
    }

    private void _bindEventListeners()
    {
        var timer = new Timer(20000);

        timer.Elapsed += async (_, _) =>
        {
            await SendArray([new JObject()
            {
                ["m"] = "t",
                ["t"] = DateTimeOffset.Now.ToUnixTimeSeconds()
            }]).ConfigureAwait(false);
        };

        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private readonly Dictionary<string, List<Action<JObject>>> _events = [];
    
    public void On(string evtn, Action<JObject> listener)
    {
        if (!_events.ContainsKey(evtn)) _events.Add(evtn, []);
        _events[evtn].Add(listener);
    }

    public void Off(string @event, Action<JObject> listener)
    {
        if (!_events.TryGetValue(@event, out var value)) return;
        value.Remove(listener);
    }

    public void Emit(string @event, JObject arg)
    {
        if (!_events.TryGetValue(@event, out var listeners)) return;
        
        foreach (var listener in listeners)
        {
            listener(arg);
        }
    }

    public async Task SendArray(JArray messages)
    {
        var data = messages.ToString();
        var bytes = Encoding.UTF8.GetBytes(data);
        await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
    }

    public async Task SetChannel(string id)
    {
        await SendArray([new JObject()
        {
            ["m"] = "ch",
            ["_id"] = id
        }]).ConfigureAwait(false);
    }

    public async Task SendChat(string message)
    {
        var lines = message.Split('\n');

        foreach (var line in lines)
        {
            await SendArray([
                new JObject()
                {
                    ["m"] = "a",
                    ["message"] = line
                }
            ]).ConfigureAwait(false);
        }
    }
}
