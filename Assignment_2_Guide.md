# Software Quality Engineering - Assignment 2 Guide

This document contains everything you need to understand the **FYP Business Process Application (Shop-Management-System)** and complete the **Software Quality Engineering Assignment 2** requirements from your teacher.

---

## 🚀 1. Project Overview & Login Credentials

### What is the project?
The project you developed is a **C# Desktop Application (WinForms)** named **Shop Management System**. It connects to a local Microsoft SQL Server DB (Database Name: `BSE6`). It performs CRUD (Create, Read, Update, Delete) operations for various business entities: `Customers`, `Vendors`, `Products`, `Stocks`, and `Orders`.

### Application Flow & Roles
The application has one role: Administrator. It starts at a Login window (`MyLogin.cs`). Upon a successful login, it opens the Main Menu (`FormMenu.cs`) which gives access to Insert, Update, Delete, and View panels for all entities.

### Administrator Credentials
Based on the database initialization script (`SQL file.sql`, Line 80):
* **Username:** `admin`
* **Password:** `admin`

---

## 📝 Task 1. Test Cases (MM-Path & Sequence Diagram)

An **MM-Path (Module-to-Module Path)** maps the execution flow of data or control from one module (or method) to another across your application. Here are two sample Test Cases formatted exactly as the teacher requested based on your codebase.

### Test Case 1: Valid Administrator Login

| Sequence | MM Path (Module sequence) | Sequence Diagram Summary | Test Case Details |
| :--- | :--- | :--- | :--- |
| **1** | `UI Input -> MyLogin.loginButton_Click()` | User enters "admin/admin" and clicks Login. | **Test ID:** TC_LOG_01 |
| **2** | `MyLogin -> Connect.connect()` | `MyLogin` requests a DB connection from `Connect` class. | **Objective:** Verify successful login with correct admin credentials. |
| **3** | `MyLogin -> SQL Server` | Executes `SELECT * FROM LOGIN` parameter query via `SqlDataAdapter`. | **Pre-condition:** App is running, DB is active using `BSE6`. |
| **4** | `SQL Server -> MyLogin` | Returns dataset showing 1 row matched. | **Input Data:** Username = "admin", Password = "admin". |
| **5** | `MyLogin -> FormMenu.Show()` | Validation passes; `MyLogin` hides, instantiates `FormMenu` and shows Menu. | **Expected Result:** Success messagebox "Login Successful!" appears, and Main Menu opens. |

### Test Case 2: Insert New Customer Validation Failure

| Sequence | MM Path (Module sequence) | Sequence Diagram Summary | Test Case Details |
| :--- | :--- | :--- | :--- |
| **1** | `UI Input -> CustomerInsert.submitButton_Click()` | User inputs data with a 9-digit phone number. | **Test ID:** TC_CUST_INS_02 |
| **2** | `CustomerInsert (Validation Check)` | Check evaluates `phno.Text.Length != 10`. | **Objective:** Verify the application blocks customer insertion if Phone Number is not exactly 10 digits. |
| **3** | `CustomerInsert -> MessageBox.Show()` | Phone validation fails, triggers the Error MessageBox. | **Pre-condition:** User is logged in and on the Customer Insert Screen. |
| **4** | `MessageBox -> User` | Application alerts the user and aborts DB connections early. | **Input Data:** CName="John", Ph=123456789, email=a@a.com, Addr="NYC". |
| **5** | *Return/Terminate* | Path ends before calling `Connect.connect()`. | **Expected Result:** Error msg "Enter valid Phone number" is shown. DB is not updated. |

---

## 🐛 Task 2. Faults & Issues Reporting

During code review of your application, several critical structural issues and bugs were identified. You should use these for your fault report to maximize your score:

