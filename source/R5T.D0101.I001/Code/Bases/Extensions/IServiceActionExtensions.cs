using System;

using R5T.T0062;
using R5T.T0063;


namespace R5T.D0101.I001
{
    public static class IServiceActionExtensions
    {
        /// <summary>
        /// Adds the <see cref="FileBasedProjectRepository"/> implementation of <see cref="IProjectRepository"/> as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IProjectRepository> ForwardFileBasedProjectRepositoryToProjectRepositoryAction(this IServiceAction _,
            IServiceAction<IFileBasedProjectRepository> fileBasedProjectRepositoryAction)
        {
            var serviceAction = _.New<IProjectRepository>(services => services.ForwardFileBasedProjectRepositoryToProjectRepository(
                fileBasedProjectRepositoryAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="FileBasedProjectRepository"/> implementation of <see cref="IFileBasedProjectRepository"/> as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IFileBasedProjectRepository> AddFileBasedProjectRepositoryAction(this IServiceAction _,
            IServiceAction<IProjectRepositoryFilePathsProvider> projectRepositoryFilePathsProviderAction)
        {
            var serviceAction = _.New<IFileBasedProjectRepository>(services => services.AddFileBasedProjectRepository(
                projectRepositoryFilePathsProviderAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="FileBasedProjectRepository"/> implementation of <see cref="IProjectRepository"/> as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IProjectRepository> AddFileBasedProjectRepositoryAsProjectRepositoryAction(this IServiceAction _,
            IServiceAction<IProjectRepositoryFilePathsProvider> projectRepositoryFilePathsProviderAction)
        {
            var serviceAction = _.New<IProjectRepository>(services => services.AddFileBasedProjectRepositoryAsProjectRepository(
                projectRepositoryFilePathsProviderAction));

            return serviceAction;
        }
    }
}
