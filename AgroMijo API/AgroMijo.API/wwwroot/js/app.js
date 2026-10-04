const API_URL = "";


/* =========================================================
   ELEMENTOS
========================================================= */

const profileIdInput =
    document.getElementById("profileId");

const dateFromInput =
    document.getElementById("dateFrom");

const dateToInput =
    document.getElementById("dateTo");

const searchButton =
    document.getElementById("searchButton");

const clearButton =
    document.getElementById("clearButton");

const backButton =
    document.getElementById("backButton");

const message =
    document.getElementById("message");

const resultsSection =
    document.getElementById("resultsSection");

const detailSection =
    document.getElementById("detailSection");

const reportsContainer =
    document.getElementById("reportsContainer");

const reportDetail =
    document.getElementById("reportDetail");

const pagination =
    document.getElementById("pagination");

const resultsSummary =
    document.getElementById("resultsSummary");

const compareButton =
    document.getElementById("compareButton");

const comparisonStatus =
    document.getElementById("comparisonStatus");


/* =========================================================
   ESTADO
========================================================= */

let reportesSeleccionados = [];

let reportesActuales = [];

let paginaActual = 1;

const REPORTES_POR_PAGINA = 10;


/* =========================================================
   EVENTOS
========================================================= */

searchButton.addEventListener(
    "click",
    buscarReportes
);

clearButton.addEventListener(
    "click",
    limpiarBusqueda
);

backButton.addEventListener(
    "click",
    volverAReportes
);
compareButton.addEventListener(
    "click",
    compararSeleccionados
);


/* =========================================================
   BUSCAR
========================================================= */

async function buscarReportes() {

    const profileId =
        profileIdInput.value.trim().toUpperCase();

    const fechaDesde =
        dateFromInput.value;

    const fechaHasta =
        dateToInput.value;


    if (!profileId && !fechaDesde && !fechaHasta) {

        mostrarMensaje(
            "Ingresa un perfil o selecciona un rango de fechas.",
            true
        );

        return;
    }


    if (
        (fechaDesde && !fechaHasta) ||
        (!fechaDesde && fechaHasta)
    ) {

        mostrarMensaje(
            "Debes seleccionar las dos fechas.",
            true
        );

        return;
    }


    if (
        fechaDesde &&
        fechaHasta &&
        fechaDesde > fechaHasta
    ) {

        mostrarMensaje(
            "La fecha inicial no puede ser posterior a la fecha final.",
            true
        );

        return;
    }


    mostrarMensaje(
        "Consultando...",
        false
    );


    resultsSection.classList.add("hidden");
    detailSection.classList.add("hidden");

    reportsContainer.innerHTML = "";
    pagination.innerHTML = "";


    try {

        let url;


        // Perfil + fechas
        if (
            profileId &&
            fechaDesde &&
            fechaHasta
        ) {

            url =
                `${API_URL}/api/reports/date` +
                `?desde=${encodeURIComponent(fechaDesde)}` +
                `&hasta=${encodeURIComponent(fechaHasta)}` +
                `&profileId=${encodeURIComponent(profileId)}`;
        }


        // Solo fechas
        else if (
            fechaDesde &&
            fechaHasta
        ) {

            url =
                `${API_URL}/api/reports/date` +
                `?desde=${encodeURIComponent(fechaDesde)}` +
                `&hasta=${encodeURIComponent(fechaHasta)}`;
        }


        // Solo perfil
        else {

            url =
                `${API_URL}/api/reports/profile/` +
                encodeURIComponent(profileId);
        }


        const response =
            await fetch(url);


        if (!response.ok) {

            throw new Error(
                "No se pudo consultar el servidor."
            );
        }


        const reports =
            await response.json();


        reportesActuales =
            Array.isArray(reports)
                ? reports
                : [reports];


        paginaActual = 1;

        mostrarReportes();


    }
    catch (error) {

        console.error(error);

        mostrarMensaje(
            "No se pudo conectar con el servidor.",
            true
        );
    }
}


