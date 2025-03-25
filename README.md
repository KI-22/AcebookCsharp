# AceBook - README in progress

**Note:** this is a copy of the original private repo ([here](https://github.com/aysinakpinar/csharp-acebook-mvc-umbrella)).

## Facebook Clone
Group project | C# | 10-days | Makers Software Engineering Bootcamp

#### My Contributions:
- 'Like' button functionality (on all relevant pages)
- User stories
- Planning for testing
- Wireframes


## Quickstart

First, clone this repository. Then:

- Install the .NET Entity Framework CLI
  * `dotnet tool install --global dotnet-ef`
- Create the database/s in `psql`
  * `CREATE DATABASE acebook_csharp_development;`
  * `CREATE DATABASE acebook_csharp_test;`
- Run the migration to create the tables
  * `cd` into `/Acebook`
  * `dotnet ef database update`
  * `DATABASE_NAME=acebook_csharp_development dotnet ef database update`
- Start the application, with the development database
  * `DATABASE_NAME=acebook_csharp_development dotnet watch run`
- Go to `http://localhost:5287/`

## Running the Tests

- Install Chromedriver
  * `brew install chromedriver`
- Start the application, with the default (test) database
  * `dotnet watch run`
- Open a second terminal session and run the tests
  * `dotnet test`

### Troubleshooting

If you see a popup about not being able to open Chromedriver...
- Go to **System Preferences > Security and Privacy > General**
- There should be another message about Chromedriver there
- If so, Click on **Allow Anyway**

