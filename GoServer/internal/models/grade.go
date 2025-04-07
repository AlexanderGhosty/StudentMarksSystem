package models

// Grade represents a student's grade in a specific subject along with associated identifiers and student name.
// Id is the unique identifier of the grade record.
// StudentId refers to the unique identifier of the student.
// SubjectId is the unique identifier of the subject.
// GradeValue represents the numerical grade value.
// StudentName contains the name of the student associated with the grade.
type Grade struct {
	Id          int    `json:"id"`
	StudentId   int    `json:"student_id"`
	SubjectId   int    `json:"subject_id"`
	GradeValue  int    `json:"grade"`
	StudentName string `json:"student_name"`
}
