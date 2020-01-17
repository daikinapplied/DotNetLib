# Introduction
<img src="Images/facebook.png" width="64" align="right" alt="Facebook Project Logo"/>
This is a Facebook library that is written in C# using .NET Standard, with unit testing using .NET Core.  Its purpose is to make it a little easier to call the Facebook Graph API, via Web Api calls and using JSON, to return C# objects with populated data (when successful).

# Getting Started
This presumes a strong understanding of the following tools and technologies:

- Visual Studio 2017 or 2019 Community, Professional, or Enterprise
- .NET Standard 2.0 libraries
- .NET Core 2.2 (for xUnit Testing)
- C# programming language
- Facebook Graph API (last tested on v3.3)

# About non-expiring Access Token
It appears that if you change your login for your Facebook account, the non-expiring access token will become invalidated.  In this case, a new non-expiring access token must be generated.

# Prerequisites
1. A page must be set up ahead of time, with knowledge of its friendly name or ID.  It will be in the format of https://www.facebook.com/*pageid*, where **pageid** is your specific name or ID for that page.
2. An application needs to be set up in Facebook via their [Developer Website](https://developers.facebook.com).  This will generate an **App Secret**, which is used later as part of the non-expiring token process.  

# Facebook Graph API
Facebook Pages are accessed to allow the publishing of content posted to Facebook appear on non-Facebook destinations, such as on a website.  This allows social media to serve as a primary source of information, allowing postings to be visible without requiring all visitors to have a Facebook account.  For public content, it allows standard search engines to crawl and find information.

To use this API, you will need to perform the following steps in a timely manner (before any of the access tokens expire):

1. Setup a [Facebook App](https://developers.facebook.com), setting up an App ID and App Secret, on an account that has access to the Facebook resources desired.
2. Use the [Facebook Graph API Explorer](https://developers.facebook.com/tools/explorer) to generate a **User Token** in the **me** URL path, under an account that has access to the Facebook resources desired, with the default *manage_pags*, *pages_show_list*, and *public_profile* permissions selected.  This token will expire in 1 hour.
3. Use the [Facebook Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken) to verify the **User Token** just generated (you will notice it expires in a short amount of time) and generate an Extended User Token (Click on the **Extend Access Token** button).  Click on the **debug** button to debug and obtain new extended access token.  Its expiration will be longer, but not unlimited.
4. Take the **Extended Access Token** and your **App Secret** to generate an App Secret Proof (appsecret_proof), also known as the **hash_key**.  This can be done via website using a [HMAC Generator](https://www.freeformatter.com/hmac-generator.html) with a SHA256 message digest algorithm.
5. Return back to the [Facebook Graph API Explorer](https://developers.facebook.com/tools/explorer) and paste in the **Extended Access Token** in the **Access Token** field,  Change the URL so **me** is changed to your **pageid** and it has the following parameters `?fields=access_token&appsecret_proof=hash_key`, replacing **hash_key** from the previous step, and click on the *Submit* button.  This will generate a new Non-expiring Access Token.
6. Verify the access token is non-expiring, return back to the [Facebook Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken) and review its expiration.  You may wish to recompute the **hash_key** (appsecret_proof) if testing using Postman (or other tools outside of this library)

Now you are ready to use the Non-expiring Access Token and App Secret in the API calls, which generates the App Secret Proof when needed.

# Samples
To see the Facebook feed integration in action on a website, check out the [Art of Strength Minnesota](http://www.artofstrengthmn.com) website.  Scroll down on the home page to see the Facebook postings automatically appear when new content is added to their Facebook Page.

To see coding examples, check out the Unit Tests in [RecursiveGeek.DotNetLib.Core.Tests](..\RecursiveGeek.DotNetLib.Core.Tests\README.md) for examples on usage, especially `FacebookTests.cs`.

# Build and Test
Open in Visual Studio to build and perform Unit Testing (xUnit for .NET Core).

# References
- [Visual Studio](https://visualstudio.microsoft.com)
- [Facebook Graph API Explorer](https://developers.facebook.com/tools/explorer)
- [Facebook Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken)
- [Facebook Page Feed Reference](https://developers.facebook.com/docs/graph-api/reference/v3.2/page/feed)

# Acknowledgements
- Stack Overflow postings helped figure out how to get a non-expiring access token.
- [Postman](https://www.getpostman.com) has been an invaluable tool, prior to writing any code and validating the "plumbing".

# Documentation Overview
Check out the primary [README.md](../README.md) documentation.

~ end ~
