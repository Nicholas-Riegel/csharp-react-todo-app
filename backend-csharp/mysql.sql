CREATE DATABASE csharp_webapi_01;

USE csharp_webapi_01;

CREATE TABLE todos (
    id INT NOT NULL AUTO_INCREMENT,
    description VARCHAR(255) NOT NULL,
    completed BOOLEAN NOT NULL DEFAULT 0,
    PRIMARY KEY (id)
);
