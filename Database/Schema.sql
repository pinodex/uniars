CREATE TABLE `airlines` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `alias` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `iata` varchar(2) COLLATE utf8_unicode_ci DEFAULT NULL,
  `icao` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `callsign` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `country` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `is_active` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19849 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `airports` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `city` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `country` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `code` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `icao` varchar(4) COLLATE utf8_unicode_ci DEFAULT NULL,
  `latitude` double DEFAULT NULL,
  `longitude` double DEFAULT NULL,
  `altitude` double DEFAULT NULL,
  `tzoffset` double DEFAULT NULL,
  `dst` char(1) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timezone` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9547 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `book_passengers` (
  `book_id` int(11) NOT NULL,
  `passenger_id` int(11) NOT NULL,
  PRIMARY KEY (`passenger_id`,`book_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `books` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `flight_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `countries` (
  `id` char(2) CHARACTER SET utf8 NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `flights` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `code` varchar(45) CHARACTER SET utf8 DEFAULT NULL,
  `airline_id` int(11) NOT NULL,
  `source_id` int(11) NOT NULL,
  `destination_id` int(11) NOT NULL,
  `departure_date` datetime NOT NULL,
  `return_date` datetime DEFAULT NULL,
  `type` tinyint(1) NOT NULL DEFAULT '2',
  `has_departed` tinyint(4) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `passenger_contacts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `passenger_id` int(11) DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `line_1` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `line_2` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `line_3` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `city` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `state` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `province` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `country_id` char(2) CHARACTER SET utf8 DEFAULT NULL,
  `postal_code` char(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `phone_number` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  `email_address` varchar(320) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `passengers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `code` varchar(45) CHARACTER SET utf8 NOT NULL,
  `prefix` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  `given_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `family_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `middle_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `suffix` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  `display_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `gender` int(1) NOT NULL DEFAULT '9',
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `public_id_UNIQUE` (`code`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `users` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `username` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `password` varchar(60) COLLATE utf8_unicode_ci NOT NULL,
  `role` varchar(45) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
