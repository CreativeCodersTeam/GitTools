using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Features.Commands.StartFeature
{
    public interface IStartFeatureCommand
    {
        Task<int> StartFeatureAsync(StartFeatureOptions options);
    }
}
