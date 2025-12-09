// app.js

const API_BASE = 'http://localhost:7755/api'; 

// ESTADO GLOBAL
let estadoApp = {
    usuario: null, 
    libros: [],    
    generos: [],
    listaIdActual: null,
    favoritosISBN: [] // AQUÍ guardaremos los ISBNs que tienes en favoritos
};
let libroActualISBN = '';

document.addEventListener('DOMContentLoaded', () => {
    console.log("App iniciada. Esperando login...");
    document.getElementById('buscador').addEventListener('input', (e) => {
        filtrarLibrosCliente(e.target.value.toLowerCase());
    });
});

/* --- UTILIDAD: Notificaciones --- */
function mostrarNotificacion(mensaje, tipo = 'info') {
    const contenedor = document.getElementById('toast-container');
    const toast = document.createElement('div');
    let colorClass = tipo === 'success' ? 'border-success' : (tipo === 'danger' ? 'border-danger' : 'border-primary');
    let icon = tipo === 'success' ? 'check-circle text-success' : (tipo === 'danger' ? 'exclamation-circle text-danger' : 'info-circle text-primary');

    toast.className = `custom-toast ${colorClass}`;
    toast.innerHTML = `<i class="fas fa-${icon} fa-lg"></i> <span>${mensaje}</span>`;
    contenedor.appendChild(toast);
    setTimeout(() => { toast.remove(); }, 3000);
}

/* ==========================================
   1. LOGIN Y CAMBIO DE VISTA
   ========================================== */

function iniciarSesion(rol) {
    if (rol === 'admin') {
        estadoApp.usuario = { rol: 'admin' };
        mostrarNotificacion("Bienvenido, Administrador", "success");
        configurarVistaAdmin();
    } else {
        document.getElementById('login-cliente-input').style.display = 'block';
    }
}

async function confirmarLoginCliente() {
    const idInput = document.getElementById('inputClienteId').value;
    if (!idInput) return mostrarNotificacion("Por favor ingresa un ID", "danger");

    try {
        const res = await fetch(`${API_BASE}/clientes/${idInput}`);
        if (res.ok) {
            estadoApp.usuario = { id: idInput, rol: 'cliente' };
            mostrarNotificacion(`Hola, Lector #${idInput}`, "success");
            configurarVistaCliente();
        } else {
            mostrarNotificacion("Cliente no encontrado", "danger");
        }
    } catch (e) { mostrarNotificacion("Error API", "danger"); }
}

function configurarVistaAdmin() {
    document.getElementById('login-screen').classList.add('d-none');
    document.getElementById('app-interface').classList.remove('d-none');
    document.getElementById('menu-admin').classList.remove('d-none');
    document.getElementById('vista-admin').classList.remove('d-none');
    
    cargarLibrosAdmin();
    cargarAutoresAdmin();
    cargarGenerosAdmin();
    cargarClientesAdmin();
}

async function configurarVistaCliente() {
    document.getElementById('login-screen').classList.add('d-none');
    document.getElementById('app-interface').classList.remove('d-none');
    document.getElementById('menu-cliente').classList.remove('d-none');
    document.getElementById('display-user-id').innerText = estadoApp.usuario.id;
    document.getElementById('hero-store').classList.remove('d-none');
    document.getElementById('vista-cliente').classList.remove('d-none');

    // 1. Primero cargamos la lista de deseos para saber qué corazones pintar
    await sincronizarFavoritos();
    // 2. Luego cargamos libros y filtros
    cargarLibrosCliente();
    cargarGenerosFiltros();
}

