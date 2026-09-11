namespace TreasureHunt;

public static class HuntConstants
{
    private static int D => DateTime.UtcNow.Day;
    private static int M => DateTime.UtcNow.Month;
    public static string TokenStep1 => $"INIT-{D * 100 + M * 13}";
    public static string TokenStep2 => $"B-{M * 100 + D * 7}";
    public const string TokenStep3 = "SEQ-512";    // revealed at step 3, required at step 4
    public const string TokenStep4 = "GQL-881";    // revealed at step 4, required for GraphQL
    public static string TokenStep5 => $"GRPC-{D * D - M * 10}";
    public static string BonusKey   => $"BONUS-{(D + M) * M}";
}