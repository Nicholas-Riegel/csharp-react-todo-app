-- POSTGRES SQL COMMANDS
-- 1. Create the database
CREATE DATABASE csharp_webapi_01;

-- 2. Connect to the database (using psql command line tool)
\c csharp_webapi_01

-- 3. Create the todos table
CREATE TABLE todos (
    id SERIAL PRIMARY KEY,
    description VARCHAR(255) NOT NULL,
    completed BOOLEAN NOT NULL DEFAULT FALSE
);
