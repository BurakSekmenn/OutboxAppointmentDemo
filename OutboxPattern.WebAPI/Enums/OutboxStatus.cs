namespace OutboxPattern.WebAPI.Enum
{
    public enum OutboxStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2        // kalıcı hata (ör. 5 denemeden sonra)
    }
}
