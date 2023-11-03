using System.Text;

namespace ConsoleApp1
{
    internal partial class Program
    {
        static void Main(string[] args)
        {

            var gameLoop = true;
            while (gameLoop)
            {
                Console.WriteLine("-> s to start\n-> e to exit");
                switch(Console.ReadLine())
                {
                    case "s":
                        StartGame();
                        break;
                    case "e":
                        gameLoop = false;
                        break;
                }
            }

        }

        private static void StartGame()
        {
            var game = new GameManager();
            game.StartGame();
            while (game.IsRuning)
            {
                Console.Clear();
                game.Update();
                Thread.Sleep(100);
            }

            Console.Clear();
            Console.WriteLine("GameOver");
            Console.WriteLine(game.Score);
        }
        public class GameManager
        {
            public int ObjectCount = 10;
            public int ObjectSpeed = 1;
            public int ObjectRadius = 5;
            public int ObjectMaxCountInRoad = 3;

            public int PlayerSpeed = 3;
            public int PlayerSize = 2;

            const int width = 180;
            const int height = 60;

            public bool IsRuning { get; private set; } = true;
            public int Score => (int)(DateTime.Now - _gameStartTime).Seconds;

            private PhysicObject[] _objectPool;
            private ControllableObject _player;

            private DateTime _gameStartTime;

            public void StartGame()
            {
                _gameStartTime = DateTime.Now;
                var rand = new Random();
                _objectPool = new PhysicObject[ObjectCount];
                SpawnEnemysCircle(ObjectCount);

                _player = new ControllableObject(new Vector2(width / 2, (int)(height * 0.8f)), Vector2.One * PlayerSpeed, PlayerSize);

            }

            private void SpawnEnemysCircle(int count)
            {
                for (int obj = 0; obj < count; obj++)
                {
                    var pos = new Vector2(obj * (ObjectRadius * 2 + (int)(PlayerSize * 2.5f)) + 10, 10 + obj / ObjectMaxCountInRoad * ObjectRadius * 2);
                    _objectPool[obj] = new PhysicObject(pos, Vector2.GetRandom() * ObjectSpeed, ObjectRadius);
                }
            }
            private void SpawnEnemysFlappy(int count)
            {
                for (int obj = 0; obj < count; obj++)
                {
                    throw new Exception(); //TODO
                }
            }

            public void Update()
            {
                Drawer.FillScreen();
                foreach (var e in _objectPool)
                {
                    e.Update();
                    if(Vector2.Distance(e.Position, _player.Position) < (_player.Radius + e.Radius))
                    {
                        IsRuning = false;
                    }
                }
                _player.Update();
                Drawer.DrawValue((int)(DateTime.Now - _gameStartTime).Seconds);
                Drawer.DrawMap();
            }

            public GameManager()
            {
                Console.WindowWidth = width;
                Console.BufferWidth = width;
                Console.WindowHeight = height;
                Console.BufferHeight = height;

            }
            public class Drawer
            {

                private static char[,] map = new char[width, height];
                public static void FillScreen(char border = '►', char background = '█')
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            map[x, y] = background;
                        }
                        map[0, y] = border;
                        map[width - 1, y] = border;

                    }
                }
                private static void SetValueInMatrix(Vector2 position, char value)
                {
                   if(position.X >= 0 && position.X < width && position.Y >= 0 && position.Y < height)
                    {
                        map[position.X, position.Y] = value;
                    }
                }

                private static void SetQueryInMatrix(Vector2 position, char[] value)
                {
                    var currentPosition = position;
                    for(int i = 0; i < value.Length; i++)
                    {
                        SetValueInMatrix(currentPosition, value[i]);
                        currentPosition.X = position.X + i + 1;
                    }
                }

                public static void DrawRect(Vector2 position, int width, int height)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            SetValueInMatrix(position + new Vector2(x, y), ' ');
                        }
                    }
                }
                public static void DrawCircle(Vector2 position, float radius)
                {
                    for (int y = - (int)radius; y < radius; y++)
                    {
                        for(int x = - (int)radius; x < radius; x++)
                        {
                            if(MathF.Sqrt(MathF.Pow((position.X + x) - position.X, 2) + MathF.Pow((position.Y + y) - position.Y, 2)) < radius)
                            {
                                SetValueInMatrix(new Vector2(x, y) + position, ' ');
                            }
                        }
                    }
                }
                public static void DrawMap()
                {
                    var bilder = new StringBuilder();
                    for (int y = 0; y < height; y++)
                    {
                        bilder.Clear();
                        for (int x = 0; x < width; x++)
                        {
                            bilder.Append(map[x, y]);
                        }
                        Console.WriteLine(bilder.ToString());
                    }
                }

                public static void DrawValue(int value)
                {
                    char[] val = value.ToString().ToCharArray();
                    SetQueryInMatrix(new Vector2(0, 1), new string('▼', 5).ToCharArray());
                    SetQueryInMatrix(new Vector2(0, 2), new string('▒', 5).ToCharArray());
                    SetQueryInMatrix(new Vector2(0, 3), new string('▲', 5).ToCharArray());
                    SetQueryInMatrix(new Vector2(val.Length, 2), val);
                }
            }

            public class PhysicObject
            {
                public int Radius = 10;
                public Vector2 Position;
                public Vector2 Velosity;
                public virtual void Update()
                {
                    Move();
                    Draw();
                }
                public virtual void Move()
                {
                    Position += Velosity;
                    if (Position.X - Radius > width && Velosity.X > 0)
                    {
                        Position.X = Radius * -2;
                    }
                    if(Position.X + Radius < 0 && Velosity.X < 0)
                    {
                        Position.X = width + Radius * 2;
                    }

                    if (Position.Y - Radius <= 0 || Position.Y + Radius >= height)
                    {
                        Velosity = new Vector2(Velosity.X, Velosity.Y * -1);
                    }
                }
                public virtual void Draw()
                {
                    Drawer.DrawCircle(Position, Radius);
                }
                public PhysicObject(Vector2 position = default, Vector2 velosity = default, int Size = 10)
                {
                    Position = position;
                    Velosity = velosity;
                    Radius = Size;
                }
            }

            
            public class ControllableObject : PhysicObject
            {
                public Vector2 Speed;
                public override void Move()
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.A:
                                Velosity = new Vector2(-Math.Abs(Speed.X), 0);
                                break;
                            case ConsoleKey.D:
                                Velosity = new Vector2(Math.Abs(Speed.X), 0);
                                break;
                            case ConsoleKey.W:
                                Velosity = new Vector2(0, -Math.Abs(Speed.X));
                                break;
                            case ConsoleKey.S:
                                Velosity = new Vector2(0, Math.Abs(Speed.X));
                                break;
                            default:
                                Velosity = Vector2.Zero;
                                break;
                        }
                        base.Move();

                        if(Position.Y + Radius > width) Position.Y = width - Radius;
                        if(Position.Y - Radius < 0) Position.Y = Radius;
                    }
                }
                public ControllableObject(Vector2 position, Vector2 velosity, int Size) : base(position, velosity, Size)
                {
                    Speed = Velosity;
                }
            }
        }

       
    }
}