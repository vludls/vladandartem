using System.ComponentModel.DataAnnotations;

namespace vladandartem.Models
{
    public class Test : RegularExpressionAttribute
    {
        public Test(string pattern, string ErrorMessageCustom) : base(pattern) {
            ErrorMessage = ErrorMessageCustom;
        }
    }
    public class Product
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Введите название товара")]
        public string Name { get; set; }
        public int Price { get; set; }

        [Required(ErrorMessage = "Добавьте изображение")]
        public string ImgPath { get; set; }

        [Required(ErrorMessage = "Введите производителя")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int Count { get; set; }
    }
}