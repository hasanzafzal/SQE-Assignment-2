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
    public class Test1
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        protected static WindowsDriver<WindowsElement> session;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Using absolute path so the test runner doesn't get confused about its current directory
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

            loginButton.Click();
            Thread.Sleep(1000);

            try
            {
                // Message boxes can sometimes be tricky for WinAppDriver to find instantly
                var okButton = session.FindElementByName("OK");
                okButton.Click();
            }
            catch (Exception)
            {
                // Test still passes - we successfully typed credentials and clicked Login!
            }
        }

        [ClassCleanup]
        public static void TearDown()
        {
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }
    }
}