/* =========================================================
   MOSTRAR REPORTES
========================================================= */

function mostrarReportes() {

    resultsSection.classList.remove("hidden");

    reportsContainer.innerHTML = "";
    pagination.innerHTML = "";


    if (reportesActuales.length === 0) {

        resultsSummary.textContent =
            "No se encontraron reportes.";

        mostrarMensaje(
            "No se encontraron reportes para los filtros seleccionados.",
            false
        );

        return;
    }


    resultsSummary.textContent =
        `${reportesActuales.length} partida(s) encontrada(s).`;


    const inicio =
        (paginaActual - 1) *
        REPORTES_POR_PAGINA;

    const fin =
        inicio +
        REPORTES_POR_PAGINA;


    const reportesPagina =
        reportesActuales.slice(
            inicio,
            fin
        );


    reportesPagina.forEach(
        report => {

            reportsContainer.appendChild(
                crearFilaReporte(report)
            );

        }
    );


    generarPaginacion();


    mostrarMensaje(
        "",
        false
    );
}


/* =========================================================
   TARJETA DE REPORTE
========================================================= */

function crearFilaReporte(report) {

    const fila = document.createElement("tr");

    const metricas =
        obtenerMetricasReporte(report);

    const estaSeleccionado =
        reportesSeleccionados.includes(
            report.reportId
        );

    fila.innerHTML = `
        <td class="comparison-cell">

            <input
                type="checkbox"
                class="report-checkbox"
                data-report-id="${escaparHtml(report.reportId)}"
                ${estaSeleccionado ? "checked" : ""}
            >

        </td>

        <td>
            <span class="table-report-id">
                ${escaparHtml(report.reportId)}
            </span>
        </td>

        <td>
            <div class="table-profile">

                <strong>
                    ${escaparHtml(report.profileAlias)}
                </strong>

                <span>
                    ${escaparHtml(report.profileId)}
                </span>

            </div>
        </td>

        <td>
            ${escaparHtml(report.fechaPartida)}
        </td>

        <td>
            ${report.ciclosJugados}
        </td>

        <td>
            ${metricas.decisiones}
        </td>

        <td>
            ${metricas.eventos}
        </td>

        <td>
            ${metricas.cosechas}
        </td>

        <td class="money-cell">
            ${formatearDinero(
        report.presupuestoFinal
    )}
        </td>

        <td>

            <button
                class="detail-button table-detail-button"
            >
                Ver
            </button>

        </td>
    `;

    const checkbox =
        fila.querySelector(
            ".report-checkbox"
        );

    checkbox.addEventListener(
        "change",
        () => manejarSeleccionReporte(
            report.reportId,
            checkbox.checked
        )
    );

    const detailButton =
        fila.querySelector(
            ".detail-button"
        );

    detailButton.addEventListener(
        "click",
        () => mostrarDetalle(report)
    );

    return fila;
}
function manejarSeleccionReporte(
    reportId,
    seleccionado
) {

    if (seleccionado) {

        if (
            reportesSeleccionados.length >= 2
        ) {
            mostrarMensaje(
                "Solo puedes seleccionar 2 partidas para compararlas.",
                true
            );

            mostrarReportes();

            return;
        }

        if (
            !reportesSeleccionados.includes(
                reportId
            )
        ) {
            reportesSeleccionados.push(
                reportId
            );
        }

    }
    else {

        reportesSeleccionados =
            reportesSeleccionados.filter(
                id => id !== reportId
            );
    }

    actualizarEstadoComparacion();
}
function actualizarEstadoComparacion() {

    const cantidad =
        reportesSeleccionados.length;

    if (cantidad === 0) {

        comparisonStatus.textContent =
            "Selecciona 2 partidas para compararlas.";

    }
    else if (cantidad === 1) {

        comparisonStatus.textContent =
            "Selecciona una partida más para comparar.";

    }
    else {

        comparisonStatus.textContent =
            "2 partidas seleccionadas.";

    }

    compareButton.disabled =
        cantidad !== 2;
}
function compararSeleccionados() {

    if (
        reportesSeleccionados.length !== 2
    ) {
        return;
    }

    const reportes =
        reportesActuales.filter(
            report =>
                reportesSeleccionados.includes(
                    report.reportId
                )
        );

    console.log(
        "Partidas seleccionadas:",
        reportes
    );

    mostrarMensaje(
        "Partidas listas para comparar.",
        false
    );
}


