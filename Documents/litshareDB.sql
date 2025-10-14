
CREATE TYPE role_t AS ENUM ('user','admin');
CREATE TYPE deal_type_t AS ENUM ('exchange','donation');


CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    about TEXT,
    role role_t NOT NULL DEFAULT 'user',
    region VARCHAR(100),
    district VARCHAR(100),
    city VARCHAR(100)
);


CREATE TABLE posts (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    title VARCHAR(50) NOT NULL,
    author VARCHAR(50),
    deal_type deal_type_t,
    description TEXT,
    photo_url VARCHAR(255)
);


CREATE TABLE genres (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL
);


CREATE TABLE books_genres (
    post_id INT REFERENCES posts(id) ON DELETE CASCADE,
    genre_id INT REFERENCES genres(id) ON DELETE CASCADE,
    PRIMARY KEY (post_id, genre_id)
);


CREATE TABLE complaints (
    id SERIAL PRIMARY KEY,
    text TEXT NOT NULL,
    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    post_id INT REFERENCES posts(id) ON DELETE CASCADE,
    complainant_id INT REFERENCES users(id) ON DELETE CASCADE
);
