-- Baseline migration. Real tables (users, projects, tasks, documents, ...) are added
-- as numbered scripts (0002_..., 0003_...) once each part of docs/spec.md is designed.
-- Never edit a script after it has been applied; add a new one instead.

CREATE TABLE IF NOT EXISTS AppInfo (
    Key   TEXT NOT NULL PRIMARY KEY,
    Value TEXT NOT NULL
);

INSERT OR IGNORE INTO AppInfo (Key, Value) VALUES ('SchemaBaseline', '0001');