/* =========================================================
   MÉTRICAS
========================================================= */

function obtenerMetricasReporte(report) {

    const ciclos =
        report.historialCiclos || [];


    let decisiones = 0;
    let eventos = 0;
    let cosechas = 0;


    let hayDatosDeDecisiones = false;


    ciclos.forEach(ciclo => {

        if (
            Array.isArray(
                ciclo.decisionesTomadas
            )
        ) {

            hayDatosDeDecisiones = true;

            decisiones +=
                ciclo.decisionesTomadas.length;
        }


        if (
            Array.isArray(
                ciclo.eventosOcurridos
            )
        ) {

            eventos +=
                ciclo.eventosOcurridos.length;
        }


        if (
            Array.isArray(
                ciclo.cosechasRealizadas
            )
        ) {

            cosechas +=
                ciclo.cosechasRealizadas.length;
        }

    });


    return {

        decisiones:
            hayDatosDeDecisiones
                ? decisiones
                : "N/D",

        eventos,

        cosechas

    };
}


/* =========================================================
   DETALLE
========================================================= */

function mostrarDetalle(report) {

    resultsSection.classList.add("hidden");

    detailSection.classList.remove("hidden");


    const metricas =
        obtenerMetricasReporte(report);


    reportDetail.innerHTML = `

        <div class="detail-header">

            <h2>
                Reporte ${escaparHtml(report.reportId)}
            </h2>

            <div class="detail-profile">

                Perfil:
                <strong>
                    ${escaparHtml(report.profileAlias)}
                </strong>

                (${escaparHtml(report.profileId)})

                <br>

                Fecha:
                ${escaparHtml(report.fechaPartida)}

                <br>

                Razón de finalización:
                ${escaparHtml(report.razonFin)}

            </div>

        </div>


        <div class="detail-summary">

            <div class="metric">

                <span class="metric-label">
                    Ciclos
                </span>

                <span class="metric-value">
                    ${report.ciclosJugados}
                </span>

            </div>


            <div class="metric">

                <span class="metric-label">
                    Decisiones
                </span>

                <span class="metric-value">
                    ${metricas.decisiones}
                </span>

            </div>


            <div class="metric">

                <span class="metric-label">
                    Eventos
                </span>

                <span class="metric-value">
                    ${metricas.eventos}
                </span>

            </div>


            <div class="metric">

                <span class="metric-label">
                    Cosechas
                </span>

                <span class="metric-value">
                    ${metricas.cosechas}
                </span>

            </div>

        </div>


        <div class="detail-financial">

            <div class="financial-card">

                <strong>
                    Presupuesto inicial
                </strong>

                ${formatearDinero(
        report.presupuestoInicial
    )}

            </div>


            <div class="financial-card">

                <strong>
                    Presupuesto final
                </strong>

                ${formatearDinero(
        report.presupuestoFinal
    )}

            </div>


            <div class="financial-card">

                <strong>
                    Ganancia acumulada
                </strong>

                ${formatearDinero(
        report.gananciaAcumulada
    )}

            </div>


            <div class="financial-card">

                <strong>
                    Gasto acumulado
                </strong>

                ${formatearDinero(
        report.gastoAcumulado
    )}

            </div>

        </div>


        <h3 class="detail-section-title">
            Historial de ciclos
        </h3>


        <div class="periods-container">

            ${generarPeriodos(
        report.historialCiclos
    )}

        </div>

    `;
}


/* =========================================================
   CICLOS
========================================================= */

