# Readme.md

The following document explains the concepts and ideas behind the Password Query Tool known as PassSearch.

## 0. Introduction

This tool is used to analyze password dump files and store/query them efficiently. Files can be imported as a Login structure into an Elastic Search DB and then queried for specific email addresses or by domains and top level domains to access the most common passwords and get great reference values for brute-force attacks or for a password-check system.

The tool was developed by Severin M. and Konstantin S. as part of an assignment.

## 1. Setup and Usage

This section describes how the service can be setup, what requirements need to be met in order to allow the service to start and how it can be used.

### 1.1. Prerequisites
To compile and debug the tool in Visual Studio, the following components are required:

- Visual Studio 2019
  - Installed Packages:
    - Asp.Net and Web Development
    - .Net 5 Runtime      
    - Container-Development Tools
    - Platform-independent .Net Core Development
    - .Net-Desktop Development
- Docker / Docker Desktop
- .Net 5 Runtime (on System or via Visual Studio)

Configuration:
- Docker-Compose Override File (see "1.5.1. Docker Compose Override")

### 1.2. Compilation and Startup

The source code can be pulled and compiled using Visual Studio. In order to launch the entire system, the Docker Compose project should be selected as the starting project. The docker environment manages all required services that make the service work. Some of the initial values will need to be overwritten in the configuration files and the docker-compose.override.yml file. An example configuration and explanation can be found in the section "1.5. Docker Compose".

Apart from starting the project with Docker you can also start the services manually if you have configured the underlying infrastructure correctly (Elastic Search DB, Minio Storage, Postgres and RabbitMq). In this case you will need to manually start the ParsingService, the DatabaseService, the ApiGateway and the two Web Apps. You will also need to make sure that the services are configured correctly. More on that will be covered in the section "1.3. Configuration".

### 1.2. Usage

This section outlines how the system can be effectively used.

#### 1.2.1. Web Apps

This section describes the usage of the two web apps.

##### 1.2.1.1. Import Web App

The import web app can be used to start password imports and check the status of running imports. Access to this app and to the parsing service should be restricted to authorized users, for example by not exposing the ports to the internet.

Upon accessing the app, users will see an overview of all file imports that have been launched. The cards will display the current status and information regarding the progress of the import. The cards also display how many lines of the file could not be properly imported and how many lines were actually imported. Currently 3 states of imports are supported:

- Analyzing (Search-Symbol) -> The files are currently being analyzed to allow maximum efficiency chunking
- Importing (Gear-Symbol) -> The files are being imported into the database;
- Finished (Checkmarks-Symbol) -> The entire file has been successfully imported into the database.

The app will periodically pull updates from the parsing service using polling.

Using the button "New Import" the user can create a new import for a file that is already in the Minio Storage. More information on how to import files can be found in Section "1.4.2. Minio". A list of all currently available files will be displayed. Upon clicking on one the user will be notified that the import has started. Returning to the overview will demonstrate that the new import has now also been launched. By now the system is analyzing the selected file and will soon begin importing the passwords into the db. The import can be cancelled using the cancel button.

All of the actions that are performed via the Import Web App can also be performed using the Parsing Api as described in Section "1.2.2. APIs".

##### 1.2.1.2. Query Web App

This web app can be used to query the imported passwords. Upon accessing the app the user can choose one of three different queries. "Search" queries will search specific login data for a given term, for example "Search by email" will return all logins in the database that belong to the given email address. "Common" queries will return the 100 most common passwords for the domain. "by domain" will return the passwords for one specific email domain. "by topleveldomain" will return the passwords for every domain that ends in the string given. If a query should not return any values or be perceived as incorrect, the application will notify the user.

The results will be displayed in a neat table that makes it easy to browse the entries.

These queries can again also be run directly on the API (see "1.2.2. APIs").

#### 1.2.2. APIs

This section will document the currently available API actions. A better definition can be obtained by accessing the swagger pages of the apis by accessing "/api/_/swagger/index.html" where _ is the route of the api you want to access.

##### 1.2.2.1. Parsing API

- /parsing/files
  - GET
  - Will get all files that are currently in the storage and are available for import as a JSON object
- /parsing/jobs
  - GET
  - Will get all jobs that have ever been started as a JSON object.
- /parsing/jobs
  - POST
  - Expected Body:
    - The name of the file
    - E.g.: "antipublic1.txt"
  - Will start the import of the file and return the guid of the job that has been created.
- /parsing/job
  - POST
  - Expected Body:
    - The guid of the job to monitor
    - E.g.: "4107303e-ed42-4b47-b98d-d939156b8de9"
  - Will get the information for a single job.
- /parsing/job:
  - DELETE
  - Expected Body:
    - The guid of the job to cancel
    - E.g.: "4107303e-ed42-4b47-b98d-d939156b8de9"
