CREATE TABLE CLIENTE 
(
    CEDULA_CLIENTE VARCHAR(9) NOT NULL PRIMARY KEY ,
    NOMBRE VARCHAR(20) NOT NULL ,
    PRIMER_APELLIDO VARCHAR(20) NOT NULL ,
    SEGUNDO_APELLIDO VARCHAR(20) NOT NULL ,
    CORREO_CLIENTE VARCHAR(50) NOT NULL,
    USUARIO VARCHAR(20) NOT NULL,
    PASSWORD_CLIENTE VARCHAR(20) NOT NULL,
    PUNTOS_ACUM INT ,
    PUNTOS_OBT INT ,
    PUNTOS_REDIM INT ,
    CONSTRAINT UNIQUE_LOGIN UNIQUE (CORREO_CLIENTE,USUARIO)
    
);

CREATE TABLE CLIENTE_DIRECCION 
(
    CEDULA_CLIENTE VARCHAR(9) NOT NULL ,
    PROVINCIA VARCHAR(50) NOT NULL ,
    CANTON VARCHAR(50) NOT NULL ,
    DISTRITO VARCHAR(50) NOT NULL ,
    PRIMARY KEY (CEDULA_CLIENTE, PROVINCIA, CANTON, DISTRITO)
);

CREATE TABLE CLIENTE_TELEFONO 
(
    CEDULA_CLIENTE VARCHAR(9) NOT NULL ,
    TELEFONO VARCHAR(20) NOT NULL
);

CREATE TABLE INSUMO
(
    NOMBRE_INSUMO VARCHAR(50) NOT NULL PRIMARY KEY ,
    COSTO INT NOT NULL ,
    MARCA VARCHAR(20) NOT NULL ,
    CEDULA_JURIDICA_PROVEEDOR VARCHAR(20) NOT NULL

);

CREATE TABLE LAVADO
(
    NOMBRE_LAVADO VARCHAR(50) NOT NULL,
    COSTO_PERSONAL INT NOT NULL,
    PRECIO INT NOT NULL,
    DURACION INT NOT NULL,
    PUNTOS_OTORGADOS INT NOT NULL,
    COSTO_PUNTOS INT NOT NULL,
    PRIMARY KEY (NOMBRE_LAVADO)

);


CREATE TABLE LAVADO_INSUMO
(
    NOMBRE_LAVADO VARCHAR(50) NOT NULL ,
    NOMBRE_INSUMO VARCHAR(50) NOT NULL 
);

CREATE TABLE LAVADO_ROL
(
    NOMBRE_LAVADO VARCHAR(50) NOT NULL ,
    ID_ROL INT NOT NULL 
);

ALTER TABLE CLIENTE_DIRECCION
ADD CONSTRAINT FK_CLIENTE_DIRECCION
FOREIGN KEY (CEDULA_CLIENTE) REFERENCES CLIENTE(CEDULA_CLIENTE);

ALTER TABLE CLIENTE_TELEFONO
ADD CONSTRAINT FK_CLIENTE_TELEFONO
FOREIGN KEY (CEDULA_CLIENTE) REFERENCES CLIENTE(CEDULA_CLIENTE);

ALTER TABLE INSUMO
ADD CONSTRAINT FK_PROVEEDOR_INSUMO
FOREIGN KEY (CEDULA_JURIDICA_PROVEEDOR) REFERENCES PROVEEDOR(CEDULA_JURIDICA_PROVEEDOR);

ALTER TABLE LAVADO_INSUMO
ADD CONSTRAINT FK_LAVADO_INSUMO_NL
FOREIGN KEY (NOMBRE_LAVADO) REFERENCES LAVADO(NOMBRE_LAVADO);

ALTER TABLE LAVADO_INSUMO
ADD CONSTRAINT FK_LAVADO_INSUMO_NI
FOREIGN KEY (NOMBRE_INSUMO) REFERENCES INSUMO(NOMBRE_INSUMO);

ALTER TABLE LAVADO_ROL
ADD CONSTRAINT FK_LAVADO_ROL_NL
FOREIGN KEY (NOMBRE_LAVADO) REFERENCES LAVADO(NOMBRE_LAVADO);

ALTER TABLE LAVADO_ROL
ADD CONSTRAINT FK_LAVADO_ROL_IR
FOREIGN KEY (ID_ROL) REFERENCES ROL(ID_ROL);

ALTER TABLE CLIENTE
ADD CONSTRAINT NON_NEGATIVE_ACUM
CHECK (PUNTOS_ACUM >= 0);

ALTER TABLE CLIENTE
ADD CONSTRAINT NON_NEGATIVE_OBT
CHECK (PUNTOS_OBT >= 0);

ALTER TABLE CLIENTE
ADD CONSTRAINT NON_NEGATIVE_REDIM
CHECK (PUNTOS_REDIM >= 0);

