using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using v1.API.DataAccess;
using v1.API.Models;

namespace v1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;

        public ShoppingController(IDataAccess _dataAccess, IConfiguration configuration)
        {
            this.dataAccess = _dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
        }

        [HttpGet("category")]
        public IActionResult GetCategoryList() 
        {
            var result = dataAccess.GetProductCategories();
            return Ok(result);
        }


        [HttpGet("products")]
        public IActionResult GetProducts(string category,string subcategory,int count)
        {
            var result=dataAccess.GetProducts(category,subcategory,count);
            return Ok(result);
        }

        [HttpGet("product/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = dataAccess.GetProduct(id);
            return Ok(result);
        }
        
        [HttpPost("Register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            user.CreatedAt=DateTime.Now.ToString(DateFormat);
            user.ModifiedAt = DateTime.Now.ToString(DateFormat);

            var result=dataAccess.InsertUser(user);

            string? message;
            if (result) message = "inserted";
            else message = "email not available";
            return Ok(message);
        }

        [HttpPost("Login")]
        public IActionResult LoginUser([FromBody] User user)
        {
            var token = dataAccess.IsUserPresent(user.Email, user.Password);
            if (token == "") token = "invalid";
            return Ok(token);
        }

        [HttpPost("AddReview")]
        public IActionResult InsertReview([FromBody]Review review)
        {
            review.CreatedAt=DateTime.Now.ToString(DateFormat);
            dataAccess.InsertReview(review);
            return Ok("inserted");
        }
      
        [HttpGet("ProductReviews/{ProductId}")]
        public IActionResult GetProductReviews(int ProductId)
        {
            var result = dataAccess.GetProductReviews(ProductId);
            return Ok(result);
        }

        [HttpPost("addCartItem/{userid}/{productid}")]
        public IActionResult insertCartItem(int userid,int productid)
        {
            var result = dataAccess.InsertCartItem(userid, productid);
            return Ok(result ? "inserted" : "not inserted");

        }
        [HttpGet("GetActiveCartOfUser/{id}")]
        public IActionResult GetActiveCartsOfUser(int id)
        {
            var result=dataAccess.GetActiveCartOfUser(id);
            return Ok(result);
        }

        [HttpGet("GetAllPreviousCartsOfUser/{id}")]
        public IActionResult FetchAllPreviousCartsOfUser(int id)
        {
            var result = dataAccess.GetAllPreviousCartsOfUser(id);
            return Ok(result);
        }

        [HttpGet("GetPaymentsMethods")]
        public IActionResult GetPaymentMethods()
        {
            var result=dataAccess.GetPaymentMethods();
            return Ok(result);
        }

        [HttpPost("InsertPayment")]
        public IActionResult InsertPayment(Payment payment)
        {
            payment.CreatedAt = DateTime.Now.ToString();
            var id = dataAccess.InsertPayment(payment);
            return Ok(id.ToString());
        }

        [HttpPost("InsertOrder")]
        public IActionResult InsertOrder(Order order)
        {
            order.CreatedAt = DateTime.Now.ToString();
            var id = dataAccess.InsertOrder(order);
            return Ok(id.ToString());
        }
    }

}
