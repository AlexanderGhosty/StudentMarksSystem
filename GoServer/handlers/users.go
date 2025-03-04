package handlers

import (
    "database/sql"
    "net/http"
    "strconv"

    "GoServer/db"
    "GoServer/models"

    "github.com/gin-gonic/gin"
    "golang.org/x/crypto/bcrypt"
)

func GetAllUsers(c *gin.Context, database *sql.DB) {
    users, err := db.GetUsers(database)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "cannot get users"})
        return
    }
    c.JSON(http.StatusOK, users)
}

func CreateUser(c *gin.Context, database *sql.DB) {
    var user models.User
    if err := c.ShouldBindJSON(&user); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid data"})
        return
    }
    // Хешируем пароль
    hashed, err := bcrypt.GenerateFromPassword([]byte(user.PasswordHash), bcrypt.DefaultCost)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "password hashing error"})
        return
    }
    // Сохраняем
    newId, err := db.CreateUser(database, user, string(hashed))
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "cannot create user"})
        return
    }
    user.ID = newId
    user.PasswordHash = ""
    c.JSON(http.StatusOK, user)
}

func UpdateUser(c *gin.Context, database *sql.DB) {
    // 1. Получаем id
    idStr := c.Param("id")
    userId, err := strconv.Atoi(idStr)
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid user id"})
        return
    }

    // 2. Читаем JSON с новыми данными
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

    // 3. Если пришёл пароль, хешируем
    var passwordHash string
    if updateData.Password != "" {
        hashedBytes, err := bcrypt.GenerateFromPassword([]byte(updateData.Password), bcrypt.DefaultCost)
        if err != nil {
            c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to hash password"})
            return
        }
        passwordHash = string(hashedBytes)
    }

    // 4. Вызываем логику обновления
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

    // Можно при желании вернуть "обновлённую" запись (сделав SELECT),
    // но для краткости просто сообщим, что всё ок
    c.JSON(http.StatusOK, gin.H{"success": true})
}

func DeleteUser(c *gin.Context, database *sql.DB) {
    idStr := c.Param("id")
    userID, err := strconv.Atoi(idStr)
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid user id"})
        return
    }

    // только администратор может удалять пользователей:
    // Проверяем роль, которую мы извлекли из JWT.
    userRoleVal, exists := c.Get("role") // "role" мы будем класть в контекст в JWT-мидлваре
    if !exists || userRoleVal.(string) != "admin" {
        c.JSON(http.StatusForbidden, gin.H{"error": "only admin can delete users"})
        return
    }

    // Вызываем функцию из db
    err = db.DeleteUser(database, userID)
    if err != nil {
        c.JSON(http.StatusNotFound, gin.H{"error": err.Error()})
        return
    }

    c.JSON(http.StatusOK, gin.H{"success": true})
}

