package handlers

import (
	"GoServer/models"
	"database/sql"
	"github.com/gin-gonic/gin"
	"github.com/golang-jwt/jwt/v4"
	"golang.org/x/crypto/bcrypt"
	"net/http"
	"time"
)

// jwtSecret is the secret key used for signing and verifying JWT tokens.
var jwtSecret = []byte("SUPER_SECRET_KEY")

// LoginHandler handles user login by validating credentials and generating a JWT token for authenticated sessions.
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

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
		"userId": user.ID,
		"role":   user.Role,
		"exp":    time.Now().Add(time.Hour * 24).Unix(), // токен на сутки
	})

	tokenString, err := token.SignedString(jwtSecret)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"success": false, "errorMessage": "failed to generate token"})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"success": true,
		"token":   tokenString,
		"user": gin.H{
			"id":    user.ID,
			"name":  user.Name,
			"login": user.Login,
			"role":  user.Role,
		},
	})
}

// GetUserByLogin retrieves a user from the database by their login identifier and returns the user details or an error.
func GetUserByLogin(db *sql.DB, login string) (models.User, error) {
	var u models.User

	row := db.QueryRow(`SELECT id, name, login, password_hash, role FROM Users WHERE login=$1`, login)
	err := row.Scan(&u.ID, &u.Name, &u.Login, &u.PasswordHash, &u.Role)
	return u, err
}
