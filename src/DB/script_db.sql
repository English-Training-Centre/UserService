/*
powered by  : (c) 2025, Ramadan Ismael - All rights reserved!!
to          : ETC - User Service
*/

-- sudo -u postgres psql
-- CREATE DATABASE etc_db_user_service;
-- GRANT ALL PRIVILEGES ON DATABASE etc_db_user_service TO ramadan;

CREATE EXTENSION IF NOT EXISTS pgcrypto; -- auto uuid


-- ROLES
CREATE TABLE IF NOT EXISTS tbRoles
(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(25) UNIQUE NOT NULL
);
-- ROLES

-- USERS
CREATE TABLE IF NOT EXISTS tbUsers
(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    fullname VARCHAR(50) UNIQUE NOT NULL,
    username VARCHAR(25) UNIQUE NOT NULL,
    email VARCHAR(25) NULL,
    phone_number VARCHAR(25) NULL,    
    role_id UUID NOT NULL REFERENCES tbRoles(id) ON DELETE RESTRICT,
    password_hash VARCHAR(255) NOT NULL,
    image_url VARCHAR(255) NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL
);
-- USERS

-- QUERIES
DROP TABLE tbUsers;

SELECT * FROM tbRoles;
SELECT * FROM tbUsers;

INSERT INTO tbRoles(name) VALUES('admin');
INSERT INTO tbUsers(fullname, username, role_id, password_hash) VALUES('Ramadan Ibraimo', 'ramadan', 'a46e78f1-83f8-42a0-9016-1d94b1a63760', '123456');

SELECT
    u.id,
    u.fullname,
    u.username,
    u.email,
    u.phone_number AS PhoneNumber,
    r.name AS Role,
    u.image_url AS ImageUrl,
    u.is_active AS IsActive,
    u.created_at AS CreatedAt,
    u.updated_at AS UpdatedAt
FROM tbUsers u
INNER JOIN tbRoles r ON r.id = u.role_id
ORDER BY u.fullname ASC;
-- QUERIES