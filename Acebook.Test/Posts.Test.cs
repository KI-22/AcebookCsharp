using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Faker;
using System.Text.RegularExpressions;
using Bogus;  // Import the Bogus namespace
using System;
using System.Threading.Tasks;


namespace Acebook.Tests
{
    public class Posts
    {
        ChromeDriver driver;
        private Bogus.Faker _faker;

        [SetUp] 
        public void Setup()
        {
            driver = new ChromeDriver();
            
        }
        [SetUp]
        public void SetUp()
        {
            _faker = new Bogus.Faker();  // Instantiate Bogus Faker
        }

        [TearDown]
        public void TearDown() 
        {
            driver.Quit();
        }
        [Test]
        public void CreatePost_OnlyWithText_PostIsDisplayed()
        {
            var email = _faker.Internet.Email();
            var username = _faker.Internet.UserName();
            var password = "Admin123*";
            var postContent = _faker.Lorem.Sentence();

            driver.Navigate().GoToUrl("http://127.0.0.1:5287");
            IWebElement signUpButton = driver.FindElement(By.Id("signup"));
            signUpButton.Click();
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.SendKeys(username);
            IWebElement emailField = driver.FindElement(By.Id("Email"));
            emailField.SendKeys(email);
            IWebElement passwordField = driver.FindElement(By.Id("Password"));
            passwordField.SendKeys(password);
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            submitButton.Click();

            // Sign in first
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
            IWebElement signInEmailField = driver.FindElement(By.Id("email"));
            signInEmailField.SendKeys(email);
            IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
            signInpasswordField.SendKeys(password);
            IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            signInsubmitButton.Click();

            // Create a new post
            IWebElement postContentFieldText = driver.FindElement(By.CssSelector(".post-input"));
            postContentFieldText.SendKeys(postContent);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            postSubmitButton.Click();

            // Verify the post is displayed
            IWebElement post = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
            Assert.IsNotNull(post);
        }

        [Test]
        public void CreatePost_OnlyWithImageUrl_PostIsDisplayed()
        {
            var email = _faker.Internet.Email();
            var username = _faker.Internet.UserName();
            var password = "Admin123*";
            var imageUrl = _faker.Image.PicsumUrl();

            driver.Navigate().GoToUrl("http://127.0.0.1:5287");
            IWebElement signUpButton = driver.FindElement(By.Id("signup"));
            signUpButton.Click();
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.SendKeys(username);
            IWebElement emailField = driver.FindElement(By.Id("Email"));
            emailField.SendKeys(email);
            IWebElement passwordField = driver.FindElement(By.Id("Password"));
            passwordField.SendKeys(password);
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            submitButton.Click();

            // Sign in first
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
            IWebElement signInEmailField = driver.FindElement(By.Id("email"));
            signInEmailField.SendKeys(email);
            IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
            signInpasswordField.SendKeys(password);
            IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            signInsubmitButton.Click();

            // Create a new post
            IWebElement postContentFieldUrl = driver.FindElement(By.CssSelector(".image-input"));
            postContentFieldUrl.SendKeys(imageUrl);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            postSubmitButton.Click();

            // Verify the post image is displayed
            IWebElement post = driver.FindElement(By.XPath("//img[contains(@src, '" + imageUrl + "')]"));
            Assert.IsNotNull(post);
        }

        [Test]
        public void CreatePost_WithTextAndImageUrl_PostIsDisplayed()
        {
            var email = _faker.Internet.Email();
            var username = _faker.Internet.UserName();
            var password = "Admin123*";
            var imageUrl = _faker.Image.PicsumUrl();
            var postContent = _faker.Lorem.Sentence();

            driver.Navigate().GoToUrl("http://127.0.0.1:5287");
            IWebElement signUpButton = driver.FindElement(By.Id("signup"));
            signUpButton.Click();
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.SendKeys(username);
            IWebElement emailField = driver.FindElement(By.Id("Email"));
            emailField.SendKeys(email);
            IWebElement passwordField = driver.FindElement(By.Id("Password"));
            passwordField.SendKeys(password);
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            submitButton.Click();

            // Sign in first
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
            IWebElement signInEmailField = driver.FindElement(By.Id("email"));
            signInEmailField.SendKeys(email);
            IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
            signInpasswordField.SendKeys(password);
            IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            signInsubmitButton.Click();

            // Create a new post
            IWebElement postContentFieldText = driver.FindElement(By.CssSelector(".post-input"));
            postContentFieldText.SendKeys(postContent);
            IWebElement postContentFieldUrl = driver.FindElement(By.CssSelector(".image-input"));
            postContentFieldUrl.SendKeys(imageUrl);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            postSubmitButton.Click();

            // Verify the post image is displayed
            IWebElement postUrl = driver.FindElement(By.XPath("//img[contains(@src, '" + imageUrl + "')]"));
            Assert.IsNotNull(postUrl);
            IWebElement postText = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
            Assert.IsNotNull(postText);
        }