function generarPeriodos(periodos) {

    if (
        !periodos ||
        periodos.length === 0
    ) {

        return `
            <p>
                No hay información de ciclos.
            </p>
        `;
    }


    return periodos.map(
        (periodo, index) => {

            const numeroPeriodo =
                periodo.numeroCiclo ??
                (index + 1);


            return `

                <details class="period-card">

                    <summary>
                        Ciclo ${numeroPeriodo}
                    </summary>


                    <div class="period-content">


                        <div class="period-summary">

                            <div>

                                <strong>
                                    Presupuesto inicial
                                </strong>

                                ${formatearDinero(
                periodo.presupuestoInicial
            )}

                            </div>


                            <div>

                                <strong>
                                    Presupuesto final
                                </strong>

                                ${formatearDinero(
                periodo.presupuestoFinal
            )}

                            </div>


                            <div>

                                <strong>
                                    Ganancia
                                </strong>

                                ${formatearDinero(
                periodo.gananciaTotal
            )}

                            </div>


                            <div>

                                <strong>
                                    Gastos
                                </strong>

                                ${formatearDinero(
                periodo.gastoTotal
            )}

                            </div>

                        </div>


                        ${generarDecisiones(
                periodo.decisionesTomadas
            )}


                        ${generarGastos(
                periodo.detalleGastos
            )}


                        ${generarEventos(
                periodo.eventosOcurridos
            )}


                        ${generarCosechas(
                periodo.cosechasRealizadas
            )}


                        <div class="jornales">

                            <strong>
                                Jornales
                            </strong>

                            <p>
                                Necesarios:
                                ${periodo.jornalesNecesarios}
                            </p>

                            <p>
                                Familiares:
                                ${periodo.jornalesFamiliaresUsados}
                            </p>

                            <p>
                                Contratados:
                                ${periodo.jornalesContratados}
                            </p>

                        </div>

                    </div>

                </details>

            `;

        }
    ).join("");
}


/* =========================================================
   DECISIONES
========================================================= */

function generarDecisiones(decisiones) {

    if (!Array.isArray(decisiones)) {

        return `

            <div class="period-detail">

                <strong>
                    Decisiones
                </strong>

                <p class="decisions-unavailable">
                    Información no disponible para
                    partidas registradas antes de la
                    incorporación de este dato.
                </p>

            </div>

        `;
    }


    if (decisiones.length === 0) {

        return `

            <div class="period-detail">

                <strong>
                    Decisiones
                </strong>

                <p>
                    No se registraron decisiones
                    en este ciclo.
                </p>

            </div>

        `;
    }


    return `

        <div class="period-detail">

            <strong>
                Decisiones tomadas
            </strong>

            <ul class="decisions-list">

                ${decisiones.map(
        decision => `
                        <li>
                            ${escaparHtml(decision)}
                        </li>
                    `
    ).join("")}

            </ul>

        </div>

    `;
}


/* =========================================================
   GASTOS
========================================================= */

function generarGastos(gastos) {

    if (
        !gastos ||
        gastos.length === 0
    ) {

        return `

            <div class="period-detail">

                <strong>
                    Gastos
                </strong>

                <p>
                    No hubo gastos registrados.
                </p>

            </div>

        `;
    }


    return `

        <div class="period-detail">

            <strong>
                Gastos
            </strong>

            <ul>

                ${gastos.map(
        gasto => `
                        <li>
                            ${escaparHtml(
            gasto.descripcion
        )}
                            —
                            ${formatearDinero(
            gasto.monto
        )}
                        </li>
                    `
    ).join("")}

            </ul>

        </div>

    `;
}


/* =========================================================
   EVENTOS
========================================================= */

function generarEventos(eventos) {

    if (
        !eventos ||
        eventos.length === 0
    ) {

        return `

            <div class="period-detail">

                <strong>
                    Eventos
                </strong>

                <p>
                    No ocurrieron eventos.
                </p>

            </div>

        `;
    }


    return `

        <div class="period-detail">

            <strong>
                Eventos
            </strong>

            <ul>

                ${eventos.map(
        evento => `
                        <li>
                            ${escaparHtml(evento)}
                        </li>
                    `
    ).join("")}

            </ul>

        </div>

    `;
}


