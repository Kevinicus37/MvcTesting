using System.Collections.Generic;

namespace MvcTesting.Models
{
    public class MediaFormat : IItemList
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public IList<Film> Films { get; set; }
    }
}