/* ==========================================
   2. TIENDA (CLIENTE)
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
    
    if(!libros || libros.length === 0) {
        contenedor.innerHTML = '<div class="col-12 text-center text-muted">No hay libros disponibles.</div>';
        return;
    }

    libros.forEach(libro => {
        // Determinamos si el corazón debe estar relleno
        const esFavorito = estadoApp.favoritosISBN.includes(libro.isbn);
        const corazonClase = esFavorito ? "fas fa-heart text-danger" : "far fa-heart";
        const accionCorazon = esFavorito ? `quitarDeLista('${libro.isbn}', false)` : `agregarDesdeCard('${libro.isbn}')`;

        contenedor.innerHTML += `
            <div class="col">
                <div class="book-card h-100">
                    <div class="card-img-wrapper" onclick="verDetalleLibro('${libro.isbn}')" style="cursor:pointer;">
                        <img src="https://source.unsplash.com/random/400x600/?book&sig=${libro.isbn}" class="book-img">
                        <div class="card-actions-overlay">
                            <!-- Botón Ojo -->
                            <button class="btn btn-icon me-2"><i class="fas fa-eye"></i></button>
                        </div>
                    </div>
                    <div class="card-body position-relative">
                        <!-- Botón Corazón Flotante en el Card -->
                        <button onclick="${accionCorazon}; event.stopPropagation();" class="btn btn-sm btn-light rounded-circle shadow-sm position-absolute top-0 end-0 mt-2 me-2 border" style="width:35px;height:35px;z-index:10;">
                            <i class="${corazonClase}"></i>
                        </button>

                        <h5 class="card-title serif-font pe-4">${libro.titulo}</h5>
                        <p class="price-tag text-warning fw-bold">$${libro.precioVenta}</p>
                    </div>
                </div>
            </div>`;
    });
}

// Helper para el botón del card
window.agregarDesdeCard = async (isbn) => {
    libroActualISBN = isbn;
    await agregarAListaDeseos();
}

// --- FILTROS ---

async function cargarGenerosFiltros() {
    try {
        const res = await fetch(`${API_BASE}/generos`);
        const generos = await res.json();
        const contenedor = document.getElementById('filtros-genero-hero');
        
        contenedor.innerHTML = `
            <button id="filter-btn-all" class="btn btn-sm rounded-pill filter-btn active-filter" 
            onclick="resetearFiltrosUI()">Todos</button>`;
        
        generos.forEach(g => {
            const nombre = g.nombre || g.Nombre;
            const id = g.generoId || g.GeneroId;
            contenedor.innerHTML += `
                <button id="filter-btn-${id}" class="btn btn-sm rounded-pill filter-btn inactive-filter" 
                onclick="filtrarPorGenero(${id})">${nombre}</button>`;
        });
    } catch (e) {}
}

function filtrarPorGenero(id) {
    const filtrados = estadoApp.libros.filter(l => (l.generoId || l.GeneroId) === id);
    renderizarTienda(filtrados);
    cambiarEstiloBotones(`filter-btn-${id}`);
}

function resetearFiltrosUI() {
    renderizarTienda(estadoApp.libros);
    cambiarEstiloBotones('filter-btn-all');
}

function cambiarEstiloBotones(idActivo) {
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.classList.remove('active-filter');
        btn.classList.add('inactive-filter');
    });
    
    const btn = document.getElementById(idActivo);
    if(btn) {
        btn.classList.remove('inactive-filter');
        btn.classList.add('active-filter');
    }
}

function filtrarLibrosCliente(termino) {
    const filtrados = estadoApp.libros.filter(l => l.titulo.toLowerCase().includes(termino));
    renderizarTienda(filtrados);
}

// --- DETALLES Y RESEÑAS ---

window.verDetalleLibro = async function(isbn) {
    libroActualISBN = isbn;
    
    const libro = estadoApp.libros.find(l => l.isbn === isbn);
    if(!libro) return;

    document.getElementById('modalTitulo').innerText = libro.titulo;
    document.getElementById('modalPrecio').innerText = `$${libro.precioVenta}`;
    document.getElementById('modalImagen').src = `https://source.unsplash.com/random/400x600/?book&sig=${isbn}`;
    
    const cont = document.getElementById('contenedor-resenas');
    cont.innerHTML = 'Cargando opiniones...';
    
    try {
        const res = await fetch(`${API_BASE}/libros/${isbn}/resenas`);
        if(res.ok) {
            const resenas = await res.json();
            cont.innerHTML = resenas.length ? '' : '<small>Sin reseñas aún.</small>';
            resenas.forEach(r => {
                const puntos = r.puntuacion || r.valoracion || r.Puntuacion;
                const comentario = r.comentario || r.Comentario;
                const titulo = r.tituloReseña || r.TituloReseña || "Reseña";
                
                cont.innerHTML += `
                <div class="mb-2 border-bottom">
                    <div class="d-flex justify-content-between">
                        <small class="fw-bold">${titulo}</small>
                        <small class="text-warning">${'★'.repeat(puntos)}</small>
                    </div>
                    <small class="text-muted d-block">"${comentario}"</small>
                </div>`;
            });
        } else {
            cont.innerHTML = '<small>No hay reseñas.</small>';
        }
    } catch(e) { cont.innerHTML = '<small>Error de conexión.</small>'; }

    new bootstrap.Modal(document.getElementById('detalleModal')).show();
}

/* ==========================================
   3. ADMIN Y CRUD (Sin cambios, funciona bien)
   ========================================== */

async function cargarLibrosAdmin() {
    const tabla = document.getElementById('admin-tabla-libros');
    tabla.innerHTML = 'Cargando...';
    try {
        const res = await fetch(`${API_BASE}/libros`);
        const libros = await res.json();
        let html = `<table class="table table-hover bg-white shadow-sm rounded">
            <thead class="table-dark"><tr><th>Título</th><th>Precio</th><th>Acción</th></tr></thead><tbody>`;
        libros.forEach(l => {
            html += `<tr>
                <td>${l.titulo}</td>
                <td>$${l.precioVenta}</td>
                <td><button onclick="eliminarLibro('${l.isbn}')" class="btn btn-sm btn-outline-danger"><i class="fas fa-trash"></i></button></td>
            </tr>`;
        });
        tabla.innerHTML = html + '</tbody></table>';
    } catch(e) { tabla.innerHTML = 'Error de conexión.'; }
}

