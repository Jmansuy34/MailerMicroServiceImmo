public class TwoFactorCacheEntry
{
    public string HashedCode { get; }
    public int AttemptsRemaining { get; set; }

    public TwoFactorCacheEntry(string hashedCode, int attemptsRemaining)
    {
        HashedCode = hashedCode;
        AttemptsRemaining = attemptsRemaining;
    }
}
