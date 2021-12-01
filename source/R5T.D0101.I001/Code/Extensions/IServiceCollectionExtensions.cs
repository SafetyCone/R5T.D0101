using System;

using Microsoft.Extensions.DependencyInjection;
using R5T.T0062;

using R5T.T0063;


namespace R5T.D0101.I001
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="FileBasedProjectRepository"/> implementation of <see cref="IProjectRepository"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection ForwardFileBasedProjectRepositoryToProjectRepository(this IServiceCollection services,
            IServiceAction<IFileBasedProjectRepository> fileBasedProjectRepositoryAction)
        {
            services
                .Run(fileBasedProjectRepositoryAction)
                .AddSingleton<IProjectRepository>(sp => sp.GetRequiredService<IFileBasedProjectRepository>());

            return services;
        }

        /// <summary>
        /// Adds the <see cref="FileBasedProjectRepository"/> implementation of <see cref="IFileBasedProjectRepository"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddFileBasedProjectRepository(this IServiceCollection services,
            IServiceAction<IProjectRepositoryFilePathsProvider> projectRepositoryFilePathsProviderAction)
        {
            services
                .Run(projectRepositoryFilePathsProviderAction)
                .AddSingleton<IFileBasedProjectRepository, FileBasedProjectRepository>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="FileBasedProjectRepository"/> implementation of <see cref="IProjectRepository"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddFileBasedProjectRepositoryAsProjectRepository(this IServiceCollection services,
            IServiceAction<IProjectRepositoryFilePathsProvider> projectRepositoryFilePathsProviderAction)
        {
            services
                .Run(projectRepositoryFilePathsProviderAction)
                .AddSingleton<IProjectRepository, FileBasedProjectRepository>();

            return services;
        }
    }
}