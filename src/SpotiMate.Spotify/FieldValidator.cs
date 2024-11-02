namespace SpotiMate.Spotify;

static internal class FieldValidator
{
    public static void Int32(string name, int value, int? min = null, int? max = null)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(name, value, $"Value must be between {min} and {max}.");
        }
    }

    public static void Length<T>(string name, T[] value, int? min = null, int? max = null)
    {
        if (value.Length < min || value.Length > max)
        {
            throw new ArgumentOutOfRangeException(name, value.Length, $"Collection length must be between {min} and {max}.");
        }
    }
}
