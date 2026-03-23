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
            string appPath = @"E:\source\repos\SQE Assignment 2\ShopManagementSystem\bin\Debug\ShopManagementSystem.exe";
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
            //var fakeButton = session.FindElementByAccessibilityId("NonExistentButtonThatDoesn'tExist");
            //fakeButton.Click();
        }

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

