namespace TeamSketch.Web.Utils
{
    public static class RoomNameGenerator
    {
        private static readonly Random _random = new();
        private const int Length = 7;

        public static string Generate()
        {
            var chars = "abcdefghijkmnoprstuvwxyz0123456789";
            var result = new char[Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[_random.Next(Length)];
            }

            return new string(result);
        }
    }
}
