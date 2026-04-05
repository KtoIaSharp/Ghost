namespace Ghost.Services;

public class NicknameGenerator
{
    // Thread-safe Random (.NET 6+)
    public string GenerateNickname()
    {
        return $"Аноним_{Random.Shared.Next(1000, 9999)}";
    }
}
