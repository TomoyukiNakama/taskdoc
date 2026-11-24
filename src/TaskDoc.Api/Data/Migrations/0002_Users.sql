-- Users who may sign in. Only users registered here can log in, including AD users
-- (an AD account that authenticates but has no row here is rejected).
CREATE TABLE Users (
    Id               INTEGER PRIMARY KEY,
    UserName         TEXT    NOT NULL COLLATE NOCASE UNIQUE,  -- AD users: sAMAccountName
    DisplayName      TEXT    NOT NULL,
    Email            TEXT    NULL,
    AuthType         TEXT    NOT NULL CHECK (AuthType IN ('Local', 'Ad')),
    PasswordHash     TEXT    NULL,                            -- Local users only
    IsAdmin          INTEGER NOT NULL DEFAULT 0 CHECK (IsAdmin IN (0, 1)),
    IsActive         INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1)),
    FailedLoginCount INTEGER NOT NULL DEFAULT 0,
    LockoutEndAt     TEXT    NULL,
    LastLoginAt      TEXT    NULL,
    CreatedAt        TEXT    NOT NULL,
    UpdatedAt        TEXT    NOT NULL,
    CHECK ((AuthType = 'Local' AND PasswordHash IS NOT NULL) OR (AuthType = 'Ad' AND PasswordHash IS NULL))
);

-- Refresh tokens are stored as SHA-256 hashes, never in plain text.
CREATE TABLE RefreshTokens (
    Id          INTEGER PRIMARY KEY,
    UserId      INTEGER NOT NULL REFERENCES Users (Id) ON DELETE CASCADE,
    TokenHash   TEXT    NOT NULL UNIQUE,
    CreatedAt   TEXT    NOT NULL,
    ExpiresAt   TEXT    NOT NULL,
    RevokedAt   TEXT    NULL,
    ReplacedById INTEGER NULL REFERENCES RefreshTokens (Id)
);

CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens (UserId);
