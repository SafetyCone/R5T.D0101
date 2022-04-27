using System;

using R5T.T0094;


namespace R5T.D0101.I001.Entities
{
    public class Project : INamedIdentifiedFilePathed
    {
        public Guid Identity { get; set; }

        public string Name { get; set; }
        public string FilePath { get; set; }
    }
}
