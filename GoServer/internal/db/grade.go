package db

import (
	"GoServer/internal/models"
	"database/sql"
	"fmt"
)

// AddGrade inserts a new grade record into the database and returns an error if the operation fails.
func AddGrade(db *sql.DB, grade models.Grade) error {
	_, err := db.Exec(`
        INSERT INTO Grades (student_id, subject_id, grade)
        VALUES ($1, $2, $3)
    `, grade.StudentId, grade.SubjectId, grade.GradeValue)
	return err
}

// GetGradesBySubject retrieves a list of grades for a specific subject from the database using the given subject ID.
// Returns a slice of models.Grade or an error if the database query fails.
// The 'db' parameter represents the database connection.
func GetGradesBySubject(db *sql.DB, subjectId int) ([]models.Grade, error) {
	rows, err := db.Query(`
        SELECT g.id, g.student_id, g.subject_id, g.grade,
               u.name as student_name
        FROM grades g
        JOIN users u ON g.student_id = u.id
        WHERE g.subject_id = $1
    `, subjectId)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var grades []models.Grade
	for rows.Next() {
		var gr models.Grade
		if err := rows.Scan(&gr.Id, &gr.StudentId, &gr.SubjectId, &gr.GradeValue, &gr.StudentName); err != nil {
			return nil, err
		}
		grades = append(grades, gr)
	}
	return grades, nil
}

// GetGradesByStudent retrieves all grades for a specified student by their ID from the database.
// It returns a slice of Grade models and an error if any issue occurs during the query or data scanning.
// The database connection is passed as an argument, along with the student ID to filter records.
func GetGradesByStudent(db *sql.DB, studentId int) ([]models.Grade, error) {
	rows, err := db.Query(`
        SELECT g.id, g.student_id, g.subject_id, g.grade,
               u.name as student_name
        FROM grades g
        JOIN users u ON g.student_id = u.id
        WHERE g.student_id = $1
    `, studentId)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var grades []models.Grade
	for rows.Next() {
		var gr models.Grade
		if err := rows.Scan(&gr.Id, &gr.StudentId, &gr.SubjectId, &gr.GradeValue, &gr.StudentName); err != nil {
			return nil, err
		}
		grades = append(grades, gr)
	}
	return grades, nil
}

// DeleteGrade removes a grade record from the database by its ID and returns an error if the operation fails or no rows are affected.
func DeleteGrade(db *sql.DB, gradeId int) error {
	result, err := db.Exec(`DELETE FROM Grades WHERE id=$1`, gradeId)
	if err != nil {
		return err
	}
	rowsAffected, _ := result.RowsAffected()
	if rowsAffected == 0 {
		return fmt.Errorf("grade with id=%d not found", gradeId)
	}
	return nil
}
