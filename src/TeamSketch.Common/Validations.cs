using System.Text.RegularExpressions;

namespace TeamSketch.Common;

public static partial class Validations
{
    public static string? ValidateNickname(string nickname)
    {
        if (!IsAlphanumeric(nickname))
        {
            return "Nicknames can contain only letters, numbers, and spaces.";
        }

        if (nickname.Trim().Length < 2)
        {
            return "Nicknames can be from 2 to 30 characters long.";
        }

        return null;
    }

    public static string? ValidateRoomName(string roomName)
    {
        if (!IsAlphanumeric(roomName))
        {
            return "Room names only contain letters and numbers.";
        }

        if (roomName.Trim().Length != 7)
        {
            return "Room names are 7 characters long.";
        }

        return null;
    }

    private static bool IsAlphanumeric(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text = text.Trim();

        return AlphanumericRegex().IsMatch(text);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9\s]*$")]
    private static partial Regex AlphanumericRegex();
}
