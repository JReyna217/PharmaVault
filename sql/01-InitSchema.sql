CREATE TABLE users (
    user_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    registration_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE medicine_catalog (
    catalog_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(250) NOT NULL,
    pharmaceutical_form VARCHAR(50) NOT NULL,
    dosage VARCHAR(50),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE inventory (
    inventory_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    catalog_id INT NOT NULL REFERENCES medicine_catalog(catalog_id),
    quantity INT NOT NULL DEFAULT 0,
    purchase_date DATE,
    expiration_date DATE NOT NULL,
    prescription_notes TEXT,
    date_added TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE system_responses (
    code VARCHAR(6) PRIMARY KEY,
    message_es TEXT NOT NULL,
    message_en TEXT NOT NULL
);

CREATE TABLE error_logs (
    log_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    origin_layer VARCHAR(15) NOT NULL,
    main_object VARCHAR(50) NOT NULL,
    method_name VARCHAR(50) NOT NULL,
    description VARCHAR(4000),
    error_message VARCHAR(4000) NOT NULL,
    error_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    user_id INT REFERENCES users(user_id) ON DELETE SET NULL,
    incident_number UUID DEFAULT gen_random_uuid() NOT NULL
);