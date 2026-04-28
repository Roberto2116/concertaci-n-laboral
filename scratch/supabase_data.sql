-- Supabase Data Migration Script

-- Data for Cat_SAP
INSERT INTO Cat_SAP (id_sap, Descripcion_sap) VALUES (1, E'SUAP Tampico');
INSERT INTO Cat_SAP (id_sap, Descripcion_sap) VALUES (2, E'SAP Poza rica');
INSERT INTO Cat_SAP (id_sap, Descripcion_sap) VALUES (3, E'SAP Monterrey');

SELECT setval(pg_get_serial_sequence('cat_sap', 'id_sap'), COALESCE((SELECT MAX(id_sap) FROM Cat_SAP), 1));

-- Data for Estatus
INSERT INTO Estatus (id_estatus, tipo_estatus, descripcion) VALUES (1, E'NUEVO', E'Nuevo asunto.');
INSERT INTO Estatus (id_estatus, tipo_estatus, descripcion) VALUES (2, E'EN PROCESO', E'Asunto trabajandose por el responsable.');
INSERT INTO Estatus (id_estatus, tipo_estatus, descripcion) VALUES (3, E'ATENDIDO', E'Asunto terminado por llegar al 100% de avance.');
INSERT INTO Estatus (id_estatus, tipo_estatus, descripcion) VALUES (4, E'NO PROCEDE', E'Asunto cancelado.');

SELECT setval(pg_get_serial_sequence('estatus', 'id_estatus'), COALESCE((SELECT MAX(id_estatus) FROM Estatus), 1));

-- Data for Organismos
INSERT INTO Organismos (id_organismo, Organismo, Descripcion) VALUES (1, E'PMXC', E'PEMEX CORPORATIVO');
INSERT INTO Organismos (id_organismo, Organismo, Descripcion) VALUES (2, E'PTRI', E'PEMEX TRANSFORMACION INDUSTRIAL');
INSERT INTO Organismos (id_organismo, Organismo, Descripcion) VALUES (3, E'PEP', E'PEMEX EXPLORACIÓN Y PRODUCCIÓN');
INSERT INTO Organismos (id_organismo, Organismo, Descripcion) VALUES (4, E'PLOG', E'PEMEX LOGÍSTICA');

SELECT setval(pg_get_serial_sequence('organismos', 'id_organismo'), COALESCE((SELECT MAX(id_organismo) FROM Organismos), 1));

-- Data for AS_CatSecSind
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (0, E'Administración');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (1, E'Cd. Madero, Tamaulipas');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (3, E'Altamira, Tamaulipas');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (13, E'Cerro azul, Veracruz');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (21, E'Arbol grande');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (25, E'Naranjos, Veracruz');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (30, E'Poza rica, Veracruz');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (33, E'Tampico, Tamaulipas');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (36, E'Reynosa, Tamaulipas');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (40, E'TAD''s zona Monterrey');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (49, E'Cadereyta, Nuevo Leon');
INSERT INTO AS_CatSecSind (SeccionSindical, Descripcion) VALUES (51, E'Tuxpan, Veracruz');

SELECT setval(pg_get_serial_sequence('as_catsecsind', 'seccionsindical'), COALESCE((SELECT MAX(SeccionSindical) FROM AS_CatSecSind), 1));

