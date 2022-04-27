using System;

using AppType = R5T.T0097.Project;
using EntityType = R5T.D0101.I001.Entities.Project;


namespace System
{
    public static class ProjectExtensions
    {
        public static AppType ToAppType(this EntityType entityType)
        {
            var output = new AppType
            {
                FilePath = entityType.FilePath,
                Identity = entityType.Identity,
                Name = entityType.Name,
            };

            return output;
        }
    }
}
