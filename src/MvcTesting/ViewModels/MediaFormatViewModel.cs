using MvcTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
