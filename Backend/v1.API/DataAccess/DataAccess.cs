using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Windows.Input;
using v1.API.Models;

namespace v1.API.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string dbConnection;
        private readonly string dateFormat;


        public DataAccess(IConfiguration _configuration)
        {
            this.configuration = _configuration;
            dbConnection = this.configuration["ConnectionStrings:DB"];
            dateFormat = this.configuration["Constants:DateFormat"];
        }

        public Offer GetOffer(int id)
        {
            var offer = new Offer();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM Offers WHERE OfferId="+id+";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    offer.Id = (int)r["OfferId"];
                    offer.Title = (string)r["Title"];
                    offer.Discount = (int)r["Discount"];

                }
            }
            return offer;
        }

      
        public List<ProductCategory> GetProductCategories()
        {
            var productCategories= new List<ProductCategory>();
            using (SqlConnection connection = new(dbConnection))//===THIS WILL CONNECT TO SQL CONNECTION STRING. QUERY WILL EXECUTE IN THIS CONNECTION
            {
                SqlCommand command = new() //COMMAND WILL EXECUTE THE QUERY
                {
                    Connection = connection
                };
                string query = "SELECT * FROM ProductCategories;";
                command.CommandText = query;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader(); // SQL DATA WILL STORE IN THE READER METHOD
                while (reader.Read()) //we can read data through read mehtod from reader
                {
                    var category = new ProductCategory()
                    {
                        Id = (int)reader["CategoryId"],
                        Category = (string)reader["Category"],
                        SubCategory = (string)reader["SubCategory"]
                    };
                    productCategories.Add(category);

                }
            }
            return productCategories;
        }

        public ProductCategory GetProductCategory(int id)
        {
            var productCategory = new ProductCategory();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM ProductCategories Where CategoryId="+id+";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    productCategory.Id = (int)r["CategoryId"];
                    productCategory.Category = (string)r["Category"];
                    productCategory.SubCategory = (string)r["SubCategory"];

                }
            }
            return productCategory;
        }

        public List<Product> GetProducts(string category, string subcategory, int count)
        {
            var products = new List<Product>();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT TOP " + count + " * FROM Products WHERE CategoryId IN (SELECT CategoryId FROM ProductCategories WHERE Category=@c AND SubCategory=@s) ORDER BY newid();";
                command.CommandText = query;                
                command.Parameters.Add("@c", System.Data.SqlDbType.NVarChar).Value = category;
                command.Parameters.Add("@s", System.Data.SqlDbType.NVarChar).Value = subcategory;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    var product = new Product()
                    {
                        Id = (int)r["ProductId"],
                        Title = (string)r["Title"],
                        Description=(string)r["Description"],
                        Price = (double)r["Price"],
                        Quantity = (int)r["Quantity"],
                        ImageName = (string)r["ImageName"]
                    };
                    var categoryid = (int)r["CategoryId"];
                    product.ProductCategory=GetProductCategory(categoryid);

                    var offerid = (int)r["OfferId"];
                    product.Offer = GetOffer(offerid);

                    products.Add(product);

                }
            }

            return products;
        }

        public Product GetProduct(int id)
        {
            var product= new Product();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM Products WHERE ProductId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {

                    product.Id = (int)r["ProductId"];
                    product.Title = (string)r["Title"];
                    product.Description = (string)r["Description"];
                    product.Price = (double)r["Price"];
                    product.Quantity = (int)r["Quantity"];
                    product.ImageName = (string)r["ImageName"];
                    
                    var categoryid = (int)r["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)r["OfferId"];
                    product.Offer = GetOffer(offerid);


                }
            }

            return product;
        }

        public bool InsertUser(User user)
        {
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE Email='" + user.Email + "';";

                command.CommandText = query;

                int count=(int) command.ExecuteScalar();
                if (count>0)    
                {
                    connection.Close();
                    return false;
                }

                query = "INSERT INTO Users (FirstName,LastName,Address,Mobile,Email,Password,CreatedAt,ModifiedAt)VALUES(@fn,@ln,@add,@mb,@em,@pwd,@cat,@mat);";
                command.CommandText = query;
                command.Parameters.Add("@fn", System.Data.SqlDbType.NVarChar).Value = user.FirstName;
                command.Parameters.Add("@ln", System.Data.SqlDbType.NVarChar).Value = user.LastName;
                command.Parameters.Add("@add", System.Data.SqlDbType.NVarChar).Value = user.Address;
                command.Parameters.Add("@mb", System.Data.SqlDbType.NVarChar).Value = user.Mobile;
                command.Parameters.Add("@em", System.Data.SqlDbType.NVarChar).Value = user.Email;
                command.Parameters.Add("@pwd", System.Data.SqlDbType.NVarChar).Value = user.Password;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = user.CreatedAt;
                command.Parameters.Add("@mat", System.Data.SqlDbType.NVarChar).Value = user.ModifiedAt;

                command.ExecuteNonQuery();
            }
            return true;

        }
    
    
        public string IsUserPresent(string email, string password)
        {
            User user = new();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email='" + email + "' AND Password='" + password +"';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();
                if(count == 0)
                {
                    connection.Close();
                    return "";
                }
                query = "SELECT * FROM Users WHERE Email ='" + email + "' AND Password='" + password + "';";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = (int)reader["UserId"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Email = (string)reader["Email"];
                    user.Address = (string)reader["Address"];
                    user.Mobile = (string)reader["Mobile"];
                    user.Password = (string)reader["Password"];
                    user.CreatedAt = (string)reader["CreatedAt"];
                    user.ModifiedAt = (string)reader["ModifiedAt"];
                }
                string key = "MNU66iBl2374289364273846";
                string duration = "60";
                var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                //algo for encode
                var credentials=new SigningCredentials(symmetrickey,SecurityAlgorithms.HmacSha256);
                //===the data that contain inside an arrray
                var claims = new[]
                {
                    new Claim("id",user.Id.ToString()),
                    new Claim("firstname",user.FirstName),
                    new Claim("lastname",user.LastName),
                    new Claim("address",user.Address),
                    new Claim("mobile",user.Mobile),
                    new Claim("email",user.Email),
                    new Claim("createdat",user.CreatedAt),
                    new Claim("modifiedat",user.ModifiedAt)
                };

                var jwtToken = new JwtSecurityToken(
                    issuer:"localhost",
                    audience:"locahost",
                    claims:claims,
                    expires:DateTime.Now.AddMinutes(Int32.Parse(duration)),
                    signingCredentials:credentials
                    );

                // now we convert this token into string.
                return new JwtSecurityTokenHandler().WriteToken(jwtToken);

            }
            return "";
        }

        public void InsertReview(Review review)
        {
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "INSERT INTO Reviews(UserId,ProductId,Review,CreatedAt)VALUES(@uid,@pid,@rv,@cat);";
                command.CommandText = query;
                command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = review.User.Id;
                command.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = review.Product.Id;
                command.Parameters.Add("@rv",  System.Data.SqlDbType.NVarChar).Value = review.Value;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = review.CreatedAt;

                connection.Open();
                command.ExecuteNonQuery();              

            }
          
        }

        public List<Review> GetProductReviews(int productId)
        {
            User user = new();
            using (SqlConnection connection = new(dbConnection))
            {
                var reviews=new List<Review>();
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM reviews WHERE ProductId='" + productId + "'ORDER BY ReviewId DESC;";
                command.CommandText = query;
                connection.Open();
                SqlDataReader reader =command.ExecuteReader();

                while (reader.Read())
                {
                    var review = new Review()
                    {
                        Id = (int)reader["ReviewId"],
                        Value = (string)reader["Review"],
                        CreatedAt = (string)reader["CreatedAt"]
                    };
                    var userid = (int)reader["UserId"];
                    review.User = GetUser(userid);

                    var productid = (int)reader["ProductId"];
                    review.Product=GetProduct(productId);
                    reviews.Add(review);
                    
                }
                return reviews;
            }
        }

        public User GetUser(int id)
        {
            var user = new User();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM Users WHERE UserId="+ id +";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    user.Id = (int)r["UserId"];
                    user.FirstName = (string)r["FirstName"];
                    user.LastName = (string)r["LastName"];
                    user.Email = (string)r["Email"];
                    user.Address = (string)r["Address"];
                    user.Mobile = (string)r["Mobile"];
                    user.Password = (string)r["Password"];
                    user.CreatedAt = (string)r["CreatedAt"];
                    user.ModifiedAt = (string)r["ModifiedAt"];
                }
            }
            return user;
        }

        public bool InsertCartItem(int userId, int productId)
        {
            User user = new();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection=connection
                };
                connection.Open();
                string query = "SELECT COUNT(*) FROM Carts WHERE UserId=" + userId + "AND Ordered='false';";
                command.CommandText= query;
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    query = "INSERT INTO Carts(UserId, Ordered, OrderedOn) VALUES (" + userId + ",'false','');";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }

                query = "SELECT CartId FROM Carts WHERE UserId=" + userId + "AND Ordered='false';";
                command.CommandText= query;
                int cartId=(int)command.ExecuteScalar();

                query = "INSERT INTO CartItems(CartId, ProductId) VALUES ("+ cartId +", "+ productId +");";
                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
        }

        public Cart GetActiveCartOfUser(int userId)
        {
            var cart = new Cart();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
                string query = "SELECT COUNT(*) FROM Carts WHERE UserId=" + userId + "AND Ordered='false';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    return cart;
                }

                query = "SELECT CartId From Carts WHERE UserId=" + userId + "AND Ordered='false';";
                command.CommandText=(query);
                int cartid = (int)command.ExecuteScalar();

                query = "select * from CartItems where CartId=" + cartid + ";";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CartItem item = new()
                    {
                        Id = (int)reader["CartItemId"],
                        Product = GetProduct((int)reader["ProductId"])
                    };
                    cart.CartItems.Add(item);
                }
                cart.Id = cartid;
                cart.User = GetUser(userId);
                cart.Ordered = false;
                cart.OrderedOn = "";
            }
            return cart;
        }

        public Cart GetCart(int cartid)
        {
            var cart = new Cart();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM CartItems WHERE CartId=" + cartid + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CartItem item = new()
                    {
                        Id = (int)reader["CartItemId"],
                        Product = GetProduct((int)reader["ProductId"])
                    };
                    cart.CartItems.Add(item);
                }
                reader.Close();

                query = "SELECT * FROM Carts WHERE CartId=" + cartid + ";";
                command.CommandText= query;
                reader=command.ExecuteReader();
                while (reader.Read())
                {
                    cart.Id = cartid;
                    cart.User = GetUser((int)reader["UserId"]);
                    cart.Ordered = bool.Parse((string)reader["Ordered"]);
                    cart.OrderedOn = (string)reader["OrderedOn"];
                }
                reader.Close();

            }
            return cart;
        }

        public List<Cart> GetAllPreviousCartsOfUser(int userId)
        {
            var carts = new List<Cart>();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT CartId FROM Carts WHERE UserId=" + userId + "AND Ordered='true';";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var cartid = (int)reader["CartId"];
                    carts.Add(GetCart(cartid));
                }
               
            }
            return carts;
        }

        public List<PaymentMethod> GetPaymentMethods()
        {
            var result=new List<PaymentMethod>();
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
               
                string query = "SELECT * FROM PaymentMethods;";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                   PaymentMethod paymentMethod = new()
                    {
                        Id = (int)reader["PaymentMethodId"],
                        Type = (string)reader["Type"],
                        Provider = (string)reader["Provider"],
                        Available = bool.Parse((string)reader["Available"]),
                       Reason = (string)reader["Reason"]
                   };
                    result.Add(paymentMethod);
                }
            }
            return result;
        }

        public int InsertOrder(Order order)
        {
            int value = 0;

            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "INSERT INTO Orders (UserId, CartId, PaymentId, CreatedAt) values (@uid, @cid, @pid, @cat);";

                command.CommandText = query;
                command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = order.User.Id;
                command.Parameters.Add("@cid", System.Data.SqlDbType.Int).Value = order.Cart.Id;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = order.CreatedAt;
                command.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = order.Payment.Id;

                connection.Open();
                value = command.ExecuteNonQuery();

                if (value > 0)
                {
                    query = "UPDATE Carts SET Ordered='true', OrderedOn='" + DateTime.Now.ToString(dateFormat) + "' WHERE CartId=" + order.Cart.Id + ";";
                    command.CommandText = query;
                    command.ExecuteNonQuery();

                    query = "SELECT TOP 1 Id FROM Orders ORDER BY Id DESC;";
                    command.CommandText = query;
                    value = (int)command.ExecuteScalar();
                }
                else
                {
                    value = 0;
                }
            }

            return value;
        }

        public int InsertPayment(Payment payment)
        {
            int value = 0;
            using (SqlConnection connection = new(dbConnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = @"INSERT INTO Payments (PaymentMethodId, UserId, TotalAmount, ShippingCharges, AmountReduced, AmountPaid, CreatedAt) 
                                VALUES (@pmid, @uid, @ta, @sc, @ar, @ap, @cat);";

                command.CommandText = query;
                command.Parameters.Add("@pmid", System.Data.SqlDbType.Int).Value = payment.PaymentMethod.Id;
                command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = payment.User.Id;
                command.Parameters.Add("@ta", System.Data.SqlDbType.NVarChar).Value = payment.TotalAmount;
                command.Parameters.Add("@sc", System.Data.SqlDbType.NVarChar).Value = payment.ShipingCharges;
                command.Parameters.Add("@ar", System.Data.SqlDbType.NVarChar).Value = payment.AmountReduced;
                command.Parameters.Add("@ap", System.Data.SqlDbType.NVarChar).Value = payment.AmountPaid;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = payment.CreatedAt;

                connection.Open();
                value = command.ExecuteNonQuery();

                if (value > 0)
                {
                    query = "SELECT TOP 1 Id FROM Payments ORDER BY Id DESC;";
                    command.CommandText = query;
                    value = (int)command.ExecuteScalar();
                }
                else
                {
                    value = 0;
                }
            }
            return value;
        }
    }
}
