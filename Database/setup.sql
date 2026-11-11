	IF DB_ID('two_plus_two_s5c2') IS NULL
    CREATE DATABASE two_plus_two_s5c2;
GO

USE two_plus_two_s5c2;
GO

IF OBJECT_ID('patient_info', 'U') IS NULL
BEGIN
    CREATE TABLE patient_info (
        PatientID                   INT IDENTITY(1001,1) PRIMARY KEY,
        FirstName                   NVARCHAR(50)  NOT NULL,
        MiddleName                  NVARCHAR(50)  NULL,
        Surname                     NVARCHAR(50)  NOT NULL,
        age                         INT           NULL,
        Birthdate                   DATE          NULL,
        gender                      NVARCHAR(10)  NULL,
        Address                     NVARCHAR(200) NULL,
        Emergencycontactinformation NVARCHAR(200) NULL,
        phonenumber                 NVARCHAR(20)  NULL,
        civilstatus                 NVARCHAR(20)  NULL,
        PatientImage                VARBINARY(MAX) NULL
    );

    INSERT INTO patient_info
        (FirstName, MiddleName, Surname, age, Birthdate, gender, Address,
         Emergencycontactinformation, phonenumber, civilstatus)
    VALUES
    ('Rene',    'Santos', 'Baterbonia', 67, '1959-03-14', 'Male',
     'Blk 5 Lot 12, Brgy. San Isidro, Taguig City',
     'Maria Baterbonia - Asawa - 09171234501', '9171234567', 'Married'),
    ('Queen',   'Reyes',  'Yasmin',     45, '1981-07-02', 'Female',
     'Purok 3, Brgy. Central Signal, Taguig City',
     'Ali Yasmin - Kapatid - 09181234502', '9181234568', 'Single'),
    ('Dogie',   'Cruz',   'Dela Rosa',  32, '1994-05-09', 'Male',
     '24 Mabini St., Brgy. Ususan, Taguig City',
     'Linda Dela Rosa - Ina - 09191234503', '9191234569', 'Single'),
    ('Renejay', 'Garcia', 'Mendoza',    19, '2007-02-18', 'Male',
     'Phase 2, Brgy. Pinagsama, Taguig City',
     'Rosa Mendoza - Ina - 09201234504', '9201234570', 'Single');
END
GO

IF OBJECT_ID('Appointments', 'U') IS NULL
BEGIN
    CREATE TABLE Appointments (
        AppointmentID   INT IDENTITY(1001,1) PRIMARY KEY,
        AppointmentDate DATETIME      NOT NULL,
        PatientName     NVARCHAR(100) NOT NULL,
        Service         NVARCHAR(100) NOT NULL,
        Status          NVARCHAR(20)  NOT NULL DEFAULT 'Pending'
    );

    INSERT INTO Appointments (AppointmentDate, PatientName, Service, Status) VALUES
    ('2026-11-12 09:00', 'Rene Baterbonia',  'General Check-up',  'Confirmed'),
    ('2026-11-12 10:30', 'Queen Yasmin',     'Prenatal Check-up', 'Pending'),
    ('2026-11-13 08:30', 'Dogie Dela Rosa',  'Blood Pressure',    'Confirmed'),
    ('2026-11-13 13:00', 'Renejay Mendoza',  'Immunization',      'Pending'),
    ('2026-11-14 09:30', 'Rene Baterbonia',  'Follow-up',         'Cancelled'),
    ('2026-11-15 11:00', 'Queen Yasmin',     'Dental Check-up',   'Confirmed'),
    ('2026-11-16 14:00', 'Dogie Dela Rosa',  'General Check-up',  'Pending'),
    ('2026-11-17 10:00', 'Renejay Mendoza',  'Consultation',      'Cancelled');
END
GO

IF OBJECT_ID('Reports', 'U') IS NULL
BEGIN
    CREATE TABLE Reports (
        ReportID      INT IDENTITY(1001,1) PRIMARY KEY,
        ReportType    NVARCHAR(50)  NOT NULL,
        Description   NVARCHAR(200) NULL,
        PeriodFrom    DATE NOT NULL,
        PeriodTo      DATE NOT NULL,
        Status        NVARCHAR(20)  DEFAULT 'Generated',
        DateGenerated DATETIME      DEFAULT GETDATE()
    );

    INSERT INTO Reports (ReportType, Description, PeriodFrom, PeriodTo, Status) VALUES
    ('Patient Report',     'Total of 4 registered patients',        '2026-10-01', '2026-10-31', 'Completed'),
    ('Appointment Report', '25 appointments scheduled this month',  '2026-10-01', '2026-10-31', 'Completed'),
    ('Staff Report',       '6 active staff members',                '2026-10-01', '2026-10-31', 'Completed'),
    ('Patient Report',     'Patients by gender: 2 Male, 2 Female',  '2026-11-01', '2026-11-10', 'Generated'),
    ('Appointment Report', '8 appointments so far this month',      '2026-11-01', '2026-11-10', 'Pending'),
    ('Staff Report',       'Staff attendance summary',              '2026-11-01', '2026-11-10', 'Pending');
END
GO

SELECT 'patient_info' AS TableName, COUNT(*) AS Rows FROM patient_info
UNION ALL SELECT 'Appointments', COUNT(*) FROM Appointments
UNION ALL SELECT 'Reports', COUNT(*) FROM Reports;
GO
