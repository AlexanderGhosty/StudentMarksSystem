package db

import (
	"database/sql"
	"fmt"
)

// OpenDB initializes a connection to a PostgreSQL database using the provided connection string.
// It returns a *sql.DB instance upon successful connection or an error if the connection fails.
// The function also verifies the connection by pinging the database before returning.
func OpenDB(connStr string) (*sql.DB, error) {
	db, err := sql.Open("postgres", connStr)
	if err != nil {
		return nil, err
	}

	if err := db.Ping(); err != nil {
		return nil, err
	}
	fmt.Println("Connected to Postgres!")
	return db, nil
}
