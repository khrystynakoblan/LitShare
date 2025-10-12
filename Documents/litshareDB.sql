CREATE TABLE locations (
    id SERIAL PRIMARY KEY,
    region VARCHAR(128) NOT NULL,
    district VARCHAR(128) NOT NULL,
    city VARCHAR(128) NOT NULL,
    UNIQUE (region, district, city)
);

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    about TEXT,
    location_id INT REFERENCES locations(id) ON DELETE SET NULL,
    role VARCHAR(10) CHECK (role IN ('user','admin')) DEFAULT 'user',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE books (
    id SERIAL PRIMARY KEY,
    title VARCHAR(500) NOT NULL,
    author VARCHAR(300) NOT NULL,
    UNIQUE (title, author)
);

CREATE TABLE genres (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) UNIQUE NOT NULL
);

CREATE TABLE books_genres (
    book_id INT REFERENCES books(id) ON DELETE CASCADE,
    genre_id INT REFERENCES genres(id) ON DELETE CASCADE,
    PRIMARY KEY (book_id, genre_id)
);

CREATE TABLE posts (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    book_id INT NOT NULL REFERENCES books(id) ON DELETE RESTRICT,
    deal_type VARCHAR(10) CHECK (deal_type IN ('exchange','donation')) NOT NULL,
    description TEXT,
    photo_url VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE complaints (
    id SERIAL PRIMARY KEY,
    text TEXT NOT NULL,
    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    post_id INT NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    complainant_id INT REFERENCES users(id) ON DELETE SET NULL
);