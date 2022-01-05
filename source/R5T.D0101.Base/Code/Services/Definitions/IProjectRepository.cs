using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using R5T.Magyar;

using R5T.T0064;
using R5T.T0097;


namespace R5T.D0101
{
    [ServiceDefinitionMarker]
    public interface IProjectRepository : IServiceDefinition
    {
        #region Project

        Task AddProject(Project project);

        Task AddProjects(IEnumerable<Project> project);

        Task<WasFound<Project>> HasProject(Project project);

        /// <summary>
        /// Identity is unique.
        /// </summary>
        Task<WasFound<Project>> HasProject(Guid identity);
        //public Task<WasFound<Project>> HasProjectByIdentity(Guid projectIdentity);
        //public Task<Dictionary<Guid, WasFound<Project>>> HasProjectsByIdentities(IEnumerable<Guid> identities);

        /// <summary>
        /// Name is not unique. (There might be multiple project files with the same file name at different file paths.)
        /// </summary>
        Task<WasFound<Project[]>> HasProjects(string name);

        /// <summary>
        /// File path is unique since a project is defined by its project file path.
        /// </summary>
        Task<WasFound<Project>> HasProject(string filePath);

        Task<WasFound<T>> HasProjectSelect<T>(Func<IProject, bool> predicate, Func<IProject, T> selector);

        Task<Dictionary<TKey, WasFound<TValue>>> HasProjectValues<TKey, TValue>(
            IEnumerable<TKey> keys,
            Func<IProject, TKey> keySelector,
            Func<IProject, TValue> valueSelector);

        Task<Dictionary<string, WasFound<Project>>> HasProjectsByFilePath(IEnumerable<string> filePaths);

        /// <summary>
        /// Name and file path are unique.
        /// </summary>
        Task<WasFound<Project>> HasProject(string name, string filePath);
        //public Task<WasFound<Project>> HasProjectByNameAndFilePath(string name, string filePath);

        Task<Project[]> GetAllProjects();
        Task<string[]> GetAllProjectFilePaths();

        // No update.
        ///// <summary>
        ///// Will check that the input <paramref name="project"/> has the same identity value as the input <paramref name="projectIdentity"/>.
        ///// </summary>
        //public Task UpdateProjectByIdentity(Guid projectIdentity, Project project);

        // Skup update-by-file-path.

        Task<bool> DeleteProject(Guid projectIdentity);

        Task<Dictionary<Guid, bool>> DeleteProjects(IEnumerable<Guid> projectIdentities);

        Task<bool> DeleteProject(string filePath);

        #endregion

        #region Project Name Selections

        Task AddProjectNameSelection(ProjectNameSelection projectNameSelection);

        Task AddProjectNameSelections(IEnumerable<ProjectNameSelection> projectNameSelections);

        Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(ProjectNameSelection projectNameSelection);

        /// <summary>
        /// Name is unique.
        /// </summary>
        Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(string projectName);

        /// <summary>
        /// Identity is unique.
        /// </summary>
        Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(Guid projectIdentity);

        Task<ProjectNameSelection[]> GetAllProjectNameSelections();

        Task UpdateProjectNameSelection(string projectName, Guid newProjectIdentity);

        Task<bool> DeleteProjectNameSelection(string projectName);

        Task<Dictionary<string, bool>> DeleteProjectNameSelections(IEnumerable<string> projectNames);

        Task<bool> DeleteProjectNameSelection(Guid projectIdentity);

        Task ClearAllProjectNameSelections();

        #endregion

        #region Ignored Project Names

        Task AddIgnoredProjectName(string projectName);

        Task<WasFound<string>> HasIgnoredProjectName(string projectName);

        Task<string[]> GetAllIgnoredProjectNames();

        // No update.

        Task<bool> DeleteIgnoredProjectName(string projectName);

        #endregion

        #region Duplicate Project Name Selections

        Task AddDuplicateProjectNameSelection(ProjectNameSelection duplicateProjectNameSelection);

        Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(ProjectNameSelection projectNameSelection);

        /// <summary>
        /// Project name is unique.
        /// </summary>
        Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(string projectName);

        /// <summary>
        /// Project identity is unique.
        /// </summary>
        Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(Guid projectIdentity);

        Task<ProjectNameSelection[]> GetAllDuplicateProjectNameSelections();

        Task UpdateDuplicateProjectNameSelection(string projectName, Guid newProjectIdentity);

        Task<bool> DeleteDuplicateProjectNameSelection(string projectName);

        Task<bool> DeleteDuplicateProjectNameSelection(Guid projectIdentity);

        #endregion
    }
}