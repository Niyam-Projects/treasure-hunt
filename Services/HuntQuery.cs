namespace TreasureHunt;

public class HuntQuery
{
    [GraphQLDescription("Unlock the next step by providing the correct token as an argument.")]
    public BonusPayload? Unlock(string token) =>
        token == HuntConstants.TokenStep4
            ? new BonusPayload(
                Key:         HuntConstants.TokenStep5,
                NextStep:    "Call gRPC service hunt.Hunt/Complete on this host — use reflection to discover the schema.",
                Instruction: "Pass your token in the 'token' field of CompleteRequest.")
            : null;
}

public record BonusPayload(
    [property: GraphQLDescription("Your token for the next step.")]
    string Key,
    [property: GraphQLDescription("Where to go next.")]
    string NextStep,
    [property: GraphQLDescription("What to do when you get there.")]
    string Instruction
);