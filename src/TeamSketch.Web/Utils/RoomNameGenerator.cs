using System.Security.Cryptography;
using System.Text;

namespace TeamSketch.Web.Utils
{
    public static class RoomNameGenerator
    {
        private static readonly char[] chars = "abcdefghkmnprstuvwxyz123456789".ToCharArray();
        private const int Length = 7;

        public static string Generate()
        {
            var data = new byte[4 * Length];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }

            var result = new StringBuilder(Length);
            for (int i = 0; i < Length; i++)
            {
                var random = BitConverter.ToUInt32(data, i * 4);
                var index = random % chars.Length;

                result.Append(chars[index]);
            }

            return result.ToString();
        }
    }
}
