package db

import (
    "database/sql"
    "GoServer/models"
)

func GetSubjects(db *sql.DB) ([]models.Subject, error) {
    rows, err := db.Query(`SELECT id, title, teacher_id FROM Subjects`)
    if err != nil {
        return nil, err
    }
    defer rows.Close()

    var subjects []models.Subject
    for rows.Next() {
        var s models.Subject
        if err := rows.Scan(&s.ID, &s.Title, &s.TeacherID); err != nil {
            return nil, err
        }
        subjects = append(subjects, s)
    }
    return subjects, nil
}

func CreateSubject(db *sql.DB, subj models.Subject) (int, error) {
    var newId int
    err := db.QueryRow(`INSERT INTO Subjects (title, teacher_id)
                        VALUES($1, $2) RETURNING id`,
        subj.Title, subj.TeacherID).Scan(&newId)
    if err != nil {
        return 0, err
    }
    return newId, nil
}

func DeleteSubject(db *sql.DB, subjectId int) error {
    _, err := db.Exec(`DELETE FROM Subjects WHERE id=$1`, subjectId)
    return err
}

func UpdateSubject(db *sql.DB, subj models.Subject) error {
    // Простейшая реализация — всегда обновляем и title, и teacher_id
    // Если нужно гибко, можно делать сборку UPDATE как в UpdateUser
    _, err := db.Exec(`
        UPDATE Subjects
           SET title = $1,
               teacher_id = $2
         WHERE id = $3
    `, subj.Title, subj.TeacherID, subj.ID)
    return err
}
