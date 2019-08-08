# Blaise_Case_Backup

The Blaise Case Backup application is a Windows service that runs on a virtual machine hosting a Blaise 5 server.
The application works on a timer and preiodically saves a copy of all of the case data from the Blaise system to a backup location on the local drive. This data is then overwritten each time the system is backed up.

# Populate the key values in the app.config file.

The following keys need to be added to the App.config file for the program to work correctly.
Fill in the associated values as required.

    <add key="BlaiseServerHostName" value=""/>
    <add key="BlaiseServerUserName" value=""/>
    <add key="BlaiseServerPassword" value=""/>
    

# Install the Log4Net package via NuGet.

  ```
  Install-Package log4net
  ```

# Blaise API
Ensure you have the latest version of Blaise 5 installed from the Statistics Netherlands FTP.

To use the API's:
  - Right Click the "References" object under the project in the VS Solution Explorer
  - Select "Add Reference"
  - Use the "Browse" tab and navigate to "C:\Program Files (x86)\StatNeth\Blaise5\Bin"
  - In this folder all the required API's for Blaise interaction are available.
  - Install the following Blaise APIs:
    - StatNeth.Blaise.API.DataInterface
    - StatNeth.Blaise.API.ServerManager
    - StatNeth.Blaise.API.DataLink
    - StatNeth.Blaise.API.DataRecord

# Installing the Service

  - Build the Solution
    - In Visual Studio select "Release" as the Solution Configuration
    - Select the "Build" menu
    - Select "Build Solution" from the "Build" menu
  - Copy the release files (/bin/release/) to the install location on the server
  - Uninstall any previous installs
    - Stop the service from running
    - Open a command prompt as administrator
    - Navigate to the windows service installer location
      - cd c:\Windows\Microsoft.NET\Framework\v4.0.30319\
    - Run installUtil.exe /U from this location and pass it the location of the service executable
      - InstallUtil.exe /U {install location}\BlaiseCaseHandler.exe
  - Run the installer against the release build
    - Open a command prompt as administrator
    - Navigate to the windows service installer location
      - cd c:\Windows\Microsoft.NET\Framework\v4.0.30319\
    - Run installUtil.exe from this location and pass it the location of the service executable
      - InstallUtil.exe {install location}\BlaiseCaseHandler.exe
    - Set the service to delayed start
    - Start the service
