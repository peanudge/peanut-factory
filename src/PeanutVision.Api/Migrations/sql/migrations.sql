CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "CapturedImages" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_CapturedImages" PRIMARY KEY,
    "FilePath" TEXT NOT NULL,
    "Width" INTEGER NOT NULL,
    "Height" INTEGER NOT NULL,
    "FileSizeBytes" INTEGER NOT NULL,
    "Format" TEXT NOT NULL,
    "CapturedAt" TEXT NOT NULL
);

CREATE INDEX "IX_CapturedImages_CapturedAt" ON "CapturedImages" ("CapturedAt");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260607130917_InitialCreate', '10.0.3');

COMMIT;

