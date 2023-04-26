import { UtilityService } from './../services/utility.service';
import { RegisterComponent } from './../register/register.component';
import { LoginComponent } from './../login/login.component';
import { Component, ElementRef, OnInit, Type, ViewChild, ViewContainerRef } from '@angular/core';
import { Category, NavigationItem } from '../models/models';
import { NavigationService } from '../services/navigation.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  navigationList:NavigationItem[]=[];

  @ViewChild('modalTitle') modalTitle!:ElementRef;

  // ===HERE WE ARE TAKING ONLY THE VIEW REFERENCE OF VARIABLE NOT THE ELEMENT.
  @ViewChild('container',{read:ViewContainerRef,static:true}) container!:ViewContainerRef;

  cartItems:number=0;

  constructor(private navigationService:NavigationService,public utilityService:UtilityService) { }

  ngOnInit(): void {
    // ====GETTING CATEGORY LIST FROM SERVICE
    
      this.navigationService.getCategoryList().subscribe((list:Category[])=>{
            for(let item of list){
                let present=false;
                for (let navItem of this.navigationList){
                  if (navItem.category==item.category) {
                      navItem.subcategories.push(item.subCategory);
                      present=true;                
                  }
                }
                if(!present){
                  this.navigationList.push({
                    category:item.category,
                    subcategories:[item.subCategory],
                  });
                }
            }      
      });
      //===For Cart
      if(this.utilityService.isLoggedIn()){
        this.navigationService.getActiveCartOfUser(this.utilityService.getUser().id).subscribe((res)=>{
          this.cartItems=res.cartItems.length;
        });
      }

      
      this.utilityService.changeCart.subscribe((res:any)=>{
         if (parseInt(res)==0)this.cartItems=0;
         else this.cartItems += parseInt(res);
      });
  }

  // ======FIRST OF ALL I WILL CLEAR THE ALL THE THINGS INSIDE THE CONTAINER
    openModal(name:string){
        this.container.clear();

        let componentType!: Type<any>; //TYPE :Represents a type that a Component or other object is instances of.
        if(name=='login') {
          componentType=LoginComponent;
          this.modalTitle.nativeElement.textContent='Enter Login Information';
        }
        if(name=='register'){
          componentType=RegisterComponent
          this.modalTitle.nativeElement.textContent='Enter Registration Information';
        }

        this.container.createComponent(componentType); //===HERE I CREATED THE LOGIN COMPONENT
    }

}
