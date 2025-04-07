package db

import (
	"GoServer/internal/models"
	"database/sql"
)

// GetSubjects retrieves all subjects from the database and returns them as a slice of models.Subject.
// It takes a database connection object as a parameter and returns any error encountered during execution.
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

// CreateSubject inserts a new subject into the database and returns the newly created subject ID or an error if any occurs.
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

// DeleteSubject removes a record from the Subjects table in the database based on the provided subjectId. Returns an error if any occurs.
func DeleteSubject(db *sql.DB, subjectId int) error {
	_, err := db.Exec(`DELETE FROM Subjects WHERE id=$1`, subjectId)
	return err
}

// UpdateSubject updates an existing subject record in the database with the provided title and teacher_id fields.
// It takes a database connection and a Subject model as input and returns an error if the operation fails.
func UpdateSubject(db *sql.DB, subj models.Subject) error {
	_, err := db.Exec(`
        UPDATE Subjects
           SET title = $1,
               teacher_id = $2
         WHERE id = $3
    `, subj.Title, subj.TeacherID, subj.ID)
	return err
}
