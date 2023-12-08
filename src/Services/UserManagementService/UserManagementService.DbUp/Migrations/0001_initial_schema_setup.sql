CREATE TABLE IF NOT EXISTS category
(
    id   INT,
    name VARCHAR,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS level
(
    id      INT,
    max_exp BIGINT,
    min_exp BIGINT,
    name    VARCHAR,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS achievement
(
    id          INT,
    name        VARCHAR,
    unlock_date TIMESTAMPTZ,
    description VARCHAR,
    reward      BIGINT,
    icon        VARCHAR,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS timed_criteria
(
    id   INT GENERATED ALWAYS AS IDENTITY,
    goal BIGINT,
    type VARCHAR, --monthly, weekly, exp_monthly, attendance_monthly,
    PRIMARY KEY (id)
);


CREATE TABLE IF NOT EXISTS unlockable_progress
(
    id       INT GENERATED ALWAYS AS IDENTITY,
    progress BIGINT,
    date     TIMESTAMPTZ,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS category_attendance_criteria
(
    id          INT GENERATED ALWAYS AS IDENTITY,
    goal        BIGINT,
    category_id INT,
    PRIMARY KEY (id),
    FOREIGN KEY (category_id) REFERENCES category (id)
);

CREATE TABLE IF NOT EXISTS monthly_exp_goal
(
    id                                  INT GENERATED ALWAYS AS IDENTITY,
    description                         VARCHAR,
    start_date                          TIMESTAMPTZ,
    user_id                             VARCHAR,
    monthly_exp_goal_unlock_criteria_id INT,
    PRIMARY KEY (id),
    FOREIGN KEY (monthly_exp_goal_unlock_criteria_id) REFERENCES timed_criteria (id)
);

CREATE TABLE IF NOT EXISTS progress
(
    id        INT GENERATED ALWAYS AS IDENTITY,
    user_id   VARCHAR,
    total_exp BIGINT,
    stage     INT,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS user_achievement
(
    achievement_id INT,
    user_id        VARCHAR,
    unlocked_date  TIMESTAMPTZ,
    FOREIGN KEY (achievement_id) REFERENCES achievement (id),
    PRIMARY KEY (achievement_id, user_id)
);

CREATE TABLE IF NOT EXISTS unlockable_achievement_progress
(
    unlockable_progress_id INT,
    achievement_id         INT,
    user_id                VARCHAR,
    FOREIGN KEY (unlockable_progress_id) REFERENCES unlockable_progress (id),
    FOREIGN KEY (achievement_id) REFERENCES achievement (id),
    primary key (unlockable_progress_id, achievement_id, user_id)
);


CREATE TABLE IF NOT EXISTS unlockable_monthly_goal_progress
(
    unlockable_progress_id INT,
    goal_id                INT,
    user_id                VARCHAR,
    FOREIGN KEY (unlockable_progress_id) REFERENCES unlockable_progress (id),
    FOREIGN KEY (goal_id) REFERENCES monthly_exp_goal (id),
    primary key (unlockable_progress_id, goal_id, user_id)
);

INSERT INTO category (id, name)
VALUES (0, 'Un Assigned'),
       (1, 'Concerts'),
       (2, 'Festivals'),
       (3, 'Conferences'),
       (4, 'Workshops'),
       (5, 'Seminars'),
       (6, 'Arts and Culture'),
       (7, 'Food and Drink'),
       (8, 'Charity and Fundraising'),
       (9, 'Health and Wellness'),
       (10, 'Technology'),
       (11, 'Business and Entrepreneurship'),
       (12, 'Education'),
       (13, 'Family and Kids'),
       (14, 'Outdoor and Adventure'),
       (15, 'Comedy'),
       (16, 'Film and Cinema'),
       (17, 'Music'),
       (18, 'Performing Arts'),
       (19, 'Classic Literature'),
       (20, 'Drinks'),
       (21, 'Fitness and Workouts'),
       (22, 'Foods'),
       (23, 'Games'),
       (24, 'Gardening'),
       (25, 'Healthy Living and Self Care'),
       (26, 'Home and Garden'),
       (27, 'Parties'),
       (28, 'Religions'),
       (29, 'Shopping'),
       (30, 'Social Issues'),
       (31, 'Sports'),
       (32, 'Theater');
