import { FormControl } from '@angular/forms';
import { Product, Review } from './../models/models';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NavigationService } from '../services/navigation.service';
import { UtilityService } from '../services/utility.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {

  imageIndex:number=1;
  product!:Product;
  reviewControl=new FormControl('');
  showError:boolean=false;
  reviewSaved:boolean=false;
  otherReviews:Review[]=[];

  constructor(private activatedRoute:ActivatedRoute,
    private navigationService:NavigationService,
    public utilityService:UtilityService
    ) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params:any)=>{
      let id= params.id;
      this.navigationService.getProduct(id).subscribe((res:any)=>{
          this.product=res; 
          this.fetchAllProductReviews();         
      });

    });
  }

  submitReview(){
    let review=this.reviewControl.value;
    if (review =='' || review == null) {
      this.showError=true;
      return;      
    }

    let userid=this.utilityService.getUser().id;
    let productid=this.product.id;

    this.navigationService.addReview(userid,productid,review).subscribe((res)=>{
      this.reviewSaved=true;
      this.fetchAllProductReviews();
      this.reviewControl.setValue("");
    });

  }

  fetchAllProductReviews(){
    this.otherReviews=[];
    this.navigationService.getAllReviewsOfProduct(this.product.id).subscribe((res:any)=>{
        for(let review of res){
          this.otherReviews.push(review)          
        }
    });

  }

}
