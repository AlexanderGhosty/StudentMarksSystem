package db

import (
	"GoServer/models"
	"database/sql"
	"fmt"
)

//func AddOrUpdateGrade(db *sql.DB, grade models.Grade) error {
//    // ON CONFLICT (student_id, subject_id) DO UPDATE SET grade ...
//    // При условии, что в схеме есть уникальный индекс
//    _, err := db.Exec(`
//        INSERT INTO Grades(student_id, subject_id, grade)
//        VALUES($1, $2, $3)
//        ON CONFLICT (student_id, subject_id)
//        DO UPDATE SET grade=EXCLUDED.grade
//    `, grade.StudentId, grade.SubjectId, grade.GradeValue)
//    return err
//}

func AddGrade(db *sql.DB, grade models.Grade) error {
	_, err := db.Exec(`
        INSERT INTO Grades (student_id, subject_id, grade)
        VALUES ($1, $2, $3)
    `, grade.StudentId, grade.SubjectId, grade.GradeValue)
	return err
}

func GetGradesBySubject(db *sql.DB, subjectId int) ([]models.Grade, error) {
	// Извлекаем только те поля, которые есть в модели Grade
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

func GetGradesByStudent(db *sql.DB, studentId int) ([]models.Grade, error) {
	// Извлекаем данные оценки для указанного студента
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
