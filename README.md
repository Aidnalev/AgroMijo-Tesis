# 🌱 AgroMijo

> Videojuego serio para la práctica de la toma de decisiones en la gestión agrícola de Unidades Agrícolas Familiares (UAF) de la provincia Comunera de Santander.

![Estado](https://img.shields.io/badge/Estado-Videojuego%20finalizado-brightgreen)
![Unity](https://img.shields.io/badge/Unity-6-black)
![C%23](https://img.shields.io/badge/C%23-Programación-purple)
![RUP](https://img.shields.io/badge/Metodología-RUP-blue)
![Licencia](https://img.shields.io/badge/Proyecto-Académico-green)

---

## 📖 Descripción

**AgroMijo** es un videojuego serio desarrollado como proyecto de grado del programa de Ingeniería de Sistemas e Informática de la **Universidad Industrial de Santander (UIS)**.

El proyecto está orientado a jóvenes del entorno rural de la provincia Comunera de Santander y busca proporcionar un espacio interactivo para **practicar la toma de decisiones relacionadas con la gestión agrícola de una Unidad Agrícola Familiar (UAF)**.

Durante una partida, el jugador administra los recursos disponibles de una UAF, analiza las condiciones de diferentes parcelas, planifica cultivos, realiza inversiones y responde ante situaciones que pueden afectar el desarrollo de la producción.

Las decisiones tomadas durante la partida generan consecuencias que permiten al jugador observar y analizar los resultados de sus estrategias productivas.

---

## 🎯 Objetivo

Desarrollar un videojuego serio que permita a jóvenes del entorno rural **practicar la toma de decisiones relacionadas con la gestión agrícola de Unidades Agrícolas Familiares (UAF)**, mediante un entorno interactivo en el que puedan experimentar diferentes alternativas y observar sus consecuencias sin asumir los riesgos asociados a una decisión en un contexto productivo real.

---

## 🎮 Funcionamiento general

Una partida de AgroMijo se desarrolla alrededor de una **Unidad Agrícola Familiar compuesta por cuatro parcelas** con características variables.

El jugador dispone de recursos económicos que debe administrar para tomar decisiones relacionadas con la producción agrícola.

Entre las acciones disponibles se encuentran:

* Consultar las características de las parcelas.
* Revisar las condiciones de suelo y disponibilidad de agua.
* Consultar información relacionada con los cultivos.
* Seleccionar cultivos para las parcelas.
* Administrar los recursos económicos disponibles.
* Realizar mejoras sobre las parcelas.
* Atender eventos que pueden afectar el desarrollo de la actividad agrícola.
* Consultar el estado y los resultados de la partida.
* Finalizar la partida y registrar la información generada.

El resultado de las decisiones depende de las condiciones de la UAF y de las situaciones que se presenten durante el desarrollo de la partida.

---

## 🌾 Características principales

### Gestión de la UAF

El jugador administra una unidad compuesta por cuatro parcelas que presentan diferentes condiciones.

Cada parcela puede presentar características relacionadas con:

* Tipo de suelo.
* Disponibilidad de agua.
* Acceso vial.
* Mejoras realizadas.
* Cultivo establecido.

### 🌱 Gestión de cultivos

El jugador puede consultar información de los cultivos disponibles y seleccionar aquellos que considere apropiados para las condiciones de cada parcela.

La información utilizada para la toma de decisiones contempla aspectos como:

* Costo de semillas.
* Rendimiento.
* Duración del cultivo.
* Jornales requeridos.
* Compatibilidad con las condiciones del terreno.

### 💰 Administración de recursos

Las decisiones realizadas durante la partida tienen un impacto sobre los recursos económicos disponibles.

El jugador debe considerar los costos de producción, las inversiones realizadas y los posibles resultados de sus decisiones.

### 🏗️ Mejoras de las parcelas

Las parcelas pueden recibir diferentes mejoras que modifican sus condiciones o los resultados de la producción.

Entre ellas se encuentran:

* Acceso vial.
* Sistema de riego.
* Fertilización.

### 🌦️ Eventos

Durante una partida pueden presentarse diferentes eventos que representan situaciones que pueden afectar las condiciones de la UAF.

Los eventos pueden pertenecer a diferentes categorías y producir efectos sobre aspectos como:

* Producción.
* Economía.
* Infraestructura.
* Condiciones de las parcelas.

El jugador debe interpretar cada situación y tomar las decisiones disponibles para responder ante ella.

### 👤 Perfiles

El sistema permite gestionar diferentes perfiles de jugador en un mismo dispositivo.

Cada perfil utiliza un alias visible para el usuario y cuenta con un identificador que permite diferenciar las sesiones registradas para su posterior consulta.

### 📊 Registro y consulta de información

La información generada durante las partidas puede ser registrada mediante la API del proyecto.

Los datos pueden ser consultados posteriormente mediante un **dashboard web**, permitiendo revisar información asociada a los perfiles y a las sesiones de juego.

---

## 🖥️ Componentes del sistema

AgroMijo está compuesto actualmente por dos componentes principales:

### 🎮 Videojuego

Aplicación desarrollada con **Unity** y **C#**, encargada de implementar la experiencia de juego, las mecánicas de gestión agrícola, la interfaz y el registro de las sesiones.

### 🌐 API

Servicio web encargado de recibir y gestionar la información generada por el videojuego y proporcionar los datos necesarios para su posterior consulta.

La API permite conectar el videojuego con los componentes externos utilizados para la persistencia y consulta de la información.

### 📊 Dashboard

Aplicación web destinada a la consulta de los registros generados durante las partidas.

Permite realizar consultas utilizando diferentes criterios y visualizar la información registrada para facilitar el seguimiento y análisis de las sesiones.

---

## 🔄 Flujo general del sistema

```text
┌─────────────────────┐
│      Jugador        │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│      AgroMijo       │
│      Videojuego     │
└──────────┬──────────┘
           │
           │ Registra información
           ▼
┌─────────────────────┐
│     AgroMijo API    │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│   Persistencia de   │
│     información     │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│      Dashboard      │
│       Web           │
└─────────────────────┘
```

---

## 🛠️ Tecnologías

| Tecnología         | Uso                                              |
| ------------------ | ------------------------------------------------ |
| **Unity 6**        | Desarrollo del videojuego                        |
| **C#**             | Programación del videojuego y lógica del sistema |
| **ASP.NET / .NET** | Desarrollo de la API                             |
| **Git**            | Control de versiones                             |
| **GitHub**         | Gestión y almacenamiento del código fuente       |
| **StarUML**        | Modelado del sistema durante el desarrollo       |

---

## 📂 Estructura del repositorio

El repositorio contiene los diferentes componentes desarrollados para AgroMijo:

```text
AgroMijo-Tesis/
│
├── AgroMijo Tesis/
│   └── Proyecto de Unity
│
├── AgroMijo API/
│   └── AgroMijo.API/
│
├── .gitignore
└── README.md
```

> Los documentos académicos, modelos y demás artefactos utilizados durante el desarrollo del trabajo de grado no necesariamente forman parte de este repositorio.

---

## ✅ Estado del proyecto

El desarrollo de AgroMijo ha finalizado como producto de software del Trabajo de Grado II del programa de Ingeniería de Sistemas e Informática de la Universidad Industrial de Santander.

El proyecto incluye el videojuego, los componentes necesarios para el registro de información generada durante las partidas y las herramientas desarrolladas para su consulta.
---

## 🎓 Proyecto académico

**AgroMijo** es desarrollado como trabajo de grado de la:

**Escuela de Ingeniería de Sistemas e Informática**
**Universidad Industrial de Santander — UIS**

### Autores

* **Johan Sebastián Suárez Chacón**
* **Miguel Daniel Velandia Pinilla**

### Director

* **Urbano Eliécer Gómez Prada**

---

## 🌾 Lema

> **“Aprender haciendo, decidir jugando.”**

AgroMijo busca proporcionar un espacio interactivo en el que los jugadores puedan **practicar la toma de decisiones agrícolas, experimentar con diferentes alternativas y observar sus consecuencias** dentro de un entorno controlado.

---

## 📌 Repositorio

El código fuente del proyecto se encuentra disponible en:

**[Aidnalev/AgroMijo-Tesis](https://github.com/Aidnalev/AgroMijo-Tesis)**

Este repositorio corresponde al desarrollo académico del proyecto de grado.

