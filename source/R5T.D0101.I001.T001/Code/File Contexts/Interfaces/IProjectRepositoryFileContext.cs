using System;

using R5T.T0128;


namespace R5T.D0101.I001
{
    public interface IProjectRepositoryFileContext
    {
        FileSet<Entities.Project> Projects { get; }
    }
}
