using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDemo.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "名稱是必填的")]
        [MaxLength(30)]
        [DisplayName("類別名稱")]
        public string Name { get; set; }
    }
}
