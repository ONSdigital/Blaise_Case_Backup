# Blaise_Case_Backup

Blaise Case Backup is a Windows service for backing up Blaise databases. The service works on a timer and periodically saves a copy of all the databases on a Blaise server park to a local location. 
The backed up data is overwritten each time the service is run.

# Setup Development Environment

Clone the git repository to your IDE of choice. Visual Studio 2019 is recommended.

Populate the key values in the App.config file accordingly. **Never commit App.config with key values.**

Build the solution to obtain the necessary references.

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
      - InstallUtil.exe /U {install location}\BlaiseCaseBackup.exe
  - Run the installer against the release build
    - Open a command prompt as administrator
    - Navigate to the windows service installer location
      - cd c:\Windows\Microsoft.NET\Framework\v4.0.30319\
    - Run installUtil.exe from this location and pass it the location of the service executable
      - InstallUtil.exe {install location}\BlaiseCaseBackup.exe
    - Set the service to delayed start
    - Start the service
