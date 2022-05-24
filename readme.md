# Build instructions

### Requirements:
- [Node.js](https://nodejs.org/en/download/) v16.14.2 or higher
- [.Net SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) v6.0.300 or higher


## Application1

### How to build
- Open a command prompt in `/Application1` directory
- Execute `dotnet publish -c Release -o <ENTER_OUTPUT_PATH>`
- Navigate to <ENTER_OUTPUT_PATH> and open `Application1.exe`


## Application2

### Angular dependencies
- Open a command prompt in the `/Application2/ClientApp` directory
- Execute `npm install` command

### How to build
- Open a command prompt in the `/Application2` folder
- Execute `dotnet publish -r win-x64 --self-contained true -c Release -o <ENTER_OUTPUT_PATH>`
- Navigate to <ENTER_OUTPUT_PATH> and execute the `Application2.exe` file
- Open a browser and Navigate to http://localhost:5000