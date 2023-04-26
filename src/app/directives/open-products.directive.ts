import { Directive,HostListener, Input } from '@angular/core';
import { Category } from '../models/models';
import { Router } from '@angular/router';

@Directive({
  selector: '[OpenProducts]'
})
export class OpenProductsDirective {

  //  ===THIS WILL USED WHEN SOMEONE CLICK ON PRODUCT CATEGORY THEN SHOW THE PRODUCTS OF THAT CATEGORY

  @Input() category:Category={
    id:0,
    category:'',
    subCategory:'',
  };

  // HOST LISTENER APPLY ON ANY HOST (LINK) THEN IT WILL CHECK REPEATABLY
  @HostListener('click') openProducts(){
      this.router.navigate(['/products'],{
        queryParams:{
          category:this.category.category,
          subcategory:this.category.subCategory
        },
      });
  }

  constructor(private router:Router) { }

}
