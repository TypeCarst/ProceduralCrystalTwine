using System.Linq;

public class Helpers
{
    public static string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string  chars  = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
                                    .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}