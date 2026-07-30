--DAY 1: DATABASE, TABLES & JOINS

--ASSIGNMENT1: Database Setup (DDL)

--1. Create a database named OrderManagementDB.
create database OrderManagementDB;

--2. Use the database.
use OrderManagementDB;

--3. Create tables: Customers, Products, Orders, OrderItems with proper keys and constraints.

CREATE TABLE customers
(
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) UNIQUE NOT NULL,
    City NVARCHAR(100) NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

create table products
(
    productid int primary key identity(1,1),
    productname nvarchar(150) not null,
    price int not null,
    stockquantity int default 0,
    category nvarchar(100) null,
    createdDate datetime default getdate()
);

create table orders
(
    orderid int primary key identity(1,1),
    customerid int references customers(customerid),
    orderdate datetime default getdate(),
    status nvarchar(30) default 'Pending',
    totalamount decimal(10,2) null
);

create table orderitems
(
    orderitemid int primary key identity(1,1),
    orderid int references Orders(orderid),
    productid int references products(productid),
    quantity int not null check(quantity>0),
    unitprice decimal(10,2) not null
);


--ASSIGNMENT2: ALTER Operations

--1. Add PhoneNumber to Customers.
alter table customers add phonenumber varchar(10);

--2. Modify Product price to decimal.
alter table products alter column price decimal(10,2);

--3. Rename FullName to CustomerName.
exec sp_rename 'customers.fullname', 'customername', 'column';

--4. Drop IsActive column.
alter table customers drop constraint DF__customers__isact__5DCAEF64;
alter table customers drop column isactive;
alter table orders drop column totalamount;


--ASSIGNMENT3: DROP Operations

--1. Drop and recreate OrderItems.
drop table orderitems;

--2. Drop database after backup.
--USE master; -- can't drop a DB while "inside" it
--DROP DATABASE OrderManagementDB;


--ASSIGNMENT4: Joins

--1. Orders with customer names.
select 
    o.*, 
    c.customername 
from orders as o 
inner join customers as c on o.customerid = c.customerid;

--2. Products never ordered.
select 
    p.productname 
from products as p 
left join orderitems as oi on p.productid = oi.productid 
where oi.productid is null;

--3. Order details with total price.
select 
    p.productname, 
    oi.quantity, 
    oi.unitprice, 
    oi.quantity * oi.unitprice as totalprice 
from products as p 
inner join orderitems as oi on p.productid = oi.productid;

--4. Customers with multiple orders.
select 
    c.customername 
from customers as c 
inner join orders as o on c.customerid = o.customerid 
group by c.customername 
having count(o.orderid) > 1;

--5. Orders without items.
select 
    o.orderid 
from orders as o 
left join orderitems as oi on o.orderid = oi.orderid 
where oi.productid is null;


--DAY2: DML


--ASSIGNMENT5: INSERT

--Insert multiple customers, products, orders, and order items.

INSERT INTO Customers (CustomerName, Email, City, PhoneNumber, CreatedDate)
VALUES 
('Sneha Patel', 'sneha.patel@email.com', 'Pune', '9012345678', '2026-01-10'),
('Vikram Rao', 'vikram.rao@email.com', 'Chennai', '9023456789', '2026-03-15'),
('Neha Gupta', 'neha.gupta@email.com', 'Hyderabad', '9034567890', '2026-05-20');

insert into products (productname, price, stockquantity, category, createddate) 
values 
('Soap', 89.9, 23, 'bath', '2026-05-23'),
('Shampoo', 120, 12, 'bath', '2026-06-05'),
('bottle', 23.6, 32, 'utensils', '2026-08-09');

INSERT INTO Orders (CustomerId, Status, OrderDate)
VALUES 
(4, 'Pending', '2026-01-15'),
(5, 'Completed', '2026-02-20'),
(6, 'Pending', '2026-04-10'),
(4, 'Completed', '2026-06-01');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES
(5, 6, 3, 89.9),
(5, 7, 2, 120),
(6, 8, 5, 23.6),
(7, 6, 1, 89.9),
(8, 7, 4, 120);

--ASSIGNMENT6: UPDATE

--1. Increase price conditionally.
update products 
set price = price + (0.1 * price) 
where category = 'Electronics';

--2. Update order status.
update orders 
set status = 'Shipped' 
where orderid = 5;

--3. Reduce stock on order.
update products 
set stockquantity = stockquantity - 3 
where productname = 'bottle';

--4. Update customer city.
update customers 
set city = 'Mohali' 
where customerid = 3;


--ASSIGNMENT7: DELETE

--1. Remove cancelled orders.
delete from orderitems 
where orderid in (select orderid from orders where status = 'Cancelled');

delete from orders 
where status = 'Cancelled';

--2. Delete unused products.
delete from products 
where productid not in (select productid from orderitems);

--3. Remove customers with no orders.
delete from customers 
where customerid not in (select customerid from orders);


--ASSIGNMENT8: Queries

--1. Order count per customer.
select 
    c.customerid, 
    c.customername, 
    count(o.orderid) as ordercount 
from customers as c 
left join orders as o on c.customerid = o.customerid 
group by c.customerid, c.customername;

--2. Revenue per order.
select 
    o.orderid, 
    sum(oi.quantity * oi.unitprice) as totalrevenue 
from orders as o 
inner join orderitems as oi on o.orderid = oi.orderid 
group by o.orderid;

--3. Top 3 products.
select top 3
    p.productid, 
    p.productname, 
    sum(oi.quantity * oi.unitprice) as totalrevenue 
from products as p 
inner join orderitems as oi on p.productid = oi.productid 
group by p.productid, p.productname 
order by totalrevenue desc;

