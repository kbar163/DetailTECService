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
('409837345','Erick', 'Barrantes', 'Cerdas', 'correo01@gmail.com', 'kevin', '12345',0, 0, 0);

INSERT INTO TRABAJADOR
VALUES
('303450213', '1986-03-23', 'Mariano', 'Vega', 'Arce', 0,1,'2020-02-17','12345'),
('102340567', '1995-06-07', 'Jose', 'Ramos', 'Rojas', 1,0,'2021-10-10','67890'),
('207650385', '1989-08-12', 'Maria', 'Neruda', 'Salguero', 2,1,'2022-03-11','maria');

INSERT INTO SUCURSAL
VALUES
('Montes de Oca', '25446675', '303450213', 'San José', 'Montes De Oca', 'San Pedro', '2019-12-06','2020-08-19');

INSERT INTO PROVEEDOR
VALUES
('2557897','ImportaCarWash','2323232','Alajuela','San Carlos','Quesada','proveedor@importacar.com');

INSERT INTO CLIENTE_TELEFONO
VALUES
('409837345','89881111'),
('409837345','89882222');

INSERT INTO CLIENTE_DIRECCION
VALUES
('409837345','San José','Montes De Oca','San Pedro'),
('409837345','Cartago','Turrialba','Turrialba');

INSERT INTO INSUMO
VALUES
('Cera','5000','McGuire','2557897'),
('Jabon','12000','Patito','2557897'),
('Aspiradora','50000','Aspirator','2557897'),
('Nice','3500','Blackout','2557897'),
('Esponja','2300','Esponjoso','2557897');

INSERT INTO LAVADO
VALUES
('Lavado y aspirado',2000,5000,45,5,20),
('Lavado encerado',3000,7000,60,10,30),
('Lavado premium',3000,7000,60,10,30),
('Pulido',3000,7000,60,10,30);

INSERT INTO LAVADO_INSUMO
VALUES
('Lavado y aspirado', 'Jabon'),
('Lavado y aspirado','Esponja'),
('Lavado y aspirado','Aspiradora'),
('Lavado encerado', 'Jabon'),
('Lavado encerado','Esponja'),
('Lavado encerado','Cera'),
('Lavado premium','Aspiradora'),
('Lavado premium', 'Jabon'),
('Lavado premium','Esponja'),
('Lavado premium','Cera'),
('Lavado premium','Nice'),
('Pulido','Cera');


INSERT INTO LAVADO_ROL
VALUES
('Lavado y aspirado', 1),
('Lavado encerado', 1),
('Lavado premium', 1),
('Lavado premium', 2),
('Pulido', 2);

INSERT INTO CITA (CEDULA_CLIENTE, PLACA_VEHICULO, NOMBRE_SUCURSAL, NOMBRE_LAVADO, CEDULA_TRABAJADOR, HORA, FACTURADA)
VALUES
('409837345','CCC111','Montes de Oca','Lavado y aspirado','102340567','2022-11-03 13:00:00','FALSE'),
('409837345','AAA111','Montes de Oca','Lavado premium','303450213','2022-12-15 15:30:00','FALSE');



