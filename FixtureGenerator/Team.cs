using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FixtureGenerator
{
    class Team
    {
        public Int32 ID;
        public string Name;
        public string Code;
        public bool Dummy;
  
        public Team()
        {

        }
        public Team(Int32 id, string name, string code)
        {
            ID = id;
            Name = name;
            Code = code;
        }

    }
}
