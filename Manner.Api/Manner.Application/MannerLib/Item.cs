using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib
{
    public class Item
    {
        public string ID { get; set; }
        public string Description { get; set; }

        public Item(string ID, string Description)
        {

            this.ID = ID;
            this.Description = Description;
        }

    }
}
