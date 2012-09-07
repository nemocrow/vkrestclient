VkRestClient
============

This is a Vkontakte (http://vk.com) REST API client that I was using for a WP7 project. The API is incomplete and is in kinda alpha/dead stage. But if you are actually going to use this, feel free to contact me and I'll try to be of as much help as possible.

Sample code
===========

Basic usage looks like this:
```
// Authenticate
var authResponse = await VkApi.Methods.Auth.GetToken(
    'my_login', 'my_password',
    'my_app_id', 'my_app_key')
     .ExecuteFree();

// Check if authentication succeeded
if (!authResponse.IsSuccess) throw new Exception("Couldn't authenticate!");

// Create a session object
var userSession = new VkApiSession(
    @"http://api.vk.com",
    authResponse.Data);

// Get ten last messages
var getMessagesResponse = await VkApi.Methods.Messages.Get(10)
    .ExecuteIn(userSession);

// Check if everything's OK
if (!getMessagesResponse.IsSuccess) throw new Exception("Couldn't fetch messages!");

// Print them out!
Console.WriteLine("Your last 10 (at most) messages are:\n" + string.Join("\n", from m in getMessagesResponse.Data.Messages select m.Body));
```

Dependencies
============

I used RestSharp for HTTP transport and Newtonsoft JSON.NET to parse JSON. I also made some use of Async CTP, although the API itself is usable with plain continuation callbacks.

Precautions
===========

The tests are integration-style and should cover almost all the code.

All basic stuff like authentication and sending/receiving messages works. What might not work as you expect (or not work at all) is the long polling.

License
=======

I think the BSD license is suitable for this kind of software.