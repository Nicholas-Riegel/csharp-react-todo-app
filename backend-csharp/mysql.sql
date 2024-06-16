CREATE DATABASE csharp_webapi_01;

USE csharp_webapi_01;

CREATE TABLE todos (
    id INT NOT NULL AUTO_INCREMENT,
    todo_text varchar(255) NOT NULL,
    PRIMARY KEY (`id`)
) 