package models

// Subject represents a subject entity with an ID, title, and an optional associated teacher ID.
type Subject struct {
	ID        int    `json:"id"`
	Title     string `json:"title"`
	TeacherID *int   `json:"teacher_id,omitempty"`
}
