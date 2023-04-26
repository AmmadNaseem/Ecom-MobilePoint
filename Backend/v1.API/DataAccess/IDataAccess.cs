using v1.API.Models;

namespace v1.API.DataAccess
{
    public interface IDataAccess
    {
        List<ProductCategory>GetProductCategories();
        ProductCategory GetProductCategory(int id);
        Offer GetOffer(int id);
        List<Product> GetProducts(string category,string subcategory,int count);    
        Product GetProduct(int id);

        bool InsertUser(User user);

        string IsUserPresent(string email, string password);

        void InsertReview(Review review);

        List<Review> GetProductReviews(int productId);
        User GetUser(int id);
        bool InsertCartItem(int userId, int productId);

        Cart GetActiveCartOfUser(int userId);
        Cart GetCart(int cartid);
        List<Cart> GetAllPreviousCartsOfUser(int userId);

        List<PaymentMethod> GetPaymentMethods();

        int InsertPayment(Payment payment);
        int InsertOrder(Order order);
    }
}