##### 1.2.2.2. Query API

- /queries/email/{email}
  - GET
  - Will get 100 passwords for the email address {email} as a JSON object
- /queries/search/username/{username}
  - GET
  - Will get 100 passwords for the username {username} as a JSON object
- /queries/common/tld/{tld}
  - GET
  - Will get the 100 most common passwords for email adresses in domains with the top level domain {tld} as a JSON object
- /queries/common/domain/{domain}
  - GET
  - Will get the 100 most common passwords for email addresses in the domain {domain} as a JSON object
- /queries/debug/domain/{domain}
  - GET
  - Will get a selection of accounts with email addresses in the domain {domain} as a JSON object
  - This query is only used to show that data is inside the DB and to get an idea of what values will yield results for the other queries.
- /queries/debug/tld/{tld}
  - GET
  - Will get a selection of accounts with email addresses in the domains with the top level domain {tld} as a JSON object
  - This query is only used to show that data is inside the DB and to get an idea of what values will yield results for the other queries.
- /queries/debug/amount
  - GET
  - Will get the total amount of entries in the db

##### 1.2.2.3. Further Info

The Query API in particular can be used effectively to call a list of passwords for a Top-Level-Domain and then check if a user is about the register with a very common password. The user can then be warned that his password is a well known one and that he should choose a different one.

Using the Parsing API different tools can be written that allow management of the database in case the web app is not a preferred choice.

Both APIs can be tested marvelously using the swagger API documentation as described in "1.2.2. APIs"

### 1.3. Configuration
All custom services (2.1.2.) need special configuration using the ASP.NET configuration infrastructure - in this case appsettings.json or appsettings.development.json. These files can be found in the folders of the DatabaseAccess- and Parsing-Services.

Among all configuration options are:
  - ElasticSearch host (DatabaseAccess Service)
  - RabbitMq host and credentials (Parsing and DatabaseAccess Service)
  - Postgres Db connection string (Parsing Service)
  - Minio host and endpoint (Parsing Service)

#### 1.3.1. Parsing Service
The parsing service requires a connection string "parsingApp" which gives the exact host that the ParsingWebApp will access from. The reason for this is that this way the parsing app is protected behind the api gateway and can still access the parsing API itself.

### 1.4. Infrastructure

This section documents the infrastructure that is required for the service to work efficiently. The architecture and the benefits arising from it will be explained in greater detail in Section "2. Architecture."

#### 1.4.1. Elastic Search DB

This service builds the foundation of our application. The parsed passwords are loaded directly into an Elastic Search Cluster and are thus easily searchable using our queries. The Database is exclusively accessed by our DatabaseAccess service. The user has no need to access this service directly, however, if access is provided to a user group it can be more easily maintained by checking the status of the service and the internal nodes. To access the service, you need to access the database address in your browser using the HTTP protocol.

#### 1.4.2. Minio

This service provides a powerful way to access files that our micro-services use in order to get access to the files more efficiently. This does involve first transferring the files into the Minio S3 Storage so that our apps can access it quicker and more efficiently. The transfer can be started by opening the minio address in a web browser and login in using the credentials defined in the configuration for the services (Section "1.3. Configuration"). Upon login in, you will be able to drag and drop in files that you want to make available in your import service. If you access and upload the files locally, the service will quickly store all files in the cloud storage.

While it is not efficient to upload the files to minio first, it is a necessity to ensure that file can correctly be parsed and no file-system-based errors will disrupt the process.

#### 1.4.3. RabbitMq

This service manages the distribution of parsed passwords in case multiple services are being used for parsing and importing. This service will not need to be used by regular users, however, if access to it is made available the current workload can be examined in great detail using the RabbitMq Admin interface. To access it, you simply need to access the address of the service in your browser.

#### 1.4.4. Postgres

This database holds the state of import jobs. As each file gets parsed and indexed, the db will update which parts of the file are done and which are still on their way to processing. There must be a correct configuration to this database in the Parsing Service. 

### 1.5. Docker Compose

The docker-compose.yml file outlines the complete structure of the service. All required services are defined such as the elastic search DB and the minio storage. This makes it very easy to launch the entire system without needing to configure all services locally. The compose can be started using the docker CLI or using Visual Studio.

It also introduces some dependencies to technically ensure that all infrastructure services are up when our services launch. Because this does not work 100% the code tries to handle the case of a slow starting service as seen in the `ElasticDatabaseHelperService.cs` method `Setup()`.

#### 1.5.1. Docker Compose Override

