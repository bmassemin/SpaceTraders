namespace SpaceTradersAPI.Factories;

public static class TokenRepoFactory
{
    public static ITokenRepository CreateFileTokenRepo()
    {
        return new FileTokenRepository();
    }
}