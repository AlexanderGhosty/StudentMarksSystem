package db

import (
	"GoServer/internal/models"
	"database/sql"
	"fmt"
	"strings"
)

// GetUsers retrieves all users from the Users table in the database and returns a slice of User models or an error.
func GetUsers(db *sql.DB) ([]models.User, error) {
	rows, err := db.Query(`SELECT id, name, login, role FROM Users`)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var users []models.User
	for rows.Next() {
		var u models.User
		if err := rows.Scan(&u.ID, &u.Name, &u.Login, &u.Role); err != nil {
			return nil, err
		}
		users = append(users, u)
	}
	return users, nil
}

// CreateUser inserts a new user record into the database and returns the generated user ID or an error if the operation fails.
func CreateUser(db *sql.DB, user models.User, passwordHash string) (int, error) {
	var newId int
	err := db.QueryRow(`INSERT INTO Users(name, login, password_hash, role)
                        VALUES($1,$2,$3,$4) RETURNING id`,
		user.Name, user.Login, passwordHash, user.Role).Scan(&newId)
	if err != nil {
		return 0, err
	}
	return newId, nil
}

// DeleteUser removes a user from the database specified by the given userId and returns an error if the operation fails.
// If no user is found with the specified ID, an error indicating the absence of the user will be returned.
// Requires a valid database connection and an integer representing the user ID as input parameters.
func DeleteUser(db *sql.DB, userId int) error {
	result, err := db.Exec(`DELETE FROM Users WHERE id=$1`, userId)
	if err != nil {
		return err
	}

	rowsAffected, _ := result.RowsAffected()
	if rowsAffected == 0 {
		return fmt.Errorf("no user with id=%d found", userId)
	}
	return nil
}

// UpdateUser updates user details in the database with non-empty fields in the provided User struct.
// Returns an error if the update operation fails.
func UpdateUser(db *sql.DB, user models.User) error {
	setClauses := []string{}
	params := []interface{}{}
	idx := 1

	if user.Name != "" {
		setClauses = append(setClauses, fmt.Sprintf("name=$%d", idx))
		params = append(params, user.Name)
		idx++
	}
	if user.Login != "" {
		setClauses = append(setClauses, fmt.Sprintf("login=$%d", idx))
		params = append(params, user.Login)
		idx++
	}
	if user.Role != "" {
		setClauses = append(setClauses, fmt.Sprintf("role=$%d", idx))
		params = append(params, user.Role)
		idx++
	}
	if user.PasswordHash != "" {
		setClauses = append(setClauses, fmt.Sprintf("password_hash=$%d", idx))
		params = append(params, user.PasswordHash)
		idx++
	}

	if len(setClauses) == 0 {
		return nil
	}

	setPart := strings.Join(setClauses, ", ")
	query := fmt.Sprintf("UPDATE Users SET %s WHERE id=$%d", setPart, idx)
	params = append(params, user.ID)

	_, err := db.Exec(query, params...)
	return err
}
