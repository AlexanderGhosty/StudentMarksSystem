package db

import (
	"GoServer/models"
	"database/sql"
	"fmt"
)

// AddGrade inserts a new grade entry into the Grades table in the database and returns any encountered error.
func AddGrade(db *sql.DB, grade models.Grade) error {
	_, err := db.Exec(`
        INSERT INTO Grades (student_id, subject_id, grade)
        VALUES ($1, $2, $3)
    `, grade.StudentId, grade.SubjectId, grade.GradeValue)
	return err
}

// GetGradesBySubject retrieves a list of grades filtered by a specific subject ID from the database.
// The function takes a database connection and a subject ID as parameters and returns a slice of Grade models or an error.
func GetGradesBySubject(db *sql.DB, subjectId int) ([]models.Grade, error) {
	rows, err := db.Query(`
        SELECT id, student_id, subject_id, grade
        FROM Grades
        WHERE subject_id = $1
    `, subjectId)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var grades []models.Grade
	for rows.Next() {
		var gr models.Grade
		if err := rows.Scan(&gr.Id, &gr.StudentId, &gr.SubjectId, &gr.GradeValue); err != nil {
			return nil, err
		}
		grades = append(grades, gr)
	}
	return grades, nil
}

// GetGradesByStudent retrieves a list of grades for a specified student by their ID from the database.
// It takes a database connection and the student ID as parameters and returns a slice of grades or an error.
// Returns an error if the query execution or data scanning fails.
func GetGradesByStudent(db *sql.DB, studentId int) ([]models.Grade, error) {
	rows, err := db.Query(`
        SELECT id, student_id, subject_id, grade
        FROM Grades
        WHERE student_id = $1
    `, studentId)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var grades []models.Grade
	for rows.Next() {
		var gr models.Grade
		if err := rows.Scan(&gr.Id, &gr.StudentId, &gr.SubjectId, &gr.GradeValue); err != nil {
			return nil, err
		}
		grades = append(grades, gr)
	}
	return grades, nil
}

// DeleteGrade removes a grade from the database by its ID. Returns an error if the grade ID is not found or the query fails.
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
