# AceBook

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

## Updating the Database

Changes are applied to the database programatically, using files called _migrations_, which live in the `/Migrations` directory. The process is as follows...

- To update an existing table
  * For example, you might want to add a title to the `Post` model
  * In which case, you would add a new field there
- To create a new table
  * For example, you might want to add a table called Comments
  * First, create the `Comment` model
  * Then go to AcebookDbContext
  * And add this `public DbSet<Comment>? Comments { get; set; }` 
- Generate the migration file
  * `cd` into `/Acebook`
  * Decide what you wan to call the migration file
  * `AddTitleToPosts` or `CreateCommentsTable` would be good descriptive names
  * Then do `dotnet ef migrations add ` followed by the name you chose
  * E.g.  `dotnet ef migrations add AddTitleToPosts`
- Run the migration
  * `dotnet ef database update`

### Troubleshooting

#### Seeing `role "postgres" does not exist`?

Your application tries to connect to the database as a user called `postgres`, which is normally created automatically when you install PostgresQL. If the `postgres` user doesn't exist, you'll see `role "postgres" does not exist`.

To fix it, you'll need to create the `postgres` user.

Try this in your terminal...

```
; createuser -s postgres
```

If you see `command not found: createuser`, start a new `psql` session and do this...

```sql
create user postgres;
```

#### Want to Change an Existing Migration?

Don't edit the migration files after they've been applied / run. If you do that, it'll probably lead to problems. If you decide that the migration you just applied wasn't quite right for some reason, you have two options

- Create and run another migration (using the process above)

OR...

- Rollback / undo the last migration
- Then edit the migration file before re-running it

How do you rollbacl a migration? Let's assume that you have two migrations, both of which have been applied.

1. CreatePostsAndUsers
2. AddTitleToPosts

To rollback the second, you again use `dotnet ef database update` but this time adding the name of the last 'good' migration. In this case, that would be `CreatePostsAndUsers`. So the command is...

```shell
; dotnet ef database update CreatePostsAndUsers
```
### Changed the order of the localhost connections in Acebook/Properties/launchSettings.json    

  from "applicationUrl": "https://localhost:7196;http://localhost:5287" to "applicationUrl": "http://localhost:5287;https://localhost:7196"

### Nullable warnings were resolved
  csharp-acebook-mvc-template/Acebook/Controllers/UsersController.cs(27,7)
  csharp-acebook-mvc-template/Acebook/Controllers/PostsController.cs(33,7)
  csharp-acebook-mvc-template/Acebook/Controllers/SessionsController.cs(27,20)

### Some Changes in Tests to stop warnings (Assert is updated)
  csharp-acebook-mvc-template/Acebook.Test/LandingPageTests.cs(25,5):constraint model, Assert.That(actual, Is.EqualTo(expected)), was used instead of the classic model, Assert.AreEqual(expected, actual)

  csharp-acebook-mvc-template/Acebook.Test/UserManagement.Test.cs(37,7): constraint model, Assert.That(actual, Is.EqualTo(expected)), was used instead of the classic model, Assert.AreEqual(expected, actual)

  csharp-acebook-mvc-template/Acebook.Test/UserManagement.Test.cs(62,7):constraint model, Assert.That(actual, Is.EqualTo(expected)), was used instead of the classic model, Assert.AreEqual(expected, actual)

## Pull Request10 RemovePosts
  Added a delete action to the PostsController.
  Updated the Profile and Posts/Index views to display the "Delete Post" button.
  Improved styling for the Profile and Posts/Index pages.


## Pull Request17 Add Comments
  The Comments table has been created in the models.
  Comment relationships have been set up in AcebookDbContext.
  The GetComments and CreateComment actions have been implemented in CommentsController.
  Comments are displayed under each individual post, along with an add comment form in Views/Posts/GetPost.cshtml.
  Comments are ordered newest first, showing the timestamp of when they were posted.
  Each comment is displayed with the commenter's name and profile picture.

  //Next Steps:
  Display the number of likes and comments on individual post pages.
  Show the comment count for each post on the feed and profile pages.
  Allow users to delete their own comments.