async function cargarClientesAdmin() {
    const res = await fetch(`${API_BASE}/clientes`);
    const clientes = await res.json();
    const lista = document.getElementById('admin-lista-clientes');
    lista.innerHTML = '';
    clientes.forEach(c => {
        const nombre = c.nombreUsuario || c.NombreUsuario || "Sin Nombre";
        const email = c.email || c.Email || "";
        lista.innerHTML += `<div class="list-group-item d-flex justify-content-between"><span>${nombre}</span><span class="text-muted">${email}</span></div>`;
    });
}

async function cargarAutoresAdmin() {
    const res = await fetch(`${API_BASE}/autores`);
    const autores = await res.json();
    const cont = document.getElementById('admin-lista-autores');
    cont.innerHTML = '';
    autores.forEach(a => {
        const nombre = a.nombreCompleto || a.NombreCompleto || "Anónimo";
        const id = a.autorId || a.AutorId;
        let inicial = "A";
        if(nombre && nombre !== "Anónimo") inicial = nombre.charAt(0).toUpperCase();
        cont.innerHTML += `<div class="col"><div class="card p-2 text-center shadow-sm"><h3>${inicial}</h3><h6>${nombre}</h6><button onclick="eliminarAutor(${id})" class="btn btn-sm text-danger">x</button></div></div>`;
    });
}

async function cargarGenerosAdmin() {
    const res = await fetch(`${API_BASE}/generos`);
    const generos = await res.json();
    const lista = document.getElementById('admin-lista-generos');
    lista.innerHTML = '';
    generos.forEach(g => {
        const nombre = g.nombre || g.Nombre;
        lista.innerHTML += `<li class="list-group-item">${nombre}</li>`;
    });
}

window.eliminarLibro = async (isbn) => {
    if(confirm("¿Borrar?")) { await fetch(`${API_BASE}/libros/${isbn}`, {method:'DELETE'}); cargarLibrosAdmin(); }
};
window.eliminarAutor = async (id) => {
    if(confirm("¿Borrar?")) { await fetch(`${API_BASE}/autores/${id}`, {method:'DELETE'}); cargarAutoresAdmin(); }
};
window.crearGeneroRapido = async () => {
    const n = prompt("Nombre:");
    if(n) { await fetch(`${API_BASE}/generos`, {method:'POST', headers:{'Content-Type':'application/json'}, body:JSON.stringify({nombre:n})}); cargarGenerosAdmin(); }
};

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
    await fetch(`${API_BASE}/libros`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(nuevoLibro) });
    bootstrap.Modal.getInstance(document.getElementById('modalCrearLibro')).hide();
    cargarLibrosAdmin();
});

/* ==========================================
   4. LÓGICA INTELIGENTE DE LISTA DE DESEOS (ARREGLADA)
   ========================================== */

// Nueva función central para sincronizar qué es favorito y qué no
async function sincronizarFavoritos() {
    const listaId = await obtenerListaIdUsuario();
    if (!listaId) return;

    try {
        const res = await fetch(`${API_BASE}/listasdeseos/${listaId}`);
        const data = await res.json();
        const listaLibros = data.libros || data.Libros || data.items || data.Items || [];
        
        // Guardamos SOLO los ISBNs en el estado para comprobar rápido
        estadoApp.listaIdActual = listaId;
        estadoApp.favoritosISBN = listaLibros.map(l => l.isbn || l.ISBN);
        
        // Actualizamos la tienda (para pintar corazones) si ya está cargada
        if(estadoApp.libros.length > 0) renderizarTienda(estadoApp.libros);

    } catch (e) { console.error("Error sync favoritos", e); }
}

async function obtenerListaIdUsuario() {
    const userId = estadoApp.usuario.id;
    try {
        const res = await fetch(`${API_BASE}/clientes/${userId}/listasdeseos`);
        const listas = await res.json();
        if (listas && listas.length > 0) return listas[0].listaId || listas[0].ListaId; 

        // Si no tiene, creamos
        const nuevaLista = { clienteId: parseInt(userId), nombre: "Favoritos", descripcion: "Mi lista", esPublica: true };
        const createRes = await fetch(`${API_BASE}/listasdeseos`, { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(nuevaLista)});
        const listaCreada = await createRes.json();
        return listaCreada.listaId || listaCreada.ListaId;
    } catch (e) { return null; }
}

