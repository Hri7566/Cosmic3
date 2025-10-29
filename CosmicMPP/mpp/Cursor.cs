using System.Numerics;
using Newtonsoft.Json.Linq;

namespace CosmicMPP.mpp;

public class Cursor(Client cl)
{
    private Client client = cl;
    
    private Vector2 _position = new Vector2(50, 50);
    private Vector2 _velocity = new Vector2(0.4f, 0.285714285714f);
    private Vector2 _acceleration = Vector2.Zero;

    private float _angle = 0.0f;
    private float _angle2 = 0.0f;

    public Task StartUpdate()
    {
        return Task.Run(async () =>
        {
            for (;;)
            {
                await Task.Delay(1000 / 20);
                await client.SendArray([new JObject()
                {
                    ["m"] = "m",
                    ["x"] = _position.X,
                    ["y"] = _position.Y,
                }]);
            }
        });
    }
    
    public Task StartAnimation()
    {
        return Task.Run(async () =>
        {
            for (;;)
            {
                await Task.Delay(1000 / 60);

                _angle += 1.5f;

                if (_angle >= 360.0f)
                {
                    _angle -= 360.0f;
                }

                var a = Single.DegreesToRadians(_angle);
                
                _position.Y = MathF.Sin(a * 3) * 5 + 50;
                _position.X = MathF.Cos(a) * 5 + 50;
            }
        });
    }
}