The Docker Compose file only outlines the general infrastructure, however, some additional configuration may be performed using the docker-compose.override.yml file.
This file should only be saved locally and will differ from environment to environment. Requirements include bindings to filepaths and port configurations as well as environment configurations such as passwords and usernames. The file needs to be placed in the exact same location as the docker-compose.yml

The following is an example of a possible configuration for the override file where `[MY FILE PATH]` equals a path of your choosing where docker should mount the services' folders to. This configuration needs to match the values configured in the Section "1.3. Configuration". It alone should suffice to allow the system to start and function correctly. The most important part is to keep the correct ports for the services if you want to make them available outside of the docker environment and to ensure the environment variables are correct.

```yml
services:
  es01:
    container_name: es01
    environment:
      - discovery.type=single-node
    ports:
      - "1040:9200"
    volumes:
      - "[MY FILE PATH]:/usr/share/elasticsearch/data"

  postgres:
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"

  passwordquerytool.backend.services.databaseaccess:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+:443;http://+:80
    ports:
      - "1001:80"
      - "1002:443"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro

  minio:
    volumes:
      - "[MY FILE PATH]:/data1"
    ports:
      - "1020:9000"
    environment:
      MINIO_ROOT_USER: minio
      MINIO_ROOT_PASSWORD: minio123
    command: server /data1

  rabbitmq:
    ports:
      - "1030:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=rabbitmq
      - RABBITMQ_DEFAULT_PASS=rabbitmq

  passwordquerytool.backend.services.parsing:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+:443;http://+:80
    ports:
      - "1011:80"
      - "1012:443"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro

  passwordquerytool.webapp.server:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+:443;http://+:80
    ports:
      - "1080:80"
      - "1443:443"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro

  passwordquerytool.apigateway:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+:443;http://+:80
    ports:
      - "81:80"
      - "444:443"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro
```

## 2. Architecture

The entire system was built using a modified version of Micro Service architecture. The main reason for this is scalability. As the application should be able to import terrabytes of files, any non-distributed app would most likely not suffice. Using this model the parsing and database services as well as the DB can be scaled to allow performant importing and querying of millions of data rows. This system can be given as much resources as needed and can thus go from being hosted to store a few gigabytes to storing terabytes of data and still performing well.

### 2.1. General Architecture

This section documents the general architecture of the system.

#### 2.1.1. Existing Services

The system uses the following services to create an efficient workflow:

- Elastic Search DB
  - Docker Service "es01"
  - A NoSQL Database service that specializes in fast queries.
  - Passwords are imported into and queried from this service for performant access. This service can be split into multiple nodes to increase performance even more.
- Minio
  - Docker Service "minio"
  - A file storage service that allows integrity of files for our services.
  - The password files are stored in this service to ensure quick and stable access to the files for our services. This way no operating system or manual deletion will interrupt the import process and the system does not need to reserve ungodly amounts of memory for the import process. Minio also guarantuees the integrity of the data.
- RabbitMq
  - Docker Service "rabbitmq"
  - A Message Broker service that utilizes queues to distribute information in a micro service environment.
  - This service is used to create queues which distribute the workloads between possibly multiple instances of parsing and importing services. It also offers a decent way to store not-processed workloads in order to ensure that ALL correct passwords from a file are eventually imported. This makes the entire construct more failure resistant.
- Postgres
  - Docker Service "postgres"
  - A powerful SQL Database.
  - Holds the state of the import jobs.

#### 2.1.2. Custom Services

The following services were created to be used specifically as parts of this system:

- Api Gateway
  - Docker Service "passwordquerytool.apigateway"
  - Project "PasswordQueryTool.Apigateway"
  - This service is used to put a powerful gateway infront of all services to ensure regulated access to the system. It automatically reroutes the requests to the correct services. It can be further configured in code to only allow authorized users to access the services, however, this is as of now not implemented.
- Parsing Service
  - Docker Service "passwordquerytool.backend.services.parsing"
  - Project "PasswordQueryTool.Backend.Services.Parsing"
  - This service deals with parsing the files from the Minio storage. It orchestrates the import from the splitting of a file, to the extraction of individual passwords and their indexation into the ES instance. It only handles a set amount of chunks at a time. The entire communication runs over a message bus (in this case RabbitMq). This service also offers the Parsing API.
- DatabaseAccess Service
  - Docker Service "passwordquerytool.backend.services.databaseaccess"
  - Project "PasswordQueryTool.Backend.Services.DatabaseAccess"
  - This service receives the incomming login data and then imports it into the Elastic Search DB using the Elastic Search NEST Client. By assigning more resources to this service (for example by using multiple instances) and the Elastic Search DB, the performance of the application can be drastically increased.
