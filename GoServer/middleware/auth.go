package middleware

import (
    "net/http"
    "strings"

    "github.com/gin-gonic/gin"
    "github.com/golang-jwt/jwt/v4"
)

var jwtSecret = []byte("SUPER_SECRET_KEY")

func AuthMiddleware() gin.HandlerFunc {
    return func(c *gin.Context) {
        // Извлекаем заголовок Authorization
        authHeader := c.GetHeader("Authorization")
        if authHeader == "" {
            c.JSON(http.StatusUnauthorized, gin.H{"error": "missing Authorization header"})
            c.Abort()
            return
        }

        // Формат: "Bearer <token>"
        parts := strings.SplitN(authHeader, " ", 2)
        if len(parts) != 2 || parts[0] != "Bearer" {
            c.JSON(http.StatusUnauthorized, gin.H{"error": "invalid Authorization header format"})
            c.Abort()
            return
        }
        tokenStr := parts[1]

        // Парсим токен
        token, err := jwt.Parse(tokenStr, func(t *jwt.Token) (interface{}, error) {
            // Проверка метода подписи
            if _, ok := t.Method.(*jwt.SigningMethodHMAC); !ok {
                return nil, jwt.NewValidationError("invalid signing method", jwt.ValidationErrorSignatureInvalid)
            }
            return jwtSecret, nil
        })

        if err != nil || !token.Valid {
            c.JSON(http.StatusUnauthorized, gin.H{"error": "invalid token"})
            c.Abort()
            return
        }

        // Извлекаем claims – если они MapClaims
        claims, ok := token.Claims.(jwt.MapClaims)
        if !ok {
            c.JSON(http.StatusUnauthorized, gin.H{"error": "invalid token claims"})
            c.Abort()
            return
        }

        // Проверяем (не истёк ли токен) – если надо вручную
        // if float64(time.Now().Unix()) > claims["exp"].(float64) {
        //     c.JSON(http.StatusUnauthorized, gin.H{"error": "token expired"})
        //     c.Abort()
        //     return
        // }

        // Сохраняем userId, role в контекст
        userIdFloat, ok1 := claims["userId"].(float64)
        roleStr, ok2 := claims["role"].(string)
        if !ok1 || !ok2 {
            c.JSON(http.StatusUnauthorized, gin.H{"error": "invalid token payload"})
            c.Abort()
            return
        }

        c.Set("userId", int(userIdFloat))
        c.Set("role", roleStr)

        c.Next()
    }
}
