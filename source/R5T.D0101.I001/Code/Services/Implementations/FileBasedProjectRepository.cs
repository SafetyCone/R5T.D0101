using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar;

using R5T.T0097;
using R5T.T0064;


namespace R5T.D0101.I001
{
    [ServiceImplementationMarker]
    public class FileBasedProjectRepository : IFileBasedProjectRepository, IServiceImplementation
    {
        public List<Project> Projects { get; } = new List<Project>();
        public List<ProjectNameSelection> ProjectNameSelections { get; } = new List<ProjectNameSelection>();
        public List<string> IgnoredProjectNames { get; } = new List<string>();
        public List<ProjectNameSelection> DuplicateProjectNameSelections { get; } = new List<ProjectNameSelection>();

        private IProjectRepositoryFilePathsProvider ProjectRepositoryFilePathsProvider { get; }


        public FileBasedProjectRepository(
            IProjectRepositoryFilePathsProvider projectRepositoryFilePathsProvider)
        {
            this.ProjectRepositoryFilePathsProvider = projectRepositoryFilePathsProvider;
        }

        private bool IsEmpty()
        {
            var output = true
                && this.Projects.None()
                && this.ProjectNameSelections.None()
                && this.IgnoredProjectNames.None()
                && this.DuplicateProjectNameSelections.None()
                ;

            return output;
        }

        public void ClearAll()
        {
            this.DuplicateProjectNameSelections.Clear();
            this.IgnoredProjectNames.Clear();
            this.ProjectNameSelections.Clear();
            this.Projects.Clear();
        }

        /// <summary>
        /// If the repository is not empty, empties it.
        /// </summary>
        public void EnsureIsEmpty()
        {
            var isEmpty = this.IsEmpty();
            if (!isEmpty)
            {
                this.ClearAll();
            }
        }

        /// <summary>
        /// Throws an exception if the repository is not empty.
        /// </summary>
        public void VerifyIsEmpty()
        {
            var isEmpty = this.IsEmpty();
            if (!isEmpty)
            {
                throw new Exception("Repository is not empty.");
            }
        }

        #region IFileBasedProjectRepository

        public async Task Load()
        {
            this.VerifyIsEmpty();

            // Projects.
            var projectsListingJsonFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectsListingJsonFilePath();

            var projects = Instances.FileSystemOperator.FileExists(projectsListingJsonFilePath)
                ? Instances.FileSystemOperator.LoadProjectEntriesFromJsonFile(projectsListingJsonFilePath)
                : Array.Empty<Project>()
                ;

            this.Projects.AddRange(projects);

            // Project name selections.
            var projectNameSelectionsTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectNameSelectionsTextFilePath();

            var projectNameValues = Instances.FileSystemOperator.FileExists(projectNameSelectionsTextFilePath)
                ? Instances.DuplicateValuesOperator.LoadDuplicateValueSelections(projectNameSelectionsTextFilePath)
                : new Dictionary<string, string>()
                ;

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var projectsByFilePath = this.Projects.ToDictionary(xProject => xProject.FilePath);

            var projectNameSelections = projectNameValues
                .Select(xPair =>
                {
                    var projectForFilePath = projectsByFilePath[xPair.Value];

                    var output = new ProjectNameSelection
                    {
                        ProjectName = xPair.Key,
                        ProjectIdentity = projectForFilePath.Identity
                    };

                    return output;
                })
                ;

            this.ProjectNameSelections.AddRange(projectNameSelections);

            // Ignored project names.
            var ignoredProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetIgnoredProjectNamesTextFilePath();

            var ignoredProjectNames = Instances.FileSystemOperator.FileExists(ignoredProjectNamesTextFilePath)
                ? Instances.IgnoredValuesOperator.LoadIgnoredValues(ignoredProjectNamesTextFilePath)
                : new HashSet<string>()
                ;

            this.IgnoredProjectNames.AddRange(ignoredProjectNames);

            // Duplicate project name selections.
            var duplicateProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetDuplicateProjectNamesTextFilePath();

            var duplicateValueSelections = Instances.FileSystemOperator.FileExists(duplicateProjectNamesTextFilePath)
                ? Instances.DuplicateValuesOperator.LoadDuplicateValueSelections(duplicateProjectNamesTextFilePath)
                : new Dictionary<string, string>()
                ;

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var duplicateProjectNameSelections = duplicateValueSelections
                .Select(xPair =>
                {
                    var projectForFilePath = projectsByFilePath[xPair.Value];

                    var output = new ProjectNameSelection
                    {
                        ProjectName = xPair.Key,
                        ProjectIdentity = projectForFilePath.Identity
                    };

                    return output;
                })
                ;

            this.DuplicateProjectNameSelections.AddRange(duplicateProjectNameSelections);
        }

