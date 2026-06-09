```
    ██████                ███           █████      ███████████                           
  ███░░░░███             ░░░           ░░███      ░░███░░░░░███                          
 ███    ░░███ █████ ████ ████   ██████  ░███ █████ ░███    ░███  ██████    █████   █████ 
░███     ░███░░███ ░███ ░░███  ███░░███ ░███░░███  ░██████████  ░░░░░███  ███░░   ███░░  
░███   ██░███ ░███ ░███  ░███ ░███ ░░░  ░██████░   ░███░░░░░░    ███████ ░░█████ ░░█████ 
░░███ ░░████  ░███ ░███  ░███ ░███  ███ ░███░░███  ░███         ███░░███  ░░░░███  ░░░░███
 ░░░██████░██ ░░████████ █████░░██████  ████ █████ █████       ░░████████ ██████  ██████ 
   ░░░░░░ ░░   ░░░░░░░░ ░░░░░  ░░░░░░  ░░░░ ░░░░░ ░░░░░         ░░░░░░░░ ░░░░░░  ░░░░░░  
```

> Sistema de gestión de tickets de soporte técnico con .NET 9 y Clean Architecture

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?style=flat-square&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0+-4479A1?style=flat-square&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=flat-square&logo=docker)](https://www.docker.com/)
[![CI](https://img.shields.io/badge/CI-GitHub%20Actions-2088FF?style=flat-square&logo=githubactions)](https://github.com/features/actions)

---

## Sobre el Proyecto

QuickPass es una API REST backend desarrollada en **.NET 9** utilizando **Clean Architecture** y principios **SOLID** para la gestión del ciclo de vida de tickets de soporte técnico. El proyecto está diseñado como una demostración técnica de desarrollo de software profesional, incorporando patrones como *Repository Pattern*, separación estricta de responsabilidades, validaciones automáticas y una suite completa de pruebas unitarias.

---

## Arquitectura del Software

El sistema implementa una arquitectura limpia dividida en 4 capas para mantener un desacoplamiento estricto:

*   **QuickPass.Domain**: Contiene las entidades principales (`Ticket`, `User`, `Role`, `Account`, `TicketHistory`), enumeraciones y excepciones base. No tiene dependencias de librerías externas.
*   **QuickPass.Application**: Define los contratos de servicios e interfaces de persistencia, DTOs de entrada y salida, lógica de negocio y validación automática mediante *FluentValidation*.
*   **QuickPass.Infrastructure**: Implementa la base de datos a través de *Entity Framework Core* con MySQL, configuración de JWT Bearer y codificación PBKDF2 para contraseñas.
*   **QuickPass.API**: Capa de presentación que expone los controladores REST, configuración de middlewares (CORS, JWT, GlobalExceptionHandler) y Swagger/OpenAPI.

---

## Funcionalidades Implementadas

### Autenticación y Autorización
*   Registro de usuarios e inicialización de perfil (`User` y `Account`) de forma atómica mediante transacciones en base de datos.
*   Autenticación JWT Bearer segura con claims personalizados (`sub`, `accountId`, `role`).
*   Hash de contraseñas mediante algoritmo PBKDF2 (`IPasswordHasher`).
*   Sistema de autorización basado en roles (`Administrador`, `Tecnico`, `Usuario`).

### Gestión de Tickets (CRUD completo y Reglas de Negocio)
*   **Crear**: Clientes autenticados pueden registrar tickets con título, descripción, prioridad y categoría.
*   **Consultar**: Retorno contextual según el rol (Administrador ve todos, Técnico ve los asignados a él, Usuario ve solo los propios).
*   **Actualizar**: El creador del ticket puede editarlo siempre y cuando el estado sea `Abierto`.
*   **Eliminar**: El creador del ticket (si está `Abierto`) o un Administrador (en cualquier estado) pueden borrarlo del sistema.

### Historial de Transición y Comentarios
*   Registro automático de auditoría en la tabla `ticket_histories` por cada cambio de estado (creación, asignación, auto-asignación, resolución, cierre y reapertura).
*   Guarda el estado previo, el nuevo estado, la fecha, el autor del cambio y el comentario asociado.
*   Endpoint seguro para consultar el historial completo de un ticket.

### Calidad de Código y Resiliencia
*   **Manejo Global de Excepciones**: Implementación de `IExceptionHandler` de ASP.NET Core que formatea cualquier error no controlado bajo el estándar RFC 7807 (`ProblemDetails`).
*   **Validaciones Declarativas**: Uso de *FluentValidation* en peticiones de entrada.
*   **Pruebas Unitarias**: Suite de pruebas con **xUnit** y **Moq** para la lógica de negocio de tickets.
*   **Contenedores de Desarrollo**: Dockerfile multi-stage y Docker Compose integrados.
*   **Integración Continua**: Pipeline de GitHub Actions que corre compilación y pruebas en cada push.

---

## Cómo Iniciar el Proyecto

### Opción 1: Con Docker Compose (Recomendado)
Para levantar la API y la base de datos MySQL de forma automática (compilando el código y corriendo los tests durante el proceso):

```bash
docker compose up --build
```
La API estará disponible en `http://localhost:8080` con Swagger interactivo configurado como página de inicio.

### Opción 2: Desarrollo Local
1. Asegúrate de tener instalado el SDK de .NET 9.
2. Configura tu base de datos MySQL local y ajusta la cadena de conexión en los User Secrets de .NET:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=quickpass_db;Uid=tu_usuario;Pwd=tu_contrasena;" --project src/QuickPass.API
   ```
3. Ejecuta las migraciones de EF Core o inicializa tu base de datos.
4. Inicia el servidor de desarrollo:
   ```bash
   dotnet run --project src/QuickPass.API
   ```

---

## Pruebas Unitarias

La suite de pruebas valida exhaustivamente las reglas de negocio de los tickets (permisos de actualización, transiciones de estado válidas, borrados lógicos, etc.).

Para ejecutar las pruebas:
```bash
dotnet test
```
