# TrueLayerTest

1. In  __appsettings.Development.json__  add ClientId,ClientSecret and RedirectUri
```
 "TrueLayerApi": {
    "OAuthServerUrl": "https://auth.truelayer.com",
    "RedirectUri": "http://7ef020fc.ngrok.io/callback",
    "ClientId": "",
    "ClientSecret": "",
    "ResourceApi": "https://api.truelayer.com"

  }
```
2. Install ngrok and  setup callback url in https://console.truelayer.com/settings for 
http://localhost:58606/ 
```
ngrok http -host-header=localhost 58606
```
Update the appsettings __RedirectUri__ with  http://*.ngrok.io/callback
 
3. Start Project in VS. It will launch http://localhost:58606/swagger  and click Authorize. It will lead to the following screen below:
![alt text](https://github.com/siddhant-gauchan/TrueLayerTest/blob/master/image/authorize.PNG "Authorize").
4. Click Authorize for Oauth flow and it will be redirected to truleLayer auth url. Enter the login username/password.
5. After sucessfull login. it will redirect to http://*.ngrok.io/callback. 
6. Copy the acccessToken as shown below and Authorize for Bearer 
  ![alt text](https://github.com/siddhant-gauchan/TrueLayerTest/blob/master/image/accesstoken.PNG "Access Token")
7.Go back to previous http://localhost:58606/swagger and authorize using __BearerScheme__ 

8.Execute endpoints from SwaggerUI   
  
# UnitTest
1. Unit test is written using MSTest and Moq.
2. Right click the Test Project and Run Unit Test

# UserInfo Persistance
Use CookieAuthentication Scheme to Persist userInfo. Stores all user info in ClaimsIdentity. It relies on cookie security mechanism.
You can make the cookie using HttpOnly to avoid xss attack

```
var user = storage.Get<List<UserModel>>("userInfo");
                    
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user[0].FullName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user[0].FullName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, string.Join(",", user[0].Emails)));
                    identity.AddClaim(new Claim(ClaimTypes.MobilePhone, string.Join(",",user[0].Phones)));
                    identity.AddClaim(new Claim(ClaimTypes.Expiration, string.Join(",", accessToken.ExpiryDate)));
                    identity.AddClaim(new Claim("RefreshToken", string.Join(",", accessToken.RefreshToken)));
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                    new AuthenticationProperties 
                    { 
                        IsPersistent = true, 
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20) 
                    });
```
