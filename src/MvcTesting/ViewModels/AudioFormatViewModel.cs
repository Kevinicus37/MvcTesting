using MvcTesting.Models;
using System.Collections.Generic;

namespace MvcTesting.ViewModels
{
    public class AudioFormatViewModel
    {
        public List<AudioFormat> Items { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }

        public AudioFormatViewModel()
        {
            Controller = "AudioFormat";
        }
    }
}
