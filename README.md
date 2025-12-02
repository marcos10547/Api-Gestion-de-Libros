# Setup local db (Sql server)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04
# Api-Gestion-de-Libros
Esta aplicación de consola gestionará el catálogo de libros y los datos de autores de una pequeña editorial. Es un tema que permite establecer relaciones claras, múltiples atributos y diversas opciones de filtrado.
# Comando simple

#Setup local db (Sql server)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04

Guía para levantar tu entorno (API + SQL Server)

Actualmente tienes dos definiciones de SQL Server. Vamos a priorizar el docker-compose.yml porque ahí es donde tu API sabe cómo encontrar a la base de datos.

Paso 1: Limpieza (¡Importante!)

Si ejecutaste el comando docker run que pegaste al principio, es probable que tengas un contenedor suelto ocupando recursos o nombres. Vamos a borrarlo para evitar conflictos.

Ejecuta esto en tu terminal:

# 1. Detener todos los contenedores que estén corriendo
docker stop $(docker ps -aq)

# 2. Borrar los contenedores detenidos (para empezar limpio)
docker rm $(docker ps -aq)


(Nota: Esto borra todos los contenedores de tu máquina. Si tienes otros proyectos importantes corriendo, borra solo el de SQL Server buscando su ID con docker ps).

Paso 2: Levantar el Docker Compose

Asegúrate de estar en la carpeta donde guardaste el archivo docker-compose.yml y ejecuta:

# -d: Lo ejecuta en segundo plano (detached)
# --build: Fuerza a reconstruir la imagen de tu API por si cambiaste código
docker compose up -d --build


Paso 3: Verificar que todo esté bien

Ahora ejecuta docker compose ps. Deberías ver algo así:

Name

Command

State

Ports

literat-api-1

dotnet Literat...

Up

0.0.0.0:7663->8080/tcp

sqlserver-1

/opt/mssql/bin...

Up

0.0.0.0:3667->1433/tcp

Puntos clave de tu configuración:

Tu API (Literat):

No la busques en el puerto 8080 (ese es interno).

Desde tu navegador o Postman, accede a ella por: http://localhost:7663/swagger (o tu ruta de api).

Tu Base de Datos (SQL Server):

No intentes conectar por el 1433 (ese es el interno).

Si usas SQL Management Studio o DBeaver, conecta a:

Server: localhost,3667 (Nota la coma para puerto en algunos clientes, o usa :3667)

User: sa

Password: tuContraseñaSegura(!)1234

Troubleshooting (Solución de problemas)

Si la API se cierra inmediatamente (State: Exited), suele ser porque intenta conectar a la base de datos antes de que esta esté lista.

Mira los logs de la API:

docker compose logs -f literat-api


Si ves errores de conexión, simplemente espera unos segundos y reinicia solo la API (el SQL server tarda unos 10-20 segundos en arrancar por primera vez):

docker compose restart literat-api