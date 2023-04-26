import { NavigationService } from '../services/navigation.service';
import { Category, Product } from './../models/models';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-suggested-products',
  templateUrl: './suggested-products.component.html',
  styleUrls: ['./suggested-products.component.scss']
})
export class SuggestedProductsComponent implements OnInit {

  @Input() count:number=3;

  @Input() category:Category={
    id:0,
    category:'',
    subCategory:''
  };
  products:Product[]=[];

  constructor(private navigationService:NavigationService) { }

  ngOnInit(): void {
    this.navigationService.getProducts(this.category.category,this.category.subCategory,this.count).subscribe((res:any[])=>{
      for(let product of res){
        this.products.push(product);
      }

    });
  }

}
