﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Ship
    {
        public string Name { get; set; }
        public int Length { get; set; }



        public virtual string ToString()
        {
            return string.Format(Name + " | " + Length.ToString());
        }


    }
}
