package db

import (
    "database/sql"
    "GoServer/models"
)

func AddOrUpdateGrade(db *sql.DB, grade models.Grade) error {
    // ON CONFLICT (student_id, subject_id) DO UPDATE SET grade ...
    // При условии, что в схеме есть уникальный индекс
    _, err := db.Exec(`
        INSERT INTO Grades(student_id, subject_id, grade)
        VALUES($1, $2, $3)
        ON CONFLICT (student_id, subject_id)
        DO UPDATE SET grade=EXCLUDED.grade
    `, grade.StudentId, grade.SubjectId, grade.GradeValue)
    return err
}

func GetGradesBySubject(db *sql.DB, subjectId int) ([]models.Grade, error) {
    rows, err := db.Query(`
        SELECT g.id, g.student_id, u.name, g.grade
        FROM Grades g
        JOIN Users u ON g.student_id = u.id
        WHERE g.subject_id=$1
    `, subjectId)
    if err != nil {
        return nil, err
    }
    defer rows.Close()

    var grades []models.Grade
    for rows.Next() {
        var gr models.Grade
        var studentName string
        if err := rows.Scan(&gr.Id, &gr.StudentId, &studentName, &gr.GradeValue); err != nil {
            return nil, err
        }
        gr.StudentName = studentName
        grades = append(grades, gr)
    }
    return grades, nil
}

func GetGradesByStudent(db *sql.DB, studentId int) ([]models.Grade, error) {
    rows, err := db.Query(`
        SELECT g.id, g.subject_id, s.title, g.grade
        FROM Grades g
        JOIN Subjects s ON g.subject_id = s.id
        WHERE g.student_id=$1
    `, studentId)
    if err != nil {
        return nil, err
    }
    defer rows.Close()

    var grades []models.Grade
    for rows.Next() {
        var gr models.Grade
        var subjectTitle string
        if err := rows.Scan(&gr.Id, &gr.SubjectId, &subjectTitle, &gr.GradeValue); err != nil {
            return nil, err
        }
        gr.SubjectTitle = subjectTitle
        grades = append(grades, gr)
    }
    return grades, nil
}
