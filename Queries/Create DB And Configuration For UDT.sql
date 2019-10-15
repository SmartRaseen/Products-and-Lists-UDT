USE UDT;

CREATE DATABASE UDT;

ALTER DATABASE UDT SET TRUSTWORTHY ON

sp_configure 'show advanced options',1;
GO
RECONFIGURE;

sp_configure 'clr enabled',1;
GO
RECONFIGURE;

sp_configure 'show advanced options',0;
GO
RECONFIGURE;
