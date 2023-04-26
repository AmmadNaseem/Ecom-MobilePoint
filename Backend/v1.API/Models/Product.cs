namespace v1.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
        public ProductCategory ProductCategory { set; get; } =new ProductCategory();
        public Offer Offer { get; set; } = new Offer();
        public double Price { get;set; }
        public int Quantity { get; set; }
        public string ImageName { get; set; }=string.Empty; 
    }
}
