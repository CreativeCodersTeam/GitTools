using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature
{
    public interface IFinishFeatureCommand
    {
        Task<int> FinishFeatureAsync(FinishFeatureOptions options);
    }
}
