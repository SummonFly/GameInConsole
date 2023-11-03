namespace ConsoleApp1
{
    internal partial class Program
    {
        public struct Vector2
        {
            public int X;
            public int Y;

            public static readonly Vector2 Zero = new Vector2(0, 0);
            public static readonly Vector2 One = new Vector2(1, 1);
            public static readonly Vector2 Up = new Vector2(0, 1);
            public static readonly Vector2 Down = new Vector2(0, -1);
            public static readonly Vector2 Left = new Vector2(-1, 0);
            public static readonly Vector2 Right = new Vector2(1, 0);

            public Vector2(int x = 0, int y = 0)
            {
                X = x;
                Y = y;
            }
            public static Vector2 operator+(Vector2 left, Vector2 right)
            {
                return new Vector2(left.X + right.X, left.Y + right.Y);
            }

            public static bool operator ==(Vector2 left, Vector2 right)
            {
                return left.X == right.X && left.Y == right.Y;
            }
            public static bool operator !=(Vector2 left, Vector2 right)
            {
                return !(left == right);
            }
            public static Vector2 operator *(Vector2 left, int right)
            {
                return new Vector2(left.X * right, left.Y * right);
            }
            public static Vector2 operator *(Vector2 left, Vector2 right)
            {
                return new Vector2(left.X * right.X, left.Y * right.Y);
            }
            public static Vector2 GetRandom()
            {
                var r = new Random();
                return new Vector2(r.Next(2) == 1? -1: 1, r.Next(2) == 1 ? -1 : 1);
            }
            public static float Distance(Vector2 v1, Vector2 v2)
            {
                return MathF.Sqrt(MathF.Pow(v2.X - v1.X, 2) + MathF.Pow(v2.Y - v1.Y, 2));
            }

            
        }

    }
}