using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LaunchpadTest.Models {
    public class ImageModel {
        [Key]
        public int ImageID { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public string FullSizeLocaiton { get; set; }

        public string faceLocation { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
    }
}