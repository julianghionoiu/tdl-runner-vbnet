using System;

namespace BeFaster.Runner.Exceptions
{
    public class SolutionNotImplementedException : Exception
    {
        public SolutionNotImplementedException()
            : base("Solution not implemented")
        {
        }
    }
}
