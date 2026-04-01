-- SQL скрипт для ручного створення таблиць бази даних CinemaManagementDb
-- Виконайте цей скрипт в pgAdmin або через psql

-- Створення таблиці Persons (базова таблиця для Person)
CREATE TABLE IF NOT EXISTS "Persons" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Age" INTEGER NOT NULL
);

-- Створення таблиці Customers (Table-per-Type наслідування від Person)
CREATE TABLE IF NOT EXISTS "Customers" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Age" INTEGER NOT NULL,
    "Email" VARCHAR(200) NOT NULL
);

-- Створення таблиці Employees (Table-per-Type наслідування від Person)
CREATE TABLE IF NOT EXISTS "Employees" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Age" INTEGER NOT NULL,
    "Position" VARCHAR(100) NOT NULL
);

-- Створення таблиці Films (базова таблиця для Film)
CREATE TABLE IF NOT EXISTS "Films" (
    "Id" UUID PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Genre" VARCHAR(100) NOT NULL,
    "Duration" INTEGER NOT NULL
);

-- Створення таблиці Movies (Table-per-Type наслідування від Film)
CREATE TABLE IF NOT EXISTS "Movies" (
    "Id" UUID PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Genre" VARCHAR(100) NOT NULL,
    "Duration" INTEGER NOT NULL,
    "Director" VARCHAR(200) NOT NULL,
    "Budget" DECIMAL(18,2) NOT NULL
);

-- Створення таблиці Cartoons (Table-per-Type наслідування від Film)
CREATE TABLE IF NOT EXISTS "Cartoons" (
    "Id" UUID PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Genre" VARCHAR(100) NOT NULL,
    "Duration" INTEGER NOT NULL,
    "Studio" VARCHAR(100) NOT NULL,
    "Is3D" BOOLEAN NOT NULL
);

-- Створення таблиці Tickets зі зв'язками один-до-багатьох
CREATE TABLE IF NOT EXISTS "Tickets" (
    "Id" UUID PRIMARY KEY,
    "CustomerId" UUID NOT NULL,
    "FilmId" UUID NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    CONSTRAINT "FK_Tickets_Customers" FOREIGN KEY ("CustomerId") 
        REFERENCES "Customers"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Tickets_Films" FOREIGN KEY ("FilmId") 
        REFERENCES "Films"("Id") ON DELETE CASCADE
);

-- Створення індексів для покращення продуктивності
CREATE INDEX IF NOT EXISTS "IX_Tickets_CustomerId" ON "Tickets"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Tickets_FilmId" ON "Tickets"("FilmId");

-- Коментарі до таблиць
COMMENT ON TABLE "Persons" IS 'Базова таблиця для Person (Table-per-Type)';
COMMENT ON TABLE "Customers" IS 'Таблиця клієнтів, наслідується від Person';
COMMENT ON TABLE "Employees" IS 'Таблиця співробітників, наслідується від Person';
COMMENT ON TABLE "Films" IS 'Базова таблиця для Film (Table-per-Type)';
COMMENT ON TABLE "Movies" IS 'Таблиця фільмів, наслідується від Film';
COMMENT ON TABLE "Cartoons" IS 'Таблиця мультфільмів, наслідується від Film';
COMMENT ON TABLE "Tickets" IS 'Таблиця квитків зі зв''язками один-до-багатьох';

