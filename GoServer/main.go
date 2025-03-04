package main

import (
    "log"
    "github.com/gin-gonic/gin"
    "GoServer/db"
    "GoServer/handlers"
    "GoServer/middleware"
    _ "github.com/lib/pq"
)

func main() {
    // Подключаемся к БД
    database, err := db.OpenDB("host=localhost port=5432 user=postgres password=postgres dbname=marks sslmode=disable")
    if err != nil {
        log.Fatalf("Cannot open DB: %v", err)
    }

    r := gin.Default()

    // Не требует авторизации
    r.POST("/login", func(c *gin.Context) {
        handlers.LoginHandler(c, database)
    })

    // Защищённая группа
    authorized := r.Group("/")
    authorized.Use(middleware.AuthMiddleware(database)) // Простая мидлварь
    {
        authorized.GET("/users", func(c *gin.Context) { handlers.GetAllUsers(c, database) })
        authorized.POST("/users", func(c *gin.Context) { handlers.CreateUser(c, database) })
        authorized.DELETE("/users/:id", func(c *gin.Context) { handlers.DeleteUser(c, database) })
        authorized.PUT("/users/:id", func(c *gin.Context) { handlers.UpdateUser(c, database)})

        authorized.GET("/subjects", func(c *gin.Context) { handlers.GetAllSubjects(c, database) })
        authorized.POST("/subjects", func(c *gin.Context) { handlers.CreateSubject(c, database) })
        authorized.DELETE("/subjects/:id", func(c *gin.Context) { handlers.DeleteSubject(c, database) })
        authorized.PUT("/subjects/:id", func(c *gin.Context) { handlers.UpdateSubject(c, database) })

        authorized.GET("/grades", func(c *gin.Context) { handlers.GetGrades(c, database) })
        authorized.POST("/grades", func(c *gin.Context) { handlers.AddOrUpdateGrade(c, database) })
    }

    r.Run(":8080")
}
