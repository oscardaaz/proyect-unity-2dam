# Proyecto Unity 2DAM

Videojuego desarrollado con Unity para el módulo de Programación Multimedia y Dispositivos Móviles (2º DAM).

---

## Requisitos

- Unity 6.3 LTS (6000.3.12f1) (misma versión que el proyecto)
- Git

---

## Abrir en Unity Hub

Tienes dos opciones:

### Opción A: Desde el repositorio (recomendado)

1. Abre Unity Hub
2. Pulsa **Add** > **Add from repository**
3. Pega la URL: `https://github.com/oscardaaz/proyect-unity-2dam.git`
4. Elige carpeta de destino y confirma

### Opción B: Clonar manualmente y abrir desde disco

```bash
git clone git@github.com:oscardaaz/proyect-unity-2dam.git
cd proyect-unity-2dam
```

1. Abre Unity Hub
2. Pulsa **Add** > **Add project from disk**
3. Selecciona la carpeta `proyect-unity-2dam/`
4. Abre el proyecto con la versión correcta de Unity

---

## Flujo de trabajo con Git

**Nunca trabajéis directamente en `main`.** Seguid estos pasos:

### 1. Antes de empezar una tarea, crea una rama

```bash
git checkout main
git pull
git checkout -b nombre-de-la-rama
```

Ejemplos de nombres de rama:
- `feature/movimiento-jugador`
- `feature/menu-principal`
- `fix/bug-colisiones`

### 2. Trabaja y haz commits

```bash
git add .
git commit -m "tipo: descripción clara de lo que hiciste"
```

**Convención de commits:**

| Tipo | Cuándo usarlo |
|------|---------------|
| `feat:` | Nueva funcionalidad |
| `fix:` | Corrección de bug |
| `docs:` | Documentación |
| `refactor:` | Reestructuración sin cambiar funcionalidad |
| `chore:` | Mantenimiento (gitignore, configs...) |

Ejemplos:
- `feat: añadir sistema de movimiento del jugador`
- `fix: corregir colisión con plataformas`
- `docs: actualizar README con instrucciones de build`

### 3. Sube tu rama a GitHub

```bash
git push -u origin nombre-de-la-rama
```

### 4. Crea una Pull Request

- Ve al repositorio en GitHub
- Pulsa **Compare & pull request**
- Describe los cambios que has hecho
- Asigna a **Oscar** como revisor
- Espera a que sea aprobada antes de mergear

### 5. Tras el merge, borra tu rama local

```bash
git checkout main
git pull
git branch -d nombre-de-la-rama
```

---

## Normas

- No hacer merge a `main` sin aprobacion de PR
- No subir la carpeta `Library/`, `Temp/` ni `Logs/` (ya estan en el .gitignore)
- Un commit = un cambio concreto, no mezcles varias cosas en el mismo commit
