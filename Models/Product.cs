namespace vladandartem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string ImgPath { get; set; }
        public string Manufacturer { get; set; }
        public int CategoryId { get; set; }
        public int Count { get; set; }
    }
}