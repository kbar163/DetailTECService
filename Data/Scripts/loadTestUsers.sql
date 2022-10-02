INSERT INTO ROL
VALUES
(0, 'Gerente'),
(1, 'Lavador'),
(2, 'Pulidor');

INSERT INTO TIPO_PAGO
VALUES
(0, 'Semanal'),
(1, 'Bisemanal');

INSERT INTO CLIENTE
VALUES 
('409837345','Erick', 'Barrantes', 'Cerdas', 'correo01@gmail.com', 'user01', 'password01',9),
('709837345','Manuel', 'Brenes', 'Salazar', 'correo02@gmail.com', 'user02', 'password02',0),
('609837345','Sofia', 'Ramirez', 'Gomez', 'correo03@gmail.com', 'user03', 'password03',5),
('209837345','Veronica', 'Sandoval', 'Sanchez', 'correo04@gmail.com', 'user04', 'password04',12);

INSERT INTO TRABAJADOR
VALUES
('303450213', '1999-12-07', 'Erick', 'Barrantes', 'Cerdas', 0,1,'2020-01-01','password05'),
('102340567', '1999-12-07', 'Jose', 'Ramos', 'Rojas', 1,1,'2020-01-01','password06'),
('207650385', '1999-12-07', 'Maria', 'Nunes', 'Salguero', 1,1,'2020-01-01','password07'),
('408730769', '1999-12-07', 'Brienne', 'Mejia', 'Lopez', 1,1,'2020-01-01','password08'),
('222847727', '1999-12-07', 'Ariana', 'Romero', 'Carrillo', 0,1,'2021-01-01','password09'),
('123121467', '1973-03-02', 'Marco', 'Riversa', 'Meneses', 0,1,'2022-01-01','password10');

INSERT INTO SUCURSAL
VALUES
('Montes de Oca', '25446675', '303450213', 'San José', 'Montes de Oca', 'San Pedro', '2019-01-01','2020-01-01'),
('San Carlos', '24438899', '222847727', 'Alajuela', 'San Carlos', 'Quesada', '2018-01-01','2021-01-01'),
('Alajuela', '25128341', '123121467', 'Alajuela', 'Alajuela', 'Alajuela', '2017-01-01','2022-01-01');
