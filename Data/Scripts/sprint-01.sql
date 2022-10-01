CREATE TABLE ROL
(
    ID_ROL INT NOT NULL PRIMARY KEY ,
    TIPO VARCHAR(20) NOT NULL ,

);

CREATE TABLE TIPO_PAGO
(
    ID_TIPO_PAGO INT NOT NULL PRIMARY KEY ,
    TIPO VARCHAR(20) NOT NULL ,

);

CREATE TABLE TRABAJADOR 
(
    CEDULA_TRABAJADOR VARCHAR(9) NOT NULL PRIMARY KEY ,
    FECHA_NACIMIENTO DATE NOT NULL ,
    NOMBRE VARCHAR(20) NOT NULL ,
    PRIMER_APELLIDO VARCHAR(20) NOT NULL ,
    SEGUNDO_APELLIDO VARCHAR(20) NOT NULL ,
    ID_ROL INT NOT NULL,
    ID_PAGO INT NOT NULL,
    FECHA_INGRESO DATE NOT NULL ,
    PASSWORD_TRABAJADOR VARCHAR(20) NOT NULL
);

CREATE TABLE SUCURSAL 
(
    NOMBRE_SUCURSAL VARCHAR(20) NOT NULL PRIMARY KEY ,
    TELEFONO VARCHAR(20) NOT NULL ,
    CEDULA_TRABAJADOR_GERENTE VARCHAR(9) NOT NULL ,
    PROVINCIA VARCHAR(20) NOT NULL ,
    CANTON VARCHAR(20) NOT NULL ,
    DISTRITO VARCHAR(20) NOT NULL ,
    FECHA_APERTURA DATE NOT NULL ,
    FECHA_INICIO_GERENCIA DATE NOT NULL
);

CREATE TABLE PROVEEDOR 
(
    CEDULA_JURIDICA_PROVEEDOR VARCHAR(20) NOT NULL PRIMARY KEY ,
    NOMBRE VARCHAR(20) NOT NULL ,
    CONTACTO VARCHAR(20) NOT NULL  ,
    PROVINCIA VARCHAR(20) NOT NULL ,
    CANTON VARCHAR(20) NOT NULL ,
    DISTRITO VARCHAR(20) NOT NULL ,
    CORREO_ELECTRONICO VARCHAR(20) NOT NULL
    
);

CREATE TABLE INSUMO
(
    NOMBRE_INSUMO VARCHAR(20) NOT NULL PRIMARY KEY ,
    COSTO INT NOT NULL ,
    MARCA VARCHAR(20) NOT NULL ,
    CEDULA_JURIDICA_PROVEEDOR VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES PROVEEDOR(CEDULA_JURIDICA_PROVEEDOR)

);

CREATE TABLE CLIENTE 
(
    CEDULA_CLIENTE VARCHAR(9) NOT NULL PRIMARY KEY ,
    NOMBRE VARCHAR(20) NOT NULL ,
    PRIMER_APELLIDO VARCHAR(20) NOT NULL ,
    SEGUNDO_APELLIDO VARCHAR(20) NOT NULL ,
    CORREO_CLIENTE VARCHAR(20) NOT NULL,
    USUARIO VARCHAR(20) NOT NULL,
    PASSWORD_CLIENTE VARCHAR(20) NOT NULL,
    PUNTOS_ACUM INT NOT NULL,
    CONSTRAINT UNIQUE_LOGIN UNIQUE (CORREO_CLIENTE,USUARIO)
);

ALTER TABLE TRABAJADOR
ADD CONSTRAINT FK_ROL_TRABAJADOR
FOREIGN KEY (ID_ROL) REFERENCES ROL(ID_ROL);

ALTER TABLE TRABAJADOR
ADD CONSTRAINT FK_TIPOPAGO_TRABAJADOR
FOREIGN KEY (ID_PAGO) REFERENCES TIPO_PAGO(ID_TIPO_PAGO);

ALTER TABLE SUCURSAL
ADD CONSTRAINT FK_TRABAJADOR_SUCURSAL
FOREIGN KEY (CEDULA_TRABAJADOR_GERENTE) REFERENCES TRABAJADOR(CEDULA_TRABAJADOR);

ALTER TABLE INSUMO
ADD CONSTRAINT FK_PROVEEDOR_INSUMO
FOREIGN KEY (CEDULA_JURIDICA_PROVEEDOR) REFERENCES PROVEEDOR(CEDULA_JURIDICA_PROVEEDOR);
