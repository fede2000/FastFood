FastFood
FastFood es una aplicación de ejemplo que gestiona pedidos de comida, incluyendo características como el manejo de usuarios, productos, pagos y reportes de ventas. Este proyecto está diseñado utilizando tecnologías modernas de desarrollo web y patrones de diseño para ilustrar buenas prácticas en la construcción de aplicaciones.

Tabla de Contenidos

· Descripción del Proyecto

· Características

· Tecnologías Utilizadas

· Estructura del Proyecto

· Configuración del Entorno

· Uso de la Aplicación

Descripción del Proyecto

FastFood es una aplicación para gestionar pedidos en un entorno de restaurante de comida rápida. La aplicación permite realizar las siguientes operaciones:

·Usuarios: Registro, autenticación, y gestión de usuarios.

·Productos: Creación, actualización, visualización y eliminación de productos.

·Pedidos: Manejo de detalles de pedidos.

·Pagos: Gestión de pagos.

·Reportes: Generación de reportes de ventas.


Características

Gestión de Usuarios: Permite el registro, autenticación y administración de usuarios.

Gestión de Productos: CRUD (Crear, Leer, Actualizar, Eliminar) para productos.

Gestión de Pedidos: Manejo de detalles de pedidos.

Gestión de Pagos: Inserción de pagos y manejo de pagos simples.

Reportes de Ventas: Generación de reportes de ventas por período.


Tecnologías Utilizadas
· .NET Core: Framework para el desarrollo de aplicaciones web y APIs.

· ASP.NET Core: Plataforma para construir aplicaciones web y servicios API.

· Dapper: Micro ORM para acceso a datos en SQL Server.

· SQL Server: Sistema de gestión de bases de datos relacional.

· Microsoft.Data.SqlClient: Cliente de SQL Server para .NET.

· Configuración de la Aplicación: Utiliza IConfiguration para manejar la configuración de la aplicación.

Estructura del Proyecto

El proyecto está organizado en varios archivos y carpetas:

· FastFood.Models: Contiene los modelos de datos utilizados en la aplicación, como User, Product, Payment, etc.

· FastFood.Rules: Contiene las reglas de negocio y la lógica de acceso a datos para manejar usuarios, productos, pagos, y reportes.

· Program.cs: Configura y arranca la aplicación.

· Startup.cs: Configura los servicios y el pipeline de solicitud HTTP.


Clases Principales

· UserRule: Maneja operaciones relacionadas con usuarios (registro, autenticación, etc.).

· ProductRule: Maneja operaciones relacionadas con productos (CRUD).

· PaymentRule: Maneja operaciones relacionadas con pagos.

· SellingReportRule: Genera reportes de ventas.

Configuración del Entorno

1 - Clonar el Repositorio

git clone https://github.com/fede2000/FastFood.git

2- Restaurar Paquetes

Navega a la carpeta del proyecto y restaura los paquetes NuGet:

cd FastFood
dotnet restore

Configurar la Base de Datos

Asegúrate de tener SQL Server instalado y en funcionamiento.

Configura la cadena de conexión en el archivo appsettings.json bajo la clave FastFoodDB.

Ejecutar la Aplicación

Para ejecutar la aplicación, usa el siguiente comando:

dotnet run
La aplicación estará disponible en http://localhost:5000.

Uso de la Aplicación

Usuarios: Regístrate y autentícate para acceder a la aplicación.

Productos: Añade, actualiza o elimina productos.

Pedidos: Crea y gestiona pedidos.

Pagos: Realiza pagos y visualiza el historial de pagos.

Reportes: Genera reportes de ventas según el período seleccionado.


