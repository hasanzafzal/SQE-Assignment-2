use [BSE6];
/*
Creating the customer table
*/
if not exists (select * from dbo.sysobjects where id = object_id(N'[CUSTOMER]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE  CUSTOMER (
	C_ID INTEGER PRIMARY KEY,
	CNAME VARCHAR(50),
	ADDRESS VARCHAR(75),
	PHONE_NUMBER BIGINT,
	EMAIL VARCHAR(25));
/*
Creating the vendor table
*/

if not exists (select * from dbo.sysobjects where id = object_id(N'[VENDOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE VENDOR (
	VID INT PRIMARY KEY,
	VNAME VARCHAR(50),
	ADDRESS VARCHAR(75),
	PHONE_NUMBER BIGINT,
	EMAIL VARCHAR(25));
/*
Creating the product table
*/
if not exists (select * from dbo.sysobjects where id = object_id(N'[PRODUCT]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE PRODUCT
(
	PID INT PRIMARY KEY,
	PNAME VARCHAR(50),
	AMOUNT DECIMAL(12,2),
	VID INT CONSTRAINT FK1 FOREIGN KEY REFERENCES VENDOR(VID) ON DELETE CASCADE);

/*
Creating the myorder table.
*/
if not exists (select * from dbo.sysobjects where id = object_id(N'[MYORDER]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)	
CREATE TABLE MYORDER(
	ORD_ID INT PRIMARY KEY,
	PID INT CONSTRAINT FK2 FOREIGN KEY REFERENCES PRODUCT(PID) ON DELETE CASCADE,
	CID INT CONSTRAINT FK3 FOREIGN KEY REFERENCES CUSTOMER(C_ID) ON DELETE CASCADE,
	DATE DATE,
	AMOUNT DECIMAL(12,2));
/*
Creating the order details table.
*/

if not exists (select * from dbo.sysobjects where id = object_id(N'[ORDER_DETAILS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE ORDER_DETAILS(
	ORD_ID INT CONSTRAINT F6 FOREIGN KEY REFERENCES MYORDER(ORD_ID) ON DELETE CASCADE,
	PID INT CONSTRAINT F7 FOREIGN KEY REFERENCES PRODUCT(PID),
	QUANTITY INT,
	PRIMARY KEY(PID,ORD_ID));
/*
Creating the stock table.
*/

if not exists (select * from dbo.sysobjects where id = object_id(N'[STOCK]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE STOCK(
	PID INT CONSTRAINT FK4 FOREIGN KEY REFERENCES PRODUCT(PID) ON DELETE CASCADE,
	QUANTITY INT,
	PRIMARY KEY(PID));


/*
Creating the login table.
*/

if not exists (select * from dbo.sysobjects where id = object_id(N'[LOGIN]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE LOGIN
(
	UserName varchar(25),
	Password varchar(20),
	primary key(UserName,Password));

/*
Initial setup add login details.
*/

INSERT INTO LOGIN VALUES ('admin','admin');

/*
Check the table structure and contents.
*/
select * from LOGIN;
select * from CUSTOMER;
select * from MYORDER;
select * from ORDER_DETAILS;

/*
Creating the VIEW to view deatils of Order
*/
IF EXISTS (SELECT name FROM   sysobjects WHERE  name = 'ORDER_VIEW' AND 	  type = 'V')
	DROP VIEW ORDER_VIEW
GO

CREATE VIEW ORDER_VIEW AS
SELECT M.ORD_ID,C.C_ID,C.CNAME,C.ADDRESS,C.PHONE_NUMBER,C.EMAIL,D.PID,P.PNAME,D.QUANTITY,M.AMOUNT,M.DATE
FROM MYORDER M JOIN ORDER_DETAILS D ON M.ORD_ID = D.ORD_ID,CUSTOMER C,PRODUCT P
WHERE M.CID = C.C_ID AND P.PID = D.PID;
GO
/*
Creating VIEW for checking available stocks.
*/
IF EXISTS (SELECT name FROM   sysobjects WHERE  name = 'CURRENTLY_AVAILABLE_STOCK' AND 	  type = 'V')
	DROP VIEW CURRENTLY_AVAILABLE_STOCK
GO

CREATE VIEW CURRENTLY_AVAILABLE_STOCK AS
SELECT P.PID,P.PNAME,P.AMOUNT,S.QUANTITY
FROM PRODUCT P JOIN STOCK S ON P.PID = S.PID;
GO
/*
Creating a VIEW to view products available.
*/
IF EXISTS (SELECT name FROM   sysobjects WHERE  name = 'PRODUCT_VIEW' AND 	  type = 'V')
	DROP VIEW PRODUCT_VIEW
GO
CREATE VIEW PRODUCT_VIEW AS
SELECT P.PID,P.PNAME,P.AMOUNT,P.VID,V.VNAME
FROM PRODUCT P JOIN VENDOR V ON P.VID=V.VID;
GO
/*
Check the status of the views.
*/
SELECT * FROM PRODUCT_VIEW;
SELECT * FROM VENDOR;
SELECT * FROM CURRENTLY_AVAILABLE_STOCK;
GO

/*
Creating the STORED_PROCEDURE to insert customer data.
*/
IF EXISTS (SELECT name FROM   sysobjects WHERE  name = 'customer_in' AND type = 'P')
	DROP PROCEDURE customer_in
GO
CREATE PROCEDURE customer_in 
	@cid int,
	@cname varchar(50),
	@address varchar(75),
	@phone_number bigint,
	@email varchar(25)
AS
	INSERT INTO CUSTOMER VALUES(@cid,@cname,@address,@phone_number,@email);