-- Data for Descripcion_corta
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (54, E'ACONDICIONAMIENTO DE INSTALACIONES');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (55, E'AFORE - JUB PEMEX');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (56, E'AGOTAMIENTO Y REACOMODO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (57, E'ASEGURADORA AZTECA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (58, E'AUSENTISMO PROLONGADO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (60, E'CAMBIO DE ATRIBUTOS DE PLAZAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (61, E'CAPACITACION');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (62, E'COBERTURA DE VACANTES');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (63, E'COMISION SINDICAL CLA. 251 CCTV');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (64, E'COMISIONES LOCALES MIXTAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (65, E'DESBLOQUEO DE PLAZAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (66, E'DOTACIÓN DE ROPA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (67, E'ESCALAFONES');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (68, E'HERRAMIENTA Y EQUIPO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (69, E'INCENTIVO A LA PERMANENCIA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (70, E'JUBILACION');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (71, E'PAGO CAMBIO DE SALARIO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (72, E'PAGO DE ARRASTRE Y COMIDAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (73, E'PAGO DE JORNADA ELECTORAL');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (74, E'PAGO DE VACACIONES');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (75, E'PAGO DE VIATICOS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (76, E'PAGO GUARDERIA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (77, E'PAGO INSALUBRE');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (78, E'PAGO JORNADAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (79, E'PAGO POR REEMBOLSO DE MEDICAMENTO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (80, E'PAGO PRIMA DE ANTIGÏEDAD');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (81, E'PAGO PROTESIS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (82, E'PAGO TIEMPO EXTRA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (83, E'PAGOS DESCENSO DE NIVEL');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (84, E'PAGOS JORNADAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (85, E'PERMUTA DEFINITIVA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (86, E'PRESTAMO ADMINISTRATIVO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (87, E'PRESTAMOS SINDICALES');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (88, E'PRIMA VACACIONAL');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (89, E'RECLAMO CONSECUENCIAS (MOV. DESC/SUP)');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (90, E'RECUPERACION DE SALARIOS Y PRESTACIONES');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (91, E'REPETICION DE RONDAS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (92, E'RIESGO DE TRABAJO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (93, E'SERVICIOS MEDICOS');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (94, E'SUBSIDIO POR ENFERMEDAD ORDINARIA');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (95, E'SUELDO A FAMILIARES DE TRAB.FALLECIDO');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (96, E'TIEMPO EXTRA / LAB. DESC. OBIT.');
INSERT INTO Descripcion_corta (id_descripcionCorta, tipo_descripcion) VALUES (97, E'TRANSPORTE DE PERSONAL');

SELECT setval(pg_get_serial_sequence('descripcion_corta', 'id_descripcioncorta'), COALESCE((SELECT MAX(id_descripcionCorta) FROM Descripcion_corta), 1));

-- Data for AS_CatGerencia
INSERT INTO AS_CatGerencia (Clave_gerencia, Descripcion, Gerencia) VALUES (200222110, E'Gerencia Regional de relaciones laborales y recursos Humanos Sureste', E'Sureste');
INSERT INTO AS_CatGerencia (Clave_gerencia, Descripcion, Gerencia) VALUES (200322120, E'Gerencia Regional de relaciones laborales y recursos Humanos Sur', E'Sur');
INSERT INTO AS_CatGerencia (Clave_gerencia, Descripcion, Gerencia) VALUES (200422130, E'Gerencia Regional de relaciones laborales y recursos Humanos Altiplano', E'Altiplano');
INSERT INTO AS_CatGerencia (Clave_gerencia, Descripcion, Gerencia) VALUES (200522140, E'Gerencia Regional de relaciones laborales y recursos Humanos norte', E'Norte');

SELECT setval(pg_get_serial_sequence('as_catgerencia', 'clave_gerencia'), COALESCE((SELECT MAX(Clave_gerencia) FROM AS_CatGerencia), 1));

-- Data for AS_CatCentros
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2005, E'PETRÓLEOS MEXICANOS, TAMPICO, TAMPS.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2006, E'PETRÓLEOS MEXICANOS, GUADALUPE, NL.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2007, E'PETRÓLEOS MEXICANOS, POZA RICA, VER.', 2, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2010, E'PETRÓLEOS MEXICANOS, CD. MADERO, TAMPS.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2016, E'PETRÓLEOS MEXICANOS, CHIHUAHUA, CHIH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2020, E'PETRÓLEOS MEXICANOS, CADEREYTA, NL.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2021, E'PETRÓLEOS MEXICANOS, REYNOSA, TAMPS.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2022, E'PETRÓLEOS MEXICANOS, GOMEZ PALACIO, DGO.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2067, E'PETRÓLEOS MEXICANOS, CAMARGO, CHIH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2068, E'PETRÓLEOS MEXICANOS, SAN RAFAEL, N. L.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2069, E'PETRÓLEOS MEXICANOS, TORREÓN, COAH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2070, E'PETRÓLEOS MEXICANOS, CERRO AZUL - NARANJOS, VER.', 2, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2071, E'PETRÓLEOS MEXICANOS, TUXPAN, VER.', 2, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2072, E'PETRÓLEOS MEXICANOS, ALTAMIRA,TAMPS.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2073, E'PETRÓLEOS MEXICANOS, CD. VICTORIA,TAMPS.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2074, E'PETRÓLEOS MEXICANOS, EBANO, SLP.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2075, E'PETRÓLEOS MEXICANOS, MONTERREY, NL.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2078, E'PETRÓLEOS MEXICANOS, MONCLOVA, COAH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2079, E'PETRÓLEOS MEXICANOS, CD JUÁREZ, CHIH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2080, E'PETRÓLEOS MEXICANOS, NUEVO LAREDO, TAMPS.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2087, E'PETRÓLEOS MEXICANOS, SALTILLO, COAH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2090, E'PETRÓLEOS MEXICANOS, SANTA CATARINA, N.L.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2094, E'PETRÓLEOS MEXICANOS, HUAUCHINANGO, PUE.', 2, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2096, E'PETRÓLEOS MEXICANOS, SAN LUIS POTOSÍ, SLP.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2102, E'PETRÓLEOS MEXICANOS, TAMPICO ALTO, VER.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2106, E'PETRÓLEOS MEXICANOS, PAPANTLA, VER.', 2, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2109, E'PETRÓLEOS MEXICANOS, CACALILAO, VER.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2110, E'PETRÓLEOS MEXICANOS, PÁNUCO, VER.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2114, E'PETRÓLEOS MEXICANOS, ESCOBEDO, N.L.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2123, E'PETRÓLEOS MEXICANOS, CIUDAD VALLESÍ, SLP.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2135, E'PETRÓLEOS MEXICANOS, EL MANTE, TAMPS .', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2136, E'PETRÓLEOS MEXICANOS, DURANGO, DGO.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2137, E'PETRÓLEOS MEXICANOS, HIDALGO DEL PARRAL, CHIH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2138, E'PETRÓLEOS MEXICANOS, SABINAS, COAH.', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2140, E'PETRÓLEOS MEXICANOS, NARANJOS AMATLÁN VER.', 2, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2150, E'PETRÓLEOS MEXICANOS, SAN FERNANDO, TAMPS .', 3, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (2152, E'PETRÓLEOS MEXICANOS, MATA REDONDA, VER.', 1, 1, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3003, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, POZA RICA, VER.', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3006, E'ACTIVO DE EXPLORACIÓN MARINA NORTE', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3007, E'ACTIVO DE EXPLORACIÓN TERRESTRE NORTE', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3018, E'ACTIVO DE PRODUCCIÓN REYNOSA', 3, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3019, E'ACTIVO DE PRODUCCIÓN POZA RICA-ALTAMIRA', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3022, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, REYNOSA, TAMPS.', 3, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3023, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, TAMPICO, TAMPS.', 1, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3028, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, ALTAMIRA, TAMPS.', 1, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3029, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, CERRO AZUL, VER.', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3032, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, HUAUCHINANGO, PUE.', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3033, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, TUXPAN, VER.', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3034, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, CIUDAD MADERO, TAMPS.', 1, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3036, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, EBANO,SLP.', 1, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3038, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, NUEVO LAREDO, TAMPS.', 3, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3040, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, NARANJOS, VER.', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3046, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, PÁNUCO, VER.', 1, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (3047, E'PEMEX EXPLORACIÓN Y PRODUCCIÓN, ÁLAMO, VER.', 2, 3, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4002, E'REFINERÍA DE MADERO', 1, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4003, E'REFINERÍA DE CADEREYTA', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4011, E'COMPLEJO PROCESADOR DE GAS BURGOS', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4016, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CADEREYTA, NL.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4017, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, POZA RICA, VER.', 2, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4018, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, ARENQUE, TAMPS.', 1, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4021, E'COMPLEJO PROCESADOR DE GAS ARENQUE', 1, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4024, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CD. MADERO, TAMPS.', 1, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4025, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, BURGOS, TAMPS.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4038, E'COMPLEJO PROCESADOR DE GAS POZA RICA', 2, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4048, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CHIHUAHUA, CHI.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4049, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CD. JUÁREZ, CHI.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4050, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CD. VALLES, SLP.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4051, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CD. VICTORIA, TAMPS.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4054, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, DURANGO, DGO.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4058, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, GÓMEZ PALACIO, DGO.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4069, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, MONCLOVA, COAH.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4071, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, NUEVO LAREDO, TAMPS.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4078, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, REYNOSA, TAMPS.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4080, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, SALTILLO, COAH.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4083, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, SANTA CATARINA, NL.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4098, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, PIEDRAS NEGRAS, COAH.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4100, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, MONTERREY, NL.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4102, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CAMARGO, CHI.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4105, E'PEMEX TRANSFORMACIÓN INDUSTRIAL,TUXPAN, VER.', 2, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4107, E'PEMEX TRNSFORMACIÓN INDUSTRIAL,ESCOLIN,VER.', 2, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4109, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, CD. MANTE,TAMPS.', 1, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4110, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, HIDALGO DEL PARRAL, CHI.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (4112, E'PEMEX TRANSFORMACIÓN INDUSTRIAL, SABINAS, COAH.', 3, 2, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5005, E'PEMEX LOGÍSTICA POZA RICA, VER.', 2, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5007, E'PEMEX LOGÍSTICA SANTA CATARINA, N.L.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5009, E'PEMEX LOGÍSTICA, BURGOS, TAMPS.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5010, E'PEMEX LOGÍSTICA ALTAMIRA, TAMPS.', 1, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5014, E'PEMEX LOGÍSTICA, CHIHUAHUA, CHIH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5015, E'PEMEX LOGÍSTICA, CD. MADERO, TAMPS.', 1, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5016, E'PEMEX LOGÍSTICA MONTERREY, NL.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5017, E'PEMEX LOGÍSTICA, TORREÓN, COAH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5018, E'PEMEX LOGÍSTICA, REYNOSA, TAMPS.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5019, E'PEMEX LOGÍSTICA, CD. VICTORIA,TAMPS.', 1, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5037, E'PEMEX LOGÍSTICA CADEREYTA JIMÉNEZ, NL.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5038, E'PEMEX LOGÍSTICA MONCLOVA, COAH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5039, E'PEMEX LOGÍSTICA NUEVO LAREDO, TAMPS.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5040, E'PEMEX LOGÍSTICA SABINAS, COAH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5067, E'PEMEX LOGÍSTICA, SALTILLO, COAH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5068, E'PEMEX LOGÍSTICA, CD. JUÁREZ, CHIH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5069, E'PEMEX LOGÍSTICA, GOMEZ PALACIO, DGO.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5070, E'PEMEX LOGÍSTICA, DURANGO, DGO.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5071, E'PEMEX LOGÍSTICA, HIDALGO DEL PARRAL, CHIH.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5074, E'PEMEX LOGÍSTICA, CD. MANTE, TAMPS.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5075, E'PEMEX LOGÍSTICA, CD. VALLES, SLP.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5077, E'PEMEX LOGÍSTICA, MATEHUALA, SLP.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5084, E'PEMEX LOGÍSTICA, HERMOSILLO, SON.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5096, E'PEMEX LOGÍSTICA, TUXPAN, VER.', 2, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5102, E'PEMEX LOGÍSTICA, GUADALUPE, N.L.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5110, E'PEMEX LOGÍSTICA SAN FERNANDO, TAMPS.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5116, E'PEMEX LOGÍSTICA, SAN PEDRO GARZA GARCIA, N.L.', 3, 4, 200522140);
INSERT INTO AS_CatCentros (clave_centro, Desc_centroTrabajo, id_sap, Id_organismo, clave_gerencia) VALUES (5117, E'PEMEX LOGÍSTICA CERRO AZUL, VER.', 2, 4, 200522140);

SELECT setval(pg_get_serial_sequence('as_catcentros', 'clave_centro'), COALESCE((SELECT MAX(clave_centro) FROM AS_CatCentros), 1));

-- Data for Subgerencia
INSERT INTO Subgerencia (Clave_subgerencia, nombre_Subgerencia, descripcion, clave_gerencia) VALUES (200522141, E'OAON', E'OFICINA DE ADMINISTRACION DE LA OPERACION NORTE', 200522140);
INSERT INTO Subgerencia (Clave_subgerencia, nombre_Subgerencia, descripcion, clave_gerencia) VALUES (200522142, E'SUAP TAMPICO', E'SUPERINTENDENCIA DE ADMINISTRACION DE PERSONAL TAMPICO', 200522140);
INSERT INTO Subgerencia (Clave_subgerencia, nombre_Subgerencia, descripcion, clave_gerencia) VALUES (200522188, E'SRIDH', E'SUBGERENCIA REGIONAL DE INTEGRACION Y DESARROLLO HUMANO', 200522140);
INSERT INTO Subgerencia (Clave_subgerencia, nombre_Subgerencia, descripcion, clave_gerencia) VALUES (200522190, E'SRCL NORTE', E'SUBGERENCIA REGIONAL DE CONCERTACION LABORAL NORTE', 200522140);
INSERT INTO Subgerencia (Clave_subgerencia, nombre_Subgerencia, descripcion, clave_gerencia) VALUES (200722142, E'SAP POZA RICA', E'SUBGERENCIA DE ADMINISTRACION DE PERSONAL POZA RICA', 200522140);
INSERT INTO Subgerencia (Clave_subgerencia, nombre_Subgerencia, descripcion, clave_gerencia) VALUES (209022142, E'SAP MONTERREY', E'SUBGERENCIA DE ADMINISTRACION DE PERSONAL MONTERREY', 200522140);

SELECT setval(pg_get_serial_sequence('subgerencia', 'clave_subgerencia'), COALESCE((SELECT MAX(Clave_subgerencia) FROM Subgerencia), 1));

-- Data for Dep_personal
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200522141, E'OAON', E'OFICINA DE ADMINISTRACION DE LA OPERACION NORTE', 200522141);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200522142, E'SUAP TAMPICO', E'SUAP TAMPICO', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200522146, E'AOS TAMPICO', E'AREA DE OPERACION Y SERVICIOS TAMPICO', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200522188, E'SRIDH', E'SUBGERENCIA REGIONAL DE INTEGRACION Y DESARROLLO HUMANO NORTE', 200522188);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200522190, E'SRCL NORTE', E'SUBGERENCIA REGIONAL DE CONCERTACION LABORAL NORTE', 200522190);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200522192, E'GESTION LABORAL TAMPICO', E'SUPERINTENDENCIA DE GESTION LABORAL TAMPICO', 200522190);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200722142, E'SAP POZA RICA', E'SAP POZA RICA', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200722143, E'DP POZA RICA', E'DEPARTAMENTO DE PERSONAL POZA RICA', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200722144, E'DP CPG POZA RICA', E'DEPARTAMENTO DE PERSONAL CPG POZA RICA', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200722146, E'CSP DUCTOS POZA RICA', E'CENTRO DE SERVICIOS AL PERSONAL DUCTOS POZA RICA', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200722147, E'CSP HOSPITAL REGIONAL POZA RICA', E'CENTRO DE SERVICIOS AL PERSONAL HOSPITAL REGIONAL POZA RICA', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (200722191, E'GESTION LABORAL POZA RICA', E'SUPERINTENDENCIA DE GESTION LABORAL POZA RICA', 200522190);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (201022143, E'DP TERMINAL MADERO', E'DEPARTAMENTO DE PERSONAL TERMINAL MADERO', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (201022144, E'DP MADERO', E'DEPARTAMENTO DE PERSONAL MADERO', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (201022148, E'DP HOSPITAL REGIONAL MADERO', E'DEPARTAMENTO DE PERSONAL HOSPITAL REGIONAL MADERO', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (201622146, E'AOS CHIHUAHUA - CAMARGO', E'AREA DE OPERACION Y SERVICIOS CHIHUAHUA - CAMARGO', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (202022143, E'DP CADEREYTA', E'DEPARTAMENTO DE PERSONAL CADEREYTA', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (202022144, E'AOS HOSPITAL CADEREYTA', E'AREA DE OPERACION Y SERVICIOS HOSPITAL CADEREYTA', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (202122143, E'DP CPG BURGOS', E'DEPARTAMENTO DE PERSONAL CPG BURGOS', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (202122144, E'DP ACTIVO REYNOSA', E'DEPARTAMENTO DE PERSONAL ACTIVO REYNOSA', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (202122145, E'CSP HOSPITAL REGIONAL REYNOSA', E'CENTRO DE SERVICIOS AL PERSONAL HOSPITAL REGIONAL REYNOSA', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (206822146, E'CSP SAN RAFAEL', E'CENTRO DE SERVICIOS AL PERSONAL SAN RAFAEL', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (206922146, E'AOS TORREON', E'AREA DE OPERACION Y SERVICIOS TORREON', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (207022145, E'CSP CERRO AZUL', E'CENTRO DE SERVICIOS AL PERSONAL CERRO AZUL', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (207022146, E'CSP NARANJOS', E'CENTRO DE SERVICIOS AL PERSONAL NARANJOS', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (207122146, E'AOS TUXPAN', E'AREA DE OPERACION Y SERVICIOS TUXPAN', 200722142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (207222143, E'DP ALTAMIRA', E'DEPARTAMENTO DE PERSONAL ALTAMIRA', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (207322144, E'AOS SECTOR DUCTOS CIUDAD VICTORIA', E'AREA DE OPERACION Y SERVICIOS SECTOR DUCTOS VICTORIA', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (207422149, E'AOS HOSPITAL GENERAL EBANO', E'AREA DE OPERACION Y SERVICIOS HOSPITAL GENERAL EBANO', 200522142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (209022142, E'SAP MONTERREY', E'SAP MONTERREY', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (209022143, E'DP MONTERREY', E'DEPARTAMENTO DE PERSONAL MONTERREY', 209022142);
INSERT INTO Dep_personal (clave_depto, Nombre_Dep, descripcion, clave_subgerencia) VALUES (209022191, E'GESTION LABORAL MONTERREY', E'SUPERINTENDENCIA DE GESTION LABORAL MONTERREY', 200522190);

SELECT setval(pg_get_serial_sequence('dep_personal', 'clave_depto'), COALESCE((SELECT MAX(clave_depto) FROM Dep_personal), 1));

-- Data for usuario
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'admin', E'Admin', E'1234', E'SUBGERENTE', 200522142, 79, E'ADMINISTRADOR', E'2026-04-09 12:10:46', E'admin@sistema.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'1001', E'JUAN PEREZ (MISMO DEPTO)', E'1234', E'Operativo', 200522142, 0, E'Usuario', E'2026-03-06 12:03:12', E'juan@sistema.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'2002', E'MARIA LOPEZ (DP MONTERREY)', E'1234', E'Operativo', 209022143, 0, E'Usuario', E'2026-03-09 10:59:22', E'maria@sistema.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'2116', E'beto', E'2112', E'Operativo', 200522142, 3, E'Admin', E'2026-04-08 11:43:15', E'beto@sistema.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'2121', E'user de prueba', E'2121', E'operativo', 200522142, 0, E'Operativo', E'2026-03-30 10:07:01', E'user@prueba.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'4321', E'juan', E'eCjZ/8keXEz9D2/m7ecIu2ccI0lhRy0L6phq/UVaVfA=:/ye/hxx1QKVi7EAEMgqXlw==', E'Operativo', 200522142, 0, E'Operativo', E'2026-04-01 11:11:24', E'juan@pemex.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'jefe1', E'Usuario Jefe', E'1234', E'JEFE', 200522142, 0, E'OPERADOR', E'2026-04-23 12:22:37', E'jefe@test.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'super1', E'Usuario Super', E'1234', E'SUPERINTENDENTE', 200522142, 0, E'ADMINISTRADOR', E'2026-04-23 12:22:37', E'super@test.com', 1);
INSERT INTO usuario (Ficha, nombre, contraseña, Estrato, clave_depto, contador, tipo, fecha_ultimaEntrada, Correo, estatus) VALUES (E'gerente1', E'Usuario Gerente', E'1234', E'GERENTE', 200522142, 0, E'ADMINISTRADOR', E'2026-04-23 12:22:37', E'gerente@test.com', 1);

-- Data for Asuntos
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (1, E'2026-03-06', E'OPERATIVO', E'PRUEBA', E'2026-03-06', 3, 209022143, E'SINDICAL', 0, 1, 2020, 60, 3, E'admin', E'SOLO ES DE PRUEBA (INDICAR QUE CAMBIOS REALIZAR PARA MODIFICAR TODO)', E'CUALQUIER DETALLE QUE NO ESTE EN BASE A LO QUE SE PIDA SE ACTUALIZA EN BASE AL DESEO DEL USUARIO', E'2026-03-24', E'2026-03-20', 100, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (2, E'2026-03-09', E'LEGAL', E'PRUEBA 2', E'2026-03-10', 2, 200722142, E'SINDICAL', 30, 1, 2007, 54, 3, E'admin', E'SE ESTA COMPROBANDO FUNCIONALIDAD DEL SISTEMA', NULL, E'2026-03-20', E'2026-03-30', 100, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (13, E'2026-03-01', E'ADMINISTRATIVO', E'AFDS', E'2026-03-03', 2, 200722146, E'SINDICAL', 30, 1, 2007, 56, 2, E'admin', E'ASDSAD', E'ADFSDFDA', E'2026-03-25', E'2026-03-24', 97, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (14, E'2026-03-11', E'ADMINISTRATIVO', E'ADFADF', E'2026-03-13', 2, 200722143, E'ADMINISTRATIVA', 30, 1, 2007, 63, 2, E'admin', E'ADSFDF', E'ADFSADF', E'2026-03-20', E'2026-03-15', 63, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (15, E'2026-03-11', E'ADMINISTRATIVO', E'WFSDF', E'2026-03-11', 1, 200522142, E'ADMINISTRATIVA', 1, 1, 2005, 56, 2, E'admin', E'DSFSAF', E'WEFWQ', E'2026-03-27', E'2026-03-14', 79, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (16, E'2026-03-19', E'ADMINISTRATIVO', E'THNFGFDSAGS', E'2026-03-19', 3, 209022142, E'ADMINISTRATIVA', 49, 1, 2006, 56, 3, E'admin', E'SDGFSDG', E'FGFDG', E'2026-03-24', E'2026-03-20', 100, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (17, E'2026-03-20', E'ADMINISTRATIVO', E'GFDSF', E'2026-03-20', 1, 200522142, E'ADMINISTRATIVA', 1, 1, 2010, 66, 2, E'admin', E'FDSA', E'FDSGG', E'2026-04-01', E'2026-03-31', 95, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (18, E'2026-04-01', E'OPERATIVO', E'DSADSAD', E'2026-04-01', 1, 200522142, E'ADMINISTRATIVA', 33, 1, 2010, 55, 2, E'admin', NULL, NULL, E'2026-04-08', E'2026-04-15', 60, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (19, E'2026-04-01', E'LEGAL', E'EOI', E'2026-04-01', 1, 200522142, E'ADMINISTRATIVA', 1, 1, 2010, 54, 2, E'admin', NULL, NULL, E'2026-04-06', E'2026-04-15', 90, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (20, E'2026-04-01', E'ADMINISTRATIVO', E'NOMBRE', E'2026-04-07', 3, 202122144, E'ADMINISTRATIVA', 36, 1, 2021, 67, 2, E'2116', NULL, NULL, E'2026-04-08', E'2026-05-01', 41, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (21, E'2026-04-01', E'ADMINISTRATIVO', E'GFCHGJF', E'2026-04-02', 3, 207122146, E'ADMINISTRATIVA', 13, 1, 2006, 93, 2, E'admin', NULL, NULL, E'2026-04-08', E'2026-04-21', 10, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (22, E'2026-04-02', E'Caso', E'DAF', E'2026-04-03', 3, 202122144, E'ADMINISTRATIVA', 40, 1, 2069, 66, 1, E'2116', E'AR', E'AFSD', NULL, E'2026-04-10', 0, 1);
INSERT INTO Asuntos (id_asunto, Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, Instruccion, Observaciones, Fecha_atencion, Fecha_Compromiso, Porcentaje_avance, Eliminado) VALUES (23, E'2026-04-09', E'CASO', E'DSADAS', E'2026-04-10', 3, 200522188, E'ADMINISTRATIVA', 30, 1, 2016, 57, 1, E'admin', E'SDFSDF', E'FASDFSDF', NULL, E'2026-04-13', 0, 1);

SELECT setval(pg_get_serial_sequence('asuntos', 'id_asunto'), COALESCE((SELECT MAX(id_asunto) FROM Asuntos), 1));

-- Data for Seguimiento
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (5, 15, E'FSDFSDG', E'2026-03-19 10:04:11');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (6, 15, E'CVN', E'2026-03-19 10:15:32');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (7, 14, E'GSFG', E'2026-03-19 10:15:39');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (8, 14, E'FDSJFSDJFODSIJFDFAFASDASDF', E'2026-03-19 10:16:17');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (9, 15, E'FSDGFG', E'2026-03-19 10:51:26');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (10, 15, E'RWERWET', E'2026-03-19 11:39:45');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (11, 14, E'DGFVC', E'2026-03-19 11:40:23');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (12, 16, E'CXFGHTER', E'2026-03-19 12:16:46');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (13, 16, E'GDSF', E'2026-03-20 09:19:50');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (14, 15, E'SDF', E'2026-03-20 10:37:58');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (15, 1, E'DSFASD', E'2026-03-20 10:38:06');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (16, 2, E'DSFA', E'2026-03-20 10:38:15');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (17, 13, E'DSFSDF', E'2026-03-20 10:38:23');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (18, 17, E'KSDJFAÑLK', E'2026-03-23 11:45:23');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (19, 17, E'AVANCE NUEVO', E'2026-03-24 09:12:28');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (20, 15, E'CAMBIO DETALLE', E'2026-03-25 09:23:08');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (21, 13, E'APUNTO DE TERMINAR DETALLE COMPLETO', E'2026-03-25 09:23:45');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (22, 17, E'SDFHGFDS', E'2026-04-01 11:45:28');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (23, 20, E'AVANCE 1', E'2026-04-08 11:44:32');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (24, 18, E'ANOTANDO EL AVANCE AL 60%', E'2026-04-08 11:45:16');
INSERT INTO Seguimiento (id_seguimiento, num_Asunto, Descripcion, fecha_Seguimiento) VALUES (25, 21, E'SGDSGFD', E'2026-04-08 12:49:54');

SELECT setval(pg_get_serial_sequence('seguimiento', 'id_seguimiento'), COALESCE((SELECT MAX(id_seguimiento) FROM Seguimiento), 1));

-- Data for Mensaje
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (1, E'admin', E'BUEN DIA', E'2026-03-06 09:58:08', E'PERSONAL', E'1001', NULL, 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (2, E'admin', E'BUEN DIA GENERAL', E'2026-03-06 09:58:27', E'SAP', NULL, NULL, 1);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (3, E'1001', E'MARIA', E'06/03/2026 10:22', E'PERSONAL', E'2002', NULL, 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (4, E'1001', E'TODOS', E'06/03/2026 10:23', E'SAP', NULL, NULL, 1);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (5, E'admin', E'BUEN DIA DEPARTAMENTO', E'2026-03-06 10:49:54', E'DEPTO', NULL, E'209022143', 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (6, E'admin', E'MHTDMHJFJMH', E'2026-03-06 12:02:24', E'PERSONAL', E'1001', NULL, 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (7, E'admin', E'HG,JGJ', E'2026-03-06 12:04:13', E'SAP', NULL, NULL, 1);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (8, E'admin', E'FUNCIONA', E'2026-03-11 10:18:19', E'SAP', NULL, NULL, 1);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (9, E'admin', E'SDF', E'2026-03-11 10:18:57', E'SAP', NULL, NULL, 1);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (10, E'admin', E'SDFSF', E'2026-03-11 10:29:59', E'SAP', NULL, NULL, 1);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (11, E'admin', E'PRUEBA DE MENSAJE POSTEADO CON TEMPORIZADOR 1', E'2026-03-11 10:30:26', E'SAP', NULL, NULL, 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (12, E'admin', E'PRUEBA DE QUE EL MENSAJE SALE BIEN A QUIEN VA DIRIGIDO', E'2026-03-24 11:04:58', E'PERSONAL', E'2116', NULL, 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (13, E'admin', E'BUEN DIA DEPARTAMENTO', E'2026-03-25 09:41:26', E'DEPTO', NULL, E'200522142', 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (14, E'admin', E'FSDAGFAFS', E'2026-03-25 09:41:39', E'PERSONAL', E'admin', NULL, 0);
INSERT INTO Mensaje (id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino, Archivado) VALUES (15, E'admin', E'HOLA', E'2026-04-08 11:43:43', E'PERSONAL', E'2116', NULL, 0);

SELECT setval(pg_get_serial_sequence('mensaje', 'id_mensaje'), COALESCE((SELECT MAX(id_mensaje) FROM Mensaje), 1));

-- Data for Bitacora_Sesiones
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (1, E'admin', E'2026-03-31 11:46:32', E'2026-03-31 11:49:09');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (2, E'admin', E'2026-03-31 11:58:07', E'2026-03-31 12:01:08');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (3, E'admin', E'2026-03-31 12:16:02', E'2026-03-31 12:53:46');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (4, E'admin', E'2026-04-01 09:13:56', E'2026-04-01 10:03:11');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (5, E'admin', E'2026-04-01 10:09:01', E'2026-04-01 10:21:08');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (6, E'admin', E'2026-04-01 10:22:27', E'2026-04-01 11:02:33');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (7, E'admin', E'2026-04-01 11:03:08', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (8, E'admin', E'2026-04-01 11:07:09', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (9, E'admin', E'2026-04-01 11:47:18', E'2026-04-01 12:57:39');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (10, E'admin', E'2026-04-06 09:00:14', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (11, E'admin', E'2026-04-06 09:28:37', E'2026-04-06 09:29:01');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (12, E'admin', E'2026-04-06 09:38:54', E'2026-04-06 09:50:26');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (13, E'admin', E'2026-04-06 09:52:32', E'2026-04-06 10:17:11');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (14, E'admin', E'2026-04-06 10:29:03', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (15, E'admin', E'2026-04-06 10:30:25', E'2026-04-06 10:34:55');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (16, E'admin', E'2026-04-06 10:39:24', E'2026-04-06 10:42:55');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (17, E'admin', E'2026-04-06 10:55:35', E'2026-04-06 11:08:08');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (18, E'admin', E'2026-04-06 11:13:52', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (19, E'admin', E'2026-04-06 11:34:35', E'2026-04-06 12:39:09');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (20, E'admin', E'2026-04-07 08:59:11', E'2026-04-07 10:00:17');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (21, E'admin', E'2026-04-07 10:09:27', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (22, E'admin', E'2026-04-07 10:11:39', E'2026-04-07 10:12:58');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (23, E'admin', E'2026-04-07 10:13:07', E'2026-04-07 10:13:30');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (24, E'admin', E'2026-04-07 10:15:12', E'2026-04-07 10:18:49');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (25, E'admin', E'2026-04-07 10:20:33', E'2026-04-07 10:26:15');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (26, E'admin', E'2026-04-07 10:31:28', E'2026-04-07 10:41:36');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (27, E'admin', E'2026-04-07 10:54:07', E'2026-04-07 11:23:53');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (28, E'admin', E'2026-04-07 11:25:44', E'2026-04-07 12:44:49');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (29, E'admin', E'2026-04-08 09:01:18', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (30, E'admin', E'2026-04-08 09:29:52', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (31, E'2116', E'2026-04-08 11:24:29', E'2026-04-08 11:24:58');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (32, E'admin', E'2026-04-08 11:30:42', E'2026-04-08 11:38:54');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (33, E'admin', E'2026-04-08 11:42:40', E'2026-04-08 11:46:47');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (34, E'2116', E'2026-04-08 11:43:15', E'2026-04-08 11:46:50');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (35, E'admin', E'2026-04-08 12:39:55', E'2026-04-08 12:40:12');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (36, E'admin', E'2026-04-08 12:41:22', E'2026-04-08 14:16:47');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (37, E'admin', E'2026-04-08 20:38:12', E'2026-04-08 20:41:49');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (38, E'admin', E'2026-04-08 20:42:07', E'2026-04-08 20:45:16');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (39, E'admin', E'2026-04-08 20:45:54', E'2026-04-08 20:48:13');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (40, E'admin', E'2026-04-09 08:57:08', E'2026-04-09 09:01:21');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (41, E'admin', E'2026-04-09 09:01:50', E'2026-04-09 09:07:06');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (42, E'admin', E'2026-04-09 09:07:18', E'2026-04-09 09:11:37');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (43, E'admin', E'2026-04-09 09:12:05', E'2026-04-09 09:22:53');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (44, E'admin', E'2026-04-09 09:23:44', E'2026-04-09 09:26:13');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (45, E'admin', E'2026-04-09 09:27:08', E'2026-04-09 09:27:11');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (46, E'admin', E'2026-04-09 09:29:54', E'2026-04-09 09:33:55');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (47, E'admin', E'2026-04-09 09:34:11', E'2026-04-09 09:38:55');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (48, E'admin', E'2026-04-09 09:44:12', E'2026-04-09 09:49:15');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (49, E'admin', E'2026-04-09 09:49:27', E'2026-04-09 09:51:59');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (50, E'admin', E'2026-04-09 09:52:11', E'2026-04-09 09:54:16');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (51, E'admin', E'2026-04-09 09:58:45', E'2026-04-09 10:03:35');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (52, E'admin', E'2026-04-09 10:05:12', E'2026-04-09 10:11:08');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (53, E'admin', E'2026-04-09 10:13:46', E'2026-04-09 10:14:14');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (54, E'admin', E'2026-04-09 10:16:41', E'2026-04-09 10:20:58');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (55, E'admin', E'2026-04-09 10:28:09', E'2026-04-09 10:29:30');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (56, E'admin', E'2026-04-09 10:40:43', E'2026-04-09 10:42:05');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (57, E'admin', E'2026-04-09 10:43:42', E'2026-04-09 10:49:57');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (58, E'admin', E'2026-04-09 10:51:27', E'2026-04-09 10:55:21');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (59, E'admin', E'2026-04-09 10:56:38', E'2026-04-09 11:04:06');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (60, E'admin', E'2026-04-09 11:04:19', E'2026-04-09 11:11:02');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (61, E'admin', E'2026-04-09 11:12:37', E'2026-04-09 11:14:32');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (62, E'admin', E'2026-04-09 11:17:35', E'2026-04-09 11:17:46');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (63, E'admin', E'2026-04-09 11:33:43', E'2026-04-09 11:37:55');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (64, E'admin', E'2026-04-09 11:38:12', E'2026-04-09 11:42:30');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (65, E'admin', E'2026-04-09 11:42:50', E'2026-04-09 11:46:13');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (66, E'admin', E'2026-04-09 11:46:37', E'2026-04-09 11:50:13');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (67, E'admin', E'2026-04-09 11:51:14', NULL);
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (68, E'admin', E'2026-04-09 11:55:58', E'2026-04-09 11:56:42');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (69, E'admin', E'2026-04-09 12:00:25', E'2026-04-09 12:00:47');
INSERT INTO Bitacora_Sesiones (Id_Sesion, Ficha_Usuario, Fecha_Entrada, Fecha_Salida) VALUES (70, E'admin', E'2026-04-09 12:10:46', E'2026-04-09 12:10:48');

SELECT setval(pg_get_serial_sequence('bitacora_sesiones', 'id_sesion'), COALESCE((SELECT MAX(Id_Sesion) FROM Bitacora_Sesiones), 1));

