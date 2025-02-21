using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using Bogus;



namespace Acebook.Test
{
    public class Posts
    {
        ChromeDriver driver;
        Faker faker;
        private string testPassword = "Admin123*";

        [SetUp] 
        public void Setup()
        {
            driver = new ChromeDriver();
            faker = new Faker();
        }

        [TearDown]
        public void TearDown() 
        {
            driver.Quit();
        }

    private void CreateAccount(string username, string email, bool isPrivate)
    {
      string name = faker.Name.FullName();
      string password = testPassword;

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      driver.FindElement(By.Name("FullName")).SendKeys(name);
      driver.FindElement(By.Name("Name")).SendKeys(username);
      driver.FindElement(By.Name("Email")).SendKeys(email);
      driver.FindElement(By.Name("Password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
      driver.FindElement(By.Name("email")).SendKeys(email);
      driver.FindElement(By.Name("password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();

      if (isPrivate)
      {
        driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
        driver.FindElement(By.Name("email")).SendKeys(email);
        driver.FindElement(By.Name("password")).SendKeys(password);
        driver.FindElement(By.CssSelector("input[type='submit']")).Click();
        driver.Navigate().GoToUrl($"http://127.0.0.1:5287/{username}/edit");
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        IWebElement privacyCheckbox = driver.FindElement(By.XPath("//input[@type='checkbox' and @id='IsPrivate']"));
        if (!privacyCheckbox.Selected)
        {
          privacyCheckbox.Click();
        }
        driver.FindElement(By.CssSelector("input[type='submit']")).Click();
      }
    }

    private void Login(string email, string password)
    {

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
      driver.FindElement(By.Name("email")).SendKeys(email);
      driver.FindElement(By.Name("password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();
    }

    public void Logout()
    {
      driver.Navigate().GoToUrl("http://127.0.0.1:5287/posts");
      driver.FindElement(By.CssSelector("button.btn.btn-secondary")).Click();
    }






        [Test]
        public void CreatePost_OnlyWithText_PostIsDisplayed_1()
        {
            string email = faker.Internet.Email();
            string username = faker.Internet.UserName();
            var postContent = faker.Lorem.Sentence();

            CreateAccount(username, email, false);
            Login(email, testPassword);

            // Create a new post
            IWebElement postContentFieldText = driver.FindElement(By.CssSelector(".post-input"));
            postContentFieldText.SendKeys(postContent);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("button.post-btn"));
            postSubmitButton.Click();

            // Verify the post is displayed
            IWebElement post = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
            Assert.That(post, Is.Not.Null);   
        }

        [Test]
        public void CreatePost_OnlyWithImageUrl_PostIsDisplayed_2()
        {
            string email = faker.Internet.Email();
            string username = faker.Internet.UserName();
            var imageUrl = faker.Image.PicsumUrl();

            CreateAccount(username, email, false);
            Login(email, testPassword);

            // Create a new post
            IWebElement postContentFieldUrl = driver.FindElement(By.CssSelector(".image-input"));
            postContentFieldUrl.SendKeys(imageUrl);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("button.post-btn"));
            postSubmitButton.Click();

            // Verify the post image is displayed
            IWebElement post = driver.FindElement(By.XPath("//img[contains(@src, '" + imageUrl + "')]"));
            Assert.That(post, Is.Not.Null);
            
        }

        [Test]
        public void CreatePost_WithTextAndImageUrl_PostIsDisplayed_3()
        {
            string email = faker.Internet.Email();
            string username = faker.Internet.UserName();
            var imageUrl = faker.Image.PicsumUrl();
            var postContent = faker.Lorem.Sentence();

            CreateAccount(username, email, false);
            Login(email, testPassword);

            // Create a new post
            IWebElement postContentFieldText = driver.FindElement(By.CssSelector(".post-input"));
            postContentFieldText.SendKeys(postContent);
            IWebElement postContentFieldUrl = driver.FindElement(By.CssSelector(".image-input"));
            postContentFieldUrl.SendKeys(imageUrl);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("button.post-btn"));
            postSubmitButton.Click();

            // Verify the post image is displayed
            IWebElement postUrl = driver.FindElement(By.XPath("//img[contains(@src, '" + imageUrl + "')]"));
            Assert.That(postUrl, Is.Not.Null);
            IWebElement postText = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
            Assert.That(postText, Is.Not.Null);
        }
        // [Test]
        // public void CreatePost_OnlyWithImageFile_PostIsDisplayed()
        // {
        //     var email = _faker.Internet.Email();
        //     var username = _faker.Internet.UserName();
        //     var password = "Admin123*";
        //     var imageFilePath = "/Users/arthurbotto/week-11-acebook/csharp-acebook-mvc-umbrella/Acebook/wwwroot/images/il_1588xN.1965501418_pspx.png";
    


        //     driver.Navigate().GoToUrl("http://127.0.0.1:5287");
        //     IWebElement signUpButton = driver.FindElement(By.Id("signup"));
        //     signUpButton.Click();
        //     IWebElement nameField = driver.FindElement(By.Id("Name"));
        //     nameField.SendKeys(username);
        //     IWebElement emailField = driver.FindElement(By.Id("Email"));
        //     emailField.SendKeys(email);
        //     IWebElement passwordField = driver.FindElement(By.Id("Password"));
        //     passwordField.SendKeys(password);
        //     IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        //     submitButton.Click();
            
        //     // Sign in first
        //     driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
        //     IWebElement signInEmailField = driver.FindElement(By.Id("email"));
        //     signInEmailField.SendKeys(email);
        //     IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
        //     signInpasswordField.SendKeys(password);
        //     IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        //     signInsubmitButton.Click();

        //     // Create a new post
        //     IWebElement postContentField = driver.FindElement(By.Id("PostImageFile"));
        //     postContentField.SendKeys(imageFilePath);
        //     IWebElement postSubmitButton = driver.FindElement(By.Id("submit"));
        //     postSubmitButton.Click();
            
        //     // Wait for the post image to be displayed
        //     var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        //     wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//img[contains(@src, 'images/il_1588xN.1965501418_pspx.png')]")));

        //     // Verify the post image is displayed
        //     IWebElement post = driver.FindElement(By.XPath("//img[contains(@src, '/images/il_1588xN.1965501418_pspx.png')]"));
        //     Assert.That(post, Is.Not.Null);
        // }
        // [Test]
        // public void CreatePost_WithTextAndImageFile_PostIsDisplayed()
        // {
        //     var email = _faker.Internet.Email();
        //     var username = _faker.Internet.UserName();
        //     var password = "Admin123*";
        //     var imageFilePath = "/Users/arthurbotto/week-11-acebook/csharp-acebook-mvc-umbrella/Acebook/wwwroot/images/il_1588xN.1965501418_pspx.png";
        //     var postContent = _faker.Lorem.Sentence();
    


        //     driver.Navigate().GoToUrl("http://127.0.0.1:5287");
        //     IWebElement signUpButton = driver.FindElement(By.Id("signup"));
        //     signUpButton.Click();
        //     IWebElement nameField = driver.FindElement(By.Id("Name"));
        //     nameField.SendKeys(username);
        //     IWebElement emailField = driver.FindElement(By.Id("Email"));
        //     emailField.SendKeys(email);
        //     IWebElement passwordField = driver.FindElement(By.Id("Password"));
        //     passwordField.SendKeys(password);
        //     IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        //     submitButton.Click();
            
        //     // Sign in first
        //     driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
        //     IWebElement signInEmailField = driver.FindElement(By.Id("email"));
        //     signInEmailField.SendKeys(email);
        //     IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
        //     signInpasswordField.SendKeys(password);
        //     IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
        //     signInsubmitButton.Click();

        //     // Create a new post
        //     IWebElement postContentField = driver.FindElement(By.Id("PostImageFile"));
        //     postContentField.SendKeys(imageFilePath);
        //     IWebElement postContentFieldText = driver.FindElement(By.Id("PostText"));
        //     postContentFieldText.SendKeys(postContent);
        //     IWebElement postSubmitButton = driver.FindElement(By.Id("submit"));
        //     postSubmitButton.Click();
            
        //     // Wait for the post image to be displayed
        //     var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        //     wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//img[contains(@src, 'images/il_1588xN.1965501418_pspx.png')]")));

        //     // Verify the post image is displayed
        //     IWebElement postFile = driver.FindElement(By.XPath("//img[contains(@src, '/images/il_1588xN.1965501418_pspx.png')]"));
        //     Assert.That(postFile, Is.Not.Null);
        //     IWebElement postText = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
        //     Assert.That(postText, Is.Not.Null);
        // }
    }
}
