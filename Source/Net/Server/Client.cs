namespace Pong.Net.Server
{
    public class Client(long id, string name, int x, int y)
    {
        public long Id { get; } = id;
        public string Name { get; set; } = name;
        public int SpawnX { get; set; } = x;
        public int SpawnY { get; set; } = y;
    }
}