window.agregarAListaDeseos = async function() {
    const listaIdReal = await obtenerListaIdUsuario();
    if (!listaIdReal) return;

    try {
        const res = await fetch(`${API_BASE}/listasdeseos/${listaIdReal}/libros`, {
            method: 'POST',
            headers: {'Content-Type':'application/json'},
            body: JSON.stringify({libroISBN: libroActualISBN})
        });

        if(res.ok) {
            mostrarNotificacion("¡Añadido a favoritos!", 'success');
            await sincronizarFavoritos(); // Actualizar corazones
        } else {
            mostrarNotificacion("Ya está en tu lista", "danger");
        }
    } catch(e) { mostrarNotificacion("Error", "danger"); }
}

// --- ARREGLO "PANTALLA OSCURA": Función de pintar HTML separada del Offcanvas ---
async function pintarHTMLListaDeseos() {
    const div = document.getElementById('lista-deseos-contenido');
    div.innerHTML = '<div class="text-center p-3"><div class="spinner-border spinner-border-sm"></div></div>';

    const listaIdReal = await obtenerListaIdUsuario();
    if (!listaIdReal) return;

    try {
        const res = await fetch(`${API_BASE}/listasdeseos/${listaIdReal}`);
        const data = await res.json();
        div.innerHTML = '';
        
        const listaLibros = data.libros || data.Libros || [];
        if(!listaLibros.length) {
            div.innerHTML = '<p class="text-muted p-2 text-center">Lista vacía</p>';
        } else {
            listaLibros.forEach(l => {
                const titulo = l.titulo || l.Titulo;
                const isbn = l.isbn || l.ISBN;
                div.innerHTML += `
                <div class="card p-2 mb-2 d-flex flex-row justify-content-between align-items-center">
                    <span class="text-truncate" style="max-width: 180px;">${titulo}</span>
                    <button onclick="quitarDeLista('${isbn}', true)" class="btn btn-sm text-danger" title="Quitar">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>`;
            });
        }
        // Actualizamos estado global
        estadoApp.favoritosISBN = listaLibros.map(l => l.isbn || l.ISBN);
        renderizarTienda(estadoApp.libros); // Refrescar iconos tienda
    } catch(e) { div.innerHTML = 'Error'; }
}

window.cargarMiListaDeseos = function() {
    // 1. Abrimos el offcanvas
    const offcanvasEl = document.getElementById('offcanvasLista');
    const offcanvas = bootstrap.Offcanvas.getOrCreateInstance(offcanvasEl); // USAMOS getOrCreateInstance
    offcanvas.show();
    
    // 2. Pintamos el contenido
    pintarHTMLListaDeseos();
}

window.quitarDeLista = async function(isbn, esDesdeOffcanvas = false) {
    if (!estadoApp.listaIdActual) await sincronizarFavoritos();
    
    await fetch(`${API_BASE}/listasdeseos/${estadoApp.listaIdActual}/libros/${isbn}`, {method:'DELETE'});
    
    // TRUCO: Si estamos en el offcanvas, solo repintamos el HTML, NO abrimos el offcanvas de nuevo
    if (esDesdeOffcanvas) {
        pintarHTMLListaDeseos();
    } else {
        // Si borramos desde la tarjeta (tienda), actualizamos iconos
        await sincronizarFavoritos();
    }
}

window.borrarListaCompleta = async function() {
    if(!confirm("¿Borrar todo?")) return;
    await fetch(`${API_BASE}/listasdeseos/${estadoApp.listaIdActual}`, {method:'DELETE'});
    
    const offcanvasEl = document.getElementById('offcanvasLista');
    const offcanvas = bootstrap.Offcanvas.getInstance(offcanvasEl); // Obtener instancia existente
    if(offcanvas) offcanvas.hide(); // Cerrar correctamente
    
    mostrarNotificacion("Lista eliminada", "info");
    await sincronizarFavoritos();
}

/* ==========================================
   5. RESEÑAS
   ========================================== */

window.abrirFormularioResena = function() {
    new bootstrap.Modal(document.getElementById('modalCrearResena')).show();
}

document.getElementById('formResena').addEventListener('submit', async (e) => {
    e.preventDefault();
    const dto = {
        libroISBN: libroActualISBN,
        clienteId: parseInt(estadoApp.usuario.id),
        tituloReseña: document.getElementById('resenaTitulo').value,
        puntuacion: parseInt(document.getElementById('resenaValor').value),
        comentario: document.getElementById('resenaTexto').value,
        fechaReseña: new Date().toISOString()
    };

    try {
        const res = await fetch(`${API_BASE}/resenas`, { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(dto)});
        if(res.ok) {
            document.getElementById('formResena').reset();
            bootstrap.Modal.getInstance(document.getElementById('modalCrearResena')).hide();
            mostrarNotificacion("Reseña publicada", "success");
            verDetalleLibro(libroActualISBN);
        } else { mostrarNotificacion("Error al publicar.", "danger"); }
    } catch(e) { console.error(e); }
});