--4. High value customers.
select top 5
    c.customerid, 
    c.customername, 
    sum(oi.quantity * oi.unitprice) as totalrevenue 
from customers as c 
inner join orders as o on c.customerid = o.customerid 
inner join orderitems as oi on o.orderid = oi.orderid 
group by c.customerid, c.customername 
order by totalrevenue desc;

--5. Monthly order counts.
select 
    month(orderdate) as ordermonth, 
    count(orderid) as ordercount 
from orders 
group by month(orderdate);


--DAY 3: STORED PROCEDURES & VIEWS

--ASSIGNMENT9: Stored Procedures

--1. Add customer with email validation.

create procedure addcustomer 
    @customername nvarchar(100), 
    @email nvarchar(150), 
    @city nvarchar(100) 
as
begin
    if @email like '%@%'
    begin
        insert into customers(customername, email, city) 
        values (@customername, @email, @city);
    end
    else
    begin
        print 'Invalid Email';
    end
end;

EXEC AddCustomer @CustomerName = 'Test User', @Email = 'test@email.com', @City = 'Delhi';
EXEC AddCustomer @CustomerName = 'Bad User', @Email = 'notanemail', @City = 'Delhi';

--2. Place order with stock check.

create procedure placeorder
    @customerid int, 
    @productid int, 
    @quantity int 
as 
begin 
    declare @currentstock int; 
    select @currentstock = stockquantity from products where productid = @productid; 
    
    if(@currentstock >= @quantity)
    begin 
        print 'enough stock';
        insert into orders (customerid, status) values (@customerid, 'Completed');
        
        declare @neworderid int; 
        set @neworderid = scope_identity();
        
        declare @unitprice decimal(10,2);
        select @unitprice = price from products where productid = @productid;
        
        insert into orderitems (orderid, productid, quantity, unitprice) 
        values (@neworderid, @productid, @quantity, @unitprice);
        
        update products 
        set stockquantity = stockquantity - @quantity 
        where productid = @productid;
    end 
    else 
    begin 
        print 'insufficient stock'; 
    end 
end;

EXEC PlaceOrder @CustomerId = 2, @ProductId = 3, @Quantity = 2;

--3. Get order history.

create procedure getorderhistory 
    @customerid int 
as 
begin 
    select 
        c.customerid, 
        c.customername, 
        o.orderid, 
        o.orderdate, 
        o.status, 
        sum(oi.quantity * oi.unitprice) as totalamount 
    from customers as c 
    inner join orders as o on c.customerid = o.customerid 
    inner join orderitems as oi on o.orderid = oi.orderid 
    where c.customerid = @customerid 
    group by c.customerid, c.customername, o.orderid, o.orderdate, o.status;
end;

EXEC GetOrderHistory @CustomerId = 1;


--ASSIGNMENT10: Views


--1. Order summary view.

create view ordersummary 
as 
select 
    o.orderid, 
    c.customername, 
    o.orderdate, 
    o.status, 
    sum(oi.quantity * oi.unitprice) as totalamount 
from customers as c 
inner join orders as o on c.customerid = o.customerid 
inner join orderitems as oi on o.orderid = oi.orderid 
group by o.orderid, c.customername, o.orderdate, o.status;

SELECT * FROM OrderSummary;

--2. Product sales view.

create view productsales 
as 
select 
    p.productid, 
    p.productname, 
    sum(oi.quantity * oi.unitprice) as totalamount 
from products as p 
left join orderitems as oi on p.productid = oi.productid 
group by p.productid, p.productname;

SELECT * FROM ProductSales ORDER BY totalamount DESC;

--3. Active customers view.

create view activecustomers 
as 
select 
    c.customerid, 
    c.customername, 
    c.email, 
    c.city, 
    count(o.orderid) as ordercount 
from customers as c 
left join orders as o on c.customerid = o.customerid 
group by c.customerid, c.customername, c.email, c.city 
having count(o.orderid) >= 1;

SELECT * FROM ActiveCustomers;


--ASSIGNMENT11: Advanced Tasks


--1. Transaction with rollback.

begin transaction; 
    insert into customers (customername, email) values ('Sanjil', 'sanjil@gmail.com');
rollback;

SELECT * FROM Customers;

--2. Error handling in procedures.

alter procedure placeOrder 
    @customerid int, 
    @productid int, 
    @quantity int 
as 
begin
    begin try 
        begin transaction 
        
        declare @currentstock int; 
        select @currentstock = stockquantity from products where productid = @productid; 
        
        if(@currentstock >= @quantity) 
        begin 
            print 'enough stock'; 
            insert into orders (customerid, status) values (@customerid, 'Completed'); 
            
            declare @neworderid int; 
            set @neworderid = scope_identity(); 
            
            declare @unitprice decimal(10,2); 
            select @unitprice = price from products where productid = @productid; 
            
            insert into orderitems (orderid, productid, quantity, unitprice) 
            values (@neworderid, @productid, @quantity, @unitprice); 
            
            update products 
            set stockquantity = stockquantity - @quantity 
            where productid = @productid; 
        end 
        else 
        begin 
            print 'insufficient stock'; 
        end 
        
        commit 
    end try 
    begin catch 
        rollback;
        print 'error:' + error_message();
    end catch
end;

--3. Modify views.

alter view activecustomers 
as 
select 
    c.customerid, 
    c.customername, 
    c.email, 
    c.city, 
    c.phonenumber, 
    count(o.orderid) as ordercount 
from customers as c 
left join orders as o on c.customerid = o.customerid 
group by c.customerid, c.customername, c.email, c.city, c.phonenumber 
having count(o.orderid) >= 1;

SELECT * FROM ActiveCustomers;