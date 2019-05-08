using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vladandartem.Data.Models
{
    public class Test : RegularExpressionAttribute
    {
        public Test(string pattern, string ErrorMessageCustom) : base(pattern)
        {
            ErrorMessage = ErrorMessageCustom;
        }
    }
    [Serializable]
    public class Product
    {
        public int Id { get; set; }

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

        public List<ProductDetailField> ProductDetailFields { get; set; }

        public int Count { get; set; }
    }
}