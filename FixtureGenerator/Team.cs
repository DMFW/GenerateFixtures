using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FixtureGenerator
{
    class Team
    {
        public short ID;
        public string Name;
        public string Code;
        public bool Dummy;
  
        public Team()
        {

        }
        public Team(short id, string name, string code)
        {
            ID = id;
            Name = name;
            Code = code;
        }

    }
}
