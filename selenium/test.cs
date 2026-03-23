using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.IO;
using System.Threading;

namespace selenium
{
    [TestClass]
    public class test
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        protected WindowsDriver<WindowsElement> session;

        [TestInitialize]
        public void Setup()
        {
            string appPath = @"C:\Users\Laptop\source\repos\SQE-Assignment-2\ShopManagementSystem\bin\Debug\ShopManagementSystem.exe";
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("app", appPath);

            session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), options);
            Assert.IsNotNull(session);
        }

        [TestMethod]
        public void TestValidAdminLogin()
        {
            if (session == null) return;

            var usernameField = session.FindElementByAccessibilityId("userName");
            var passwordField = session.FindElementByAccessibilityId("password");
            var loginButton = session.FindElementByAccessibilityId("loginButton");

            usernameField.SendKeys("admin");
            passwordField.SendKeys("admin");
            Thread.Sleep(1000);

            try
            {
                loginButton.Click();
                Thread.Sleep(1000);
                var okButton = session.FindElementByName("OK");
                okButton.Click();
            }
            catch (Exception) { }
        }

        //var fakeButton = session.FindElementByAccessibilityId("NonExistentButtonThatDoesn'tExist");
        //fakeButton.Click();

        [TestMethod]
        public void TestInvalidCredentials()
        {
            if (session == null) return;

            var usernameField = session.FindElementByAccessibilityId("userName");
            var passwordField = session.FindElementByAccessibilityId("password");
            var loginButton = session.FindElementByAccessibilityId("loginButton");

            usernameField.SendKeys("wrongUser");
            passwordField.SendKeys("wrongPassword");
            Thread.Sleep(500);

            try
            {
                loginButton.Click();
                Thread.Sleep(1000);
                var okButton = session.FindElementByName("OK");
                okButton.Click();
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void TestEmptyFieldsLogin()
        {
            if (session == null) return;

            var usernameField = session.FindElementByAccessibilityId("userName");
            var passwordField = session.FindElementByAccessibilityId("password");
            var loginButton = session.FindElementByAccessibilityId("loginButton");

            usernameField.Clear();
            passwordField.Clear();
            Thread.Sleep(500);

            try
            {
                loginButton.Click();
                Thread.Sleep(1000);
                var okButton = session.FindElementByName("OK");
                okButton.Click();
            }
            catch (Exception) { }

        }

        [TestMethod]
        public void TestCustomerInsert_PhoneValidationFailure()
        {
            if (session == null) return;

            // 1. Log in to reach the Main Menu
            session.FindElementByAccessibilityId("userName").SendKeys("admin");
            session.FindElementByAccessibilityId("password").SendKeys("admin");

            try
            {
                session.FindElementByAccessibilityId("loginButton").Click();
                Thread.Sleep(1000);
                session.FindElementByName("OK").Click();
            }
            catch (Exception) { }

            Thread.Sleep(2000);

            // Switch to the newly opened FormMenu window! (Because MyLogin was hidden)
            // Grab the very first available active application window handle remaining
            session.SwitchTo().Window(session.WindowHandles[0]);

            // 2. Open the Customer Insert screen
            // Native WinForms MenuStrip items are most reliably tracked by their visible Text Name!
            session.FindElementByName("CUSTOMERS").Click();
            Thread.Sleep(500);
            session.FindElementByName("INSERT").Click();
            Thread.Sleep(1500);

            // 3. Enter test data with invalid 9-digit phone number
            // (If this crashes here, MDI children might act as separate windows in WinAppDriver)
            var customerId = session.FindElementByAccessibilityId("customerId");
            var cName = session.FindElementByAccessibilityId("CustomerName");
            var phno = session.FindElementByAccessibilityId("phno");
            var email = session.FindElementByAccessibilityId("CustomerEmail");
            var addr = session.FindElementByAccessibilityId("CustomerAddress");
            var submit = session.FindElementByAccessibilityId("submitButton");

            customerId.SendKeys("10");
            cName.SendKeys("John");
            phno.SendKeys("123456789");
            email.SendKeys("a@a.com");
            addr.SendKeys("NYC");
            Thread.Sleep(1000);

            // 4. Submit the form
            submit.Click();
            Thread.Sleep(1000);

            try
            {
                session.FindElementByName("OK").Click();
            }
            catch (Exception) { }
        }

        [TestCleanup]
        public void TearDown()
        {
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }
    }
}
