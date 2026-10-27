# Sistema de Gestión de Facturación y Recordatorios

## 1. Descripción del proyecto

Aplicación Fullstack desarrollada para consultar facturas de clientes y procesar recordatorios de pago. El sistema identifica las facturas según su estado, simula el envío de correos electrónicos y actualiza los estados en MongoDB.

## 2. Tecnologías utilizadas

- **Backend:** C# y ASP.NET Core con .NET 10.
- **Frontend:** Angular.
- **Base de datos:** MongoDB.
- **Pruebas unitarias:** xUnit y Moq.
- **Arquitectura:** servicios, repositorios e inyección de dependencias.

## 3. Funcionalidades

- Consultar las facturas registradas en MongoDB.
- Filtrar facturas por estado.
- Visualizar número de factura, cliente, correo, valor, fecha de vencimiento y estado.
- Procesar recordatorios de pago.
- Simular el envío de correos electrónicos.
- Actualizar el estado de las facturas después de procesar correctamente la notificación.
- Ejecutar pruebas unitarias de la lógica de negocio.

## 4. Reglas de negocio

| Estado inicial | Acción | Estado final |
|---|---|---|
| `primerrecordatorio` | Simular el envío del segundo recordatorio | `segundorecordatorio` |
| `segundorecordatorio` | Simular el aviso de desactivación | `desactivado` |
| `pagada` | No requiere procesamiento | `pagada` |

El estado de una factura se actualiza únicamente después de que el servicio de correo simulado finaliza correctamente y se intenta actualizar el registro en MongoDB.

**Nota:** el envío de correos está simulado. La aplicación no envía mensajes reales.

## 5. Estructura del proyecto

```text
Facturacion/
├── Facturacion.ApiService/
│   ├── Models/
│   ├── Repositories/
│   ├── Services/
│   └── Program.cs
├── Facturacion.AppHost/
├── Facturacion.ServiceDefaults/
├── Facturacion.Tests/
│   └── Services/
├── facturacion-frontend/
└── README.md
```

- **Facturacion.ApiService:** API, modelos, servicios, repositorios y endpoints.
- **Facturacion.AppHost:** configuración de ejecución de la aplicación distribuida.
- **Facturacion.ServiceDefaults:** configuración compartida de servicios.
- **Facturacion.Tests:** pruebas unitarias de la lógica de negocio.
- **facturacion-frontend:** interfaz de usuario desarrollada en Angular.

## 6. Requisitos previos

Antes de ejecutar la aplicación, se necesita:

1. SDK de .NET compatible con el proyecto.
2. Node.js y npm compatibles con la versión de Angular utilizada.
3. Una instancia de MongoDB, local o en la nube.
4. Acceso a la base de datos y una colección de facturas con datos de prueba.

## 7. Configuración de MongoDB

Configurar los valores `MongoDb:ConnectionString` y `MongoDb:DatabaseName` mediante la configuración de la aplicación o variables de entorno.

La configuración debe proporcionar:

- Una cadena de conexión válida.
- El nombre de la base de datos.
- Acceso a la colección `Facturas`.

No publicar contraseñas, cadenas de conexión con credenciales ni otros secretos en el repositorio.

## 8. Ejecución del backend

Desde la carpeta raíz de la solución, ejecutar:

```powershell
dotnet run --project .\Facturacion.ApiService\Facturacion.ApiService.csproj
```

La API utilizará la dirección y el puerto indicados por su configuración de inicio.

Endpoints principales:

- `GET /api/facturas`: consulta todas las facturas.
- `GET /api/facturas/estado/{estado}`: consulta las facturas por estado.
- `POST /api/facturas/procesar-recordatorios`: procesa los recordatorios pendientes.
- `GET /mongodb-test`: comprueba la conexión con MongoDB.

Para probar los endpoints, utilizar la dirección base que muestre la terminal al iniciar la API.

## 9. Ejecución del frontend

Abrir una segunda terminal y ejecutar:

```powershell
cd .\facturacion-frontend
npm install
npm.cmd start
```

El frontend está configurado para ejecutarse en:

`http://localhost:4200/`

Verificar que la dirección de la API configurada en el servicio Angular coincida con el puerto en el que se está ejecutando el backend.

## 10. Ejecución de las pruebas unitarias

Desde la carpeta raíz de la solución, ejecutar:

```powershell
dotnet test .\Facturacion.Tests\Facturacion.Tests.csproj
```

Las pruebas cubren las transiciones de estado, el comportamiento de las facturas pagadas y el manejo de errores durante el envío simulado del correo.

## 11. Estado de las pruebas

En el entorno de desarrollo se ejecutaron correctamente cinco pruebas unitarias, sin errores.

## 12. Consideraciones y mejoras futuras

- El envío de correos es simulado; no se envían mensajes reales.
- MongoDB debe configurarse para cada entorno.
- Las direcciones de la API y la configuración de CORS deben ajustarse al entorno de ejecución.
- Antes de un despliegue productivo, se recomienda configurar un proveedor real de correo, gestionar los secretos de forma segura y fortalecer el manejo de reintentos y notificaciones duplicadas.
