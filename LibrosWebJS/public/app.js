// app.js

// ⚠️ Asegúrate de que este puerto coincida con tu docker-compose (7655)
const API_BASE = 'http://localhost:7655/api'; 

/* ==========================================
   ESTADO GLOBAL DE LA APP
   Guardamos quién es el usuario para no perder el rastro
   ========================================== */
let estadoApp = {
    usuario: null, // Ejemplo: { id: 1, rol: 'cliente' } o { rol: 'admin' }
    libros: [],    // Cache local de libros para buscar rápido sin ir a la API
    generos: []
};
let libroActualISBN = ''; // Para saber qué libro estamos reseñando o añadiendo

document.addEventListener('DOMContentLoaded', () => {
    // Inicialmente no cargamos nada, esperamos al Login
    console.log("App iniciada. Esperando login...");
    
    // Listener para el buscador en tiempo real (Vista Cliente)
    document.getElementById('buscador').addEventListener('input', (e) => {
        filtrarLibrosCliente(e.target.value.toLowerCase());
    });
});

/* --- UTILIDAD: Notificaciones Bonitas (Toast) --- */
function mostrarNotificacion(mensaje, tipo = 'info') {
    const contenedor = document.getElementById('toast-container');
    const toast = document.createElement('div');
    
    // Colores según tipo
    let colorClass = 'border-primary';
    let icon = 'info-circle text-primary';
    
    if(tipo === 'success') { colorClass = 'border-success'; icon = 'check-circle text-success'; }
    if(tipo === 'danger')  { colorClass = 'border-danger'; icon = 'exclamation-circle text-danger'; }

    toast.className = `custom-toast ${colorClass}`;
    toast.innerHTML = `<i class="fas fa-${icon} fa-lg"></i> <span>${mensaje}</span>`;
    
    contenedor.appendChild(toast);

    // Auto eliminar a los 3 segundos
    setTimeout(() => {
        toast.style.animation = 'slideIn 0.3s reverse forwards';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

/* ==========================================
   1. SISTEMA DE LOGIN (Simulado)
   ========================================== */

function iniciarSesion(rol) {
    if (rol === 'admin') {
        estadoApp.usuario = { rol: 'admin' };
        mostrarNotificacion("Bienvenido, Administrador", "success");
        configurarVistaAdmin();
    } else {
        // Mostrar input para pedir ID
        document.getElementById('login-cliente-input').style.display = 'block';
    }
}

async function confirmarLoginCliente() {
    const idInput = document.getElementById('inputClienteId').value;
    if (!idInput) return mostrarNotificacion("Por favor ingresa un ID", "danger");

    // Validar si el cliente existe en la API
    try {
        const res = await fetch(`${API_BASE}/clientes/${idInput}`);
        if (res.ok) {
            estadoApp.usuario = { id: idInput, rol: 'cliente' };
            mostrarNotificacion(`Hola de nuevo, Lector #${idInput}`, "success");
            configurarVistaCliente();
        } else {
            mostrarNotificacion("Cliente no encontrado. Prueba ID 1", "danger");
        }
    } catch (e) {
        mostrarNotificacion("Error de conexión con la API", "danger");
    }
}

function configurarVistaAdmin() {
    // Ocultar Login, Mostrar App
    document.getElementById('login-screen').classList.add('d-none');
    document.getElementById('app-interface').classList.remove('d-none');
    
    // Configurar UI Admin
    document.getElementById('menu-admin').classList.remove('d-none');
    document.getElementById('vista-admin').classList.remove('d-none');
    
    // Cargar datos iniciales
    cargarLibrosAdmin();
    cargarAutoresAdmin();
    cargarGenerosAdmin();
    cargarClientesAdmin();
}

function configurarVistaCliente() {
    // Ocultar Login, Mostrar App
    document.getElementById('login-screen').classList.add('d-none');
    document.getElementById('app-interface').classList.remove('d-none');

    // Configurar UI Cliente
    document.getElementById('menu-cliente').classList.remove('d-none');
    document.getElementById('display-user-id').innerText = estadoApp.usuario.id;
    document.getElementById('hero-store').classList.remove('d-none'); // Mostrar Banner Hero
    document.getElementById('vista-cliente').classList.remove('d-none');

    // Cargar datos
    cargarLibrosCliente();
    cargarGenerosFiltros();
}

/* ==========================================
   2. LÓGICA VISTA CLIENTE (Tienda)
   ========================================== */

async function cargarLibrosCliente() {
    try {
        const res = await fetch(`${API_BASE}/libros`);
        estadoApp.libros = await res.json();
        renderizarTienda(estadoApp.libros);
    } catch (error) { mostrarNotificacion("Error cargando libros", "danger"); }
}

function renderizarTienda(libros) {
    const contenedor = document.getElementById('contenedor-libros');
    contenedor.innerHTML = '';
    
    if(libros.length === 0) {
        contenedor.innerHTML = '<div class="col-12 text-center text-muted">No se encontraron libros.</div>';
        return;
    }

    libros.forEach(libro => {
        contenedor.innerHTML += `
            <div class="col">
                <div class="book-card h-100">
                    <div class="card-img-wrapper">
                        <img src="https://source.unsplash.com/random/400x600/?book&sig=${libro.isbn}" class="book-img" alt="${libro.titulo}">
                        <div class="card-actions-overlay">
                            <button onclick="verDetalleLibro('${libro.isbn}')" class="btn btn-icon" title="Ver detalle">
                                <i class="fas fa-eye"></i>
                            </button>
                        </div>
                    </div>
                    <div class="card-body">
                        <h5 class="card-title serif-font">${libro.titulo}</h5>
                        <p class="price-tag text-warning fw-bold">$${libro.precioVenta}</p>
                    </div>
                </div>
            </div>`;
    });
}

// Filtros de Género en el Hero
async function cargarGenerosFiltros() {
    try {
        const res = await fetch(`${API_BASE}/generos`);
        const generos = await res.json();
        const contenedor = document.getElementById('filtros-genero-hero');
        
        contenedor.innerHTML = `<button class="btn btn-light rounded-pill btn-sm fw-bold" onclick="renderizarTienda(estadoApp.libros)">Todos</button>`;
        
        generos.forEach(g => {
            contenedor.innerHTML += `
                <button class="btn btn-outline-light rounded-pill btn-sm" 
                onclick="filtrarPorGenero(${g.generoId})">${g.nombre}</button>`;
        });
    } catch (e) {}
}

function filtrarPorGenero(id) {
    const filtrados = estadoApp.libros.filter(l => l.generoId === id);
    renderizarTienda(filtrados);
}

function filtrarLibrosCliente(termino) {
    const filtrados = estadoApp.libros.filter(l => l.titulo.toLowerCase().includes(termino));
    renderizarTienda(filtrados);
}

// Detalles y Reseñas
async function verDetalleLibro(isbn) {
    libroActualISBN = isbn;
    const libro = estadoApp.libros.find(l => l.isbn === isbn);
    
    document.getElementById('modalTitulo').innerText = libro.titulo;
    document.getElementById('modalPrecio').innerText = `$${libro.precioVenta}`;
    document.getElementById('modalISBN').innerText = `ISBN: ${isbn}`;
    document.getElementById('modalImagen').src = `https://source.unsplash.com/random/400x600/?book&sig=${isbn}`;
    
    // Cargar reseñas
    const contResenas = document.getElementById('contenedor-resenas');
    contResenas.innerHTML = '<div class="spinner-border spinner-border-sm"></div>';
    
    try {
        const res = await fetch(`${API_BASE}/libros/${isbn}/resenas`);
        const resenas = await res.json();
        
        contResenas.innerHTML = resenas.length ? '' : '<small class="text-muted">Sé el primero en opinar.</small>';
        
        resenas.forEach(r => {
            contResenas.innerHTML += `
                <div class="mb-2 border-bottom pb-1">
                    <div class="d-flex justify-content-between">
                        <small class="fw-bold">Usuario ${r.clienteId}</small>
                        <small class="text-warning">${'★'.repeat(r.valoracion)}</small>
                    </div>
                    <small class="text-muted d-block">"${r.comentario}"</small>
                </div>`;
        });
    } catch(e) { contResenas.innerHTML = 'Error cargando reseñas'; }

    new bootstrap.Modal(document.getElementById('detalleModal')).show();
}

/* ==========================================
   3. LÓGICA VISTA ADMIN (Gestión Completa)
   ========================================== */

// --- A. LIBROS ---
async function cargarLibrosAdmin() {
    const tabla = document.getElementById('admin-tabla-libros');
    tabla.innerHTML = '<div class="text-center p-3"><div class="spinner-border"></div></div>';
    
    try {
        const res = await fetch(`${API_BASE}/libros`);
        const libros = await res.json();
        
        let html = `<table class="table table-hover align-middle shadow-sm bg-white rounded">
            <thead class="table-dark"><tr><th>ISBN</th><th>Título</th><th>Precio</th><th>Acción</th></tr></thead>
            <tbody>`;
        
        libros.forEach(l => {
            html += `<tr>
                <td><span class="badge bg-light text-dark border">${l.isbn}</span></td>
                <td>${l.titulo}</td>
                <td class="fw-bold">$${l.precioVenta}</td>
                <td>
                    <button onclick="eliminarLibro('${l.isbn}')" class="btn btn-sm btn-outline-danger" title="Eliminar">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>`;
        });
        tabla.innerHTML = html + `</tbody></table>`;
    } catch(e) { tabla.innerHTML = 'Error al cargar inventario.'; }
}

// CREAR LIBRO (POST)
document.getElementById('formCrearLibro').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const nuevoLibro = {
        isbn: document.getElementById('newIsbn').value,
        titulo: document.getElementById('newTitulo').value,
        precioVenta: parseFloat(document.getElementById('newPrecio').value),
        numPaginas: parseInt(document.getElementById('newPaginas').value) || 0,
        autorId: parseInt(document.getElementById('newAutorId').value),
        generoId: parseInt(document.getElementById('newGeneroId').value),
        fechaPublicacion: new Date().toISOString()
    };

    try {
        const res = await fetch(`${API_BASE}/libros`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(nuevoLibro)
        });

        if (res.ok) {
            mostrarNotificacion('Libro añadido al inventario', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalCrearLibro')).hide();
            document.getElementById('formCrearLibro').reset();
            cargarLibrosAdmin(); // Recargar tabla
        } else {
            const errorData = await res.json();
            mostrarNotificacion('Error: ' + JSON.stringify(errorData).substring(0, 50), 'danger');
        }
    } catch (e) { console.error(e); }
});

// BORRAR LIBRO (DELETE)
async function eliminarLibro(isbn) {
    if(!confirm("¿Borrar libro permanentemente?")) return;
    try {
        const res = await fetch(`${API_BASE}/libros/${isbn}`, { method: 'DELETE' });
        if(res.ok) {
            mostrarNotificacion('Libro eliminado', 'success');
            cargarLibrosAdmin();
        } else {
            mostrarNotificacion('No se puede borrar (tiene dependencias)', 'danger');
        }
    } catch(e) { console.error(e); }
}

// --- B. AUTORES, GÉNEROS, CLIENTES ---

// --- CORRECCIÓN 1: Autores (Evita el crash de a.nombre[0]) ---
async function cargarAutoresAdmin() {
    const res = await fetch(`${API_BASE}/autores`);
    const autores = await res.json();
    console.log("Autores recibidos:", autores); // Para ver en consola qué llega

    const cont = document.getElementById('admin-lista-autores');
    cont.innerHTML = '';
    
    autores.forEach(a => {
        // TRUCO: Buscamos el dato en minúscula O en mayúscula O usamos un valor por defecto
        const nombre = a.nombre || a.Nombre || "Anónimo";
        const apellido = a.apellido || a.Apellido || "";
        const id = a.autorId || a.AutorId;
        
        // Obtenemos la inicial de forma segura
        const inicial = nombre.charAt(0).toUpperCase();

        cont.innerHTML += `
        <div class="col">
            <div class="card p-3 text-center shadow-sm h-100 border-0">
                <div class="rounded-circle bg-light mx-auto d-flex align-items-center justify-content-center mb-2" style="width:50px;height:50px;font-weight:bold;">
                    ${inicial}
                </div>
                <h6 class="mb-1">${nombre} ${apellido}</h6>
                <small class="text-muted mb-2">ID: ${id}</small>
                <button onclick="eliminarAutor(${id})" class="btn btn-sm btn-outline-danger w-100 mt-auto">Eliminar</button>
            </div>
        </div>`;
    });
}

// --- CORRECCIÓN 2: Clientes (Arregla el "undefined undefined") ---
async function cargarClientesAdmin() {
    const res = await fetch(`${API_BASE}/clientes`);
    const clientes = await res.json();
    console.log("Clientes recibidos:", clientes); // Para depurar

    const lista = document.getElementById('admin-lista-clientes');
    lista.innerHTML = '';
    
    clientes.forEach(c => {
        // TRUCO: Prevenimos fallos si el campo viene vacío o con otra mayúscula
        const nombre = c.nombre || c.Nombre || "Sin Nombre";
        const apellido = c.apellido || c.Apellido || "";
        const email = c.email || c.Email || "Sin Email";
        const id = c.clienteId || c.ClienteId;

        lista.innerHTML += `<div class="list-group-item d-flex justify-content-between align-items-center">
            <div>
                <i class="fas fa-user text-muted me-2"></i>
                <strong>${nombre} ${apellido}</strong>
            </div>
            <div class="text-end">
                <small class="d-block text-muted">${email}</small>
                <span class="badge bg-light text-dark border">ID ${id}</span>
            </div>
        </div>`;
    });
}

async function cargarGenerosAdmin() {
    const res = await fetch(`${API_BASE}/generos`);
    const generos = await res.json();
    const lista = document.getElementById('admin-lista-generos');
    lista.innerHTML = '';
    generos.forEach(g => {
        lista.innerHTML += `<li class="list-group-item d-flex justify-content-between align-items-center">
            <span>ID ${g.generoId}: <strong>${g.nombre}</strong></span>
            <span class="badge bg-secondary rounded-pill">Activo</span>
        </li>`;
    });
}



async function eliminarAutor(id) {
    if(confirm("¿Borrar autor?")) {
        await fetch(`${API_BASE}/autores/${id}`, { method: 'DELETE' });
        cargarAutoresAdmin();
        mostrarNotificacion('Autor eliminado', 'success');
    }
}

async function crearGeneroRapido() {
    const nombre = prompt("Nombre del nuevo género:");
    if(nombre) {
        await fetch(`${API_BASE}/generos`, { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify({nombre}) });
        cargarGenerosAdmin();
        mostrarNotificacion('Género creado', 'success');
    }
}

/* ==========================================
   4. FUNCIONES CLIENTE (Lista Deseos / Reseñas)
   ========================================== */

async function agregarAListaDeseos() {
    // IMPORTANTE: En tu API, "ID Lista" no es igual a "ID Cliente" necesariamente.
    // Aquí hacemos una "trampa" asumiendo que el cliente ID 1 tiene la lista ID 1.
    // Lo correcto sería un endpoint GET /clientes/{id}/listasdeseos
    const listaId = estadoApp.usuario.id; 

    try {
        const res = await fetch(`${API_BASE}/listasdeseos/${listaId}/libros`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ libroISBN: libroActualISBN })
        });
        if(res.ok) mostrarNotificacion("¡Guardado en favoritos!", 'success');
        else mostrarNotificacion("No se pudo guardar (¿Ya existe en la lista?)", 'danger');
    } catch(e) { console.error(e); }
}

