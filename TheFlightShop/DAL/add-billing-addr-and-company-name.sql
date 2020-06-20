alter table contacts
add BillingAddress1 varchar(255) null;

alter table contacts
add BillingAddress2 varchar(100) null;

alter table contacts
add BillingCity varchar(100) null;

alter table contacts
add BillingState varchar(55) null;

alter table contacts
add BillingZip varchar(15) null;

alter table contacts
add CompanyName varchar(100) null;

alter table orders
add AttentionTo varchar(100) null;

alter table orders 
add CustomShippingType varchar(55) null;