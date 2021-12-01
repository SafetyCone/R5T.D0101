using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0101;
using R5T.T0097;


namespace System
{
    public static class IProjectRepositoryExtensions
    {
        public static async Task<bool> DeleteProjectOnly(this IProjectRepository projectRepository,
            Project project)
        {
            project.VerifyIsIdentitySet();

            var output = await projectRepository.DeleteProject(project.Identity);
            return output;
        }

        /// <summary>
        /// Deletes both a project, and if it is a selected project, it's selected project entry.
        /// </summary>
        public static async Task<bool> DeleteProjectAndDependentData(this IProjectRepository projectRepository,
            Project project)
        {
            var output = await projectRepository.DeleteProjectOnly(project);

            await projectRepository.DeleteProjectNameSelection(project.Name);

            return output;
        }

        /// <summary>
        /// Chooses <see cref="DeleteProjectAndDependentData(IProjectRepository, Project)"/> as the default.
        /// </summary>
        public static async Task<bool> DeleteProject(this IProjectRepository projectRepository,
            Project project)
        {
            var output = await projectRepository.DeleteProjectAndDependentData(project);
            return output;
        }

        /// <summary>
        /// Will throw if any of the project file paths are not found.
        /// For the non-throwing version, see <see cref="IProjectRepository.HasProjectsByFilePath(IEnumerable{string})"/>.
        /// </summary>
        public static async Task<Dictionary<string, Project>> GetProjectsByFilePathByFilePath(this IProjectRepository projectRepository,
            IEnumerable<string> filePaths)
        {
            var projectWasFoundByFilePath = await projectRepository.HasProjectsByFilePath(filePaths);

            // Throw if not found.
            var projectNotFoundPairs = projectWasFoundByFilePath
                .Where(xPair => xPair.Value.NotFound())
                ;

            if (projectNotFoundPairs.Any())
            {
                var notFoundFilePaths = projectNotFoundPairs
                    .Select(xPair => xPair.Key)
                    ;

                var lines = notFoundFilePaths
                    .Select(x => StringHelper.ToLine(x))
                    ;

                var linesAsLine = String.Concat(lines);

                throw new Exception($"Project file paths not found:{linesAsLine}");
            }

            // Now output.
            var output = projectWasFoundByFilePath.ToDictionaryFromWasFounds();
            return output;
        }

        public static async Task<Project[]> GetProjectsByFilePath(this IProjectRepository projectRepository,
            IEnumerable<string> filePaths)
        {
            var projectsByFilePathByFilePath = await projectRepository.GetProjectsByFilePathByFilePath(filePaths);

            var output = projectsByFilePathByFilePath.Values.ToArray();
            return output;
        }
    }
}
