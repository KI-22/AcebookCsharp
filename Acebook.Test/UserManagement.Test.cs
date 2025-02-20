using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Bogus;
using OpenQA.Selenium.Support.UI;



namespace Acebook.Test
{
  public class UserManagement
  {
    ChromeDriver driver;
    Faker faker;

    [SetUp]
    public void Setup()
    {
      driver = new ChromeDriver();
      faker = new Faker();

    }

    [TearDown]
    public void TearDown() {
      driver.Quit();
    }

    [Test]
    public void SignUp_ValidCredentials_RedirectToSignIn_1()
    {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = faker.Internet.Email();
      string password = "Admin123*";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287");
      IWebElement nameField = driver.FindElement(By.Name("FullName"));
      nameField.SendKeys(name);
      IWebElement usernameField = driver.FindElement(By.Name("Name"));
      usernameField.SendKeys(username);
      IWebElement emailField = driver.FindElement(By.Name("Email"));
      emailField.SendKeys(email);
      IWebElement passwordField = driver.FindElement(By.Name("Password"));
      passwordField.SendKeys(password);
      IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
      submitButton.Click();
      string currentUrl = driver.Url;
      Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));
    }

    [Test]
    public void SignIn_ValidCredentials_RedirectToPosts_2() {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = faker.Internet.Email();
      string password = "Admin123*";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287");
      IWebElement nameField = driver.FindElement(By.Name("FullName"));
      nameField.SendKeys(name);
      IWebElement usernameField = driver.FindElement(By.Name("Name"));
      usernameField.SendKeys(username);
      IWebElement emailField = driver.FindElement(By.Name("Email"));
      emailField.SendKeys(email);
      IWebElement passwordField = driver.FindElement(By.Name("Password"));
      passwordField.SendKeys(password);
      IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
      submitButton.Click();

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
      emailField = driver.FindElement(By.Name("email"));
      emailField.SendKeys(email);
      passwordField = driver.FindElement(By.Name("password"));
      passwordField.SendKeys(password);
      submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
      submitButton.Click();
      string currentUrl = driver.Url;
      Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));
    }

    [Test]
    public void SignIn_InvalidCredentials_RedirectToSignIn_3() {
      string username = faker.Internet.UserName();
      string email = faker.Internet.Email();
      string password = "admin";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
      IWebElement emailField = driver.FindElement(By.Name("email"));
      emailField.SendKeys(email);
      IWebElement passwordField = driver.FindElement(By.Name("password"));
      passwordField.SendKeys(password);
      IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
      submitButton.Click();
      string currentUrl = driver.Url;
      Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));

      // Check for error message
      IWebElement errorMessage = driver.FindElement(By.CssSelector(".alert-danger"));
      Assert.That(errorMessage.Text, Is.EqualTo("Invalid email or password."));
    }

    [Test]
    public void CreateAccountWithInvalidEmail_4()
    {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = "invalidemail";
      string password = "Admin123*";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      driver.FindElement(By.Name("FullName")).SendKeys(name);
      driver.FindElement(By.Name("Name")).SendKeys(username);
      driver.FindElement(By.Name("Email")).SendKeys(email);
      driver.FindElement(By.Name("Password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();
      
      
      // Wait for validation error to appear
      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      IWebElement emailError = wait.Until(drv => drv.FindElement(By.CssSelector("span[data-valmsg-for='Email']")));

      // Get error message text and trim whitespace
      string errorMessage = emailError.Text.Trim();

      Assert.That(errorMessage, Is.EqualTo("The Email field is not a valid e-mail address."));
    }

    [Test]
    public void CreateAccountWithInvalidPassword_5()
    {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = faker.Internet.Email();
      string password = "admin123";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      driver.FindElement(By.Name("FullName")).SendKeys(name);
      driver.FindElement(By.Name("Name")).SendKeys(username);
      driver.FindElement(By.Name("Email")).SendKeys(email);
      driver.FindElement(By.Name("Password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();
      
      
      // Wait for validation error to appear
      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      IWebElement emailError = wait.Until(drv => drv.FindElement(By.CssSelector("span[data-valmsg-for='Password']")));

      // Get error message text and trim whitespace
      string errorMessage = emailError.Text.Trim();

      Assert.That(errorMessage, Is.EqualTo("Password must have at least one uppercase letter, one lowercase letter, one number, and one special character."));
    }

    [Test]
    public void CreateAccountWithEmptyPassword_6()
    {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = faker.Internet.Email();
      string password = "";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      driver.FindElement(By.Name("FullName")).SendKeys(name);
      driver.FindElement(By.Name("Name")).SendKeys(username);
      driver.FindElement(By.Name("Email")).SendKeys(email);
      driver.FindElement(By.Name("Password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();
      
      
      // Wait for validation error to appear
      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      IWebElement emailError = wait.Until(drv => drv.FindElement(By.CssSelector("span[data-valmsg-for='Password']")));

      // Get error message text and trim whitespace
      string errorMessage = emailError.Text.Trim();

      Assert.That(errorMessage, Is.EqualTo("Password is required."));
    }

    [Test]
    public void CreateAccountWithEmptyEmail_7()
    {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = "";
      string password = "Admin123*";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      driver.FindElement(By.Name("FullName")).SendKeys(name);
      driver.FindElement(By.Name("Name")).SendKeys(username);
      driver.FindElement(By.Name("Email")).SendKeys(email);
      driver.FindElement(By.Name("Password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();
      
      
      // Wait for validation error to appear
      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      IWebElement emailError = wait.Until(drv => drv.FindElement(By.CssSelector("span[data-valmsg-for='Email']")));

      // Get error message text and trim whitespace
      string errorMessage = emailError.Text.Trim();

      Assert.That(errorMessage, Is.EqualTo("The Email field is required."));
    }

    [Test]
    public void CreateAccountWithEmptyUsername_8()
    {
      string name = faker.Name.FullName();
      string username = "";
      string email = faker.Internet.Email();
      string password = "Admin123*";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      driver.FindElement(By.Name("FullName")).SendKeys(name);
      driver.FindElement(By.Name("Name")).SendKeys(username);
      driver.FindElement(By.Name("Email")).SendKeys(email);
      driver.FindElement(By.Name("Password")).SendKeys(password);
      driver.FindElement(By.CssSelector("input[type='submit']")).Click();
      
      
      // Wait for validation error to appear
      WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      IWebElement emailError = wait.Until(drv => drv.FindElement(By.CssSelector("span[data-valmsg-for='Name']")));

      // Get error message text and trim whitespace
      string errorMessage = emailError.Text.Trim();

      Assert.That(errorMessage, Is.EqualTo("Username is required."));
    }

    [Test]
    public void SignIn_EmptyFields_Field_required_message_9() {
      string email = faker.Internet.Email();
      string password = "";

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
      IWebElement emailField = driver.FindElement(By.Name("email"));
      emailField.SendKeys(email);
      IWebElement passwordField = driver.FindElement(By.Name("password"));
      passwordField.SendKeys(password);
      IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
      submitButton.Click();
      string currentUrl = driver.Url;
      Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));

      // Check for error message
      IWebElement errorMessage = driver.FindElement(By.CssSelector(".alert-danger"));
      Assert.That(errorMessage.Text, Is.EqualTo("Email and password are required."));
    }

    [Test]
    public void Logout_RedirectToSignIn_10() {
      string name = faker.Name.FullName();
      string username = faker.Internet.UserName();
      string email = faker.Internet.Email();
      string password = "Admin123*";

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

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/posts");
      IWebElement logoutButton = driver.FindElement(By.CssSelector("button.btn.btn-secondary"));
      logoutButton.Click();
      string currentUrl = driver.Url;
      Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/signin"));
    }

  }
}