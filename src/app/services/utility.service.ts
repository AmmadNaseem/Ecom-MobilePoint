import { NavigationService } from './navigation.service';
import { User, Product, Cart, Payment } from './../models/models';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UtilityService {

  changeCart=new Subject();

  constructor(private jwt:JwtHelperService,private navigationService:NavigationService) { }

  applyDiscount(price:number,discount:number):number{
      let finalPrice:number=price - price * (discount / 100);
      return finalPrice;
  }

  getUser():User{
    let token=this.jwt.decodeToken();
    let user:User={
      id:token.id,
      firstName:token.firstname,
      lastName:token.lastname,
      address:token.address,
      mobile:token.mobile,
      email:token.email,
      password:'',
      createdAt:token.createdat,
      modifiedAt:token.modifiedat
    };
    return user;
  };

  setUser(token:string){
    localStorage.setItem('user',token);
  }

  isLoggedIn(){
    return localStorage.getItem('user')?true:false;
  }

  logoutUser(){
    localStorage.removeItem('user');
  }

  addToCart(product:Product){
    let productid=product.id;
    let userid=this.getUser().id;

    this.navigationService.addToCart(userid,productid).subscribe((res)=>{
        if(res.toString()=='inserted') this.changeCart.next(1);
    });
  }

  calculatePayment(cart:Cart,payment:Payment){
    payment.totalAmount=0;
    payment.amountPaid=0;
    payment.amountReduced=0;

    for (let cartitem of cart.cartItems){
      payment.totalAmount+=cartitem.product.price;

      payment.amountPaid += cartitem.product.price - this.applyDiscount(cartitem.product.price,cartitem.product.offer.discount); //how much discount price
      payment.amountPaid += this.applyDiscount(cartitem.product.price,cartitem.product.offer.discount); //how much we pay \

      //===now for shipping charges
      if (payment.amountPaid > 50000) payment.shippingCharges=2000;
      else if(payment.amountPaid > 20000) payment.shippingCharges=1000;
      else if(payment.amountPaid > 5000) payment.shippingCharges=500;
      else payment.shippingCharges=200;

    }
    
  }

  calculatePricePaid(cart:Cart){
    let pricePaid=0;
    for(let cartitem of cart.cartItems){
      pricePaid += this.applyDiscount(cartitem.product.price,cartitem.product.offer.discount);
    }
    return pricePaid;
  }
}

