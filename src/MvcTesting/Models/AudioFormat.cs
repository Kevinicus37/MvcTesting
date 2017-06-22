using System.Collections.Generic;

namespace MvcTesting.Models
{
    public class AudioFormat : MediaProperty, IItemList
    {
        public IList<Film> Films { get; set; }

    }
}
