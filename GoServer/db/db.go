package db

import (
    "database/sql"
    "fmt"
)

func OpenDB(connStr string) (*sql.DB, error) {
    db, err := sql.Open("postgres", connStr)
    if err != nil {
        return nil, err
    }
    // Проверка соединения
    if err := db.Ping(); err != nil {
        return nil, err
    }
    fmt.Println("Connected to Postgres!")
    return db, nil
}
