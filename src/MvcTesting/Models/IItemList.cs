using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public interface IItemList
    {
        int ID { get; set; }
        string Name { get; set; }
    }
}
