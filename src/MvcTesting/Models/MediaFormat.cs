using System.Collections.Generic;

namespace MvcTesting.Models
{
    public class MediaFormat : MediaProperty, IItemList
    {
       public IList<Film> Films { get; set; }
    }
}
