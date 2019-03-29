using System.Collections.Generic;
using vladandartem.Models;

public class MainViewModel
{
    public IEnumerable<Product> cartProducts { get; set; }
    public IEnumerable<Product> paidProducts { get; set; }
}