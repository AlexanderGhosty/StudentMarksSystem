package handlers

import (
	"GoServer/internal/db"
	"database/sql"
	"net/http"
	"strconv"

	"GoServer/internal/models"
	"github.com/gin-gonic/gin"
)

// GetGrades handles HTTP GET requests to retrieve grades based on optional subject or student query parameters.
// If a subject ID is provided, it fetches grades for that subject; if a student ID is provided, it fetches grades for that student.
// When no parameters are provided, it returns an empty list of grades.
// Responds with appropriate HTTP status codes and JSON data.
func GetGrades(c *gin.Context, database *sql.DB) {
	subjectParam := c.Query("subject")
	studentParam := c.Query("student")

	if subjectParam != "" {
		subjectID, _ := strconv.Atoi(subjectParam)
		gr, err := db.GetGradesBySubject(database, subjectID)
		if err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
			return
		}
		c.JSON(http.StatusOK, gr)
		return
	} else if studentParam != "" {
		studentID, _ := strconv.Atoi(studentParam)
		gr, err := db.GetGradesByStudent(database, studentID)
		if err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
			return
		}
		c.JSON(http.StatusOK, gr)
		return
	}
	// Если нет параметров, вернём пустой список или ошибку
	c.JSON(http.StatusOK, []models.Grade{})
}

// AddGrade handles the addition of a new grade to the database and responds with success or error.
// It parses the grade data from the request body and interacts with the database to store it.
// Returns HTTP 400 if the request contains invalid data.
// Returns HTTP 500 if there is an issue with database interaction.
// Returns HTTP 200 on successful grade insertion.
func AddGrade(c *gin.Context, database *sql.DB) {
	var g models.Grade
	if err := c.ShouldBindJSON(&g); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "invalid data"})
		return
	}

	err := db.AddGrade(database, g)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to add grade"})
		return
	}

	c.JSON(http.StatusOK, gin.H{"success": true})
}

// DeleteGrade deletes a grade record by its ID from the database. Only users with "admin" or "teacher" roles are allowed.
func DeleteGrade(c *gin.Context, database *sql.DB) {
	idStr := c.Param("id")
	gradeID, err := strconv.Atoi(idStr)
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "invalid grade id"})
		return
	}

	userRoleVal, _ := c.Get("role")
	userRole := userRoleVal.(string)
	if userRole != "admin" && userRole != "teacher" {
		c.JSON(http.StatusForbidden, gin.H{"error": "only teacher or admin can delete grades"})
		return
	}

	err = db.DeleteGrade(database, gradeID)
	if err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{"success": true})
}
