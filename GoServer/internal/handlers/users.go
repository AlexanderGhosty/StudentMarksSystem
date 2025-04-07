package handlers

import (
	"GoServer/internal/db"
	"database/sql"
	"net/http"
	"strconv"

	"GoServer/internal/models"

	"github.com/gin-gonic/gin"
	"golang.org/x/crypto/bcrypt"
)

// GetAllUsers handles an HTTP GET request to retrieve all users from the database and returns them in JSON format.
// It sends a 500 Internal Server Error response if the query fails.
func GetAllUsers(c *gin.Context, database *sql.DB) {
	users, err := db.GetUsers(database)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "cannot get users"})
		return
	}
	c.JSON(http.StatusOK, users)
}

// CreateUser handles creating a new user by binding JSON input, hashing the password, and storing user data in the database.
func CreateUser(c *gin.Context, database *sql.DB) {
	var user models.User
	if err := c.ShouldBindJSON(&user); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "invalid data"})
		return
	}

	hashed, err := bcrypt.GenerateFromPassword([]byte(user.PasswordHash), bcrypt.DefaultCost)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "password hashing error"})
		return
	}

	newId, err := db.CreateUser(database, user, string(hashed))
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "cannot create user"})
		return
	}
	user.ID = newId
	user.PasswordHash = ""
	c.JSON(http.StatusOK, user)
}

// UpdateUser updates user details in the database based on the provided JSON payload and user ID from the URL parameter.
func UpdateUser(c *gin.Context, database *sql.DB) {

	idStr := c.Param("id")
	userId, err := strconv.Atoi(idStr)
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "invalid user id"})
		return
	}

	var updateData struct {
		Name     string `json:"name"`
		Login    string `json:"login"`
		Role     string `json:"role"`
		Password string `json:"password"`
	}
	if err := c.ShouldBindJSON(&updateData); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "invalid request data"})
		return
	}

	var passwordHash string
	if updateData.Password != "" {
		hashedBytes, err := bcrypt.GenerateFromPassword([]byte(updateData.Password), bcrypt.DefaultCost)
		if err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to hash password"})
			return
		}
		passwordHash = string(hashedBytes)
	}

	userToUpdate := models.User{
		ID:           userId,
		Name:         updateData.Name,
		Login:        updateData.Login,
		Role:         updateData.Role,
		PasswordHash: passwordHash, // пустая строка, если пароль не меняли
	}
	err = db.UpdateUser(database, userToUpdate)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to update user"})
		return
	}

	c.JSON(http.StatusOK, gin.H{"success": true})
}

// DeleteUser handles deleting a user specified by ID, restricted to admin role, and returns success or error responses.
func DeleteUser(c *gin.Context, database *sql.DB) {
	idStr := c.Param("id")
	userID, err := strconv.Atoi(idStr)
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "invalid user id"})
		return
	}

	userRoleVal, exists := c.Get("role") // "role" мы будем класть в контекст в JWT-мидлваре
	if !exists || userRoleVal.(string) != "admin" {
		c.JSON(http.StatusForbidden, gin.H{"error": "only admin can delete users"})
		return
	}

	err = db.DeleteUser(database, userID)
	if err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{"success": true})
}
