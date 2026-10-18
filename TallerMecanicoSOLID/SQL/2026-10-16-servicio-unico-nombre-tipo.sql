-- Ejecutar una sola vez sobre la base de datos taller_mecanico_v2.
-- Comprobar antes que no existan pares duplicados:
-- SELECT nombre, tipo, COUNT(*) AS cantidad
-- FROM servicio GROUP BY nombre, tipo HAVING COUNT(*) > 1;
-- Si la consulta devuelve filas, resolver esos duplicados antes de continuar.
ALTER TABLE servicio
    ADD CONSTRAINT uq_servicio_nombre_tipo UNIQUE (nombre, tipo);
