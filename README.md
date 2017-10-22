# accipitridae
Hawk authentication handler for HttpClient in .NET 

## Install
With Nuget:
```
Install-Package Fantasista.Accipitridae -Version 0.1.0
```

Or with Dotnet Core:
```
dotnet add package Fantasista.Accipitridae --version 0.1.0
```

See https://www.nuget.org/packages/Fantasista.Accipitridae/ for more information about the nuget package.  

## How to use accipitridae
```C#
var hawkHandler = new HawkAuthHandler(id,secret_key);
var client = new HttpClient(hawkHandler);
var res=await client.GetAsync("http://localhost:8000/hello");

res=await client.PostAsync("http://localhost:8000/hello",new StringContent("This is a test!"));

res=await client.PutAsync("http://localhost:8000/hello",new StringContent("This is a test!"));

res=await client.DeleteAsync("http://localhost:8000/hello");

```

## What kind of name is Accipitridae
I probably should not work in marketing.

## What is Hawk? (A short explanation of the protocol)
Hawk is an HMAC based authentication scheme invented by Eran Hammer.  Before you can authenticate you will need an Id and and a secret key, and uses the authorization header to authenticate the request.  
This is done by creating a newline delimited string of some basic information:  
 * The first line just says "hawk.1.header"
 * The second line is the number of seconds since 1970-01-01, UTC.  
 * The third line consists of a nonce, a random string that prevents resending of the request.  Accipitridae just uses a Guid.
 * The fourth line is the HTTP method, uppercase.
 * The fifth line is the path and query part of the url (/test/test?query=test)
 * The sixth line is the host (localhost)
 * The seventh line is the port
 * The eight line is a little unclear, but from implementations I think it is meant for a hash
 * The ninth line is app-ext-data, which is used for app-ext-data.  Accipitridae uses a SHA256 hash of the content.
 
Note that the last line also ends with a newline.  
  
Then the newline delimited string is hashed with Sha256 and the secret key.  This will return in a byte array, which is Base64 encoded.  This is the authentication hash to be used.  

The last part is to construct the authorization header.  This is done by setting Hawk as scheme and then a comma separated list consisting of some of the information from above:  
id=The id you got with your secret.  
ts=The timestamp used in the hash calculation.  
nonce=The random string used in the hash calculation.  
ext=The app-ext-data used in the hash calculation.  
mac=The hash you got from the calculation above.  

That is all.  

### Example
Let us say you been provided "test" as id and "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" as secret and you want to send a GET request to http://localhost:8000/hello:  
We start by creating the string which should be hashed:
```
hawk.1.header
1508543566
f78e0103-905f-4167-a4c5-da5f0bf225a1
GET
/hello
localhost
8000



```
Note that the line for app-ext-data is empty on a GET.  
This should be hashed to vy9EG25J1arliFSjHwcxLy6vQX0AMId9xsDqxn4uqo4=, which will give us this authorization header:  
```
authorization: Hawk id="test", ts="1508543566", nonce="f78e0103-905f-4167-a4c5-da5f0bf225a1", mac="vy9EG25J1arliFSjHwcxLy6vQX0AMId9xsDqxn4uqo4="
```

For more information see : https://alexbilbie.com/2012/11/hawk-a-new-http-authentication-scheme/