async function cargarMiListaDeseos() {
    const listaId = estadoApp.usuario.id;
    const offcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasLista'));
    offcanvas.show();
    
    const div = document.getElementById('lista-deseos-contenido');
    div.innerHTML = '<div class="text-center p-3"><div class="spinner-border spinner-border-sm"></div> Cargando...</div>';

    try {
        const res = await fetch(`${API_BASE}/listasdeseos/${listaId}`);
        if(!res.ok) {
            div.innerHTML = '<div class="alert alert-warning">No tienes lista de deseos activa. Contacta al admin.</div>';
            return;
        }
        const data = await res.json();
        div.innerHTML = '';
        
        if(!data.libros || data.libros.length === 0) {
            div.innerHTML = '<p class="text-muted text-center py-4">Tu lista está vacía.</p>';
            return;
        }

        data.libros.forEach(l => {
            div.innerHTML += `
            <div class="card p-2 mb-2 border-0 shadow-sm d-flex flex-row justify-content-between align-items-center">
                <div class="d-flex align-items-center gap-2">
                    <img src="https://source.unsplash.com/random/100x150/?book&sig=${l.isbn}" style="width:40px;height:50px;object-fit:cover;border-radius:4px;">
                    <div style="line-height:1.2">
                        <small class="d-block fw-bold text-truncate" style="max-width:140px;">${l.titulo}</small>
                        <small class="text-muted" style="font-size:0.7rem">${l.isbn}</small>
                    </div>
                </div>
                <button onclick="quitarDeLista('${l.isbn}')" class="btn btn-sm text-danger"><i class="fas fa-times"></i></button>
            </div>`;
        });
    } catch(e) { div.innerHTML = 'Error de conexión'; }
}

