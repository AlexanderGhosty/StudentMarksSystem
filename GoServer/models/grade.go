package models

type Grade struct {
    Id         int `json:"id"`
    StudentId  int `json:"student_id"`
    SubjectId  int `json:"subject_id"`
    GradeValue int `json:"grade"`
}
