using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar;

using R5T.T0094;
using R5T.T0097;
using R5T.T0064;


namespace R5T.D0101.I001
{
    [ServiceImplementationMarker]
    public class FileBasedProjectRepository : IFileBasedProjectRepository, IServiceImplementation
    {
        #region Static

        private static WasFound<ProjectNameSelection> HasDuplicateProjectNameSelection(IEnumerable<ProjectNameSelection> duplicateProjectNameSelections, Guid projectIdentity)
        {
            var projectNameSelectionOrDefault = duplicateProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectIdentity == projectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);
            return output;
        }

        private static WasFound<ProjectNameSelection> HasDuplicateProjectNameSelection(IEnumerable<ProjectNameSelection> duplicateProjectNameSelections, string projectName)
        {
            var projectNameSelectionOrDefault = duplicateProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectName)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);
            return output;
        }

        private static WasFound<ProjectNameSelection> HasDuplicateProjectNameSelection(IEnumerable<ProjectNameSelection> duplicateProjectNameSelections, ProjectNameSelection projectNameSelection)
        {
            var projectNameSelectionOrDefault = duplicateProjectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectNameSelection.ProjectName && xProjectNameSelection.ProjectIdentity == projectNameSelection.ProjectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);
            return output;
        }

        private static WasFound<string> HasIgnoredProjectName(IEnumerable<string> ignoredProjectNames, string projectName)
        {
            var projectNameOrDefault = ignoredProjectNames
                .Where(xProjectName => xProjectName == projectName)
                .SingleOrDefault();

            var output = WasFound.From(projectNameOrDefault);
            return output;
        }

        private static WasFound<ProjectNameSelection> HasProjectNameSelection(IEnumerable<ProjectNameSelection> projectNameSelections, Guid projectIdentity)
        {
            var projectNameSelectionOrDefault = projectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectIdentity == projectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);
            return output;
        }

        private static WasFound<ProjectNameSelection> HasProjectNameSelection(IEnumerable<ProjectNameSelection> projectNameSelections, string projectName)
        {
            var projectNameSelectionOrDefault = projectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectName)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);
            return output;
        }

        private static WasFound<ProjectNameSelection> HasProjectNameSelection(IEnumerable<ProjectNameSelection> projectNameSelections,
            ProjectNameSelection projectNameSelection)
        {
            var projectNameSelectionOrDefault = projectNameSelections
                .Where(xProjectNameSelection => xProjectNameSelection.ProjectName == projectNameSelection.ProjectName && xProjectNameSelection.ProjectIdentity == projectNameSelection.ProjectIdentity)
                .SingleOrDefault();

            var output = WasFound.From(projectNameSelectionOrDefault);
            return output;
        }

        private static Dictionary<ProjectNameSelection, WasFound<ProjectNameSelection>> HasProjectNameSelections(IEnumerable<ProjectNameSelection> projectNameSelections,
            IEnumerable<ProjectNameSelection> testProjectNameSelections)
        {
            var output = new Dictionary<ProjectNameSelection, WasFound<ProjectNameSelection>>();

            foreach (var testProjectNameSelection in testProjectNameSelections)
            {
                var wasFound = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, testProjectNameSelection);

                output.Add(testProjectNameSelection, wasFound);
            }

            return output;
        }

        private static Dictionary<Project, WasFound<Project>> HasProjects(IEnumerable<Project> projects, IEnumerable<Project> newProjects)
        {
            var output = new Dictionary<Project, WasFound<Project>>();

            foreach (var newProject in newProjects)
            {
                var wasFound = FileBasedProjectRepository.HasProject(projects, newProject);

                output.Add(newProject, wasFound);
            }

            return output;
        }

        //private static Dictionary<Guid, WasFound<Project>> HasProjects(IEnumerable<Project> projects, IEnumerable<Guid> projectIdentities)
        //{
        //    var output = new Dictionary<Guid, WasFound<Project>>();

        //    foreach (var projectIdentity in projectIdentities)
        //    {
        //        var hasProject = FileBasedProjectRepository.HasProject(projects, projectIdentity);

        //        output.Add(projectIdentity, hasProject);
        //    }

        //    return output;
        //}

        private static WasFound<Project> HasProject(IEnumerable<Project> projects, Guid identity)
        {
            var projectOrDefault = projects
                .Where(xProject => xProject.Identity == identity)
                .SingleOrDefault();

            var output = WasFound.From(projectOrDefault);
            return output;
        }

        private static WasFound<Project> HasProject(IEnumerable<Project> projects, string filePath)
        {
            var projectOrDefault = projects
                .Where(xProject => xProject.FilePath == filePath)
                .SingleOrDefault();

            var output = WasFound.From(projectOrDefault);
            return output;
        }

        private static WasFound<Project> HasProject(IEnumerable<Project> projects, string name, string filePath)
        {
            var projectOrDefault = projects
                .Where(xProject => xProject.Name == name && xProject.FilePath == filePath)
                .SingleOrDefault();

            var output = WasFound.From(projectOrDefault);
            return output;
        }

        private static WasFound<Project> HasProject(IEnumerable<Project> projects, Project project)
        {
            var wasFoundByIdentity = FileBasedProjectRepository.HasProject(projects, project.Identity);
            if (wasFoundByIdentity)
            {
                return wasFoundByIdentity;
            }

            // Else.
            var wasFoundByNameAndFilePath = FileBasedProjectRepository.HasProject(projects, project.Name, project.FilePath);
            return wasFoundByNameAndFilePath;
        }

        #endregion


        private IProjectRepositoryFilePathsProvider ProjectRepositoryFilePathsProvider { get; }


        public FileBasedProjectRepository(
            IProjectRepositoryFilePathsProvider projectRepositoryFilePathsProvider)
        {
            this.ProjectRepositoryFilePathsProvider = projectRepositoryFilePathsProvider;
        }

        //private bool IsEmpty()
        //{
        //    var output = true
        //        && this.Projects.None()
        //        && this.ProjectNameSelections.None()
        //        && this.IgnoredProjectNames.None()
        //        && this.DuplicateProjectNameSelections.None()
        //        ;

        //    return output;
        //}

        //public void ClearAll()
        //{
        //    this.DuplicateProjectNameSelections.Clear();
        //    this.IgnoredProjectNames.Clear();
        //    this.ProjectNameSelections.Clear();
        //    this.Projects.Clear();
        //}

        ///// <summary>
        ///// If the repository is not empty, empties it.
        ///// </summary>
        //public void EnsureIsEmpty()
        //{
        //    var isEmpty = this.IsEmpty();
        //    if (!isEmpty)
        //    {
        //        this.ClearAll();
        //    }
        //}

        ///// <summary>
        ///// Throws an exception if the repository is not empty.
        ///// </summary>
        //public void VerifyIsEmpty()
        //{
        //    var isEmpty = this.IsEmpty();
        //    if (!isEmpty)
        //    {
        //        throw new Exception("Repository is not empty.");
        //    }
        //}

        #region Files Load/Save

        private async Task<Project[]> LoadProjects()
        {
            var projectsListingJsonFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectsListingJsonFilePath();

            var projects = Instances.FileSystemOperator.FileExists(projectsListingJsonFilePath)
                ? Instances.FileSystemOperator.LoadProjectEntriesFromJsonFile(projectsListingJsonFilePath)
                : Array.Empty<Project>()
                ;

            return projects;
        }

        private async Task SaveProjects(IEnumerable<Project> projects)
        {
            var projectsListingJsonFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectsListingJsonFilePath();

            Instances.FileSystemOperator.WriteToJsonFile(
                projectsListingJsonFilePath,
                projects
                    .OrderAlphabetically(xProject => xProject.Name));
        }

        private async Task<ProjectNameSelection[]> LoadProjectNameSelections()
        {
            var projectNameSelectionsTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectNameSelectionsTextFilePath();

            var projectNameValues = Instances.FileSystemOperator.FileExists(projectNameSelectionsTextFilePath)
                ? Instances.DuplicateValuesOperator.LoadDuplicateValueSelections(projectNameSelectionsTextFilePath)
                : new Dictionary<string, string>()
                ;

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var projects = await this.LoadProjects();

            var projectsByFilePath = projects.ToDictionary(xProject => xProject.FilePath);

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
                .ToArray()
                ;

            return projectNameSelections;
        }

        private async Task SaveProjectNameSelections(IEnumerable<ProjectNameSelection> projectNameSelections)
        {
            var projectNameSelectionsTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetProjectNameSelectionsTextFilePath();

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var projects = await this.LoadProjects();

            var projectsByIdentity = projects.ToDictionary(xProject => xProject.Identity);

            var projectNameValues = projectNameSelections
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
        }

        private async Task<string[]> LoadIgnoredProjectNames()
        {
            var ignoredProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetIgnoredProjectNamesTextFilePath();

            var ignoredProjectNames = Instances.FileSystemOperator.FileExists(ignoredProjectNamesTextFilePath)
                ? Instances.IgnoredValuesOperator.LoadIgnoredValues(ignoredProjectNamesTextFilePath)
                : new HashSet<string>()
                ;

            var output = ignoredProjectNames.ToArray();
            return output;
        }

        private async Task SaveIgnoredProjectNames(IEnumerable<string> ignoredProjectNames)
        {
            var ignoredProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetIgnoredProjectNamesTextFilePath();

            Instances.IgnoredValuesOperator.SaveIgnoredValues(
                ignoredProjectNamesTextFilePath,
                ignoredProjectNames
                    .OrderAlphabetically());
        }

        private async Task<ProjectNameSelection[]> LoadDuplicateProjectNameSelections()
        {
            var duplicateProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetDuplicateProjectNamesTextFilePath();

            var duplicateValueSelections = Instances.FileSystemOperator.FileExists(duplicateProjectNamesTextFilePath)
                ? Instances.DuplicateValuesOperator.LoadDuplicateValueSelections(duplicateProjectNamesTextFilePath)
                : new Dictionary<string, string>()
                ;

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var projects = await this.LoadProjects();

            var projectsByFilePath = projects.ToDictionary(xProject => xProject.FilePath);

            var duplicateProjectNameSelections = duplicateValueSelections
                .Select(xPair =>
                {
                    // The file path will not exist in the case of a new duplicate projects.
                    // Just fill in an default identity value in this case, since it will be fine later. Only the project name will be used for duplicate testing purposes, and in regular operation there will always be a file path.
                    var filePathExists = projectsByFilePath.ContainsKey(xPair.Value);
                    if(!filePathExists)
                    {
                        var specialOutput = new ProjectNameSelection
                        {
                            ProjectName = xPair.Key,
                            ProjectIdentity = Instances.GuidOperator.DefaultGuid(),
                        };

                        return specialOutput;
                    }

                    var projectForFilePath = projectsByFilePath[xPair.Value];

                    var output = new ProjectNameSelection
                    {
                        ProjectName = xPair.Key,
                        ProjectIdentity = projectForFilePath.Identity
                    };

                    return output;
                })
                .ToArray()
                ;

            return duplicateProjectNameSelections;
        }

        private async Task SaveDuplicateProjectNameSelections(IEnumerable<ProjectNameSelection> duplicateProjectNameSelections)
        {
            var duplicateProjectNamesTextFilePath = await this.ProjectRepositoryFilePathsProvider.GetDuplicateProjectNamesTextFilePath();

            // File is formatted as {Project Name}| {Project File Path} for convenience of human analysis. So convert it.
            var projects = await this.LoadProjects();

            var projectsByIdentity = projects.ToDictionary(xProject => xProject.Identity);

            var duplicateProjectNameValues = duplicateProjectNameSelections
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

            var projects = await this.LoadProjects();

            var hasProject = FileBasedProjectRepository.HasProject(projects, project);
            if (hasProject)
            {
                throw new Exception("Project already exists.");
            }

            // Else, modify and save.
            var modifiedProjects = projects.Append(project);

            await this.SaveProjects(modifiedProjects);
        }

        public async Task AddProjects(IEnumerable<Project> newProjects)
        {
            newProjects.SetIdentitiesIfNotSet();

            var projects = await this.LoadProjects();

            // Do new projects already exist?
            var hasProjectByProject = FileBasedProjectRepository.HasProjects(projects, newProjects);

            var anyProjectsExist = hasProjectByProject
                .Where(xPair => xPair.Value.Exists)
                .Any();

            if(anyProjectsExist)
            {
                throw new Exception("Some projects already exist.");
            }

            var modifiedProjects = projects.AppendRange(newProjects);

            await this.SaveProjects(modifiedProjects);
        }

        public async Task<WasFound<Project>> HasProject(Project project)
        {
            var projects = await this.LoadProjects();

            var output = FileBasedProjectRepository.HasProject(projects, project);
            return output;
        }

        public async Task<WasFound<Project>> HasProject(Guid identity)
        {
            var projects = await this.LoadProjects();

            var output = FileBasedProjectRepository.HasProject(projects, identity);
            return output;
        }

        public async Task<WasFound<Project[]>> HasProjects(string name)
        {
            var projects = await this.LoadProjects();

            var projectOrDefault = projects
                .Where(xProject => xProject.Name == name)
                .ToArray();

            var output = WasFound.From(projectOrDefault);
            return output;
        }

        public async Task<WasFound<Project>> HasProject(string filePath)
        {
            var projects = await this.LoadProjects();

            var output = FileBasedProjectRepository.HasProject(projects, filePath);
            return output;
        }

        public async Task<WasFound<Project>> HasProject(string name, string filePath)
        {
            var projects = await this.LoadProjects();

            var output = FileBasedProjectRepository.HasProject(projects, name, filePath);
            return output;
        }

        public async Task<WasFound<T>> HasProjectSelect<T>(Func<IProject, bool> predicate, Func<IProject, T> selector)
        {
            var projects = await this.LoadProjects();

            var valueOrDefault = projects
                .Where(predicate)
                .Select(selector)
                .SingleOrDefault()
                ;

            var output = WasFound.From(valueOrDefault);
            return output;
        }

        public async Task<bool> DeleteProject(Guid projectIdentity)
        {
            var projects = await this.LoadProjects();

            var wasFound = FileBasedProjectRepository.HasProject(projects, projectIdentity);
            if (wasFound)
            {
                var modifiedProjects = projects
                    .Except(wasFound.Result, NamedIdentifiedFilePathedEqualityComparer<Project>.Instance)
                    ;

                await this.SaveProjects(modifiedProjects);
            }

            return wasFound;
        }

        public async Task<Dictionary<Guid, bool>> DeleteProjects(IEnumerable<Guid> projectIdentities)
        {
            var projects = await this.LoadProjects();

            var projectIdentitiesHash = new HashSet<Guid>(projectIdentities);

            var output = new Dictionary<Guid, bool>();

            var modifiedProjects = projects
                .Where(xProject =>
                {
                    var projectIdentity = xProject.Identity;

                    var removeProject = projectIdentitiesHash.Contains(projectIdentity);
                    if (removeProject)
                    {
                        output.Add(projectIdentity, true);
                    }
                    else
                    {
                        output.Add(projectIdentity, false);
                    }

                    var keepProject = !removeProject;
                    return keepProject;
                })
                ;

            await this.SaveProjects(modifiedProjects);

            return output;
        }

        public async Task AddProjectNameSelection(ProjectNameSelection projectNameSelection)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectNameSelection);
            if (wasFound)
            {
                throw new Exception("Project name selection already exists.");
            }

            // Else, modify and save.
            var modifiedProjectNameSelections = projectNameSelections
                .Append(projectNameSelection)
                ;

            await this.SaveProjectNameSelections(modifiedProjectNameSelections);
        }

        public async Task AddProjectNameSelections(IEnumerable<ProjectNameSelection> projectNameSelections)
        {
            var repositoryProjectNameSelections = await this.LoadProjectNameSelections();

            var hasProjectNameSelectionByProjectNameSelection = FileBasedProjectRepository.HasProjectNameSelections(
                repositoryProjectNameSelections,
                projectNameSelections);

            var anyProjectNameSelectionsExist = hasProjectNameSelectionByProjectNameSelection
                .Where(xPair => xPair.Value.Exists)
                .Any();

            if (anyProjectNameSelectionsExist)
            {
                throw new Exception("Some project name selections already exists.");
            }

            var modifiedRepositoryProjectNameSelections = repositoryProjectNameSelections.AppendRange(projectNameSelections);

            await this.SaveProjectNameSelections(modifiedRepositoryProjectNameSelections);
        }

        public async Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(ProjectNameSelection projectNameSelection)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var output = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectNameSelection);
            return output;
        }

        public async Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(string projectName)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var output = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectName);
            return output;
        }

        public async Task<WasFound<ProjectNameSelection>> HasProjectNameSelection(Guid projectIdentity)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var output = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectIdentity);
            return output;
        }

        public async Task UpdateProjectNameSelection(string projectName, Guid newProjectIdentity)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectName);
            if (!wasFound)
            {
                throw new Exception("Project name selection does not exist.");
            }

            // Else, modify and save.
            var projectNameSelection = projectNameSelections
                .Where(x => x.ProjectName == projectName)
                .Single();

            projectNameSelection.ProjectIdentity = newProjectIdentity;

            await this.SaveProjectNameSelections(projectNameSelections);
        }

        public async Task<bool> DeleteProjectNameSelection(Guid projectIdentity)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectIdentity);
            if (wasFound)
            {
                var modifiedProjectNameSelections = projectNameSelections
                    .Where(x => x.ProjectIdentity != projectIdentity)
                    ;

                await this.SaveProjectNameSelections(projectNameSelections);
            }

            return wasFound;
        }

        public async Task AddIgnoredProjectName(string projectName)
        {
            var ignoredProjectNames = await this.LoadIgnoredProjectNames();

            var wasFound = FileBasedProjectRepository.HasIgnoredProjectName(ignoredProjectNames, projectName);
            if (wasFound)
            {
                throw new Exception("Ignored project name already exists");
            }

            // Else, modify and save.
            var modifiedIgnoredProjectNames = ignoredProjectNames
                .Append(projectName)
                ;

            await this.SaveIgnoredProjectNames(modifiedIgnoredProjectNames);
        }

        public async Task<WasFound<string>> HasIgnoredProjectName(string projectName)
        {
            var ignoredProjectNames = await this.LoadIgnoredProjectNames();

            var output = FileBasedProjectRepository.HasIgnoredProjectName(ignoredProjectNames, projectName);
            return output;
        }

        public async Task<bool> DeleteIgnoredProjectName(string projectName)
        {
            var ignoredProjectNames = await this.LoadIgnoredProjectNames();

            var wasFound = FileBasedProjectRepository.HasIgnoredProjectName(ignoredProjectNames, projectName);
            if (wasFound)
            {
                var modifiedIgnoredProjectNames = ignoredProjectNames
                    .Where(x => x != projectName)
                    ;

                await this.SaveIgnoredProjectNames(modifiedIgnoredProjectNames);
            }

            return wasFound;
        }

        public async Task AddDuplicateProjectNameSelection(ProjectNameSelection duplicateProjectNameSelection)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, duplicateProjectNameSelection);
            if (wasFound)
            {
                throw new Exception("Duplicate project name selection already exists.");
            }

            // Else, modify and save.
            var modifiedDuplicateProjectNameSelections = duplicateProjectNameSelections
                .Append(duplicateProjectNameSelection)
                ;

            await this.SaveDuplicateProjectNameSelections(modifiedDuplicateProjectNameSelections);
        }

        public async Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(ProjectNameSelection projectNameSelection)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var output = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, projectNameSelection);
            return output;
        }

        public async Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(string projectName)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var output = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, projectName);
            return output;
        }

        public async Task<WasFound<ProjectNameSelection>> HasDuplicateProjectNameSelection(Guid projectIdentity)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var output = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, projectIdentity);
            return output;
        }

        public async Task UpdateDuplicateProjectNameSelection(string projectName, Guid newProjectIdentity)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, projectName);
            if (!wasFound)
            {
                throw new Exception("Duplicate project name selection does not exist.");
            }

            // Else modify and save.
            var duplicateProjectNameSelection = duplicateProjectNameSelections
                .Where(x => x.ProjectName == projectName)
                .Single();

            duplicateProjectNameSelection.ProjectIdentity = newProjectIdentity;

            await this.SaveDuplicateProjectNameSelections(duplicateProjectNameSelections);
        }

        public async Task<bool> DeleteDuplicateProjectNameSelection(Guid projectIdentity)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, projectIdentity);
            if (wasFound)
            {
                var modifiedDuplicateProjectNameSelections = duplicateProjectNameSelections
                    .Where(x => x.ProjectIdentity != projectIdentity)
                    ;

                await this.SaveDuplicateProjectNameSelections(modifiedDuplicateProjectNameSelections);
            }

            return wasFound;
        }

        public async Task<string[]> GetAllProjectFilePaths()
        {
            var projects = await this.LoadProjects();

            var output = projects
                .Select(xProject => xProject.FilePath)
                .ToArray();

            return output;
        }

        public async Task<bool> DeleteProject(string filePath)
        {
            var projects = await this.LoadProjects();

            var wasFound = FileBasedProjectRepository.HasProject(projects, filePath);
            if (wasFound)
            {
                var modifiedProjects = projects
                    .Where(x => x.FilePath != filePath)
                    ;

                await this.SaveProjects(modifiedProjects);
            }

            return wasFound;
        }

        public async Task<Project[]> GetAllProjects()
        {
            var projects = await this.LoadProjects();
            return projects;
        }

        public async Task<string[]> GetAllIgnoredProjectNames()
        {
            var ignoredProjectNames = await this.LoadIgnoredProjectNames();
            return ignoredProjectNames;
        }

        public async Task<ProjectNameSelection[]> GetAllDuplicateProjectNameSelections()
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();
            return duplicateProjectNameSelections;
        }

        public async Task<ProjectNameSelection[]> GetAllProjectNameSelections()
        {
            var allProjectNameSelections = await this.LoadProjectNameSelections();
            return allProjectNameSelections;
        }

        public async Task<bool> DeleteProjectNameSelection(string projectName)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasProjectNameSelection(projectNameSelections, projectName);
            if (wasFound)
            {
                var modifiedProjectNameSelections = projectNameSelections
                    .Where(x => x.ProjectName != projectName)
                    ;

                await this.SaveProjectNameSelections(modifiedProjectNameSelections);
            }

            return wasFound;
        }

        public async Task<Dictionary<string, bool>> DeleteProjectNameSelections(IEnumerable<string> projectNames)
        {
            var projectNameSelections = await this.LoadProjectNameSelections();

            var projectNamesHash = new HashSet<string>(projectNames);

            var output = new Dictionary<string, bool>();

            var modifiedProjectNameSelections = projectNameSelections
                .Where(xProjectNameSelection =>
                {
                    var projectName = xProjectNameSelection.ProjectName;

                    var removeProject = projectNamesHash.Contains(projectName);
                    if (removeProject)
                    {
                        output.Add(projectName, true);
                    }
                    else
                    {
                        output.Add(projectName, false);
                    }

                    var keepProject = !removeProject;
                    return keepProject;
                })
                ;

            await this.SaveProjectNameSelections(modifiedProjectNameSelections);

            return output;
        }

        public async Task<bool> DeleteDuplicateProjectNameSelection(string projectName)
        {
            var duplicateProjectNameSelections = await this.LoadDuplicateProjectNameSelections();

            var wasFound = FileBasedProjectRepository.HasDuplicateProjectNameSelection(duplicateProjectNameSelections, projectName);
            if (wasFound)
            {
                var modifiedDuplicateProjectNameSelections = duplicateProjectNameSelections
                    .Where(x => x.ProjectName != projectName)
                    ;

                await this.SaveDuplicateProjectNameSelections(modifiedDuplicateProjectNameSelections);
            }

            return wasFound;
        }

        public async Task ClearAllProjectNameSelections()
        {
            await this.SaveProjectNameSelections(EnumerableHelper.Empty<ProjectNameSelection>());
        }

        public async Task<Dictionary<string, WasFound<Project>>> HasProjectsByFilePath(IEnumerable<string> filePaths)
        {
            var projects = await this.LoadProjects();

            var filePathsSet = new HashSet<string>(filePaths);

            var foundProjects = projects
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

            return output;
        }

        public async Task<Dictionary<TKey, WasFound<TValue>>> HasProjectValues<TKey, TValue>(
            IEnumerable<TKey> keys,
            Func<IProject, TKey> keySelector,
            Func<IProject, TValue> valueSelector)
        {
            var projects = await this.LoadProjects();

            var valuesOrDefault = from key in keys
                join project in projects on key equals keySelector(project) into projectGroup
                from projectOrDefault in projectGroup.DefaultIfEmpty()
                select new { Key = key, Value = valueSelector(projectOrDefault) };

            var output = valuesOrDefault.ToDictionary(
                x => x.Key,
                x => WasFound.From(x.Value));

            return output;
        }
    }
}