#!/usr/bin/env bash
# Guía de commits para el repositorio. Ejecutar desde la carpeta SistemaBiblioteca/
# Uso:  bash docs/git_commits.sh   (después de crear el repositorio vacío en GitHub)
# Reemplace https://github.com/DavidMBS2007/Biblioteca-de-gestion-DavidBarrera.git por la URL de su repositorio.

set -e
git init -b main

git add .gitignore README.md
git commit -m "docs: agrega README inicial y .gitignore"

git add database/BibliotecaDB.sql
git commit -m "feat(bd): agrega script SQL con tablas, restricciones, vista y datos de prueba"

git add SistemaBiblioteca.sln SistemaBiblioteca/SistemaBiblioteca.csproj SistemaBiblioteca/App.config SistemaBiblioteca/Program.cs
git commit -m "chore: crea solución de Windows Forms (.NET 8) y configuración de conexión"

git add SistemaBiblioteca/Models
git commit -m "feat(models): agrega entidades con herencia y encapsulamiento"

git add SistemaBiblioteca/Data
git commit -m "feat(data): agrega conexión y DAOs con consultas parametrizadas"

git add SistemaBiblioteca/Business
git commit -m "feat(business): agrega servicios, validaciones y reglas de negocio"

git add SistemaBiblioteca/UI/UiHelper.cs SistemaBiblioteca/UI/MainForm.cs
git commit -m "feat(ui): agrega formulario principal, navegación y utilidades de interfaz"

git add SistemaBiblioteca/UI/FormsMantenimiento.cs
git commit -m "feat(ui): agrega CRUD de autores, editoriales y usuarios"

git add SistemaBiblioteca/UI/FormsLibrosPrestamos.cs
git commit -m "feat(ui): agrega CRUD de libros y gestión de préstamos y devoluciones"

git add SistemaBiblioteca/UI/FormsReportesInicio.cs
git commit -m "feat(ui): agrega módulo de reportes con exportación CSV y pantalla de inicio"

git add docs
git commit -m "docs: agrega documentación técnica y capturas"

git remote add origin https://github.com/DavidMBS2007/Biblioteca-de-gestion-DavidBarrera.git
git push -u origin main
