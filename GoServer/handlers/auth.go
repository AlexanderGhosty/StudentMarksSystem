package handlers

import (
    "database/sql"
    "net/http"
    "github.com/gin-gonic/gin"
    "GoServer/models"
    "golang.org/x/crypto/bcrypt" // для проверки пароля
    "time"
    "github.com/golang-jwt/jwt/v4"
)

// Секретный ключ для подписи (в реальном коде хранить в .env!)
var jwtSecret = []byte("SUPER_SECRET_KEY") 

func LoginHandler(c *gin.Context, database *sql.DB) {
    var loginData struct {
        Login    string `json:"login"`
        Password string `json:"password"`
    }
    if err := c.ShouldBindJSON(&loginData); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"success": false, "errorMessage": "invalid data"})
        return
    }

    user, err := GetUserByLogin(database, loginData.Login)
    if err != nil {
        c.JSON(http.StatusUnauthorized, gin.H{"success": false, "errorMessage": "user not found"})
        return
    }

    if err := bcrypt.CompareHashAndPassword([]byte(user.PasswordHash), []byte(loginData.Password)); err != nil {
        c.JSON(http.StatusUnauthorized, gin.H{"success": false, "errorMessage": "wrong password"})
        return
    }

    // Создаём claims – сюда же кладём userID и роль
    token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
        "userId": user.ID,
        "role":   user.Role,
        "exp":    time.Now().Add(time.Hour * 24).Unix(), // токен на сутки
    })

    // Подписываем
    tokenString, err := token.SignedString(jwtSecret)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"success": false, "errorMessage": "failed to generate token"})
        return
    }

    // Возвращаем клиенту
    c.JSON(http.StatusOK, gin.H{
        "success": true,
        "token":   tokenString,
        "user": gin.H{
            "id":   user.ID,
            "name": user.Name,
            "role": user.Role,
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
