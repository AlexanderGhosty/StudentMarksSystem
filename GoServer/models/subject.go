package models

type Subject struct {
    ID        int    `json:"id"`
    Title     string `json:"title"`
    TeacherID *int   `json:"teacher_id,omitempty"`
}
