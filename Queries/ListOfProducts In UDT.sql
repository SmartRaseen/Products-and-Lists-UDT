USE UDT; 

CREATE ASSEMBLY List
FROM 'C:\Users\USER\source\repos\Lists\Lists\bin\Debug\Lists.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS

CREATE TYPE List
EXTERNAL NAME List.[List]

DECLARE @var List = List :: Parse('1,Phone,25,15000.50#2,Charger,50,150.98#3,HeadSet,35,200.05#4,MemoryCard,100,350.10#5,Cover,80,200.15');
DECLARE @xmltype XML
SET @xmltype = @var.ToXml();
SELECT @xmltype;					/*this command is used to serialize the data into Xml Format, by our Program method ToXml()*/

DECLARE @list List = List :: Parse('9,Table,10,1000.80#10,Chair,15,600.35#11,Fan,25,400.55');
SELECT @list.ToString() AS Products;
INSERT INTO ListOfProducts(ProductsList) VALUES(@list.ToString()); /* This three command to used insert first product and retrieve all the product list*/

CREATE TABLE ListOfProducts(ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,ProductsList List NOT NULL);

INSERT INTO ListOfProducts(ProductsList) VALUES('9,Table,10,1000.80#10,Chair,15,600.35#11,Fan,25,400.55'); /* This command used to insert entire data into that table*/

SELECT ID,ProductsList.ToString() AS Products FROM ListOfProducts;  /* just Select this command to get entire data  */

DECLARE @productlist List;
SELECT @productlist = ProductsList FROM ListOfProducts;
SELECT @productlist.ToString() AS Products;

