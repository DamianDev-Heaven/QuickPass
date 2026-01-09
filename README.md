```
    ██████                ███           █████      ███████████                           
  ███░░░░███             ░░░           ░░███      ░░███░░░░░███                          
 ███    ░░███ █████ ████ ████   ██████  ░███ █████ ░███    ░███  ██████    █████   █████ 
░███     ░███░░███ ░███ ░░███  ███░░███ ░███░░███  ░██████████  ░░░░░███  ███░░   ███░░  
░███   ██░███ ░███ ░███  ░███ ░███ ░░░  ░██████░   ░███░░░░░░    ███████ ░░█████ ░░█████ 
░░███ ░░████  ░███ ░███  ░███ ░███  ███ ░███░░███  ░███         ███░░███  ░░░░███ ░░░░███
 ░░░██████░██ ░░████████ █████░░██████  ████ █████ █████       ░░████████ ██████  ██████ 
   ░░░░░░ ░░   ░░░░░░░░ ░░░░░  ░░░░░░  ░░░░ ░░░░░ ░░░░░         ░░░░░░░░ ░░░░░░  ░░░░░░     
                                                                                         
```

> Sistema de gestión de tickets de soporte técnico con .NET 9 y Clean Architecture

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?style=flat-square&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0+-4479A1?style=flat-square&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=flat-square)](https://github.com/DamianDev-Heaven/QuickPass)

---

## ⚠️ Estado del Proyecto

** En desarrollo activo** - MVP funcional con autenticación completa y gestión básica de tickets.

---

## Sobre el Proyecto

QuickPass es una API REST backend desarrollada con **.NET 9** que implementa **Clean Architecture** para la gestión de tickets de soporte técnico. El objetivo principal es demostrar la aplicación de principios SOLID, patrones de diseño y arquitectura escalable en un proyecto real.

El sistema proporciona autenticación JWT robusta, sistema de roles y gestión del ciclo de vida completo de tickets, desde su creación hasta su resolución.

---

## Funcionalidades Implementadas

### Autenticación y Autorización
-  Registro de usuarios con hash PBKDF2
-  Login con generación de JWT tokens
-  Sistema de roles (Usuario, Técnico, Administrador)
-  Middleware de autenticación JWT Bearer
-  Claims personalizados para autorización

### Gestión de Tickets
-  Crear tickets autenticados
-  Consultar tickets propios del usuario
-  Estados de tickets: Abierto, Asignado, En Proceso, Resuelto, Cerrado

### Arquitectura y Calidad
-  Clean Architecture en 4 capas
-  Repository Pattern
-  Dependency Injection
-  DTOs para separación de contratos
-  Documentación Swagger/OpenAPI
-  Configuración segura con User Secrets

---

## En Desarrollo

- [ ] Endpoints CRUD completos (GET by ID, GET all, UPDATE, DELETE)
- [ ] Asignación de tickets a técnicos
- [ ] Autorización basada en roles (`[Authorize(Roles)]`)
- [ ] Cambio de estados de tickets (resolver, cerrar, reabrir)
- [ ] Sistema de comentarios en tickets
- [ ] Manejo de errores con ProblemDetails
- [ ] Validaciones con FluentValidation

---

## Stack Tecnológico

**Backend**
- .NET 9.0
- ASP.NET Core Web API
- C# 13.0

**Persistencia**
- Entity Framework Core 9.0
- MySQL 8.0+
- Pomelo.EntityFrameworkCore.MySql

**Seguridad**
- JWT Bearer Authentication
- ASP.NET Core Identity (PasswordHasher)
- User Secrets para configuración sensible

**Herramientas**
- Swagger/OpenAPI
- Git/GitHub

---
