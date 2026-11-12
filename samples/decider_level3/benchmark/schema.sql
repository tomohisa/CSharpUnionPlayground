CREATE TABLE IF NOT EXISTS events (
    position INTEGER PRIMARY KEY,
    command_id TEXT NOT NULL,
    event_type TEXT NOT NULL,
    payload_json TEXT NOT NULL,
    tags_json TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS event_tags (
    position INTEGER NOT NULL REFERENCES events(position),
    category TEXT NOT NULL,
    keys_key BLOB NOT NULL,
    PRIMARY KEY (position, category, keys_key)
);
CREATE INDEX IF NOT EXISTS tags_match ON event_tags(category, keys_key, position);
CREATE INDEX IF NOT EXISTS events_type_position ON events(event_type, position);
