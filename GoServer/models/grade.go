package models

// Grade represents a student's grade for a specific subject.
// Id is the unique identifier for the grade entry.
// StudentId refers to the ID of the student associated with this grade.
// SubjectId refers to the ID of the subject associated with this grade.
// GradeValue indicates the actual grade value received by the student.
type Grade struct {
	Id         int `json:"id"`
	StudentId  int `json:"student_id"`
	SubjectId  int `json:"subject_id"`
	GradeValue int `json:"grade"`
}
