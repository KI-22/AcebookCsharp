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
#### Pull Request Feature/AddAndListPosts

- When a user logs in, they can see all posts displayed in chronological order, with the newest posts appearing first. Each post includes the username and profile image of the author, displayed next to the post content. A timestamp is shown below each post, indicating when it was created.

- Posts also include placeholders for the number of comments and reactions.

- On the /posts page, there is a form positioned above the posts where users can add new posts. The form includes the user's profile picture and allows users to create posts in three ways:

  Text only
  Text with an image URL
  Text with an uploaded image file
  If a user attempts to submit an empty post, they receive an error message.

- Testing for post creation was conducted using Bogus.Faker.Net and ChromeDriver, and all tests successfully validated the post submission rules.

- The /posts page also includes Account and Logout buttons.

  The Logout button ends the session and redirects the user to the login page.
  The Account page is currently non-functional and needs further development.


##### Pull request TestsFix

- changed line 38 on post index.cshtml to this <button type="submit" id="submit" class="post-btn">Post</button>
- added email constraint to dbContext
- added unique email validation on user controller, added lines 36 to 38
- fixed tests changing some tags to match new view templates and remove faker.net (everyone should do dotnet restore on their terminal)
- in post.test line 168 and 213, i had to change the image path to my path
- var imageFilePath = "/Users/arthurbotto/week-11-acebook/csharp-acebook-mvc-umbrella/Acebook/wwwroot/images/il_1588xN.1965501418_pspx.png";
- not sure how it works for you guys.


#### UserRelated Pull request
Changed a few things on friendship controller, had to fix that when user 1 added user 2, when user 2 accepted the request,
user 2 would see user 1 as friend, but user 1 wouldnt see user 2 as friend unless user 2 requested user 1 to be friend.
added remove friend route
added cancel request route
Added FullName, Bio, JoinedDate and IsPrivate columns in User.cs
made users profile being private to work, so it wont show your posts when users are not friends.
create account form now asks for Name as well. 
edit account allows to edit a name,  choose a bio and choose private or public profile.

