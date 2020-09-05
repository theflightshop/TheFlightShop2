# todo: need to offload existing prod orders w/ int'l addresses into CSV or something?

alter table orders
add column ConfirmationNumber varchar(25) default null;

alter table contacts 
drop column InternationalBillingAddress;

alter table contacts
drop column InternationalShippingAddress;

alter table contacts
add column CountryCode char(2) default null;

alter table contacts
add  column BillingCountryCode char(2) default null;