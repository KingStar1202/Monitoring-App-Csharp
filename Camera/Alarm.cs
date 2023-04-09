using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    public class Alarm 
    {
        public Alarm(int id , string inside , string outside , string setup)
        {
            this.id = id;
            this.inside = inside;
            this.inside = inside;
            this.setup = setup;
        }

        public int id
        {
            get;
            set;
        }
        public string inside
        {
            get;
            set;
        }

        public string outside
        {
            get;
            set;
        }
        public string setup
        {
            get;
            set;
        }
    }
}
