namespace WebShop1.Models
{
    public class OrderViewModel
    {
        public decimal TTotalAmount { get; set; }
        public List<OrderItemViewModel> OrederItems { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
