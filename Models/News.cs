using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Query;

namespace FakeNews.Models
{
    public class News
    {
        public int ID { get; set; }

        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [StringLength(9000, MinimumLength = 3)]
        [Required]
        public string Content { get; set; }

        [Display(Name = "Publish Date")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        public string Domain { get; set; }

        [RegularExpression(@"Fake|Real")]
        public string Credibility { get; set; }

        public int NumberOfLikes { get; set; }
        public int NumberOfDislike { get; set; }
    }
}
