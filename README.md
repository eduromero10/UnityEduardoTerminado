# UnityEduardoTerminado – Roll a Ball

Este proyecto es un juego realizado en Unity basado en el clásico Roll a Ball, ampliado con nuevas mecánicas como varios niveles, salto, enemigos y sistema de puntuación.

El objetivo del juego es controlar una bola, recoger todos los PickUps del nivel y evitar a un enemigo que persigue al jugador.


## Descripción del juego

El jugador controla una bola que se mueve por el escenario usando físicas.  
En cada nivel hay varios PickUps que deben recogerse para poder completar el nivel.

El juego cuenta con:
- Dos niveles diferentes
- Sistema de puntuación
- Salto
- Enemigo con comportamiento de persecución
- Fin de partida y reinicio

## Controles

- W A S D → Mover la bola  
- Espacio → Saltar  
- R → Reiniciar la partida  

En el Nivel 2, al pulsar R se vuelve al Nivel 1, empezando una nueva partida desde el principio.


## Niveles

### Nivel 1
- El objetivo es recoger todos los PickUps.
- Al recogerlos todos, se desbloquea el acceso al Nivel 2.
- Si el enemigo toca al jugador, el nivel se reinicia.

### Nivel 2
- El objetivo es recoger todos los PickUps del nivel.
- Al recoger el último PickUp:
  - el jugador deja de moverse,
  - el enemigo se detiene,
  - aparece un mensaje de victoria en pantalla.
- El nivel solo puede reiniciarse pulsando **R**, volviendo al Nivel 1.

## Sistema de puntuación

- Cada PickUp recogido suma un punto.
- La puntuación se muestra en pantalla durante la partida con un contador:
  
  `Count: X / Total`

## Enemigo

- El enemigo es un cubo que persigue al jugador.
- Utiliza físicas para moverse hacia la posición de la bola.
- Si el enemigo toca al jugador:
  - el nivel actual se reinicia.
- En el Nivel 2, el enemigo se desactiva al finalizar la partida.

## Fin de partida y reinicio

El juego puede terminar correctamente al completar los objetivos del nivel.  
Una vez finalizada la partida, el jugador puede reiniciar para volver a jugar usando la tecla **R**.

## Tecnologías utilizadas

- Unity
- Lenguaje C#
- Físicas con Rigidbody
- Input System
- TextMeshPro
- Git y GitHub


## Autor

Proyecto realizado por Eduardo Miguel Romero Afonso 
