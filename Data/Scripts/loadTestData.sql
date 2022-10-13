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
('409837345','Erick', 'Barrantes', 'Cerdas', 'correo01@gmail.com', 'user01', 'password01',9, 15, 6),
('709837345','Manuel', 'Brenes', 'Salazar', 'correo02@gmail.com', 'user02', 'password02',0, 5, 5),
('609837345','Sofia', 'Ramirez', 'Gomez', 'correo03@gmail.com', 'user03', 'password03',5, 5, 0),
('209837345','Veronica', 'Sandoval', 'Sanchez', 'correo04@gmail.com', 'user04', 'password04',12, 17, 5);

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
('Montes de Oca', '25446675', '303450213', 'San José', 'Montes De Oca', 'San Pedro', '2019-01-01','2020-01-01'),
('San Carlos', '24438899', '222847727', 'Alajuela', 'San Carlos', 'Quesada', '2018-01-01','2021-01-01'),
('Alajuela', '25128341', '123121467', 'Alajuela', 'Cantón Central', 'Alajuela', '2017-01-01','2022-01-01');

INSERT INTO PROVEEDOR
VALUES
('2557897','LlantasTEC','2323232','Alajuela','San Carlos','Quesada','LlantasTEC18@gmail.com'),
('3457897','LlantasUCR','2323232','Cartago','Turrialba','Turrialba','LlantasUCR18@gmail.com'),
('1237897','LlantasUNA','2323232','San José','Pérez Zeledón','Rivas','LlantasUNA18@gmail.com');

INSERT INTO CLIENTE_TELEFONO
VALUES
('409837345','89881111'),
('409837345','89882222'),
('409837345','89883333'),
('709837345','88871111'),
('609837345','87861111'),
('609837345','87862222'),
('209837345','86851111');

INSERT INTO CLIENTE_DIRECCION
VALUES
('409837345','San Jose','Montes De Oca','San Pedro'),
('409837345','Cartago','Turrialba','Turrialba'),
('709837345','Alajuela','Palmares','Palmares'),
('609837345','Heredia','San Isidro','San Francisco'),
('209837345','Puntarenas','Coto Brus','San Vito'),
('209837345','Puntarenas','Coto Brus','Sabalito');

INSERT INTO INSUMO
VALUES
('Lubricante','5000','TEC','2557897'),
('Jabon','12000','UNA','1237897'),
('Esponja','2300','UCR','3457897');

INSERT INTO LAVADO
VALUES
('Lavado simple',2000,5000,45,5,20),
('Lavado avanzado',3000,7000,60,10,30);

INSERT INTO LAVADO_INSUMO
VALUES
('Lavado simple', 'Jabon'),
('Lavado avanzado','Jabon'),
('Lavado avanzado','Esponja');

INSERT INTO LAVADO_ROL
VALUES
('Lavado simple', 1),
('Lavado avanzado',1),
('Lavado avanzado',2);

INSERT INTO CITA (CEDULA_CLIENTE, PLACA_VEHICULO, NOMBRE_SUCURSAL, NOMBRE_LAVADO, CEDULA_TRABAJADOR, HORA, FACTURADA)
VALUES
('209837345','AAA111','Montes de Oca','Lavado simple','102340567','2022-11-16 13:21:11',0),
('709837345','BBB111','Montes de Oca','Lavado simple','207650385','2022-10-24 13:21:11',0),
('409837345','CCC111','Montes de Oca','Lavado avanzado','408730769','2022-11-03 13:21:11',0),
('609837345','DDD111','Montes de Oca','Lavado avanzado','408730769','2022-11-16 16:00:00',0);



