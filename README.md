# DesafioBroker
Made in a challenge to write a program that could monitor a B3 asset for fluctuations in price and notify via email.

# Compiling the Project
- Open the project with [Visual Studio 2022](https://visualstudio.microsoft.com/)
- Visual Studio should use NuGet to handle any dependencies necessary
- Go to `Build -> Configuration Manager`
- Change `Active solution configuration` to select between `Debug` or `Release`
- Click `Close`
- Go to `Build -> Build Solution` or press `Ctrl + Shift + B`
- Wait for the process to complete
- An executable along with other necessary files will be placed in:
    - `\DesafioBroker\bin\Debug\net6.0` for a `Debug` build
    - `\DesafioBroker\bin\Debug\net6.0` for a `Release` build

# Running
- In a command prompt navigate to were the program executable is located
- Run the following command:

        ./DesafioBroker.exe

    Or

        ./DesafioBroker.exe [ASSET] [MAX PRICE] [MIN PRICE]

    If you wish to configure an asset to be tracked by the program

## Configuration
- The first time you execute the program you might receive the following message:

        Creating config directory in Data...
        DONE

        Creating default config files...
        DONE

        Please edit the config files on Data and them execute DesafioBroker.exe again!

- You will notice that a `Data` folder in the same directory as the executable has been created with 3 `.json` files inside
- You will need to edit this files to configure the program to run properly

### API Config
- The first config file is `ApiConfig.json`, this file is used to select a provider to quote asset prices from
- The configs are:
    - `provider`: the provider to quote prices from. For an automatic one set to `default`
    - `key`: some providers require an API key that should be set here
- **Note:** the default provider used for this application is [Alpha Vantage](https://www.alphavantage.co/), you can get a free lifetime API key from [here](https://www.alphavantage.co/support/#api-key)

## Asset Config
- The second config file is `AssetConfig.json`, this file is used to select an asset to be tracked by the application
- The configs are:
    - `assetToTrack`: the asset you want to track's symbol. Because this program was designed to be used with B3, some providers might automatticaly append `.SA` to the symbol
    - `maxPrice`: the price that the program will send a notification recommending to sell the asset
    - `minPrice`: the price that the program will send a notification recommending to buy the asset
    - `requestDelay`: delay between price quote requests. Be carefull, a really low value may lead to lots of notification emails or cause you to run out of API requests if they are limited by your provider
- **Note:** if you use:

        ./DesafioBroker.exe [ASSET] [MAX PRICE] [MIN PRICE]

    To execute the app, this config file will be updated to use this values

## Email Config
- The last config file is `EmailConfig.json`, this file is used to configure a simple SMTP server to send the email notifications from
- The configs are:
    - `smtpClient`: an SMTP client to be used. For an automatic one set to `default`
    - `senderEmail`: an email address used to send the email,
    - `senderPassword`: the password required to access the `senderEmail`
    - `senderDisplayName`: display name that will be used in the email
    - `receiverEmail`: the target address for the email notifications
- **Note:** the default client used for this application is `smtp.gmail.com`. For using a [Gmail](https://www.google.com/intl/pt/gmail/about/) account it might be needed to set up and use an App Password like described [here](https://support.google.com/accounts/answer/185833?hl=en) and use it instead of the real one

# Testing Provider

- To test the program execution you can configure it to use the Test provider by changing the `provider` in `APIConfig.json` to `test`
- This provider returns especific oscillating values between 22.57 and 22.68 designed to trick the application into sending all possible notification emails when executed with this command:

        ./DesafioBroker.exe PETR4 22.67 22.59
