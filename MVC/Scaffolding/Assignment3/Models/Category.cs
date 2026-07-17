using System.ComponentModel.DataAnnotations;

namespace Assignment3.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength (50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength (500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
}
