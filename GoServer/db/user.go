package db

import (
    "database/sql"
    "GoServer/models"
	"fmt"
)

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

func DeleteUser(db *sql.DB, userId int) error {
    _, err := db.Exec(`DELETE FROM Users WHERE id=$1`, userId)
    return err
}

func UpdateUser(db *sql.DB, user models.User) error {
    // Собираем динамический UPDATE, чтобы обновлять только те поля, которые пришли непустыми.
    // Для упрощения предположим, что менять можно сразу все: name, login, role, password_hash.

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

    // Если нечего обновлять, выходим
    if len(setClauses) == 0 {
        return nil
    }

    // Сформировать основной SQL
    // UPDATE Users SET name=$1, login=$2, role=$3, password_hash=$4 WHERE id=$5
    setPart := strings.Join(setClauses, ", ")
    query := fmt.Sprintf("UPDATE Users SET %s WHERE id=$%d", setPart, idx)
    params = append(params, user.ID)

    _, err := db.Exec(query, params...)
    return err
}

// И другие функции: GetUserByLogin и т.д.
