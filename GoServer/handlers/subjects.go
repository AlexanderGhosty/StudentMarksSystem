package handlers

import (
    "database/sql"
    "net/http"
    "strconv"

    "GoServer/db"
    "GoServer/models"
    "github.com/gin-gonic/gin"
)

func GetAllSubjects(c *gin.Context, database *sql.DB) {
    subs, err := db.GetSubjects(database)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
        return
    }
    c.JSON(http.StatusOK, subs)
}

func CreateSubject(c *gin.Context, database *sql.DB) {
    var subj models.Subject
    if err := c.ShouldBindJSON(&subj); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid data"})
        return
    }
    newId, err := db.CreateSubject(database, subj)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to create subject"})
        return
    }
    subj.ID = newId
    c.JSON(http.StatusOK, subj)
}

func DeleteSubject(c *gin.Context, database *sql.DB) {
    idStr := c.Param("id")
    id, _ := strconv.Atoi(idStr)
    if err := db.DeleteSubject(database, id); err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to delete"})
        return
    }
    c.JSON(http.StatusOK, gin.H{"success": true})
}

func UpdateSubject(c *gin.Context, database *sql.DB) {
    idStr := c.Param("id")
    subjectId, err := strconv.Atoi(idStr)
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid subject id"})
        return
    }

    var data struct {
        Title     string `json:"title"`
        TeacherID *int   `json:"teacher_id"` // может быть null
    }
    if err := c.ShouldBindJSON(&data); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "invalid request data"})
        return
    }

    subj := models.Subject{
        ID:        subjectId,
        Title:     data.Title,
        TeacherID: data.TeacherID,
    }

    if err := db.UpdateSubject(database, subj); err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to update subject"})
        return
    }

    c.JSON(http.StatusOK, gin.H{"success": true})
}