        public async Task Save()
        {
            // Projects.
            var projectsListingJsonFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectsListingJsonFilePath();

            Instances.FileSystemOperator.WriteToJsonFile(
                projectsListingJsonFilePath,
                this.Projects
                    .OrderAlphabetically(xProject => xProject.Name));

            // Project name selections.
            var projectNameSelectionsTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectNameSelectionsTextFilePath();

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var projectsByIdentity = this.Projects.ToDictionary(xProject => xProject.Identity);

            var projectNameValues = this.ProjectNameSelections
                .OrderAlphabetically(xProjectNameSelection => xProjectNameSelection.ProjectName)
                .ToDictionary(
                    xProjectNameSelection => xProjectNameSelection.ProjectName,
                    xProjectNameSelection =>
                    {
                        var projectForIdentity = projectsByIdentity[xProjectNameSelection.ProjectIdentity];

                        var projectFilePath = projectForIdentity.FilePath;
                        return projectFilePath;
                    });

            Instances.DuplicateValuesOperator.SaveDuplicateValueSelections(
                projectNameSelectionsTextFilePath,
                projectNameValues);

            // Ignored project names.
            var ignoredProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetIgnoredProjectNamesTextFilePath();

            Instances.IgnoredValuesOperator.SaveIgnoredValues(
                ignoredProjectNamesTextFilePath,
                this.IgnoredProjectNames
                    .OrderAlphabetically());

            // Duplicate project name selections.
            var duplicateProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetDuplicateProjectNamesTextFilePath();

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var duplicateProjectNameValues = this.DuplicateProjectNameSelections
                .OrderAlphabetically(xDuplicateProjectNameSelection => xDuplicateProjectNameSelection.ProjectName)
                .ToDictionary(
                    xDuplicateProjectNameSelection => xDuplicateProjectNameSelection.ProjectName,
                    xDuplicateProjectNameSelection =>
                    {
                        var projectForIdentity = projectsByIdentity[xDuplicateProjectNameSelection.ProjectIdentity];

                        var projectFilePath = projectForIdentity.FilePath;
                        return projectFilePath;
                    });

            Instances.DuplicateValuesOperator.SaveDuplicateValueSelections(
                duplicateProjectNamesTextFilePath,
                duplicateProjectNameValues);
        }

        #endregion

        public async Task AddProject(Project project)
        {
            project.SetIdentityIfNotSet();

            var hasProject = await this.HasProject(project);
            if (hasProject)
            {
                throw new Exception("Project already exists.");
            }

            this.Projects.Add(project);
        }

        public async Task<WasFound<Project>> HasProject(Project project)
        {
            var wasFoundByIdentity = await this.HasProject(project.Identity);
            if (wasFoundByIdentity)
            {
                return wasFoundByIdentity;
            }

            // Else.
            var wasFoundByNameAndFilePath = await this.HasProject(project.Name, project.FilePath);
            return wasFoundByNameAndFilePath;
        }