- Import Web App
  - This web app offers a GUI to interact with the Import Api. It provides all of the functionalities of the api but in a more user-friendly way.
  - This service is deployed as a hosted Blazor WebAssembly App. It offers a clean GUI to access the APIs of the Parsing Service. This service could in the future be modified to run as an external application that just hosts the WebApp locally, however, it is not currently intended as such. Access to this service should be restricted only to authorized users.
- Query Web App
  - Docker Service "passwordquerytool.webapp.server"
  - Project "PasswordQueryTool.WebApp.Server"
  - This service is deployed as a hosted Blazor WebAssembly App. It offers a clean GUI to access the APIs of the DatabaseAccess Service. This service could in the future be modified to run as an external application that just hosts the WebApp locally, however, it is not currently intended as such.

### 2.2. Benefits of the architecture

Because of the complicated architecture the system becomes a bit more difficult to deploy. This can be worked against by using container solutions like Docker or Kubernetes.

The benefits of this architecture are the scalability, allowing system operators to create multiple instances of services to handle massive workloads efficiently, as well the clear split of workloads. Every single service has predominantly one task (parsing the files, accessing the database, routing access to apis, etc.). This also means that every single one of these services can be replaced fairly easily with another one that provides the same interfaces and contracts. This also makes it easier to find mistakes in the services as each service is used in exactly one stage of the workflow and each stage requires at most two services and we can thus determine the faulty service quickly by knowing in which stage the error occurs.

Apart from this the structure is designed to be hosted centrally and can thus be run in a massive cluster for one organisation, providing access via the APIs to the required queries. For this case the Web Apps could be omitted. Another use case would be hosting this app locally on a PC for smaller amounts of data and using it for pentesting by importing relevant password dumps.

## 3. Models and Supporting Libraries

### 3.1. Models

This section describes the models used for this system.

### 3.1.1. Password Models

PasswordQueryTool.Models

- Email Data
  - Consists of a username and fulldomain (both strings)
  - Used to store a single email address
- Login Data
  - Consists of one Email Data model and a password (string)
  - Used to store a single password-email combination
- Query Request
  - Consists of a filter (string)
  - Used to access the Query API
  - Extra class in case extra data is ever needed (easier expandable than normal string)
- Query Response
  - Consists of an array of LoginData and a total item count (long)
  - Used to return the results of a search query
  - Total Item Count is not always implemented and mainly used for debugging
  - The array contains all relevant data entries
- MostCommonDataInstance
  - Consists of a password (string) and a count (long)
  - Used to store how often a password occured for a specific query
- MostCommonData
  - Consists of a list of MostCommonDataInstances
  - Used to transport the data instances in a single object

### 3.1.2. Import Models

PasswordQueryTool.ImportModels

- Import
  - Consists of
    - Id: The GUID of the import job
    - Name: The Name of the import job (string)
    - LinesFinished: The amount of lines that have been imported already (long)
    - InvalidLines: The amount of lines that could not be imported (long)
    - ChunksFinishedAmount: The amount of chunks that have already been parsed (long)
    - ChunksAmount: The total amount of chunks that need to be parsed (long)
    - State: The state of the job. One of the below:
      - Analzying
      - Importing
      - Finished
      - Canceled

### 3.2. Supporting Libraries

Apart from the model libraries, the following library projects are used:

- PasswordQueryTool.Backend.Database:
  - Contains a helper class and an interface for it to access the elastic search database and configure the index (table) correctly
  - Also contains the model that is used inside of the database
- PasswordQueryTool.Backend.QueueContracts:
  - This library holds the contracts for the Communication Framework MassTransit. Each message processed by MassTransit must follow a defined contract in this Library.
  - Message producers and consumers depend on it as the entire classname (including namespace) must be equal for both parties.

## 4. Ideas behind the application

The general idea behind the system was to create something that can handle massive amounts of data and query it in acceptable time. This was hard to achieve in our original attempts and resulted in us wanting to switch to a Micro-Service inspired structure. A command line tool would not suffice for such gigantic tasks as parsing a multiple terrabyte file. Apart from the resource limitations of most PCs, a single error could lead to the process completely failing and stopping somewhere in the middle which leads to very unpleasant results, such as half a file being imported which makes it even more difficult to import the rest of the file. On the other side, if our service is hosted correctly it is very resistant to such failures and can continue it's work directly after restarting, thus eliminating such problems. Additionally, you can scale this application as much as you would like to, in order to increase it's power and make sure that it can handle even terrabytes of data.

The web apps were created to provide a simple yet efficient access to the database. They can be easily updated to include more queries as they are made available by the database access service. Also, they use the Blazor WebAssembly technology to provide a modern WebApp-experience that integrates perfectly into the existing system.

## 5. Further goals

Further goals include more queries and use cases to be made available as well as more testing and cleaner code and project structure. Also, the scalability would need to be tested on a more powerful system.
