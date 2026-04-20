-- Script de creación de tablas para PostgreSQL (Supabase)

CREATE TABLE Cat_SAP (
    id_sap SERIAL PRIMARY KEY,
    Descripcion_sap TEXT 
);

CREATE TABLE Estatus (
    id_estatus SERIAL PRIMARY KEY,
    tipo_estatus TEXT,
    descripcion TEXT
);

CREATE TABLE Organismos (
    id_organismo SERIAL PRIMARY KEY,
    Organismo TEXT,
    Descripcion TEXT
);

CREATE TABLE AS_CatSecSind (
    SeccionSindical INTEGER PRIMARY KEY,
    Descripcion TEXT
);

CREATE TABLE Descripcion_corta (
    id_descripcionCorta SERIAL PRIMARY KEY,
    tipo_descripcion TEXT
);

CREATE TABLE AS_CatGerencia (
    Clave_gerencia INTEGER PRIMARY KEY,
    Descripcion TEXT,
    Gerencia TEXT
);

CREATE TABLE AS_CatCentros (
    clave_centro INTEGER PRIMARY KEY,
    Desc_centroTrabajo TEXT,
    id_sap INTEGER,
    Id_organismo INTEGER,
    clave_gerencia INTEGER,
    FOREIGN KEY (id_sap) REFERENCES Cat_SAP(id_sap),
    FOREIGN KEY (Id_organismo) REFERENCES Organismos(id_organismo),
    FOREIGN KEY (clave_gerencia) REFERENCES AS_CatGerencia(Clave_gerencia)
);

CREATE TABLE Subgerencia (
    Clave_subgerencia INTEGER PRIMARY KEY,
    nombre_Subgerencia TEXT,
    descripcion TEXT,               
    clave_gerencia INTEGER,
    FOREIGN KEY (clave_gerencia) REFERENCES AS_CatGerencia(Clave_gerencia)
);

CREATE TABLE Dep_personal (
    clave_depto INTEGER PRIMARY KEY,
    Nombre_Dep TEXT,
    descripcion TEXT,               
    clave_subgerencia INTEGER,
    FOREIGN KEY (clave_subgerencia) REFERENCES Subgerencia(Clave_subgerencia)
);

CREATE TABLE usuario (
    Ficha TEXT PRIMARY KEY, 
    nombre TEXT NOT NULL,
    contraseña TEXT NOT NULL,
    Estrato TEXT NOT NULL,
    clave_depto INTEGER NOT NULL,
    contador INTEGER NOT NULL DEFAULT 0,
    tipo TEXT NOT NULL,
    fecha_ultimaEntrada TEXT NOT NULL, 
    Correo TEXT NOT NULL, 
    estatus INTEGER DEFAULT 1,
    FOREIGN KEY (clave_depto) REFERENCES Dep_personal(clave_depto)
);

CREATE TABLE Asuntos (
    id_asunto SERIAL PRIMARY KEY,
    Fecha_recepcion TEXT NOT NULL,
    Tipo TEXT NOT NULL, 
    Nombre_oficio TEXT NOT NULL DEFAULT 'S/N', 
    Fecha_oficio TEXT NOT NULL,
    id_sap INTEGER NOT NULL,
    clave_depto INTEGER NOT NULL,
    Agenda TEXT NOT NULL,
    Sec_Sindical INTEGER NOT NULL,
    Id_Organismo INTEGER NOT NULL,
    clave_centroTrabajo INTEGER NOT NULL,
    id_descripcionCorta INTEGER NOT NULL,
    Id_estatus INTEGER NOT NULL,
    Ficha TEXT NOT NULL, 
    Instruccion TEXT,      
    Observaciones TEXT,    
    Fecha_atencion TEXT,   
    Fecha_Compromiso TEXT NOT NULL,
    Porcentaje_avance INTEGER NOT NULL DEFAULT 0,
    Eliminado INTEGER DEFAULT 0,
    FOREIGN KEY (id_sap) REFERENCES Cat_SAP(id_sap),
    FOREIGN KEY (clave_depto) REFERENCES Dep_personal(clave_depto),
    FOREIGN KEY (Sec_Sindical) REFERENCES AS_CatSecSind(SeccionSindical),
    FOREIGN KEY (Id_Organismo) REFERENCES Organismos(id_organismo),
    FOREIGN KEY (clave_centroTrabajo) REFERENCES AS_CatCentros(clave_centro),
    FOREIGN KEY (id_descripcionCorta) REFERENCES Descripcion_corta(id_descripcionCorta),
    FOREIGN KEY (Id_estatus) REFERENCES Estatus(id_estatus),
    FOREIGN KEY (Ficha) REFERENCES usuario(Ficha)
);

CREATE TABLE Seguimiento (
    id_seguimiento SERIAL PRIMARY KEY,
    num_Asunto INTEGER NOT NULL, 
    Descripcion TEXT NOT NULL,
    fecha_Seguimiento TEXT NOT NULL,
    FOREIGN KEY (num_Asunto) REFERENCES Asuntos(id_asunto) ON DELETE CASCADE
);

CREATE TABLE Mensaje (
    id_mensaje SERIAL PRIMARY KEY,
    ficha TEXT,
    mensaje_enviado TEXT,
    fecha_posteo TEXT,
    Tipo_alcance TEXT,
    Ficha_destino TEXT,
    clave_depto_destino INTEGER, 
    Archivado INTEGER DEFAULT 0, 
    FOREIGN KEY (clave_depto_destino) REFERENCES Dep_personal(clave_depto)
);

CREATE TABLE Bitacora_Sesiones (
    Id_Sesion SERIAL PRIMARY KEY,
    Ficha_Usuario TEXT NOT NULL,
    Fecha_Entrada TEXT NOT NULL,
    Fecha_Salida TEXT NULL,
    FOREIGN KEY (Ficha_Usuario) REFERENCES usuario(Ficha)
);
