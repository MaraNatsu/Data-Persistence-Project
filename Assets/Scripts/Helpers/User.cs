using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    public class User
    {
        public static string Name { get; private set; }

        public static void SaveName(string name)
        {
            Name = name;
        }
    }
}
