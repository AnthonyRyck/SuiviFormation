USE formationdb;

/******************************
* Création des tables ASP.Net *
*******************************/ 
CREATE TABLE aspnetusers (
  Id varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  UserName varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  NormalizedUserName varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  Email varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  NormalizedEmail varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  EmailConfirmed tinyint(1) NOT NULL,
  PasswordHash longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  SecurityStamp longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  ConcurrencyStamp longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PhoneNumber longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PhoneNumberConfirmed tinyint(1) NOT NULL,
  TwoFactorEnabled tinyint(1) NOT NULL,
  LockoutEnd datetime(6) DEFAULT NULL,
  LockoutEnabled tinyint(1) NOT NULL,
  AccessFailedCount int NOT NULL,
  PRIMARY KEY (Id),
  UNIQUE KEY UserNameIndex (NormalizedUserName),
  KEY EmailIndex (NormalizedEmail)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE aspnetusertokens (
  UserId varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  LoginProvider varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  Name varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  Value longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (UserId,LoginProvider,Name),
  CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES aspnetusers (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE aspnetuserlogins (
  LoginProvider varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  ProviderKey varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  ProviderDisplayName longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  UserId varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (LoginProvider,ProviderKey),
  KEY IX_AspNetUserLogins_UserId (UserId),
  CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES aspnetusers (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE aspnetuserclaims (
  Id int NOT NULL AUTO_INCREMENT,
  UserId varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  ClaimType longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  ClaimValue longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (Id),
  KEY IX_AspNetUserClaims_UserId (UserId),
  CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES aspnetusers (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE aspnetroles (
  Id varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  Name varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  NormalizedName varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  ConcurrencyStamp longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (Id),
  UNIQUE KEY RoleNameIndex (NormalizedName)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE aspnetroleclaims (
  Id int NOT NULL AUTO_INCREMENT,
  RoleId varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  ClaimType longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  ClaimValue longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (Id),
  KEY IX_AspNetRoleClaims_RoleId (RoleId),
  CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES aspnetroles (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE aspnetuserroles (
  UserId varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  RoleId varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (UserId,RoleId),
  KEY IX_AspNetUserRoles_RoleId (RoleId),
  CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES aspnetroles (Id) ON DELETE CASCADE,
  CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES aspnetusers (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/******************************
* Ajout des rôles             *
*******************************/ 
INSERT INTO aspnetroles   
(Id, Name, NormalizedName, ConcurrencyStamp)
VALUES
(uuid(), 'Administrateur', 'ADMINISTRATEUR', uuid()),
(uuid(), 'Gestionnaire', 'GESTIONNAIRE', uuid());

COMMIT;



/******************************
* Création des tables métiers *
*******************************/ 

CREATE TABLE Salle
(IdSalle int NOT NULL AUTO_INCREMENT,
NomSalle VARCHAR(50) NOT NULL, 
Description VARCHAR(250) NOT NULL,
NombrePlace int NOT NULL,
PRIMARY KEY(IdSalle));

CREATE TABLE Personnel
(IdPersonnel VARCHAR(255) NOT NULL,
Nom VARCHAR(32) NOT NULL,
Prenom VARCHAR(32) NOT NULL,
Service VARCHAR(32) NOT NULL,
IsActif BIT(1) NOT NULL,
Login VARCHAR(32) NOT NULL,
IsExterne BIT(1) NOT NULL,
PRIMARY KEY(IdPersonnel));

CREATE TABLE TypeFormation
(IdTypeFormation int NOT NULL AUTO_INCREMENT,
TitreType VARCHAR(50) NOT NULL,
PRIMARY KEY(IdTypeFormation));

INSERT INTO TypeFormation (TitreType)
VALUES("Présentation"),
("Travaux réflexion"),
("Travaux pratique");

CREATE TABLE CatalogueFormation
(IdFormation int NOT NULL AUTO_INCREMENT,
Titre VARCHAR(50) NOT NULL,
DescriptionFormation LONGTEXT NOT NULL,
DateDeFin DATE,
FichierContenu LONGBLOB,
NomDeFichier VARCHAR(45),
EstInterne BIT(1) NOT NULL,
Duree Double(5,2) NOT NULL,
IdTypeFormation INT NOT NULL,
FOREIGN KEY(IdTypeFormation) REFERENCES TypeFormation(IdTypeFormation),
PRIMARY KEY(IdFormation));

CREATE TABLE Formateur
(IdPersonnel VARCHAR(45) NOT NULL,
IdFormation int NOT NULL,
EstEncoreFormateur BIT(1) NOT NULL,
PRIMARY KEY(IdPersonnel, IdFormation),
FOREIGN KEY(IdPersonnel) REFERENCES Personnel(IdPersonnel),
FOREIGN KEY(IdFormation) REFERENCES CatalogueFormation(IdFormation));

CREATE TABLE SessionFormation
(IdSession int NOT NULL AUTO_INCREMENT,
IdFormateur VARCHAR(255) NOT NULL,
IdFormation int NOT NULL,
IdSalle int NOT NULL,
DateSession DATE NOT NULL,
PlaceDispo INT NOT NULL,
Emargement LONGBLOB NULL,
FileName VARCHAR(45) NULL,
IsArchive BIT(1) NOT NULL,
PRIMARY KEY(IdSession),
FOREIGN KEY(IdFormateur) REFERENCES Formateur(IdPersonnel),
FOREIGN KEY(IdFormation) REFERENCES CatalogueFormation(IdFormation),
FOREIGN KEY(IdSalle) REFERENCES Salle(IdSalle));

CREATE TABLE InscriptionFormation
(IdSession int NOT NULL,
IdPersonnel VARCHAR(255) NOT NULL,
IsSessionValidate BIT(1) NOT NULL,
Note int NULL,
Commentaire VARCHAR(255) NULL,
PRIMARY KEY (IdSession, IdPersonnel),
FOREIGN KEY(IdSession) REFERENCES SessionFormation(IdSession),
FOREIGN KEY(IdPersonnel) REFERENCES Personnel(IdPersonnel));

CREATE TABLE Competences
(IdCompetence int NOT NULL AUTO_INCREMENT,
Titre VARCHAR(50) NOT NULL,
DescriptionCompetence LONGTEXT NOT NULL,
PRIMARY KEY(IdCompetence));

CREATE TABLE CompetenceFormation
(IdCompetence int NOT NULL,
IdFormation int NOT NULL,
PRIMARY KEY (IdCompetence, IdFormation),
FOREIGN KEY(IdCompetence) REFERENCES Competences(IdCompetence),
FOREIGN KEY(IdFormation) REFERENCES CatalogueFormation(IdFormation));

CREATE TABLE Same
(IdSame int NOT NULL AUTO_INCREMENT,
Nom VARCHAR(50) NOT NULL,
DescriptionSame LONGTEXT NOT NULL,
PRIMARY KEY(IdSame));

INSERT INTO same (Nom, DescriptionSame)
VALUES ('Sensibilisation', 'Sensibilisé pour cette compétence'),
('Application', 'La personne applique cette compétence'),
('Maitrise', 'Maitrise cette compétence'),
('Expert', 'Maitrise et peut aussi former sur cette compétence');

CREATE TABLE SuiviSame
(IdPersonnel VARCHAR(255) NOT NULL,
IdCompetence int NOT NULL,
IdSame int NOT NULL,
DateObtention DATE,
PRIMARY KEY (IdPersonnel, IdCompetence, IdSame),
FOREIGN KEY(IdPersonnel) REFERENCES Personnel(IdPersonnel),
FOREIGN KEY(IdCompetence) REFERENCES Competences(IdCompetence),
FOREIGN KEY(IdSame) REFERENCES Same(IdSame));