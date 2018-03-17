using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUtils
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length==2)
            {
                var dbtype = args[0];
                var connectionString = args[1];
                DatabaseUtils.TestConnectionString(dbtype, connectionString);
            }
            else
            {
                throw new ArgumentException();
            }

        }
    }
}
