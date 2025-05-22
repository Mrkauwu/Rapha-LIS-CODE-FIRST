using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public interface ITestListRepository
    {
        IEnumerable<TestModel> GetAllTests();
    }
}
