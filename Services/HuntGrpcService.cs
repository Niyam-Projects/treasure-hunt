using Grpc.Core;

namespace TreasureHunt;

public class HuntGrpcService : Hunt.HuntBase
{
    public override Task<CompleteReply> Complete(CompleteRequest request, ServerCallContext context)
    {
        if (request.Token != HuntConstants.TokenStep5)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token."));

        return Task.FromResult(new CompleteReply
        {
            Success   = true,
            BonusKey  = HuntConstants.BonusKey,
            Instructions =
                $"Great job! You completed the hunt! Email your resume to hr@niyamit.com with the subject: " +
                $"'Challenge completed: {HuntConstants.TokenStep1}-{HuntConstants.TokenStep2}-" +
                $"{HuntConstants.TokenStep3}-104-{HuntConstants.TokenStep4}-" +
                $"{HuntConstants.TokenStep5}-{HuntConstants.BonusKey}'"
        });
    }
}