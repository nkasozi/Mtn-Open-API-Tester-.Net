<h1>.Net Tester for the MTN Open API </h1>

The MTN Open API is requires you to have astute knowledge about HTTP Headers. All the authorization and routing happens in the HTTP headers.
To Use the MTN API you need to go through the Steps below.

1. SignUp on their developers portal over here https://momodeveloper.mtn.com/

2. Subscribe for a product. On the developer portal, go to the products page, look for a product you like e.g collections and click on the subscribe button.
You will be given 2 keys, a primary key and a secondary one. Take note of the primary key (you will neeed it in the code). Going forward everytime you see Ocp-Apim-Subscription-Key, they mean this key.

3. Go to https://uuidgenerator.net and copy the uuid there (make sure its version 4).

4. Using postman or any other rest API testing tool, fire a create API user key using the UUID obtained in 3 wherever you see X-Reference-Id. Api documentation is here. https://momodeveloper.mtn.com/docs/services/sandbox-provisioning-api/operations/post-v1_0-apiuser. The result of this API is an empty response with an HTTP Status code of 200 ok.

5. Next you need to create an API key using the endpoint over here. https://momodeveloper.mtn.com/docs/services/sandbox-provisioning-api/operations/post-v1_0-apiuser-apikey
It also needs the UUID created in step 3 wherever you see X-Reference-Id. in the response of this request, you will get the API Key. Take note of it. 


By now you should have the Subscription-Key(Step 2), Api User (UUID in Step 3), Api Key (Step 4) and can now use the code in this tester. This code was developed using Visual Studio Community 2019 and .net core 2.1
