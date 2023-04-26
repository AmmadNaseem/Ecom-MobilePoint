import { Category, Product } from './../models/models';
import { UtilityService } from './../services/utility.service';
import { NavigationService } from './../services/navigation.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  view:'grid'|'list'='list';
  sortby:'default' | 'htl' | 'lth' ='default';
  products:Product[]=[];

  constructor(private activatedRoute:ActivatedRoute,
              private navigationService:NavigationService,
              private utilityService:UtilityService
              ) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params:any)=>{
      let category=params.category;
      let subcategory=params.subcategory;
        if (category && subcategory) {
            this.navigationService.getProducts(category,subcategory,10)
            .subscribe((res:any)=>{
              this.products=res;
            });          
        }
    });
    
  }

  sortByPrice(sortKey:string){
    //this sort(p1,p2) takes two parameters and loop on array and sort them then take next 2 element and sort all the elements of array. this can return 3 different values:+ve,-ve and zero.
    //sort a after b (>0)=will return +ve no.
    //sort a before b (<0)=will return -ve no.
    //keep original order of a and b (===0) will return zero.
    this.products.sort((a,b)=>{
      if (sortKey=='default') {
        return a.id > b.id ? 1: -1;
        //this will return +ve value.  
        //'a' will return after 'b' 
        //if 'a' -ve then it will return first.     
      }
      return (sortKey=='htl' ? 1 : -1 ) * (this.utilityService.applyDiscount(a.price,a.offer.discount) > this.utilityService.applyDiscount(b.price,b.offer.discount) ? -1 : 1);
      //if high to low then * use to return -1 and voice versa.e.g: htl= 1*-1(a>b)=-1 [a will return after b]

      // if (sortKey=='htl') {
      //   return this.utilityService.applyDiscount(a.price,a.offer.discount) > this.utilityService.applyDiscount(b.price,b.offer.discount) ? -1 : 1;
      //   // {
      //   //   return -1;
      //   //   //if 'a' is greater then it will return first and -ve number. and display first.   
      //   // }else{
      //   //   return 1;
      //   // }
            
      // }
      // if (sortKey=='lth') {
      //   return this.utilityService.applyDiscount(a.price,a.offer.discount) > this.utilityService.applyDiscount(b.price,b.offer.discount) ? 1 : -1;
      //   // if 'a' is greater then it will return last then we return +ve number for returning 'a' at the end/last 
      // }
    });

  }

}
