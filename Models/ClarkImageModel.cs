using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TV_DASH_API.Models
{
    public class ClarkImageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        public Guid ImageKey { get; set; }
        // public string Name { get; set; }
        // [Column(TypeName = "nvarchar(50)")]
        //public string Description { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string ImageName { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; }
        public int Floor { get; set; }
        [DefaultValue("1")]
        public int Order { get; set; }
    }

}
