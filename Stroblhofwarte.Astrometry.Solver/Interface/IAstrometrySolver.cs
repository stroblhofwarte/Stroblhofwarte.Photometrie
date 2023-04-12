using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Astrometry.Solver.Interface
{
    public interface IAstrometrySolver
    {
        string ErrorText { get; }
        Task<bool> Solve(string filename);
    }
}
