import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { NavigationService } from '../services/navigation.service';
import { UtilityService } from '../services/utility.service';
import { Cart, Order, Payment, PaymentMethod } from '../models/models';
import { timer } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss']
})
export class OrderComponent implements OnInit {

  selectedPaymentMethodName='';
  selectedPaymentMethod=new FormControl('0');
  paymentMethods:PaymentMethod[]=[];

  userCart:Cart={
    id:0,
    user:this.utilityService.getUser(),
    cartItems:[],
    ordered:false,
    orderedOn:''
  };

  address='';
  mobileNumber='';

  displaySpinner=false;
  message='';
  classname='';

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

  constructor(private navigationService:NavigationService,
    public utilityService:UtilityService, private router:Router) { }

  ngOnInit(): void {
    //===== GET PAYMENT METHODS
    this.navigationService.getPaymentMethods().subscribe((res)=>{
      this.paymentMethods=res;
    })
    
    this.selectedPaymentMethod.valueChanges.subscribe((res:any)=>{
        if(res=='0') this.selectedPaymentMethodName='';
        else this.selectedPaymentMethodName=res.toString();
    });

    //=======FOR GETTING CART
    this.navigationService.getActiveCartOfUser(this.utilityService.getUser().id).subscribe((res:any)=>{
      this.userCart=res;
      this.utilityService.calculatePayment(res,this.userPaymentInfo);
    });

    //=====SET ADDRESS AND MOBILE NUMBER
    this.address=this.utilityService.getUser().address;
    this.mobileNumber=this.utilityService.getUser().mobile;
  }

  getPaymentMethod(id:string){
    let x=this.paymentMethods.find((v)=>v.id ==parseInt(id));
    return x?.type + ' - ' + x?.provider;
  }

  placeOrder(){
    this.displaySpinner=true;
    let isPaymentSuccessful=this.payMoney();
    if (!isPaymentSuccessful) {
      this.displaySpinner=false;
      this.message='Something Went wrong! Payment did not happen.';
      this.classname="text-danger";
      return;      
    }

    let step=0;
    let count=timer(0,3000).subscribe((res)=>{
      ++step;
      if (step==1) {
        this.message='Processing Payment';
        this.classname="text-success";
      }
      if (step==2) {
        this.message='Payment Successful, Order is being placed.';
        this.storeOrder();
      }
      if (step==3) {
        this.message='Your Order has been placed!';
        this.displaySpinner=false;
      }
      if (step==4) {
        this.router.navigateByUrl('/home');
        count.unsubscribe();
      }
    });

  }
  payMoney(){
    return true;
  }

  storeOrder(){
    let payment:Payment;
    let pmid=0;
    if(this.selectedPaymentMethod.value){
      pmid=parseInt(this.selectedPaymentMethod.value);

      payment={
        id:0,
        paymentMethod:{
          id:pmid,
          type:'',
          provider:'',
          available:false,
          reason:'',
        },
        user:this.utilityService.getUser(),
        totalAmount:this.userPaymentInfo.totalAmount,
        shippingCharges:this.userPaymentInfo.shippingCharges,
        amountReduced:this.userPaymentInfo.amountReduced,
        amountPaid:this.userPaymentInfo.amountPaid,
        createdAt:'',
      };

      this.navigationService.insertPayment(payment).subscribe((paymentResponse: any) => {
        payment.id = parseInt(paymentResponse);
          let order: Order = {
            id: 0,
            user: this.utilityService.getUser(),
            cart: this.userCart,
            payment: payment,
            createdAt: '',
          };

          this.navigationService.insertOrder(order).subscribe((orderResponse) => {
            this.utilityService.changeCart.next(0);
          });
          
      });
      
    }
  }

}
