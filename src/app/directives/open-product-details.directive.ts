import { Router } from '@angular/router';
import { Directive, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[OpenProductDetails]'
})
export class OpenProductDetailsDirective {

  @Input() productId:number=0;

  @HostListener('click') openProductDetails(){
    window.scrollTo(0,0); // THIS IS FOR SCROLLING ON THE TOP OF THE WINDOW
    this.router.navigate(['/product-details'],{
      queryParams:{
        id:this.productId,
      },
    });
  }

  constructor(private router:Router) { }

}
