USE Test;

/***** Alter the database *****/

ALTER DATABASE Test SET TRUSTWORTHY ON

sp_configure 'show advanced options',1;
RECONFIGURE;
GO

sp_configure 'clr enabled',1;
RECONFIGURE;
GO

sp_configure 'show advanced options',0;
RECONFIGURE;

/******* Create Type & Assembly *********/

/* Register the assembly in SQL Server by using the CREATE ASSEMBLY statement */

CREATE ASSEMBLY ListProduct
FROM 'C:\Users\USER\source\repos\Lists\Lists\bin\Debug\Lists.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;

CREATE TYPE ListProduct
EXTERNAL NAME ListProduct.ListProduct;

/******** DROP Type & Assembly & Function *********/

DROP TYPE ListProduct;
DROP ASSEMBLY ListProduct;
DROP FUNCTION GetAsProductTable;

/******** Using Table Valued Parameter to Return Data in Table Format ********/

CREATE OR ALTER FUNCTION GetAsProductTable(@ListProduct ListProduct)   
RETURNS TABLE (  
   ProductID INT,  
   ProductName NVARCHAR(100),
   ProductQuantity INT,
   ProductPrice DECIMAL
)  
AS EXTERNAL NAME ListProduct.ListProduct.[GetAsProductTable];  
GO  

DECLARE @var ListProduct = ListProduct :: Parse('1,Shirts,350,600.40#2,T-Shirts,500,200.75#3,Geans,200,1200.25')
SELECT @var.ToString() AS Product;

DECLARE @var ListProduct = ListProduct :: Parse('1,Shirts,350,600.40#2,T-Shirts,500,200.75#3,Geans,200,1200.25')
SELECT * FROM dbo.GetAsProductTable(@var);




/********** ToString() ************/

DECLARE @var ListProduct = ListProduct :: Parse('3,Watch,15,1500.50,#4,Hat,20,200.10');
SELECT @var.ToString();



/******* Xml Format ********/

DECLARE @var ListProduct = ListProduct :: Parse('3,Watch,15,1500.50,#4,Hat,20,200.10');
DECLARE @xmlformat XML 
SET @xmlformat = @var.ToXml();
SELECT @xmlformat;

/*******************************************/