        [Test]
        public void CreatePost_OnlyWithImageFile_PostIsDisplayed()
        {
            var email = _faker.Internet.Email();
            var username = _faker.Internet.UserName();
            var password = "Admin123*";
            var imageFilePath = "/Users/aysinakpinar/Desktop/04_engineering_project_2/csharp-acebook-mvc-template/Acebook/wwwroot/images/il_1588xN.1965501418_pspx.png";

            driver.Navigate().GoToUrl("http://127.0.0.1:5287");
            IWebElement signUpButton = driver.FindElement(By.Id("signup"));
            signUpButton.Click();
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.SendKeys(username);
            IWebElement emailField = driver.FindElement(By.Id("Email"));
            emailField.SendKeys(email);
            IWebElement passwordField = driver.FindElement(By.Id("Password"));
            passwordField.SendKeys(password);
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            submitButton.Click();

            // Sign in first
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
            IWebElement signInEmailField = driver.FindElement(By.Id("email"));
            signInEmailField.SendKeys(email);
            IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
            signInpasswordField.SendKeys(password);
            IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            signInsubmitButton.Click();

            // Create a new post
            IWebElement postContentFieldFile = driver.FindElement(By.CssSelector(".file-input"));
            postContentFieldFile.SendKeys(imageFilePath);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            postSubmitButton.Click();

            // Wait for the post image to be displayed
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//img[contains(@src, 'images/il_1588xN.1965501418_pspx.png')]")));

            // Verify the post image is displayed
            IWebElement post = driver.FindElement(By.XPath("//img[contains(@src, '/images/il_1588xN.1965501418_pspx.png')]"));
            Assert.IsNotNull(post);
        }

        [Test]
        public void CreatePost_WithTextAndImageFile_PostIsDisplayed()
        {
            var email = _faker.Internet.Email();
            var username = _faker.Internet.UserName();
            var password = "Admin123*";
            var imageFilePath = "/Users/aysinakpinar/Desktop/04_engineering_project_2/csharp-acebook-mvc-template/Acebook/wwwroot/images/il_1588xN.1965501418_pspx.png";
            var postContent = _faker.Lorem.Sentence();

            driver.Navigate().GoToUrl("http://127.0.0.1:5287");
            IWebElement signUpButton = driver.FindElement(By.Id("signup"));
            signUpButton.Click();
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.SendKeys(username);
            IWebElement emailField = driver.FindElement(By.Id("Email"));
            emailField.SendKeys(email);
            IWebElement passwordField = driver.FindElement(By.Id("Password"));
            passwordField.SendKeys(password);
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            submitButton.Click();

            // Sign in first
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
            IWebElement signInEmailField = driver.FindElement(By.Id("email"));
            signInEmailField.SendKeys(email);
            IWebElement signInpasswordField = driver.FindElement(By.Id("password"));
            signInpasswordField.SendKeys(password);
            IWebElement signInsubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            signInsubmitButton.Click();

            // Create a new post
            IWebElement postContentFieldFile = driver.FindElement(By.CssSelector(".file-input"));
            postContentFieldFile.SendKeys(imageFilePath);
            IWebElement postContentFieldText = driver.FindElement(By.CssSelector(".post-input"));
            postContentFieldText.SendKeys(postContent);
            IWebElement postSubmitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            postSubmitButton.Click();

            // Wait for the post image to be displayed
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//img[contains(@src, 'images/il_1588xN.1965501418_pspx.png')]")));

            // Verify the post image is displayed
            IWebElement postFile = driver.FindElement(By.XPath("//img[contains(@src, '/images/il_1588xN.1965501418_pspx.png')]"));
            Assert.IsNotNull(postFile);
            IWebElement postText = driver.FindElement(By.XPath("//p[contains(text(), '" + postContent + "')]"));
            Assert.IsNotNull(postText);
        }


    }
}
