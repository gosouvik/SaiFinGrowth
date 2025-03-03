;#DropDB GatiCarRentalEasyTest

#Application GatiCarRentalWin
#Application GatiCarRentalWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef GatiCarRentalWin

*Action About...
 
*Action OK

#EndIf

#IfDef GatiCarRentalWeb
*Action Navigation(My Details)
#EndIf
