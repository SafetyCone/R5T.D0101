using System;
using System.Collections.Generic;
using System.Linq;


using R5T.D0101;

using Instances = R5T.D0101.X001.Instances;


namespace System
{
    public static partial class IProjectRepositoryExtensions
    {
        public static Guid GetProjectIdentity(this IProjectRepository _,
            string projectIdentityString)
        {
            var output = Instances.GuidOperator.FromStringStandard(projectIdentityString);
            return output;
        }

        public static IEnumerable<Guid> GetProjectIdentities(this IProjectRepository _,
            IEnumerable<string> projectIdentityStrings)
        {
            var output = projectIdentityStrings
                .Select(xProjectIdentityString => _.GetProjectIdentity(xProjectIdentityString))
                ;

            return output;
        }
    }
}
