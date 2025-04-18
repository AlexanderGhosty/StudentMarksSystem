package main

import (
	"GoServer/internal/db"
	"GoServer/internal/handlers"
	"GoServer/internal/middleware"
	"database/sql"
	"fmt"
	"log"

	"github.com/gin-gonic/gin"
	_ "github.com/lib/pq"
	"golang.org/x/crypto/bcrypt"
)

// bootstrapAdmin ensures an admin user exists in the database; creates one with default credentials if absent.
func bootstrapAdmin(database *sql.DB) {
	var exists bool
	err := database.QueryRow(`SELECT EXISTS (SELECT 1 FROM Users WHERE role = 'admin')`).Scan(&exists)
	if err != nil {
		log.Fatalf("Ошибка проверки наличия администратора: %v", err)
	}
	if !exists {
		hashed, err := bcrypt.GenerateFromPassword([]byte("admin"), bcrypt.DefaultCost)
		if err != nil {
			log.Fatalf("Ошибка хеширования пароля: %v", err)
		}
		var adminId int
		err = database.QueryRow(`
			INSERT INTO Users (name, login, password_hash, role)
			VALUES ($1, $2, $3, $4) RETURNING id
		`, "alex", "admin", string(hashed), "admin").Scan(&adminId)
		if err != nil {
			log.Fatalf("Ошибка создания администратора: %v", err)
		}
		fmt.Printf("Создан администратор по умолчанию с id %d\n", adminId)
	} else {
		fmt.Println("Администратор уже существует")
	}
}

// main initializes the database connection, sets up routes and middleware, and starts the HTTP server at port 8080.
func main() {
	database, err := db.OpenDB("host=localhost port=5432 user=postgres password=postgres dbname=postgres sslmode=disable")
	if err != nil {
		log.Fatalf("Cannot open DB: %v", err)
	}

	bootstrapAdmin(database)

	r := gin.Default()

	r.POST("/login", func(c *gin.Context) {
		handlers.LoginHandler(c, database)
	})

	authorized := r.Group("/")
	authorized.Use(middleware.AuthMiddleware())
	{
		authorized.GET("/users", func(c *gin.Context) { handlers.GetAllUsers(c, database) })
		authorized.POST("/users", func(c *gin.Context) { handlers.CreateUser(c, database) })
		authorized.DELETE("/users/:id", func(c *gin.Context) { handlers.DeleteUser(c, database) })
		authorized.PUT("/users/:id", func(c *gin.Context) { handlers.UpdateUser(c, database) })

		authorized.GET("/subjects", func(c *gin.Context) { handlers.GetAllSubjects(c, database) })
		authorized.POST("/subjects", func(c *gin.Context) { handlers.CreateSubject(c, database) })
		authorized.DELETE("/subjects/:id", func(c *gin.Context) { handlers.DeleteSubject(c, database) })
		authorized.PUT("/subjects/:id", func(c *gin.Context) { handlers.UpdateSubject(c, database) })

		authorized.GET("/grades", func(c *gin.Context) { handlers.GetGrades(c, database) })
		authorized.POST("/grades", func(c *gin.Context) { handlers.AddGrade(c, database) })
		authorized.DELETE("/grades/:id", func(c *gin.Context) { handlers.DeleteGrade(c, database) })
	}

	r.Run(":8080")
}
