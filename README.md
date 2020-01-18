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
Update the appsettings __RedirectUri__ with  http://<random>.ngrok.io/callback
 
3. Start Project in VS. It will launch http://localhost:58606/swagger  and click Authorize. It will lead to the following screen below:
![alt text](https://github.com/siddhant-gauchan/TrueLayerTest/blob/master/image/authorize.PNG "Authorize").
4. Click Authorize for Oauth flow and it will be redirected to truleLayer auth url. Enter the login username/password.
5. After sucessfull login. it will redirect to http://<random>.ngrok.io/callback. 
6. Copy the acccessToken as shown below and Authorize for Bearer 
  ![alt text](https://github.com/siddhant-gauchan/TrueLayerTest/blob/master/image/accesstoken.PNG "Access Token")
7.Go back to previous http://localhost:58606/swagger and authorize using __BearerScheme__ 

8.Execute endpoints from SwaggerUI   
  