/* =========================================================
   COSECHAS
========================================================= */

function generarCosechas(cosechas) {

    if (
        !cosechas ||
        cosechas.length === 0
    ) {

        return `

            <div class="period-detail">

                <strong>
                    Cosechas
                </strong>

                <p>
                    No se realizaron cosechas.
                </p>

            </div>

        `;
    }


    return `

        <div class="period-detail">

            <strong>
                Cosechas
            </strong>

            <ul>

                ${cosechas.map(
        cosecha => `
                        <li>
                            ${escaparHtml(cosecha)}
                        </li>
                    `
    ).join("")}

            </ul>

        </div>

    `;
}


/* =========================================================
   PAGINACIÓN
========================================================= */

function generarPaginacion() {

    pagination.innerHTML = "";


    const totalPaginas =
        Math.ceil(
            reportesActuales.length /
            REPORTES_POR_PAGINA
        );


    if (totalPaginas <= 1)
        return;


    const botonAnterior =
        document.createElement("button");

    botonAnterior.textContent = "‹";

    botonAnterior.disabled =
        paginaActual === 1;


    botonAnterior.addEventListener(
        "click",
        () => {

            if (paginaActual > 1) {

                paginaActual--;

                mostrarReportes();
            }

        }
    );


    pagination.appendChild(
        botonAnterior
    );


    for (
        let pagina = 1;
        pagina <= totalPaginas;
        pagina++
    ) {

        const boton =
            document.createElement("button");

        boton.textContent = pagina;


        if (
            pagina === paginaActual
        ) {

            boton.classList.add(
                "active"
            );
        }


        boton.addEventListener(
            "click",
            () => {

                paginaActual = pagina;

                mostrarReportes();

            }
        );


        pagination.appendChild(
            boton
        );
    }


    const botonSiguiente =
        document.createElement("button");

    botonSiguiente.textContent = "›";

    botonSiguiente.disabled =
        paginaActual === totalPaginas;


    botonSiguiente.addEventListener(
        "click",
        () => {

            if (
                paginaActual <
                totalPaginas
            ) {

                paginaActual++;

                mostrarReportes();

            }

        }
    );


    pagination.appendChild(
        botonSiguiente
    );

}


/* =========================================================
   VOLVER
========================================================= */

function volverAReportes() {

    detailSection.classList.add("hidden");

    resultsSection.classList.remove("hidden");

}


/* =========================================================
   LIMPIAR
========================================================= */

function limpiarBusqueda() {

    profileIdInput.value = "";
    dateFromInput.value = "";
    dateToInput.value = "";


    resultsSection.classList.add(
        "hidden"
    );

    detailSection.classList.add(
        "hidden"
    );


    reportsContainer.innerHTML = "";

    reportDetail.innerHTML = "";

    pagination.innerHTML = "";


    reportesActuales = [];

    paginaActual = 1;


    mostrarMensaje(
        "",
        false
    );

}


/* =========================================================
   FORMATO
========================================================= */

function formatearDinero(valor) {

    return new Intl.NumberFormat(
        "es-CO",
        {
            style: "currency",
            currency: "COP",
            maximumFractionDigits: 0
        }
    ).format(valor || 0);

}


/* =========================================================
   MENSAJES
========================================================= */

function mostrarMensaje(
    texto,
    esError
) {

    message.textContent = texto;

    message.className =
        esError
            ? "message error"
            : "message success";

}


/* =========================================================
   SEGURIDAD HTML
========================================================= */

function escaparHtml(valor) {

    if (
        valor === null ||
        valor === undefined
    ) {
        return "";
    }


    const div =
        document.createElement("div");

    div.textContent =
        String(valor);

    return div.innerHTML;

}