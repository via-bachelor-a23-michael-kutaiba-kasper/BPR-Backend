SET SCHEMA 'user_progress';
INSERT INTO level(id, min_exp, max_exp, name)
VALUES 
    -- Stage 1
(1, 0,100, 'Social Egg'),
(2, 100, 300, 'Novice Newcomer'),
(3, 300, 700, 'Social Apprentice'),
(4, 700, 1500, 'Networking Apprentice'),
(5, 1500, 2700, 'Event Explorer'),

    -- Stage 2
(6,2700, 2800, 'Social caterpillar'),
(7,2800, 3000, 'Event Cadet'),
(8,3000, 3400, 'Gathering Enthusiast'),
(9,3400, 4200, 'Networking Dynamo'),
(10,4200, 5400, 'Community Catalyst'),

    -- Stage 3
(11,5400, 5900, 'Social Pupa'),
(12,5900, 6900, 'Event Major'),
(13,6900, 8900, 'Gathering Prodigy'),
(14,8900, 12900, 'Networking Veteran'),
(15,12900, 18900, 'Community Connoisseur'),

    -- Stage 4
(16,18900, 19400, 'Social Butterfly'),
(17,19400, 20400, 'Event General'),
(18,20400, 22400, 'Gathering Legend'),
(19,22400, 26400, 'Networking Maestro'),
(20,26400, 2147483646, 'Social Icon')



