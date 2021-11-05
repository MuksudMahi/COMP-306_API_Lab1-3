CREATE DATABASE lab3;
GO

USE	lab3;
GO

CREATE TABLE [User](
	Email varchar(20) primary key not null,
	UserName varchar (30) not null,
	Password varchar (20) not null
);
GO

INSERT INTO [User] VALUES ('user@gmail.com', 'User', '123456');


select * from [User];