using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HFTest.Services
{
    public class SomeService : ISomeService
    {
        public async  Task SomeMethod()
        {
            await Task.Delay(10000);
        }
    }
}