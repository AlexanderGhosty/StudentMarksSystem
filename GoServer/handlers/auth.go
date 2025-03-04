package handlers

import (
    "database/sql"
    "net/http"
    "github.com/gin-gonic/gin"
    "GoServer/db"
    "GoServer/models"
    "golang.org/x/crypto/bcrypt" // для проверки пароля
)

func LoginHandler(c *gin.Context, database *sql.DB) {
    var loginData struct {
        Login    string `json:"login"`
        Password string `json:"password"`
    }
    if err := c.ShouldBindJSON(&loginData); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"success": false, "errorMessage": "invalid data"})
        return
    }
    // Предположим, есть функция GetUserByLogin
    user, err := GetUserByLogin(database, loginData.Login)
    if err != nil {
        c.JSON(http.StatusUnauthorized, gin.H{"success": false, "errorMessage": "user not found"})
        return
    }
    // Сравнить хеши
    if err := bcrypt.CompareHashAndPassword([]byte(user.PasswordHash), []byte(loginData.Password)); err != nil {
        c.JSON(http.StatusUnauthorized, gin.H{"success": false, "errorMessage": "wrong password"})
        return
    }
    // Авторизация успешна
    c.JSON(http.StatusOK, gin.H{
        "success": true,
        "user": gin.H{
            "id":    user.ID,
            "name":  user.Name,
            "login": user.Login,
            "role":  user.Role,
        },
    })
}

// GetUserByLogin – заглушка, может быть в файле db/user.go
func GetUserByLogin(db *sql.DB, login string) (models.User, error) {
    var u models.User
    // В таблице Users есть password_hash
    row := db.QueryRow(`SELECT id, name, login, password_hash, role FROM Users WHERE login=$1`, login)
    err := row.Scan(&u.ID, &u.Name, &u.Login, &u.PasswordHash, &u.Role)
    return u, err
}
