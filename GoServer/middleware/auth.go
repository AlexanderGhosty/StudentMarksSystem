package middleware

import (
    "database/sql"
    "net/http"

    "github.com/gin-gonic/gin"
)

func AuthMiddleware(db *sql.DB) gin.HandlerFunc {
    return func(c *gin.Context) {
        // Здесь может быть проверка заголовка Authorization
        // или cookie сессии. Для примера - упростим:
        token := c.GetHeader("X-Auth-Token")
        if token == "" {
            c.JSON(http.StatusUnauthorized, gin.H{"error": "no auth token"})
            c.Abort()
            return
        }
        // Проверка, допустим, по таблице сессий или просто пропускаем
        c.Next()
    }
}
