using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTesting.Models
{
    public enum SortPriority
    {
        [Display(Name = "Title")]
        Title,
        [Display(Name = "Title Descending")]
        TitleDesc,
        [Display(Name = "Year")]
        Year,
        [Display(Name = "Year Descending")]
        YearDesc,
        [Display(Name = "Media")]
        Media,
        [Display(Name = "Media Descending")]
        MediaDesc,
        [Display(Name = "Audio")]
        Audio,
        [Display(Name = "Audio Descending")]
        AudioDesc
    }
}
