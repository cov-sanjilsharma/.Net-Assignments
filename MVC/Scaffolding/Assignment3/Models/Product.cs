using System.ComponentModel.DataAnnotations;

namespace Assignment3.Models
{
    public class Product
    {
        [Required(ErrorMessage = "Please Enter a ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter your Name")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter the price")]
        [Range(1,10000, ErrorMessage = "Please choose a product which is under budget")]
        public int Price { get; set; }
        public Product(int id,string name, int price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
        
    }
}
