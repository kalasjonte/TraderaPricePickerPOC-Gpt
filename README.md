# TraderaPricePickerPOC-Gpt
A POC using OpenAI ChatGpt , Semantic Kernel, for suggesting second hand prices of objects listed on Tradera.

## Scope
I had some time over a weekend and wanted to try my hands on using AI and chatcompletion. The 'problem' to solve was to get the model to suggest prices of an second hand object (in this case xbox360).
This can in theory be well extended, to essentially let the user input a category and an object and fetch the current prices from ongoing auctions and make a suggested price for the user. But this kind of already exists on Tradera's website.
So i settled with essentially feeding live information about xbox360 auctions, and letting the bot decide what price the user should pay.
This project is essentially a "for fun education" for me personally. But if anyone has any use of it, feel free to fork or copy the idea :) 

## What you need to get this up and running:
Your own Api Key and AppId from tradera : https://api.tradera.com/
Your own Api Key from OpenAi (either Azure or OpenAi).

#¤ Want to check prices for a diffrent object?
Change the url parameters sent to tradera for your specific object. Tradera needs a category ID aswell which is a pain, as you need to fetch it first with a seperate request.
 
## How It Works
1. The system makes an API call to Tradera to retrieve the latest auctions for a specific object (e.g., Xbox 360).
2. It processes and filters the auction data, extracting relevant prices.
3. The AI model (powered by OpenAI's ChatGPT and Microsoft Semantic Kernel) analyzes the data and suggests a price range based on current auction listings.
