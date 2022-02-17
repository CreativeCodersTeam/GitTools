using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create
{
    public interface ICreateReleaseCommand
    {
        Task<int> ExecuteAsync(CreateReleaseOptions options);
    }
}
