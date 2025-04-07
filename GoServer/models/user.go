package models

// User represents a system user with their ID, name, login, hashed password, and role information.
type User struct {
	ID           int    `json:"id"`
	Name         string `json:"name"`
	Login        string `json:"login"`
	PasswordHash string `json:"passwordHash,omitempty"`
	Role         string `json:"role"`
}
