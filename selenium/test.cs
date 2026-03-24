using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.IO;
using System.Threading;

[assembly: DoNotParallelize]

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
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string appPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\ShopManagementSystem\bin\Debug\ShopManagementSystem.exe"));
            
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("app", appPath);
        
            session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), options);
            session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
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

            // Create a desktop session to find FormMenu
            AppiumOptions desktopOptions = new AppiumOptions();
            desktopOptions.AddAdditionalCapability("app", "Root");
            var desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), desktopOptions);

            Console.WriteLine("Listing top level windows...");
            var windows = desktopSession.FindElementsByXPath("/*/*");
            foreach (var w in windows) {
                try {
                    string name = w.GetAttribute("Name");
                    if (name.Contains("Form") || name.Contains("Hasan") || name.Contains("Shop")) {
                        Console.WriteLine("Window Name: " + name);
                    }
                } catch {}
            }

            var formMenuWindow = desktopSession.FindElementByName("FormMenu");

            // 2. Open the Customer Insert screen
            // Native WinForms MenuStrip items are most reliably tracked by their visible Text Name!
            formMenuWindow.FindElementByName("CUSTOMERS").Click();
            Thread.Sleep(500);
            formMenuWindow.FindElementByName("INSERT").Click();
            Thread.Sleep(1500);

            // 3. Enter test data with invalid 9-digit phone number
            // (If this crashes here, MDI children might act as separate windows in WinAppDriver)
            var customerId = formMenuWindow.FindElementByAccessibilityId("customerId");
            var cName = formMenuWindow.FindElementByAccessibilityId("CustomerName");
            var phno = formMenuWindow.FindElementByAccessibilityId("phno");
            var email = formMenuWindow.FindElementByAccessibilityId("CustomerEmail");
            var addr = formMenuWindow.FindElementByAccessibilityId("CustomerAddress");
            var submit = formMenuWindow.FindElementByAccessibilityId("submitButton");

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
                desktopSession.FindElementByName("OK").Click();
            }
            catch (Exception) { }

            if (desktopSession != null)
            {
                desktopSession.Quit();
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            if (session != null)
            {
                session.Quit();
                session = null;
            }

            // Ensure all instances are killed to prevent pollution between tests
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("ShopManagementSystem"))
            {
                try { process.Kill(); } catch { }
            }
        }
    }
}
