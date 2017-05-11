using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public class AudioFormat
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public IList<Film> Films { get; set; }

    }
}
