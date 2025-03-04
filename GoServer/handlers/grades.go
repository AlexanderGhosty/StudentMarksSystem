package handlers

import (
    "database/sql"
    "net/http"
    "strconv"

    "GoServer/db"
    "GoServer/models"
    "github.com/gin-gonic/gin"
)

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

func AddOrUpdateGrade(c *gin.Context, database *sql.DB) {
    var g models.Grade
    if err := c.ShouldBindJSON(&g); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid data"})
        return
    }
    err := db.AddOrUpdateGrade(database, g)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to add/update grade"})
        return
    }
    c.JSON(http.StatusOK, gin.H{"success": true})
}
