using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class MediaFormatViewModel
    {
        public List<MediaFormat> Items { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }

        public MediaFormatViewModel()
        {
            Controller = "Media";
        }
    }
}
