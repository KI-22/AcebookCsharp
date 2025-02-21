using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using Bogus;


namespace Acebook.Test;

public class LikesUnlikesTests
{
    ChromeDriver driver;
    Faker faker;

    [SetUp]
    public void Setup()
    {
        faker = new Faker();
        driver = new ChromeDriver();
    }

    [TearDown]
    public void TearDown() {
        driver.Quit();

    }


    [Test]
    public void Test_Like_A_Post_From_Feed()
    {
        // CREATE ACCOUNT //
        string name = faker.Name.FullName();
        string username = faker.Internet.UserName();
        string email = faker.Internet.Email();
        string password = "SecurePassword!123";

        driver.Navigate().GoToUrl("http://127.0.0.1:5287/");

        IWebElement fullNameField = driver.FindElement(By.Name("FullName"));
        IWebElement usernameField = driver.FindElement(By.Name("Name"));
        IWebElement emailField = driver.FindElement(By.Name("Email"));
        IWebElement passwordField = driver.FindElement(By.Name("Password")); 

        fullNameField.SendKeys(name);
        usernameField.SendKeys(username);
        emailField.SendKeys(email);
        passwordField.SendKeys(password);

        IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        submitButton.Click();

        string currentUrl = driver.Url;
        Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));

        // LOGIN //
        IWebElement loginEmailField = driver.FindElement(By.Name("email"));
        IWebElement loginPasswordField = driver.FindElement(By.Name("password"));

        loginEmailField.SendKeys(email);
        loginPasswordField.SendKeys(password);

        IWebElement loginSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        loginSubmitButton.Click();

        string NewCurrentUrl = driver.Url;
        Assert.That(NewCurrentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));


        // // CHECK POSTS before chekcking likes// //
        var postContainers = driver.FindElements(By.XPath("//div[@class='post-container']"));

        if (postContainers.Count == 0)
        {
            // Assert that no Like/Unlike buttons exist on the page when there are no posts
            var likeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Like')]"));
            var unlikeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Unlike')]"));

            // Assert that both "Like" and "Unlike" buttons are not found
            Assert.IsEmpty(likeButtons, "There should be no 'Like' buttons when there are no posts.");
            Assert.IsEmpty(unlikeButtons, "There should be no 'Unlike' buttons when there are no posts.");

        }
        else {

            // POST LIKE //

            // Locate the "Like" button for a specific post (assuming we are testing for the first post)
            // In case you have more posts, you might want to adjust the selector accordingly
            var likeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Like')]"));
            
            // Ensure the button is initially "Like"
            Assert.AreEqual("Like", likeButton.Text);

            // Click the "Like" button
            likeButton.Click();

            // Wait for the page to update (or ideally, wait for a specific element indicating the change)
            Thread.Sleep(1000); // Use WebDriverWait in production

            // Locate the button again to verify if it changed to "Unlike"
            var updatedLikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Unlike')]"));

            // Assert that the button text is now "Unlike"
            Assert.AreEqual("Unlike", updatedLikeButton.Text);

        }
    }


    [Test]
    public void Test_Like_And_Unlike_A_Post_From_Feed()
    {
        // CREATE ACCOUNT //
        string name = faker.Name.FullName();
        string username = faker.Internet.UserName();
        string email = faker.Internet.Email();
        string password = "SecurePassword!123";

        driver.Navigate().GoToUrl("http://127.0.0.1:5287/");

        IWebElement fullNameField = driver.FindElement(By.Name("FullName"));
        IWebElement usernameField = driver.FindElement(By.Name("Name"));
        IWebElement emailField = driver.FindElement(By.Name("Email"));
        IWebElement passwordField = driver.FindElement(By.Name("Password")); 

        fullNameField.SendKeys(name);
        usernameField.SendKeys(username);
        emailField.SendKeys(email);
        passwordField.SendKeys(password);

        IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        submitButton.Click();

        string currentUrl = driver.Url;
        Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));

        // LOGIN //
        IWebElement loginEmailField = driver.FindElement(By.Name("email"));
        IWebElement loginPasswordField = driver.FindElement(By.Name("password"));

        loginEmailField.SendKeys(email);
        loginPasswordField.SendKeys(password);

        IWebElement loginSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        loginSubmitButton.Click();

        string NewCurrentUrl = driver.Url;
        Assert.That(NewCurrentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));


        // // CHECK POSTS before chekcking likes// //
        var postContainers = driver.FindElements(By.XPath("//div[@class='post-container']"));

        if (postContainers.Count == 0)
        {
            // Assert that no Like/Unlike buttons exist on the page when there are no posts
            var likeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Like')]"));
            var unlikeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Unlike')]"));

            // Assert that both "Like" and "Unlike" buttons are not found
            Assert.IsEmpty(likeButtons, "There should be no 'Like' buttons when there are no posts.");
            Assert.IsEmpty(unlikeButtons, "There should be no 'Unlike' buttons when there are no posts.");

        }
        else {

            // POST LIKE (FEED) //

            // Locate the "Like" button for a specific post (assuming we are testing for the first post)
            // In case you have more posts, you might want to adjust the selector accordingly
            var likeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Like')]"));
            
            // Ensure the button is initially "Like"
            Assert.AreEqual("Like", likeButton.Text);

            // Click the "Like" button
            likeButton.Click();

            // Wait for the page to update (or ideally, wait for a specific element indicating the change)
            Thread.Sleep(1000); // Use WebDriverWait in production

            // Locate the button again to verify if it changed to "Unlike"
            var updatedLikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Unlike')]"));

            // Assert that the button text is now "Unlike"
            Assert.AreEqual("Unlike", updatedLikeButton.Text);


            // POST UNLIKE (FEED) //

            // refresh page(~)
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/posts");
            var unlikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Unlike')]"));

            Assert.AreEqual("Unlike", unlikeButton.Text);

            unlikeButton.Click();

            Thread.Sleep(1000); // Use WebDriverWait in production

            var updatedUnlikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Like')]"));

            Assert.AreEqual("Like", updatedUnlikeButton.Text);
        }
    }


    [Test]
    public void Test_Like_A_Post_From_Profile_Page_No_Posts()
    {
        // CREATE ACCOUNT //
        string name = faker.Name.FullName();
        string username = faker.Internet.UserName();
        string email = faker.Internet.Email();
        string password = "SecurePassword!123";

        driver.Navigate().GoToUrl("http://127.0.0.1:5287/");

        IWebElement fullNameField = driver.FindElement(By.Name("FullName"));
        IWebElement usernameField = driver.FindElement(By.Name("Name"));
        IWebElement emailField = driver.FindElement(By.Name("Email"));
        IWebElement passwordField = driver.FindElement(By.Name("Password")); 

        fullNameField.SendKeys(name);
        usernameField.SendKeys(username);
        emailField.SendKeys(email);
        passwordField.SendKeys(password);

        IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        submitButton.Click();

        string currentUrl = driver.Url;
        Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));

        // LOGIN //
        IWebElement loginEmailField = driver.FindElement(By.Name("email"));
        IWebElement loginPasswordField = driver.FindElement(By.Name("password"));

        loginEmailField.SendKeys(email);
        loginPasswordField.SendKeys(password);

        IWebElement loginSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        loginSubmitButton.Click();

        string NewCurrentUrl = driver.Url;
        Assert.That(NewCurrentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));

        
        // // // GO TO PROFILE PAGE // //   
        driver.Navigate().GoToUrl($"http://127.0.0.1:5287/{username}");
        

        // // CHECK POSTS before checking likes// //
        var postProfileContainers = driver.FindElements(By.XPath("//div[@class='post-content-profile']"));

        if (postProfileContainers.Count == 0)
        {
            // Assert that no Like/Unlike buttons exist on the page when there are no posts
            var likeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Like')]"));
            var unlikeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Unlike')]"));

            // Assert that both "Like" and "Unlike" buttons are not found
            Assert.IsEmpty(likeButtons, "There should be no 'Like' buttons when there are no posts.");
            Assert.IsEmpty(unlikeButtons, "There should be no 'Unlike' buttons when there are no posts.");

        }
        else {

            Console.WriteLine("  else (posts found)");

            // // POST LIKE (PROFILE) //

            // Locate the "Like" button for a specific post (assuming we are testing for the first post)
            // In case you have more posts, you might want to adjust the selector accordingly
            var likeButton = driver.FindElement(By.XPath("//div[@class='post-content-profile'][1]//button[contains(text(), 'Like')]"));
            
            // Ensure the button is initially "Like"
            Assert.AreEqual("Like", likeButton.Text);

            // Click the "Like" button
            likeButton.Click();

            // Wait for the page to update (or ideally, wait for a specific element indicating the change)
            Thread.Sleep(1000); // Use WebDriverWait in production

            // Locate the button again to verify if it changed to "Unlike"
            var updatedLikeButton = driver.FindElement(By.XPath("//div[@class='post-content-profile'][1]//button[contains(text(), 'Unlike')]"));

            // Assert that the button text is now "Unlike"
            Assert.AreEqual("Unlike", updatedLikeButton.Text);


            // POST UNLIKE (PROFILE) //

            // refresh page(~)
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/posts");
            var unlikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Unlike')]"));

            Assert.AreEqual("Unlike", unlikeButton.Text);

            unlikeButton.Click();

            Thread.Sleep(1000); // Use WebDriverWait in production

            var updatedUnlikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Like')]"));

            Assert.AreEqual("Like", updatedUnlikeButton.Text);


        }
    }



    [Test]
    public void Test_Like_A_Post_From_Profile_Page_With_A_Post()
    {
        // CREATE ACCOUNT //
        string name = faker.Name.FullName();
        string username = faker.Internet.UserName();
        string email = faker.Internet.Email();
        string password = "SecurePassword!123";
        var postContent = faker.Lorem.Sentence();

        driver.Navigate().GoToUrl("http://127.0.0.1:5287/");

        IWebElement fullNameField = driver.FindElement(By.Name("FullName"));
        IWebElement usernameField = driver.FindElement(By.Name("Name"));
        IWebElement emailField = driver.FindElement(By.Name("Email"));
        IWebElement passwordField = driver.FindElement(By.Name("Password")); 

        fullNameField.SendKeys(name);
        usernameField.SendKeys(username);
        emailField.SendKeys(email);
        passwordField.SendKeys(password);

        IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        submitButton.Click();

        string currentUrl = driver.Url;
        Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));

        // LOGIN //
        IWebElement loginEmailField = driver.FindElement(By.Name("email"));
        IWebElement loginPasswordField = driver.FindElement(By.Name("password"));

        loginEmailField.SendKeys(email);
        loginPasswordField.SendKeys(password);

        IWebElement loginSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        loginSubmitButton.Click();

        string NewCurrentUrl = driver.Url;
        Assert.That(NewCurrentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));

        
        
        // // CREATE POST // // 
        IWebElement postContentFieldText = driver.FindElement(By.CssSelector(".post-input"));
        postContentFieldText.SendKeys(postContent);
        IWebElement postSubmitButton = driver.FindElement(By.XPath("//button[contains(text(), 'Post')]"));
        postSubmitButton.Click();

        // Verify the post is displayed
        IWebElement post = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
        Assert.That(post, Is.Not.Null); 



        // // // GO TO PROFILE PAGE // //   
        driver.Navigate().GoToUrl($"http://127.0.0.1:5287/{username}");


        // // CHECK POSTS before checking likes// //
        var postProfileContainers = driver.FindElements(By.XPath("//div[@class='post-content-profile']"));

        if (postProfileContainers.Count == 0)
        {
            // Assert that no Like/Unlike buttons exist on the page when there are no posts
            var likeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Like')]"));
            var unlikeButtons = driver.FindElements(By.XPath("//button[contains(text(), 'Unlike')]"));

            // Assert that both "Like" and "Unlike" buttons are not found
            Assert.IsEmpty(likeButtons, "There should be no 'Like' buttons when there are no posts.");
            Assert.IsEmpty(unlikeButtons, "There should be no 'Unlike' buttons when there are no posts.");

        }
        else {

            // // POST LIKE (PROFILE) //

            // Locate the "Like" button for a specific post (assuming we are testing for the first post)
            // In case you have more posts, you might want to adjust the selector accordingly
            var likeButton = driver.FindElement(By.XPath("//div[@class='post-content-profile'][1]//button[contains(text(), 'Like')]"));
            
            // Ensure the button is initially "Like"
            Assert.AreEqual("Like", likeButton.Text);

            // Click the "Like" button
            likeButton.Click();

            // Wait for the page to update (or ideally, wait for a specific element indicating the change)
            Thread.Sleep(1000); // Use WebDriverWait in production

            // Locate the button again to verify if it changed to "Unlike"
            var updatedLikeButton = driver.FindElement(By.XPath("//div[@class='post-content-profile'][1]//button[contains(text(), 'Unlike')]"));

            // Assert that the button text is now "Unlike"
            Assert.AreEqual("Unlike", updatedLikeButton.Text);


            // POST UNLIKE (PROFILE) //

            // refresh page(~)
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/posts");
            var unlikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Unlike')]"));

            Assert.AreEqual("Unlike", unlikeButton.Text);

            unlikeButton.Click();

            Thread.Sleep(1000); // Use WebDriverWait in production

            var updatedUnlikeButton = driver.FindElement(By.XPath("//div[@class='post-container'][1]//button[contains(text(), 'Like')]"));

            Assert.AreEqual("Like", updatedUnlikeButton.Text);


        }
    }


}

/*

STILL TO TEST:
>> like/unlike button on an individual post's pge
>> checking the like count displayed is correct (TBC - all 3x areas)

*/
