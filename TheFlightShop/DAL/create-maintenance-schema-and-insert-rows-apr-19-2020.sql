/** CREATE TABLE */

CREATE TABLE `maintenanceItems` (
	`Id` char(38) NOT NULL,
    `Name` varchar(100) NOT NULL,
    `Description` varchar(255) NOT NULL,
    `Controller` varchar(100) NOT NULL,
    `Action` varchar(100) NOT NULL,
    `ImageFilename` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
    `Keywords` varchar(255) DEFAULT NULL,
    `IsActive` bit(1) NOT NULL,
    PRIMARY KEY (`Id`)
); 

/** INSERT ROWS INTO TABLE */

insert into maintenanceItems 
values (
	'eb016a74-c33c-4c7e-b663-1ce8ec4c6f71',
    'Cabin & Baggage Door Struts',
    'STC\'d for all Aerostar models, approx. 1 hour to install.',
    'Home',
    'CabinBaggageDoorStruts',
    'door-strut.jpg',
    '600,601P,602P',
    1
);

insert into maintenanceItems 
values (
	'67d29e64-590b-4828-9a67-ac4c8b5abe2e',
    'Cabin Door Cables',
    'Adjustable clevis, silicon casing.',
    'Home',
    'CabinDoorCables',
    'door-cable.jpg',
    'clevis,silicon',
    1
);

insert into maintenanceItems 
values (
	'3aac9a5c-deaf-4e15-bed0-97c585bf8e43',
    'Services',
    'See some of our experience and services.',
    'Home',
    'Services',
    'aerostar.jpg',
    'aerostar,ice,shield,powerpac,spoilers,gamijectors,rocky,mountain,propellers,insight,jp,instruments,great,lakes,plastech',
    1
);

insert into maintenanceItems 
values (
	'f1aaa655-61f5-41c4-bd4c-be5a0f94cb8b',
    'Modifications',
    'Various modifications for performance and cosmetics.',
    'Home',
    'Modifications',
    'power-pac-spoilers.jpg',
    'machen,aerostar,pulse,plastech,insight,powerpac,deice,de-ice,de ice,engine',
    1
);

insert into maintenanceItems 
values (
	'e2450bb1-80ac-4958-b4ef-a8dda2ce07d0',
    'Window Shades',
    'Plastech shades, can paint to match interior. Approx. 40-60 hours to install.',
    'Home',
    'WindowShades',
    'window-shades.jpg',
    'plastech,interior',
    1
);