1. **Hardcoded Database Connection String:**
   * **Fault Location:** `Connect.cs` (Line 15)
   * **Description:** The string is hardcoded explicitly to `Data Source=Dr-Tamim-Khan;`. This means the project will instantly crash on any other computer (like the teacher's) unless their PC is also named exactly `Dr-Tamim-Khan`.
   * **Severity:** Critical/Blocker.
2. **Missing Input Format Validation & Exception Leakage:**
   * **Fault Location:** `CustomerInsert.cs` (Line 59)
   * **Description:** While length is checked (`phno.Text.Length != 10`), the app doesn't check if the text is numeric. If a user types `ABCDEFGHIJ`, `Convert.ToInt64(phno.Text)` will throw a `FormatException`. The app catches this and shows `ex.Message` directly to the user.
   * **Severity:** High. Exposing raw Exception traces to UI is a security risk.
3. **Redundant Code / Inefficient Resource Management:**
   * **Fault Location:** `MyLogin.cs` & `CustomerInsert.cs`
   * **Description:** `con.Close()` is called inside the `try` block and then *again* blindly in the `finally` block logic.
   * **Severity:** Low.
4. **Tight Coupling (Lack of Separation of Concerns):**
   * **Fault Location:** Entire Application.
   * **Description:** Business database logic (SQL queries) is directly embedded inside UI Event handlers (like `loginButton_Click`). This makes unit testing impossible without UI automation.

---

## 📊 Task 3. Test Cases for Coverage & Coverage Report

Because your application business logic is tightly coupled into WinForms UI buttons, you have two options to achieve a coverage report:

1. **Refactor and Unit Test (Hard):** Separate your database methods into a `DatabaseManager` class and write **MSTest / NUnit** scripts to test just the C# methods.
2. **UI Automation Testing (Easier for Coverage):** Use a tool to click through your application automatically.

**How to generate the coverage report:**
* Use **Visual Studio Enterprise** or an extension like **Fine Code Coverage** (available free in the VS Marketplace).
* Write the tests (Unit tests or WinAppDriver UI tests). 
* Go to the top bar: **Test -> Analyze Code Coverage for All Tests**.
* Visual Studio will output a table showing **Block Coverage** mapping exactly which lines of code your test cases actually ran. Export this table as your coverage report.

---

## ⭐ Task 4. Extra Credit (Selenium & Selenium Web Driver)

**Important Distinction:** Selenium is strictly meant for website/browser testing, but your FYP project is a *Windows Desktop Application*. 

However, Microsoft provides **WinAppDriver**, which is an implementation of Selenium WebDriver for Windows Desktop applications. By using `OpenQA.Selenium.Appium.Windows.WindowsDriver`, you are completing the extra credit by applying Selenium WebDriver concepts (finding elements by ID, sending keys) to your desktop app.

Here is the C# Selenium WebDriver script you can use to automate the Login process:

```csharp
using System;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace WindowsFormsUITests
{
    public class LoginSeleniumExtraCreditTest
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        // Update this path to where your compiled .exe sits
        private const string AppPath = @"c:\Users\Laptop\Desktop\university\SpringSemester\Testing\Shop-Management-System-19032026-113216am\Shop-Management-System-master-09052022-094213am\ShopManagementSystem\bin\Debug\ShopManagementSystem.exe";

        public static void Main(string[] args)
        {
            var appOptions = new AppiumOptions();
            appOptions.AddAdditionalCapability("app", AppPath);

            // 1. Launch WinAppDriver (Selenium WebDriver for Windows)
            WindowsDriver<WindowsElement> session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appOptions);

            if (session == null)
            {
                Console.WriteLine("Failed to open application using Selenium WebDriver.");
                return;
            }

            // 2. Perform elements interactions using Selenium WebDriver strategies
            // Assuming the textboxes have AccessibilityIds or Name properties mapped.
            var usernameField = session.FindElementByAccessibilityId("userName"); 
            var passwordField = session.FindElementByAccessibilityId("password");
            var loginButton = session.FindElementByAccessibilityId("loginButton");

            // 3. Send test credentials
            usernameField.SendKeys("admin");
            passwordField.SendKeys("admin");

            // 4. Click the login button
            loginButton.Click();

            Console.WriteLine("Selenium Test Completed!");
            session.Quit();
        }
    }
}
```

**How to run this:**
1. Download and run **WinAppDriver.exe** on your machine.
2. Create a new C# Test Project in Visual Studio.
3. Add the `Appium.WebDriver` NuGet package (which includes Selenium WebDriver).
4. Run the code. It will literally launch your WinForms app and click the buttons on its own!

---
*Good luck with your submission! You now have the MM-Paths, Faults, Coverage Strategy, and the Extra Credit script completely covered.*
