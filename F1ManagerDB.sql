   -- =====================================================================
    -- 1. CRIAÇÃO DA BASE DE DADOS E CONFIGURAÇÃO DO SISTEMA
    -- =====================================================================
    CREATE DATABASE IF NOT EXISTS F1ManagerDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
    USE F1ManagerDB;

    -- =====================================================================
    -- 2. CRIAÇÃO DO UTILIZADOR DO SISTEMA E ATRIBUIÇÃO DE PRIVILÉGIOS
    -- =====================================================================
    -- Cria o utilizador administrador local do MySQL para a aplicação (se não existir)
    CREATE USER IF NOT EXISTS 'f1admin'@'localhost' IDENTIFIED BY '1234';

    -- Atribui todas as permissões sobre a BD F1ManagerDB a este utilizador
    GRANT ALL PRIVILEGES ON F1ManagerDB.* TO 'f1admin'@'localhost';

    -- Aplica as alterações de privilégios imediatamente
    FLUSH PRIVILEGES;

    -- =====================================================================
    -- 3. CRIAÇÃO DAS TABELAS DE AUTENTICAÇÃO (ASP.NET CORE IDENTITY)
    -- =====================================================================

    CREATE TABLE IF NOT EXISTS AspNetRoles (
        Id VARCHAR(255) NOT NULL,
        Name VARCHAR(256) NULL,
        NormalizedName VARCHAR(256) NULL,
        ConcurrencyStamp LONGTEXT NULL,
        PRIMARY KEY (Id),
        UNIQUE KEY RoleNameIndex (NormalizedName)
    ) ENGINE=InnoDB;

    CREATE TABLE IF NOT EXISTS AspNetUsers (
        Id VARCHAR(255) NOT NULL,
        UserName VARCHAR(256) NULL,
        NormalizedUserName VARCHAR(256) NULL,
        Email VARCHAR(256) NULL,
        NormalizedEmail VARCHAR(256) NULL,
        EmailConfirmed TINYINT(1) NOT NULL,
        PasswordHash LONGTEXT NULL,
        SecurityStamp LONGTEXT NULL,
        ConcurrencyStamp LONGTEXT NULL,
        PhoneNumber LONGTEXT NULL,
        PhoneNumberConfirmed TINYINT(1) NOT NULL,
        TwoFactorEnabled TINYINT(1) NOT NULL,
        LockoutEnd DATETIME(6) NULL,
        LockoutEnabled TINYINT(1) NOT NULL,
        AccessFailedCount INT NOT NULL,
        PRIMARY KEY (Id),
        UNIQUE KEY UserNameIndex (NormalizedUserName),
        KEY EmailIndex (NormalizedEmail)
    ) ENGINE=InnoDB;

    CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
        Id INT AUTO_INCREMENT NOT NULL,
        RoleId VARCHAR(255) NOT NULL,
        ClaimType LONGTEXT NULL,
        ClaimValue LONGTEXT NULL,
        PRIMARY KEY (Id),
        CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    CREATE TABLE IF NOT EXISTS AspNetUserClaims (
        Id INT AUTO_INCREMENT NOT NULL,
        UserId VARCHAR(255) NOT NULL,
        ClaimType LONGTEXT NULL,
        ClaimValue LONGTEXT NULL,
        PRIMARY KEY (Id),
        CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    CREATE TABLE IF NOT EXISTS AspNetUserLogins (
        LoginProvider VARCHAR(128) NOT NULL,
        ProviderKey VARCHAR(128) NOT NULL,
        ProviderDisplayName LONGTEXT NULL,
        UserId VARCHAR(255) NOT NULL,
        PRIMARY KEY (LoginProvider, ProviderKey),
        CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    CREATE TABLE IF NOT EXISTS AspNetUserRoles (
        UserId VARCHAR(255) NOT NULL,
        RoleId VARCHAR(255) NOT NULL,
        PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE,
        CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    CREATE TABLE IF NOT EXISTS AspNetUserTokens (
        UserId VARCHAR(255) NOT NULL,
        LoginProvider VARCHAR(128) NOT NULL,
        Name VARCHAR(128) NOT NULL,
        Value LONGTEXT NULL,
        PRIMARY KEY (UserId, LoginProvider, Name),
        CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    -- =====================================================================
    -- 4. CRIAÇÃO DAS TABELAS DE DOMÍNIO DESPORTIVO (APLICAÇÃO)
    -- =====================================================================

    -- Tabela de Campeonatos
    CREATE TABLE IF NOT EXISTS Campeonatos (
        Id INT AUTO_INCREMENT NOT NULL,
        Nome VARCHAR(100) NOT NULL,
        Ano INT NOT NULL,
        PRIMARY KEY (Id)
    ) ENGINE=InnoDB;

    -- Tabela de Equipas
    CREATE TABLE IF NOT EXISTS Equipas (
        Id INT AUTO_INCREMENT NOT NULL,
        Nome VARCHAR(50) NOT NULL,
        FabricanteMotor LONGTEXT NOT NULL,
        Pais LONGTEXT NOT NULL,
        ChefeEquipa VARCHAR(100) NULL,
        AnoFundacao INT NULL,
        Historia VARCHAR(1000) NULL,
        Logotipo LONGTEXT NULL,
        PRIMARY KEY (Id)
    ) ENGINE=InnoDB;

    -- Tabela de Pilotos (Relacionamento 1-N com Equipas)
    CREATE TABLE IF NOT EXISTS Pilotos (
        Id INT AUTO_INCREMENT NOT NULL,
        Nome LONGTEXT NOT NULL,
        NumeroCarro INT NOT NULL,
        UserId LONGTEXT NULL,
        Biografia VARCHAR(500) NULL,
        Peso DOUBLE NULL,
        FotoPerfil LONGTEXT NULL,
        EquipaId INT NOT NULL,
        PRIMARY KEY (Id),
        CONSTRAINT FK_Pilotos_Equipas_EquipaId FOREIGN KEY (EquipaId) REFERENCES Equipas (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    -- Tabela de Corridas (Relacionamento 1-N com Campeonatos)
    CREATE TABLE IF NOT EXISTS Corridas (
        Id INT AUTO_INCREMENT NOT NULL,
        NomeGrandePremio LONGTEXT NOT NULL,
        Circuito LONGTEXT NOT NULL,
        DataHora DATETIME(6) NOT NULL,
        CampeonatoId INT NOT NULL,
        PRIMARY KEY (Id),
        CONSTRAINT FK_Corridas_Campeonatos_CampeonatoId FOREIGN KEY (CampeonatoId) REFERENCES Campeonatos (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    -- Tabela Associativa: Resultados de Corridas (Relacionamento Muitos-para-Muitos com chave composta)
    CREATE TABLE IF NOT EXISTS ResultadosCorridas (
        PilotoId INT NOT NULL,
        CorridaId INT NOT NULL,
        PosicaoFinal INT NOT NULL,
        Pontos INT NOT NULL,
        TempoVoltaRapida LONGTEXT NOT NULL,
        PRIMARY KEY (PilotoId, CorridaId),
        CONSTRAINT FK_ResultadosCorridas_Corridas_CorridaId FOREIGN KEY (CorridaId) REFERENCES Corridas (Id) ON DELETE CASCADE,
        CONSTRAINT FK_ResultadosCorridas_Pilotos_PilotoId FOREIGN KEY (PilotoId) REFERENCES Pilotos (Id) ON DELETE CASCADE
    ) ENGINE=InnoDB;

    -- =====================================================================
    -- 5. SEED DATA (POVOAMENTO DE DADOS OFICIAIS)
    -- =====================================================================

    -- Inserção de Campeonatos
    INSERT INTO Campeonatos (Id, Nome, Ano) VALUES (1, 'Fórmula 1 - 2026', 2026)
    ON DUPLICATE KEY UPDATE Nome=VALUES(Nome);

    -- Inserção de Equipas
    INSERT INTO Equipas (Id, Nome, FabricanteMotor, Pais) VALUES
    (1, 'Scuderia Ferrari', 'Ferrari', 'Itália'),
    (2, 'Oracle Red Bull Racing', 'Red Bull Ford', 'Áustria'),
    (3, 'Mercedes-AMG Petronas F1 Team', 'Mercedes', 'Alemanha'),
    (4, 'McLaren Mastercard F1 Team', 'Mercedes', 'Reino Unido'),
    (5, 'Aston Martin Aramco F1 Team', 'Honda', 'Reino Unido'),
    (6, 'BWT Alpine F1 Team', 'Mercedes', 'França'),
    (7, 'Atlassian Williams F1 Team', 'Mercedes', 'Reino Unido'),
    (8, 'Visa Cash App Racing Bulls F1 Team', 'Red Bull Ford', 'Itália'),
    (9, 'MoneyGram Haas F1 Team', 'Ferrari', 'Estados Unidos'),
    (10, 'Audi F1 Team', 'Audi', 'Alemanha'),
    (11, 'Cadillac Formula 1 Team', 'Ferrari', 'Estados Unidos')
    ON DUPLICATE KEY UPDATE Nome=VALUES(Nome);

    -- Inserção de Pilotos
    INSERT INTO Pilotos (Id, Nome, NumeroCarro, EquipaId) VALUES
    (1, 'Charles Leclerc', 16, 1),
    (2, 'Lewis Hamilton', 44, 1),
    (3, 'Max Verstappen', 3, 2),
    (4, 'Isack Hadjar', 6, 2),
    (5, 'George Russell', 63, 3),
    (6, 'Andrea Kimi Antonelli', 12, 3),
    (7, 'Lando Norris', 1, 4),
    (8, 'Oscar Piastri', 81, 4),
    (9, 'Fernando Alonso', 14, 5),
    (10, 'Lance Stroll', 18, 5),
    (11, 'Pierre Gasly', 10, 6),
    (12, 'Franco Colapinto', 43, 6),
    (13, 'Alexander Albon', 23, 7),
    (14, 'Carlos Sainz Jr.', 55, 7),
    (15, 'Liam Lawson', 30, 8),
    (16, 'Arvid Lindblad', 41, 8),
    (17, 'Esteban Ocon', 31, 9),
    (18, 'Oliver Bearman', 87, 9),
    (19, 'Nico Hülkenberg', 27, 10),
    (20, 'Gabriel Bortoleto', 5, 10),
    (21, 'Sergio Pérez', 11, 11),
    (22, 'Valtteri Bottas', 77, 11)
    ON DUPLICATE KEY UPDATE Nome=VALUES(Nome);

    -- Inserção de Corridas (Calendário 2026)
    INSERT INTO Corridas (Id, NomeGrandePremio, Circuito, DataHora, CampeonatoId) VALUES
    (1, 'GP da Austrália', 'Albert Park Circuit', '2026-03-15 06:00:00', 1),
    (2, 'GP da China', 'Circuito Internacional de Xangai', '2026-03-22 07:00:00', 1),
    (3, 'GP do Japão', 'Circuito de Suzuka', '2026-04-05 06:00:00', 1),
    (4, 'GP do Bahrein', 'Circuito Internacional de Sakhir', '2026-04-18 16:00:00', 1),
    (5, 'GP da Arábia Saudita', 'Circuito de Rua de Jeddah', '2026-04-25 18:00:00', 1),
    (6, 'GP de Miami', 'Autódromo Internacional de Miami', '2026-05-03 20:30:00', 1),
    (7, 'GP da Emília-Romanha', 'Autodromo Enzo e Dino Ferrari (Imola)', '2026-05-17 14:00:00', 1),
    (8, 'GP de Mónaco', 'Circuito do Mónaco', '2026-05-24 14:00:00', 1),
    (9, 'GP de Espanha', 'Circuito de Barcelona-Catalunha', '2026-05-31 14:00:00', 1),
    (10, 'GP do Canadá', 'Circuito Gilles Villeneuve', '2026-06-14 19:00:00', 1),
    (11, 'GP da Áustria', 'Red Bull Ring', '2026-06-28 14:00:00', 1),
    (12, 'GP da Grã-Bretanha', 'Circuito de Silverstone', '2026-07-05 15:00:00', 1),
    (13, 'GP da Bélgica', 'Circuito de Spa-Francorchamps', '2026-07-26 14:00:00', 1),
    (14, 'GP da Hungria', 'Hungaroring', '2026-08-02 14:00:00', 1),
    (15, 'GP dos Países Baixos', 'Circuito de Zandvoort', '2026-08-30 14:00:00', 1),
    (16, 'GP de Itália', 'Autodromo Nazionale Monza', '2026-09-06 14:00:00', 1),
    (17, 'GP do Azerbaijão', 'Circuito de Rua de Baku', '2026-09-20 12:00:00', 1),
    (18, 'GP de Singapura', 'Circuito de Rua de Marina Bay', '2026-10-04 13:00:00', 1),
    (19, 'GP dos Estados Unidos', 'Circuito das Américas (COTA)', '2026-10-18 20:00:00', 1),
    (20, 'GP do México', 'Autódromo Hermanos Rodríguez', '2026-10-25 20:00:00', 1),
    (21, 'GP de São Paulo', 'Autódromo de Interlagos', '2026-11-08 17:00:00', 1),
    (22, 'GP de Las Vegas', 'Circuito de Rua de Las Vegas', '2026-11-21 22:00:00', 1),
    (23, 'GP do Qatar', 'Circuito Internacional de Lusail', '2026-11-29 17:00:00', 1),
    (24, 'GP de Abu Dhabi', 'Circuito de Yas Marina', '2026-12-06 13:00:00', 1)
    ON DUPLICATE KEY UPDATE NomeGrandePremio=VALUES(NomeGrandePremio);