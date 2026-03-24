# SQE Assignment 2
Software Quality Engineering (SEN 321)

Student Name: Hasan Zahid and Mustafa Dilawar Khan

Enrollment #: 01-131232-028 and 074

Teacher: Dr. Tamim Ahmad Khan 

Dept of SE, BUIC



# Test Architecture & Execution Flow:
The following sequence describes exactly what happens from the moment the test suite is run to when a result is reported:

Step	What happens	Responsible component

1	Developer runs tests via Visual Studio Test Explorer	MSTest runner

2	[TestInitialize] fires — Setup() resolves the .exe path dynamically	MSTest + file system

3	AppiumOptions is configured with the .exe path as the 'app' capability	Appium C# client

4	WindowsDriver sends a POST /session request to http://127.0.0.1:4723	Appium → WinAppDriver

5	WinAppDriver launches ShopManagementSystem.exe and returns a session ID	WinAppDriver → Windows OS

6	Test method executes — FindElement, SendKeys, Click calls are made	MSTest test body

7	Each call is an HTTP request to WinAppDriver, which calls Windows UIA	WinAppDriver → UIA API

8	Windows UIA locates the control and performs the action on the live .exe	Windows OS

9	[TestCleanup] fires — session.Quit() closes the app and ends the session	MSTest + WinAppDriver

10	MSTest reports Pass / Fail based on Assert results and exceptions	MSTest runner




# How to Run the Tests
Step	Action

1	Install WinAppDriver from https://github.com/microsoft/WinAppDriver/releases

2	Run WinAppDriver.exe as Administrator — it starts listening on port 4723

3	Build the ShopManagementSystem project in Debug configuration

4	Open the test project in Visual Studio

5	Open Test Explorer (Test → Test Explorer)

6	Click Run All — tests will launch, interact with, and close the .exe automatically

7	Results appear in Test Explorer with Pass / Fail status for each test method
