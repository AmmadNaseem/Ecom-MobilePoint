import { PaymentMethod } from './../models/models';
import { Component, OnInit } from '@angular/core';
import { NavigationService } from '../services/navigation.service';
import { UtilityService } from '../services/utility.service';
import { Cart, Payment } from '../models/models';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  userCart:Cart={
    id:0,
    user:this.utilityService.getUser(),
    cartItems:[],
    ordered:false,
    orderedOn:''
  };

  userPaymentInfo:Payment={
    id:0,
    user:this.utilityService.getUser(),
    paymentMethod:{
      id:0,
      type:'',
      provider:'',
      available:false,
      reason:''
    },
    totalAmount:0,
    shippingCharges:0,
    amountReduced:0,
    amountPaid:0,
    createdAt:'',
  };

  userPreviousCarts:Cart[]=[];

  constructor(private navigationService:NavigationService,
    public utilityService:UtilityService) { }

  ngOnInit(): void {
    this.navigationService.getActiveCartOfUser(this.utilityService.getUser().id).subscribe((res)=>{
      this.userCart=res;

       // Calculate Payment
       this.utilityService.calculatePayment(this.userCart,this.userPaymentInfo);
      
    });
    //  Get Previous Carts
     this.navigationService
     .getAllPreviousCarts(this.utilityService.getUser().id)
     .subscribe((res: any) => {
       this.userPreviousCarts = res;
     });
  }

}
