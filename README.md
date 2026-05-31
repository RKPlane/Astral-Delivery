# Astral Delivery

Proyecto de Dinámica — Física 3D  
Grado en Diseño y Desarrollo de Videojuegos y Experiencias Interactivas

---

## Descripción

Astral Delivery es un juego 3D en el que el jugador controla a un operario espacial en los restos de una estación destruida. Sin gravedad, todo se mueve por inercia: empujar un objeto te mueve en dirección contraria, y acoplar piezas modifica tu masa total afectando directamente a la maniobrabilidad. El objetivo es recuperar los módulos de chatarra flotantes y llevarlos a la zona de entrega.

---

## Controles

| Tecla | Acción |
|---|---|
| W / S | Impulso adelante / atrás |
| A / D | Impulso izquierda / derecha |
| Space | Impulso arriba |
| L.Ctrl | Impulso abajo |
| Shift | Impulso hacia delante |
| Q | Frenado |
| E | Acoplar chatarra cercana |
| F | Soltar |
---

## Física implementada

**Rigidbody.** Todos los objetos tienen Rigidbody con useGravity = false. La masa se recalcula en tiempo real al acoplar o soltar piezas mediante MassSystem.

**Colliders y Triggers.** CapsuleCollider en el player, BoxCollider en la chatarra, SphereCollider con isTrigger = true en la zona de entrega. OnTriggerEnter detecta al player con chatarra acoplada y ejecuta DockingSystem.DeliverAll().

**FixedJoint.** Al acoplar se crea un FixedJoint en runtime. Antes de crearlo se aplica conservación de momento: v = (m1·v1 + m2·v2) / (m1 + m2). La rotación del junk se congela con RigidbodyConstraints.FreezeRotation para evitar que su inercia rotacional desestabilice el conjunto.

**SpringJoint.** Los módulos de la estación oscilan con SpringJoint siguiendo la ley de Hooke (F = -k·x), con un impulso inicial aleatorio que simula el estado post-explosión.

**Physics Material.** Bounciness = 0.4 y dynamicFriction = 0.1 en todos los colliders de chatarra.

**Fuerzas.** Se usan ForceMode.Force para el thruster y el freno, y ForceMode.Impulse para la reacción newtoniana al lanzar objetos.

---

## Decisiones y soluciones

**rb.MoveRotation en lugar de transform.Rotate.** transform.Rotate() en FixedUpdate bypasea el motor de física. Con un FixedJoint activo, el solver detecta la violación de constraint y genera fuerzas correctoras que desestabilizan el conjunto acoplado. rb.MoveRotation() integra la rotación correctamente dentro del pipeline de física.

**Conservación de momento antes del joint.** Si se aplicara después de crear el FixedJoint, el joint absorbería la diferencia de velocidades generando un impulso interno que lanzaría el conjunto.

**SetMass en Start, no en Awake.** MassSystem llamaba a player.SetMass() en Awake, pero el Rigidbody de PlayerController podía no estar inicializado todavía. Moverlo a Start garantiza que todos los Awake han terminado.

**FreezeRotation al acoplar.** Sin esto, la inercia rotacional del junk genera un torque reactivo al girar el conjunto que empuja al player fuera de curso.

---

## Vídeo de gameplay

https://youtu.be/6OimYpYmNzk
