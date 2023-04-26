import { Category, Order, Payment, PaymentMethod, User } from './../models/models';
import { Injectable } from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http'
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {

  BASE_URL="https://localhost:7030/api/Shopping/";

  constructor(private http:HttpClient) { }

  getCategoryList(){
    let url=this.BASE_URL+'category';
    // ====this pip will use to convert data according to frontend that we taken from backend. return filter data to frontend according to our requirement we can create array by using map() this will create new array.
    return this.http.get<any[]>(url).pipe(
      map((categories)=>categories.map((category)=>{
        let mappedCategory:Category={
          id:category.id,
          category:category.category,
          subCategory:category.subCategory
        };
        return mappedCategory;
      })
      )
    );
     
  }

  getProducts(category:string,subcategory:string,count:number){
      return this.http.get<any[]>(this.BASE_URL+'products',{
        params:new HttpParams()
            .set('category',category)
            .set('subcategory',subcategory)
            .set('count',count),
      });

  }

  getProduct(id:number){
    let url= this.BASE_URL+'product/'+id;
    return this.http.get(url);
  }

  registerUser(user:User){
    let url=this.BASE_URL+"register";
    return this.http.post(url,user,{responseType:'text'});
    
  }

  loginUser(email:string,password:string){
    let url = this.BASE_URL + 'login';
    return this.http.post(
            url,
            {Email:email,Password:password},
            {responseType:'text'}
      );
  }

  addReview(userid:number,productid:number,review:string){
    let obj:any={
      User:{
        Id:userid,
      },
      Product:{
        Id:productid,
      },
      Value:review,
    };
    let url=this.BASE_URL+'AddReview';
    return this.http.post(url,obj,{responseType:'text'});
  }

  getAllReviewsOfProduct(productId:number){
    let url=this.BASE_URL+'ProductReviews/'+productId;
    return this.http.get(url);
  }

  addToCart(userid:number,productid:number){
    let url=this.BASE_URL+'addCartItem/'+userid+'/'+productid;
    return this.http.post(url,null,{responseType:'text'});
  }

  getActiveCartOfUser(userid:number){
    let url=this.BASE_URL+'GetActiveCartOfUser/'+userid;
    return this.http.get<any>(url);
  }

  getAllPreviousCarts(userid:number){
    let url=this.BASE_URL+'GetAllPreviousCartsOfUser/'+userid;
    return this.http.get(url);
  }

  getPaymentMethods(){
    let url=this.BASE_URL+ "GetPaymentsMethods";
    return this.http.get<PaymentMethod[]>(url);
  }

  insertPayment(payment: Payment) {
    return this.http.post(this.BASE_URL + 'InsertPayment', payment, {
      responseType: 'text',
    });
  }

  insertOrder(order: Order) {
    return this.http.post(this.BASE_URL + 'InsertOrder', order);
  }


}
