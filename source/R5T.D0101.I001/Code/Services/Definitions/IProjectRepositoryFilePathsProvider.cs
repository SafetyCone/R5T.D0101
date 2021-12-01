using System;
using System.Threading.Tasks;

using R5T.T0064;


namespace R5T.D0101.I001
{
    [ServiceDefinitionMarker]
    public interface IProjectRepositoryFilePathsProvider : IServiceDefinition
    {
        Task<string> GetProjectsListingJsonFilePath();
        Task<string> GetProjectNameSelectionsTextFilePath();
        Task<string> GetIgnoredProjectNamesTextFilePath();
        Task<string> GetDuplicateProjectNamesTextFilePath();
    }
}
