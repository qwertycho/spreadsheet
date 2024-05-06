using System.Text;

public static class Helpers
{
    public static string PadEnd(string s, int l)
    {
        if (string.IsNullOrEmpty(s))
        {
            return new StringBuilder().Append(' ', l).ToString();
        }
        if (s.Length >= l)
        {
            return s.Substring(0, l);
        }
        else
        {
            return new StringBuilder(s).Append(' ', l - s.Length).ToString();
        }
    }
}