        public Task<WasFound<Project>> HasProject(Guid identity)
        {
            var projectOrDefault = this.Projects
                .Where(xProject => xProject.Identity == identity)
                .SingleOrDefault();

            var output = WasFound.From(projectOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<Project[]>> HasProjects(string name)
        {
            var projectOrDefault = this.Projects
                .Where(xProject => xProject.Name == name)
                .ToArray();

            var output = WasFound.From(projectOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<Project>> HasProject(string filePath)
        {
            var projectOrDefault = this.Projects
                .Where(xProject => xProject.FilePath == filePath)
                .SingleOrDefault();

            var output = WasFound.From(projectOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<Project>> HasProject(string name, string filePath)
        {
            var projectOrDefault = this.Projects
                .Where(xProject => xProject.Name == name && xProject.FilePath == filePath)
                .SingleOrDefault();

            var output = WasFound.From(projectOrDefault);

            return Task.FromResult(output);
        }

        public async Task<bool> DeleteProject(Guid projectIdentity)
        {
            var wasFound = await this.HasProject(projectIdentity);
            if (wasFound)
            {
                this.Projects.Remove(wasFound.Result);
            }

            return wasFound;
        }

        public async Task AddProjectNameSelection(ProjectNameSelection projectNameSelection)
        {
            var wasFound = await this.HasProjectNameSelection(projectNameSelection);
            if (wasFound)
            {
                throw new Exception("Project name selection already exists.");
            }

            this.ProjectNameSelections.Add(projectNameSelection);
        }

        public Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(ProjectNameSelection projectNameSelection)
        {
            var projectNameSelectionOrDefault = this.ProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectNameSelection.ProjectName && xProjectNameSelection.ProjectIdentity == projectNameSelection.ProjectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(string projectName)
        {
            var projectNameSelectionOrDefault = this.ProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectName)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(Guid projectIdentity)
        {
            var projectNameSelectionOrDefault = this.ProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectIdentity == projectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);

            return Task.FromResult(output);
        }

        public async Task UpdateProjectNameSelection(string projectName, Guid newProjectIdentity)
        {
            var wasFound = await this.HasProjectNameSelection(projectName);
            if (!wasFound)
            {
                throw new Exception("Project name selection does not exist.");
            }

            wasFound.Result.ProjectIdentity = newProjectIdentity;
        }

        public async Task<bool> DeleteProjectNameSelection(Guid projectIdentity)
        {
            var wasFound = await this.HasProjectNameSelection(projectIdentity);
            if (wasFound)
            {
                this.ProjectNameSelections.Remove(wasFound.Result);
            }

            return wasFound;
        }

        public async Task AddIgnoredProjectName(string projectName)
        {
            var wasFound = await this.HasIgnoredProjectName(projectName);
            if (wasFound)
            {
                throw new Exception("Ignored project name already exists");
            }

            this.IgnoredProjectNames.Add(projectName);
        }

        public Task<WasFound<string>> HasIgnoredProjectName(string projectName)
        {
            var projectNameOrDefault = this.IgnoredProjectNames
                .Where(xProjectName => xProjectName == projectName)
                .SingleOrDefault();

            var output = WasFound.From(projectNameOrDefault);

            return Task.FromResult(output);
        }

        public async Task<bool> DeleteIgnoredProjectName(string projectName)
        {
            var wasFound = await this.HasIgnoredProjectName(projectName);
            if (wasFound)
            {
                this.IgnoredProjectNames.Remove(projectName);
            }

            return wasFound;
        }

        public async Task AddDuplicateProjectNameSelection(ProjectNameSelection duplicateProjectNameSelection)
        {
            var wasFound = await this.HasDuplicateProjectNameSelection(duplicateProjectNameSelection);
            if (wasFound)
            {
                throw new Exception("Duplicate project name selection already exists.");
            }

            this.DuplicateProjectNameSelections.Add(duplicateProjectNameSelection);
        }

        public Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(ProjectNameSelection projectNameSelection)
        {
            var projectNameSelectionOrDefault = this.DuplicateProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectNameSelection.ProjectName && xProjectNameSelection.ProjectIdentity == projectNameSelection.ProjectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(string projectName)
        {
            var projectNameSelectionOrDefault = this.DuplicateProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectName)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);

            return Task.FromResult(output);
        }

        public Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(Guid projectIdentity)
        {
            var projectNameSelectionOrDefault = this.DuplicateProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectIdentity == projectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);

            return Task.FromResult(output);
        }

        public async Task UpdateDuplicateProjectNameSelection(string projectName, Guid newProjectIdentity)
        {
            var wasFound = await this.HasDuplicateProjectNameSelection(projectName);
            if (!wasFound)
            {
                throw new Exception("Duplicate project name selection does not exist.");
            }

            wasFound.Result.ProjectIdentity = newProjectIdentity;
        }

        public async Task<bool> DeleteDuplicateProjectNameSelection(Guid projectIdentity)
        {
            var wasFound = await this.HasDuplicateProjectNameSelection(projectIdentity);
            if (wasFound)
            {
                this.DuplicateProjectNameSelections.Remove(wasFound.Result);
            }

            return wasFound;
        }

        public Task<string[]> GetAllProjectFilePaths()
        {
            var output = this.Projects
                .Select(xProject => xProject.FilePath)
                .ToArray();

            return Task.FromResult(output);
        }

        public async Task<bool> DeleteProject(string filePath)
        {
            var wasFound = await this.HasProject(filePath);
            if (wasFound)
            {
                this.Projects.Remove(wasFound.Result);
            }

            return wasFound;
        }

        public Task<Project[]> GetAllProjects()
        {
            var output = this.Projects.ToArray();

            return Task.FromResult(output);
        }

        public Task<string[]> GetAllIgnoredProjectNames()
        {
            var output = this.IgnoredProjectNames.ToArray();

            return Task.FromResult(output);
        }

        public Task<ProjectNameSelection[]> GetAllDuplicateProjectNameSelections()
        {
            var output = this.DuplicateProjectNameSelections.ToArray();

            return Task.FromResult(output);
        }

        public Task<ProjectNameSelection[]> GetAllProjectNameSelections()
        {
            var output = this.ProjectNameSelections.ToArray();

            return Task.FromResult(output);
        }

        public async Task<bool> DeleteProjectNameSelection(string projectName)
        {
            var wasFound = await this.HasProjectNameSelection(projectName);
            if (wasFound)
            {
                this.ProjectNameSelections.Remove(wasFound.Result);
            }

            return wasFound;
        }

        public async Task<bool> DeleteDuplicateProjectNameSelection(string projectName)
        {
            var wasFound = await this.HasDuplicateProjectNameSelection(projectName);
            if (wasFound)
            {
                this.DuplicateProjectNameSelections.Remove(wasFound.Result);
            }

            return wasFound;
        }

        public Task ClearAllProjectNameSelections()
        {
            this.ProjectNameSelections.Clear();

            return Task.CompletedTask;
        }

        public Task<Dictionary<string, WasFound<Project>>> HasProjectsByFilePath(IEnumerable<string> filePaths)
        {
            var filePathsSet = new HashSet<string>(filePaths);

            var foundProjects = this.Projects
                .Where(xProject => filePathsSet.Contains(xProject.FilePath))
                ;

            var foundFilePaths = foundProjects
                .Select(xProject => xProject.FilePath)
                ;

            var notFoundFilePaths = filePathsSet.Except(foundFilePaths);

            var output = foundProjects
                .Select(xProject => (xProject.FilePath, WasFound.From(xProject)))
                .Concat(notFoundFilePaths
                    .Select(xNotFoundFilePath => (xNotFoundFilePath, WasFound.NotFound<Project>())))
                .ToDictionary(
                    x => x.Item1,
                    x => x.Item2);

            return Task.FromResult(output);
        }
    }
}