using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar;

using R5T.D0101;
using R5T.T0097;

using Instances = R5T.D0101.X001.Instances;


namespace System
{
    public static class IProjectRepositoryExtensions
    {
        public static async Task AddDuplicateProjectNameSelection(this IProjectRepository projectRepository,
            ProjectNameSelection duplicateProjectNameSelection)
        {
            await projectRepository.AddDuplicateProjectNameSelections(EnumerableHelper.From(duplicateProjectNameSelection));
        }

        public static async Task AddIgnoredProjectName(this IProjectRepository projectRepository,
            string projectName)
        {
            await projectRepository.AddIgnoredProjectNames(EnumerableHelper.From(projectName));
        }

        public static async Task<bool> DeleteProjectOnly(this IProjectRepository projectRepository,
            Project project)
        {
            project.VerifyIsIdentitySet();

            var output = await projectRepository.DeleteProject(project.Identity);
            return output;
        }

        public static async Task<Dictionary<Guid, bool>> DeleteProjectsOnly(this IProjectRepository projectRepository,
            IEnumerable<Project> projects)
        {
            projects.VerifyAllIdentitiesAreSet();

            var projectIdentities = projects
                .Select(xProject => xProject.Identity)
                ;

            var output = await projectRepository.DeleteProjects(projectIdentities);
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

        public static async Task<Dictionary<Guid, bool>> DeleteProjectsAndDependentData(this IProjectRepository projectRepository,
            IEnumerable<Project> projects)
        {
            // Delete dependent data first.
            var projectNames = projects
                .Select(xProject => xProject.Name)
                ;

            await projectRepository.DeleteProjectNameSelections(projectNames);

            // Now delete projects.
            var output = await projectRepository.DeleteProjectsOnly(projects);
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

        public static async Task<Dictionary<Guid, bool>> DeleteProjects(this IProjectRepository projectRepository,
            IEnumerable<Project> projects)
        {
            var output = await projectRepository.DeleteProjectsAndDependentData(projects);
            return output;
        }

        public static Guid GetProjectIdentity(this IProjectRepository _,
            string identityString)
        {
            var identity = Instances.GuidOperator.FromStringStandard(identityString);
            return identity;
        }

        public static async Task<Project> GetProject(this IProjectRepository projectRepository,
            Guid identity)
        {
            var hasProject = await projectRepository.HasProject(identity);
            if(!hasProject)
            {
                throw new Exception($"Project not found for identity '{identity}'");
            }

            return hasProject.Result;
        }

        public static async Task<Dictionary<Guid, WasFound<string>>> HasProjectFilePaths(this IProjectRepository projectRepository,
            IEnumerable<Guid> projectIdentities)
        {
            var output = await projectRepository.HasProjectValues(
                projectIdentities,
                Instances.Selector.SelectIdentity<IProject>(),
                Instances.Selector.SelectFilePath<IProject>());

            return output;
        }

        public static async Task<Dictionary<Guid, string>> GetProjectFilePaths(this IProjectRepository projectRepository,
            IEnumerable<Guid> projectIdentities)
        {
            var output = await projectRepository.GetProjectValues(
                projectIdentities,
                Instances.Selector.SelectIdentity<IProject>(),
                Instances.Selector.SelectFilePath<IProject>());

            return output;
        }

        public static async Task<Dictionary<string, string>> GetProjectFilePaths(this IProjectRepository projectRepository,
            IEnumerable<string> projectIdentityStrings)
        {
            var projectIdentityStringsIdentityPairs = projectIdentityStrings
                .Distinct()
                .Select(x => new { Identity = Instances.GuidOperator.FromStringStandard(x), IdentityString = x })
                ;

            var projectFilePathsByIdentity = await projectRepository.GetProjectFilePaths(
                projectIdentityStringsIdentityPairs
                    .Select(x => x.Identity));

            var projectIdentityStringFilePathPairs =
                from xProjectIdentityStringPair in projectIdentityStringsIdentityPairs
                join xProjectFilePathPair in projectFilePathsByIdentity on xProjectIdentityStringPair.Identity equals xProjectFilePathPair.Key
                // No default if empty, since GetProjectFilePaths() will throw if any are not found.
                select new { xProjectIdentityStringPair.IdentityString, FilePath = xProjectFilePathPair.Value }
                ;

            var output = projectIdentityStringFilePathPairs
                .ToDictionary(
                    x => x.IdentityString,
                    x => x.FilePath);

            return output;
        }

        public static async Task<WasFound<string>> HasProjectFilePath(this IProjectRepository projectRepository,
            Guid identity)
        {
            var hasProjectFilePaths = await projectRepository.HasProjectFilePaths(EnumerableHelper.From(identity));

            var output = hasProjectFilePaths[identity];
            return output;
        }

        public static async Task<string> GetProjectFilePath(this IProjectRepository projectRepository,
            Guid identity)
        {
            var hasProjectFilepath = await projectRepository.HasProjectFilePath(identity);
            if(!hasProjectFilepath)
            {
                throw new Exception($"Project not found for identity: '{identity}'.");
            }

            return hasProjectFilepath.Result;
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

        public static async Task<string[]> GetDuplicateProjectNames(this IProjectRepository projectRepository)
        {
            var allDuplicateProjectionNameSelections = await projectRepository.GetAllDuplicateProjectNameSelections();

            var duplicateProjectNames = allDuplicateProjectionNameSelections
                .Select(xProjectNameSelection => xProjectNameSelection.ProjectName)
                .ToArray();

            return duplicateProjectNames;
        }

        public static async Task<Project[]> GetSelectedProjects(this IProjectRepository projectRepository)
        {
            var (projects, selectedProjectNames) = await TaskHelper.WhenAll(
                projectRepository.GetAllProjects(),
                projectRepository.GetAllProjectNameSelections());

            var selectedProjectIdentitiesHash = new HashSet<Guid>(selectedProjectNames
                .Select(x => x.ProjectIdentity));

            var selectedProjects = projects
                .Where(xProject => selectedProjectIdentitiesHash.Contains(xProject.Identity))
                .ToArray();

            return selectedProjects;
        }

        public static async Task<Dictionary<TKey, TValue>> GetProjectValues<TKey, TValue>(this IProjectRepository projectRepository,
            IEnumerable<TKey> keys,
            Func<IProject, TKey> keySelector,
            Func<IProject, TValue> valueSelector)
        {
            var hasProjectValues = await projectRepository.HasProjectValues(
                keys,
                keySelector,
                valueSelector);

            var anyNotFound = hasProjectValues.AnyNotFound();
            if(anyNotFound)
            {
                var keysNotFound = hasProjectValues.GetKeysNotFound();

                var keysNotFoundList = Instances.StringOperator.FormatAsList(keysNotFound);

                throw new Exception($"Some keys were not found: {keysNotFoundList}");
            }

            var output = hasProjectValues.ToDictionaryFromWasFounds();
            return output;
        }
    }
}
