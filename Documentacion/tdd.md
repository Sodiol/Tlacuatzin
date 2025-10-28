# Technical Design Document (TDD)  
**Juego:** Tlacuatzin: El Ladrón del Fuego  

**Integrantes del Proyecto:**  
- Mauricio Gabriel Anguiano Valencia  
- Joel Adalid Ladino Magaña  
- Oscar Ezequiel García Barajas  
- Ernesto Rosendo Licea  
- Fernando Martinez Vega  

**Universidad de Colima, Facultad de Telemática**  
**Fecha:** 01/10/2025  

---

## 1. Lista de características obtenidas del GDD  
- **Género:** Plataformas narrativo 2D (Metroidvania-lite).  
- **Duración:** 3–6 horas de juego.  
- **Estilo visual:** inspirado en arte y leyendas mexicanas.  
- **Mecánicas principales:**  
  - Saltar, trepar y colgarse con la cola.  
  - Encender fuego para abrir puertas o activar mecanismos.  
  - Esquivar y combatir contra enemigos (Jaguar Guardián, trampas, etc.).  
- **Narrativa:** lineal con elementos culturales y míticos.  
- **Modelo de negocio:** Demo gratuita + versión de pago único a bajo costo.  
- **Público objetivo:** jóvenes de 15–26 años, jugadores de PC y móvil.  

---

## 2. Elección de Game Engine  
**Motor elegido:** Godot Engine 4.4.1  

**Justificación:**  
- Soporte nativo para juegos 2D optimizados.  
- Open source y gratuito → viable para un estudio independiente.  
- Sistema modular de nodos y escenas.  
- Lenguaje **GDScript** y soporte para **C#**.  
- Físicas 2D, colisiones y animaciones integradas.  

**Ventajas frente a Unity o Unreal:**  
- Mejor rendimiento para juegos 2D.  
- Comunidad creciente y documentación amplia.  
- No requiere licencias de pago ni royalties.  

---

## 3. Planeación (Diagrama de Gantt – 14 semanas)  
1. Diseño conceptual y prototipo inicial (Semana 1–2).  
2. Arte y mockups visuales (Semana 2–4).  
3. Mecánicas básicas (movimiento, salto, colisiones) (Semana 3–5).  
4. Desarrollo de niveles y escenas (Semana 5–8).  
5. Integración de narrativa y cinemáticas (Semana 6–9).  
6. Enemigos y jefes (Semana 8–10).  
7. Audio y efectos visuales (Semana 9–11).  
8. Pruebas y debugging (Semana 11–12).  
9. Optimización y balance (Semana 12–13).  
10. Entrega demo + versión final (Semana 14).  

---

## 4. Diagramas de alto nivel  

### Arquitectura del software  
- **Capa de Presentación:** Interfaz, HUD, menús.  
- **Capa Lógica de Juego:** Mecánicas (movimiento, fuego, enemigos).  
- **Capa de Datos:** Recursos gráficos, audio y escenas en Godot.  
- **Capa Física:** Colisiones, gravedad y físicas 2D.  

### Flujo de gameplay  
`Inicio → Exploración → Plataformas → Combate → Recolección → Avance → Jefe final → Cinemática`  

---

## 5. Herramientas de arte  
- **Aseprite / Libresprite:** sprites y animaciones en pixel art.  
- **Libresprite:** texturas y fondos.  
- **Audacity:** grabación y edición de efectos de sonido.  

---

## 6. Objetos, Terreno y Escenas (2D adaptado)  
- **Escenarios:** Selva, cuevas, templos, Tierra de los Gigantes.  
- **Elementos:** plataformas, muros, trampas, puertas de fuego, fogatas.  
- **Sprites de personajes:** Tlacuatzin (jugador), enemigos (jaguares, criaturas).  
- **Estructura modular:** cada escena es una zona conectada (Metroidvania-lite).  

---

## 7. Detección de colisiones, físicas e interacciones  
- **Motor físico 2D de Godot.**  
- **Colisiones:** áreas con `CollisionShape2D`.  
- **Técnicas:**  
  - Bounding boxes y máscaras de colisión.  
  - `RigidBody2D` para objetos móviles.  
  - `CharacterBody2D` para jugador y enemigos.  

**Interacciones:**  
- Recolección de ítems (llaves, fuego).  
- Golpes y ataques del Jaguar.  
- Interruptores y puertas activadas por fuego.  

---

## 8. Lógica de juego e Inteligencia Artificial  

### Lógica del jugador  
- Implementada en **GDScript**.  
- **FSM (Finite State Machine):** caminar, saltar, colgarse, atacar.  

### IA de enemigos  
- **Enemigos básicos:** patrullaje y persecución al detectar al jugador.  
- **Jaguar Guardián:** patrones de ataque progresivos.  
- Uso de **Navigation2D** para caminos dinámicos.  

---

## 9. Networking  
- El juego será **single-player**.  
- Posible expansión futura con multijugador cooperativo online.  

---

## 10. Audio y efectos visuales  

### Audio  
- Música ambiental con instrumentos prehispánicos.  
- Efectos: fuego, pasos, rugido del jaguar.  
- Herramientas: **Audacity** + librerías libres (*Freesound*).  

### Visuales  
- Partículas de fuego, humo y chispas con el sistema de partículas de Godot.  
- Transiciones de luz y sombra para atmósfera mística.  

---

## 11. Plataforma y requerimientos de software  

### Plataforma objetivo  
- PC (Windows, Linux).  
- Posible versión móvil (Android).  

### Requisitos mínimos  
- CPU: Dual Core 2.0 GHz  
- RAM: 2 GB  
- GPU: integrada (Intel HD 4000 o superior)  
- Almacenamiento: 500 MB  

### Recomendados  
- CPU: Quad Core  
- RAM: 4 GB  
- GPU: dedicada  
- Almacenamiento: 1 GB  