async function quitarDeLista(isbn) {
    const listaId = estadoApp.usuario.id;
    await fetch(`${API_BASE}/listasdeseos/${listaId}/libros/${isbn}`, { method: 'DELETE' });
    cargarMiListaDeseos(); // Recargar offcanvas
}

async function borrarListaCompleta() {
    if(!confirm("¿Seguro que quieres borrar toda la lista?")) return;
    const listaId = estadoApp.usuario.id;
    
    await fetch(`${API_BASE}/listasdeseos/${listaId}`, { method: 'DELETE' });
    mostrarNotificacion("Lista vaciada", 'info');
    
    // Cerrar offcanvas y recargar
    const offcanvasEl = document.getElementById('offcanvasLista');
    const offcanvas = bootstrap.Offcanvas.getInstance(offcanvasEl);
    offcanvas.hide();
}

function abrirFormularioResena() {
    new bootstrap.Modal(document.getElementById('modalCrearResena')).show();
}

document.getElementById('formResena').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const dto = {
        libroISBN: libroActualISBN,
        clienteId: parseInt(estadoApp.usuario.id),
        valoracion: parseInt(document.getElementById('resenaValor').value),
        comentario: document.getElementById('resenaTexto').value
    };

    try {
        const res = await fetch(`${API_BASE}/resenas`, { 
            method: 'POST', 
            headers: {'Content-Type':'application/json'}, 
            body: JSON.stringify(dto)
        });

        if(res.ok) {
            bootstrap.Modal.getInstance(document.getElementById('modalCrearResena')).hide();
            mostrarNotificacion("Reseña publicada", "success");
            verDetalleLibro(libroActualISBN); // Recargar modal para ver la reseña nueva
        } else {
            mostrarNotificacion("Error al publicar reseña", "danger");
        }
    } catch(e) { console.error(e); }
});