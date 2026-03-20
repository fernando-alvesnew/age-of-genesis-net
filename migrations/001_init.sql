-- Schema initialization for SQL Server (DDL idempotente)

IF OBJECT_ID('dbo.users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.users (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        name VARCHAR(20) NOT NULL,
        login VARCHAR(15) NOT NULL UNIQUE,
        email VARCHAR(40) NOT NULL UNIQUE,
        user_type VARCHAR(20) NOT NULL CONSTRAINT DF_users_user_type DEFAULT ('player'),
        password VARCHAR(255) NOT NULL,
        last_ip VARCHAR(45) NULL,
        is_banned BIT NOT NULL CONSTRAINT DF_users_is_banned DEFAULT (0),
        created_at DATETIME2 NULL,
        updated_at DATETIME2 NULL,
        CONSTRAINT CK_users_user_type CHECK (user_type IN ('player', 'tutor', 'gm', 'admin'))
    );
END;

IF OBJECT_ID('dbo.pagseguro_credit_card', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.pagseguro_credit_card (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        store_carts_id BIGINT NOT NULL,
        users_id BIGINT NOT NULL,
        payment_id VARCHAR(255) NOT NULL,
        reference_id VARCHAR(255) NOT NULL UNIQUE,
        amount BIGINT NOT NULL,
        status VARCHAR(50) NOT NULL,
        description NVARCHAR(MAX) NULL,
        created_at DATETIME2 NULL,
        updated_at DATETIME2 NULL,
        deleted_at DATETIME2 NULL,
        CONSTRAINT FK_pgc_user FOREIGN KEY (users_id) REFERENCES dbo.users(id)
    );

    CREATE INDEX idx_pgc_users_id ON dbo.pagseguro_credit_card(users_id);
END;
