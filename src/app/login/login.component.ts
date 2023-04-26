import { UtilityService } from './../services/utility.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { NavigationService } from '../services/navigation.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm!:FormGroup;
  message='';

  constructor(private fb:FormBuilder,private navigationService:NavigationService, private utilityService:UtilityService) { }

  ngOnInit(): void {
    this.loginForm=this.fb.group({
      email:['',[Validators.required,Validators.email]],
      pwd:['',[Validators.required,Validators.minLength(6),Validators.maxLength(15)]]
    });
  }

  login(){
    this.navigationService.loginUser(this.Email.value,this.PWD.value).subscribe((res)=>{
        if (res.toString()!= 'invalid') {
          this.message="Logged In Successfully.";
          this.utilityService.setUser(res.toString());
          console.log(this.utilityService.getUser());
          
        } else {
          this.message="Invalid Credentials!";
        }
    });
  }

  // ====GETTER AND SETTER FOR ACCESSING THE LOGIN FORM===
    get Email():FormControl{
      return this.loginForm.get('email') as FormControl;
    }
    get PWD():FormControl{
      return this.loginForm.get('pwd') as FormControl;
    }

}
