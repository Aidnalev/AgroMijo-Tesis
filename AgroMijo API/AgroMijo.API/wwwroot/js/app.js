const API_URL = "";


/* =========================================
   ELEMENTOS DEL DOM
   ========================================= */

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

const comparisonSection =
    document.getElementById("comparisonSection");

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

const comparisonContent =
    document.getElementById("comparisonContent");

const backFromComparisonButton =
    document.getElementById(
        "backFromComparisonButton"
    );


/* =========================================
   ESTADO
   ========================================= */

let reportesActuales = [];

let paginaActual = 1;

const REPORTES_POR_PAGINA = 10;

let reportesSeleccionados = [];


/* =========================================
   GRÁFICOS
   ========================================= */

let budgetChart = null;

let cycleFinancialChart = null;

let comparisonFinancialChart = null;

let comparisonActivityChart = null;


/* =========================================
   EVENTOS
   ========================================= */

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

backFromComparisonButton.addEventListener(
    "click",
    volverAComparacion
);


/* =========================================
   BÚSQUEDA
   ========================================= */

async function buscarReportes() {

    const profileId =
        profileIdInput.value
            .trim()
            .toUpperCase();

    const fechaDesde =
        dateFromInput.value;

    const fechaHasta =
        dateToInput.value;


    if (
        !profileId &&
        !fechaDesde &&
        !fechaHasta
    ) {

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


    ocultarTodasLasSecciones();

    reportsContainer.innerHTML = "";

    pagination.innerHTML = "";


    try {

        let url;


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

        else if (
            fechaDesde &&
            fechaHasta
        ) {

            url =
                `${API_URL}/api/reports/date` +
                `?desde=${encodeURIComponent(fechaDesde)}` +
                `&hasta=${encodeURIComponent(fechaHasta)}`;

        }

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


        reportesSeleccionados = [];

        paginaActual = 1;


        destruirGraficos();

        actualizarEstadoComparacion();

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


/* =========================================
   MOSTRAR REPORTES
   ========================================= */

function mostrarReportes() {

    resultsSection.classList.remove(
        "hidden"
    );

    detailSection.classList.add(
        "hidden"
    );

    comparisonSection.classList.add(
        "hidden"
    );


    reportsContainer.innerHTML = "";

    pagination.innerHTML = "";


    if (
        reportesActuales.length === 0
    ) {

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

    actualizarEstadoComparacion();

    mostrarMensaje(
        "",
        false
    );

}


/* =========================================
   CREAR FILA
   ========================================= */

function crearFilaReporte(report) {

    const fila =
        document.createElement("tr");


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

            <div class="table-profile">

                <strong>
                    ${escaparHtml(
        report.profileAlias
    )}
                </strong>

                <span>
                    ${escaparHtml(
        report.profileId
    )}
                </span>

            </div>

        </td>


        <td>
            ${escaparHtml(
        report.fechaPartida
    )}
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
        () => {

            manejarSeleccionReporte(
                report.reportId,
                checkbox.checked
            );

        }
    );


    const detailButton =
        fila.querySelector(
            ".detail-button"
        );


    detailButton.addEventListener(
        "click",
        () => {

            mostrarDetalle(
                report
            );

        }
    );


    return fila;

}


/* =========================================
   MÉTRICAS
   ========================================= */

function obtenerMetricasReporte(report) {

    const ciclos =
        report.historialCiclos || [];


    let decisiones = 0;

    let eventos = 0;

    let cosechas = 0;


    let hayDatosDeDecisiones =
        false;


    ciclos.forEach(
        ciclo => {

            if (
                Array.isArray(
                    ciclo.decisionesTomadas
                )
            ) {

                hayDatosDeDecisiones =
                    true;

                decisiones +=
                    ciclo.decisionesTomadas.length;

            }


            if (
                Array.isArray(
                    ciclo.eventosOcurridos
                )
            ) {

                eventos +=
                    ciclo.eventosOcurridos.filter(
                        evento =>
                            typeof evento === "string" &&
                            evento.startsWith("[Nuevo]")
                    ).length;

            }


            if (
                Array.isArray(
                    ciclo.cosechasRealizadas
                )
            ) {

                cosechas +=
                    ciclo.cosechasRealizadas.length;

            }

        }
    );


    return {

        decisiones:
            hayDatosDeDecisiones
                ? decisiones
                : "N/D",

        eventos,

        cosechas

    };

}


/* =========================================
   SELECCIÓN
   ========================================= */

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
                id =>
                    id !== reportId
            );

    }


    actualizarEstadoComparacion();

}


/* =========================================
   ESTADO COMPARACIÓN
   ========================================= */

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


/* =========================================
   COMPARAR
   ========================================= */

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


    if (
        reportes.length !== 2
    ) {

        mostrarMensaje(
            "No se pudieron encontrar las partidas seleccionadas.",
            true
        );

        return;
    }


    mostrarComparacion(
        reportes[0],
        reportes[1]
    );

}


/* =========================================
   MOSTRAR COMPARACIÓN
   ========================================= */

function mostrarComparacion(
    reporteA,
    reporteB
) {

    resultsSection.classList.add(
        "hidden"
    );

    detailSection.classList.add(
        "hidden"
    );

    comparisonSection.classList.remove(
        "hidden"
    );


    destruirGraficos();


    const metricasA =
        obtenerMetricasReporte(
            reporteA
        );


    const metricasB =
        obtenerMetricasReporte(
            reporteB
        );


    comparisonContent.innerHTML = `

        <div class="comparison-header">

            <h2>
                Comparación de partidas
            </h2>

            <p>
                Comparación de los resultados
                registrados en las dos partidas seleccionadas.
            </p>

        </div>


        <div class="comparison-table-wrapper">

            <table class="comparison-table">

                <thead>

                    <tr>

                        <th>
                            Métrica
                        </th>

                        <th>
                            Partida 1
                        </th>

                        <th>
                            Partida 2
                        </th>

                    </tr>

                </thead>


                <tbody>

                    <tr>

                        <th>
                            Perfil
                        </th>

                        <td>

                            ${escaparHtml(
        reporteA.profileAlias
    )}

                            <span
                                class="comparison-profile-id"
                            >
                                ${escaparHtml(
        reporteA.profileId
    )}
                            </span>

                        </td>

                        <td>

                            ${escaparHtml(
        reporteB.profileAlias
    )}

                            <span
                                class="comparison-profile-id"
                            >
                                ${escaparHtml(
        reporteB.profileId
    )}
                            </span>

                        </td>

                    </tr>


                    <tr>

                        <th>
                            Fecha
                        </th>

                        <td>
                            ${escaparHtml(
        reporteA.fechaPartida
    )}
                        </td>

                        <td>
                            ${escaparHtml(
        reporteB.fechaPartida
    )}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Ciclos jugados
                        </th>

                        <td>
                            ${reporteA.ciclosJugados}
                        </td>

                        <td>
                            ${reporteB.ciclosJugados}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Decisiones tomadas
                        </th>

                        <td>
                            ${metricasA.decisiones}
                        </td>

                        <td>
                            ${metricasB.decisiones}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Eventos presentados
                        </th>

                        <td>
                            ${metricasA.eventos}
                        </td>

                        <td>
                            ${metricasB.eventos}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Cosechas realizadas
                        </th>

                        <td>
                            ${metricasA.cosechas}
                        </td>

                        <td>
                            ${metricasB.cosechas}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Presupuesto inicial
                        </th>

                        <td>
                            ${formatearDinero(
        reporteA.presupuestoInicial
    )}
                        </td>

                        <td>
                            ${formatearDinero(
        reporteB.presupuestoInicial
    )}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Presupuesto final
                        </th>

                        <td>
                            ${formatearDinero(
        reporteA.presupuestoFinal
    )}
                        </td>

                        <td>
                            ${formatearDinero(
        reporteB.presupuestoFinal
    )}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Ganancia acumulada
                        </th>

                        <td>
                            ${formatearDinero(
        reporteA.gananciaAcumulada
    )}
                        </td>

                        <td>
                            ${formatearDinero(
        reporteB.gananciaAcumulada
    )}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Gasto acumulado
                        </th>

                        <td>
                            ${formatearDinero(
        reporteA.gastoAcumulado
    )}
                        </td>

                        <td>
                            ${formatearDinero(
        reporteB.gastoAcumulado
    )}
                        </td>

                    </tr>


                    <tr>

                        <th>
                            Razón de finalización
                        </th>

                        <td>
                            ${escaparHtml(
        reporteA.razonFin
    )}
                        </td>

                        <td>
                            ${escaparHtml(
        reporteB.razonFin
    )}
                        </td>

                    </tr>

                </tbody>

            </table>

        </div>


        <div class="comparison-charts">

            <div class="chart-container">

                <h3>
                    Resultados económicos
                </h3>

                <div class="chart-canvas-wrapper">

                    <canvas
                        id="comparisonFinancialChart"
                    ></canvas>

                </div>

            </div>


            <div class="chart-container">

                <h3>
                    Actividad de las partidas
                </h3>

                <div class="chart-canvas-wrapper">

                    <canvas
                        id="comparisonActivityChart"
                    ></canvas>

                </div>

            </div>

        </div>

    `;


    crearGraficoComparacionEconomica(
        reporteA,
        reporteB
    );


    crearGraficoComparacionActividad(
        reporteA,
        reporteB
    );

}


/* =========================================
   DETALLE
   ========================================= */

function mostrarDetalle(report) {

    resultsSection.classList.add(
        "hidden"
    );

    comparisonSection.classList.add(
        "hidden"
    );

    detailSection.classList.remove(
        "hidden"
    );


    destruirGraficos();


    const metricas =
        obtenerMetricasReporte(
            report
        );


    reportDetail.innerHTML = `

        <div class="detail-header">

            <h2>
                Reporte ${escaparHtml(
        report.reportId
    )}
            </h2>


            <div class="detail-profile">

                Perfil:

                <strong>
                    ${escaparHtml(
        report.profileAlias
    )}
                </strong>

                (
                ${escaparHtml(
        report.profileId
    )}
                )

                <br>

                Fecha:
                ${escaparHtml(
        report.fechaPartida
    )}

                <br>

                Razón de finalización:
                ${escaparHtml(
        report.razonFin
    )}

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


        <div class="chart-container">

            <h3>
                Evolución del presupuesto
            </h3>

            <div class="chart-canvas-wrapper">

                <canvas
                    id="budgetChart"
                ></canvas>

            </div>

        </div>


        <div class="chart-container">

            <h3>
                Ganancias y gastos por ciclo
            </h3>

            <div class="chart-canvas-wrapper">

                <canvas
                    id="cycleFinancialChart"
                ></canvas>

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


    crearGraficoPresupuesto(report);

    crearGraficoFinancieroPorCiclo(report);

}


/* =========================================
   GRÁFICO: PRESUPUESTO
   ========================================= */

function crearGraficoPresupuesto(report) {

    const canvas =
        document.getElementById(
            "budgetChart"
        );


    if (!canvas) {
        return;
    }


    const ciclos =
        report.historialCiclos || [];


    const etiquetas =
        ciclos.map(
            (ciclo, index) =>
                `Ciclo ${ciclo.numeroCiclo ?? index + 1}`
        );


    const presupuestoInicial =
        ciclos.map(
            ciclo =>
                ciclo.presupuestoInicial || 0
        );


    const presupuestoFinal =
        ciclos.map(
            ciclo =>
                ciclo.presupuestoFinal || 0
        );


    budgetChart =
        new Chart(
            canvas,
            {

                type:
                    "line",

                data: {

                    labels:
                        etiquetas,

                    datasets: [

                        {

                            label:
                                "Presupuesto inicial",

                            data:
                                presupuestoInicial,

                            tension:
                                0.25

                        },

                        {

                            label:
                                "Presupuesto final",

                            data:
                                presupuestoFinal,

                            tension:
                                0.25

                        }

                    ]

                },

                options: {

                    responsive:
                        true,

                    maintainAspectRatio:
                        false,

                    scales: {

                        y: {

                            ticks: {

                                callback:
                                    function (value) {

                                        return formatearDineroCorto(
                                            value
                                        );

                                    }

                            }

                        }

                    },

                    plugins: {

                        tooltip: {

                            callbacks: {

                                label:
                                    function (context) {

                                        return (
                                            context.dataset.label +
                                            ": " +
                                            formatearDinero(
                                                context.raw
                                            )
                                        );

                                    }

                            }

                        }

                    }

                }

            }
        );

}


/* =========================================
   GRÁFICO: GANANCIAS Y GASTOS POR CICLO
   ========================================= */

function crearGraficoFinancieroPorCiclo(
    report
) {

    const canvas =
        document.getElementById(
            "cycleFinancialChart"
        );


    if (!canvas) {
        return;
    }


    const ciclos =
        report.historialCiclos || [];


    const etiquetas =
        ciclos.map(
            (ciclo, index) =>
                `Ciclo ${ciclo.numeroCiclo ?? index + 1}`
        );


    const ganancias =
        ciclos.map(
            ciclo =>
                ciclo.gananciaTotal || 0
        );


    const gastos =
        ciclos.map(
            ciclo =>
                ciclo.gastoTotal || 0
        );


    cycleFinancialChart =
        new Chart(
            canvas,
            {

                type:
                    "bar",

                data: {

                    labels:
                        etiquetas,

                    datasets: [

                        {

                            label:
                                "Ganancias",

                            data:
                                ganancias

                        },

                        {

                            label:
                                "Gastos",

                            data:
                                gastos

                        }

                    ]

                },

                options: {

                    responsive:
                        true,

                    maintainAspectRatio:
                        false,

                    scales: {

                        y: {

                            beginAtZero:
                                true,

                            ticks: {

                                callback:
                                    function (value) {

                                        return formatearDineroCorto(
                                            value
                                        );

                                    }

                            }

                        }

                    },

                    plugins: {

                        tooltip: {

                            callbacks: {

                                label:
                                    function (context) {

                                        return (
                                            context.dataset.label +
                                            ": " +
                                            formatearDinero(
                                                context.raw
                                            )
                                        );

                                    }

                            }

                        }

                    }

                }

            }
        );

}


/* =========================================
   GRÁFICO: COMPARACIÓN ECONÓMICA
   ========================================= */

function crearGraficoComparacionEconomica(
    reporteA,
    reporteB
) {

    const canvas =
        document.getElementById(
            "comparisonFinancialChart"
        );


    if (!canvas) {
        return;
    }


    comparisonFinancialChart =
        new Chart(
            canvas,
            {

                type:
                    "bar",

                data: {

                    labels: [

                        "Presupuesto final",

                        "Ganancia",

                        "Gastos"

                    ],

                    datasets: [

                        {

                            label:
                                obtenerNombrePartida(
                                    reporteA,
                                    "Partida 1"
                                ),

                            data: [

                                reporteA.presupuestoFinal || 0,

                                reporteA.gananciaAcumulada || 0,

                                reporteA.gastoAcumulado || 0

                            ]

                        },

                        {

                            label:
                                obtenerNombrePartida(
                                    reporteB,
                                    "Partida 2"
                                ),

                            data: [

                                reporteB.presupuestoFinal || 0,

                                reporteB.gananciaAcumulada || 0,

                                reporteB.gastoAcumulado || 0

                            ]

                        }

                    ]

                },

                options: {

                    responsive:
                        true,

                    maintainAspectRatio:
                        false,

                    scales: {

                        y: {

                            beginAtZero:
                                true,

                            ticks: {

                                callback:
                                    function (value) {

                                        return formatearDineroCorto(
                                            value
                                        );

                                    }

                            }

                        }

                    },

                    plugins: {

                        tooltip: {

                            callbacks: {

                                label:
                                    function (context) {

                                        return (
                                            context.dataset.label +
                                            ": " +
                                            formatearDinero(
                                                context.raw
                                            )
                                        );

                                    }

                            }

                        }

                    }

                }

            }
        );

}


/* =========================================
   GRÁFICO: COMPARACIÓN DE ACTIVIDAD
   ========================================= */

function crearGraficoComparacionActividad(
    reporteA,
    reporteB
) {

    const canvas =
        document.getElementById(
            "comparisonActivityChart"
        );


    if (!canvas) {
        return;
    }


    const metricasA =
        obtenerMetricasReporte(
            reporteA
        );


    const metricasB =
        obtenerMetricasReporte(
            reporteB
        );


    comparisonActivityChart =
        new Chart(
            canvas,
            {

                type:
                    "bar",

                data: {

                    labels: [

                        "Decisiones",

                        "Eventos",

                        "Cosechas"

                    ],

                    datasets: [

                        {

                            label:
                                obtenerNombrePartida(
                                    reporteA,
                                    "Partida 1"
                                ),

                            data: [

                                convertirMetricaNumerica(
                                    metricasA.decisiones
                                ),

                                metricasA.eventos,

                                metricasA.cosechas

                            ]

                        },

                        {

                            label:
                                obtenerNombrePartida(
                                    reporteB,
                                    "Partida 2"
                                ),

                            data: [

                                convertirMetricaNumerica(
                                    metricasB.decisiones
                                ),

                                metricasB.eventos,

                                metricasB.cosechas

                            ]

                        }

                    ]

                },

                options: {

                    responsive:
                        true,

                    maintainAspectRatio:
                        false,

                    scales: {

                        y: {

                            beginAtZero:
                                true,

                            ticks: {

                                precision:
                                    0

                            }

                        }

                    }

                }

            }
        );

}


/* =========================================
   CICLOS
   ========================================= */

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


/* =========================================
   DECISIONES
   ========================================= */

function generarDecisiones(decisiones) {

    if (
        !Array.isArray(decisiones)
    ) {

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


    if (
        decisiones.length === 0
    ) {

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
                            ${escaparHtml(
            decision
        )}
                        </li>

                    `
    ).join("")}

            </ul>

        </div>

    `;

}


/* =========================================
   GASTOS
   ========================================= */

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


/* =========================================
   EVENTOS
   ========================================= */

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
                            ${escaparHtml(
            evento
        )}
                        </li>

                    `
    ).join("")}

            </ul>

        </div>

    `;

}


/* =========================================
   COSECHAS
   ========================================= */

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
                            ${escaparHtml(
            cosecha
        )}
                        </li>

                    `
    ).join("")}

            </ul>

        </div>

    `;

}


/* =========================================
   PAGINACIÓN
   ========================================= */

function generarPaginacion() {

    pagination.innerHTML = "";


    const totalPaginas =
        Math.ceil(
            reportesActuales.length /
            REPORTES_POR_PAGINA
        );


    if (
        totalPaginas <= 1
    ) {

        return;
    }


    const botonAnterior =
        document.createElement(
            "button"
        );


    botonAnterior.textContent =
        "‹";


    botonAnterior.disabled =
        paginaActual === 1;


    botonAnterior.addEventListener(
        "click",
        () => {

            if (
                paginaActual > 1
            ) {

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
            document.createElement(
                "button"
            );


        boton.textContent =
            pagina;


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

                paginaActual =
                    pagina;

                mostrarReportes();

            }
        );


        pagination.appendChild(
            boton
        );

    }


    const botonSiguiente =
        document.createElement(
            "button"
        );


    botonSiguiente.textContent =
        "›";


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


/* =========================================
   NAVEGACIÓN
   ========================================= */

function volverAReportes() {

    detailSection.classList.add(
        "hidden"
    );

    comparisonSection.classList.add(
        "hidden"
    );

    resultsSection.classList.remove(
        "hidden"
    );


    destruirGraficos();

}


function volverAComparacion() {

    comparisonSection.classList.add(
        "hidden"
    );

    resultsSection.classList.remove(
        "hidden"
    );


    destruirGraficos();

    mostrarReportes();

}


/* =========================================
   LIMPIAR
   ========================================= */

function limpiarBusqueda() {

    profileIdInput.value = "";

    dateFromInput.value = "";

    dateToInput.value = "";


    ocultarTodasLasSecciones();


    reportsContainer.innerHTML = "";

    reportDetail.innerHTML = "";

    comparisonContent.innerHTML = "";

    pagination.innerHTML = "";


    reportesActuales = [];

    reportesSeleccionados = [];

    paginaActual = 1;


    destruirGraficos();

    actualizarEstadoComparacion();


    mostrarMensaje(
        "",
        false
    );

}


/* =========================================
   DESTRUIR GRÁFICOS
   ========================================= */

function destruirGraficos() {

    if (budgetChart) {

        budgetChart.destroy();

        budgetChart = null;

    }


    if (cycleFinancialChart) {

        cycleFinancialChart.destroy();

        cycleFinancialChart = null;

    }


    if (comparisonFinancialChart) {

        comparisonFinancialChart.destroy();

        comparisonFinancialChart = null;

    }


    if (comparisonActivityChart) {

        comparisonActivityChart.destroy();

        comparisonActivityChart = null;

    }

}


/* =========================================
   OCULTAR SECCIONES
   ========================================= */

function ocultarTodasLasSecciones() {

    resultsSection.classList.add(
        "hidden"
    );

    detailSection.classList.add(
        "hidden"
    );

    comparisonSection.classList.add(
        "hidden"
    );

}


/* =========================================
   NOMBRE DE PARTIDA
   ========================================= */

function obtenerNombrePartida(
    report,
    nombreAlternativo
) {

    if (
        report.profileAlias &&
        report.profileAlias.trim() !== ""
    ) {

        return report.profileAlias;

    }


    if (
        report.profileId &&
        report.profileId.trim() !== ""
    ) {

        return report.profileId;

    }


    return nombreAlternativo;

}


/* =========================================
   MÉTRICA NUMÉRICA
   ========================================= */

function convertirMetricaNumerica(
    valor
) {

    if (
        typeof valor === "number"
    ) {

        return valor;

    }


    const convertido =
        Number(valor);


    return Number.isFinite(
        convertido
    )
        ? convertido
        : 0;

}


/* =========================================
   FORMATEAR DINERO
   ========================================= */

function formatearDinero(valor) {

    return new Intl.NumberFormat(
        "es-CO",
        {
            style:
                "currency",

            currency:
                "COP",

            maximumFractionDigits:
                0
        }
    ).format(
        valor || 0
    );

}


function formatearDineroCorto(
    valor
) {

    if (
        Math.abs(valor) >= 1000000
    ) {

        return (
            "$" +
            (valor / 1000000)
                .toFixed(1) +
            " M"
        );

    }


    if (
        Math.abs(valor) >= 1000
    ) {

        return (
            "$" +
            (valor / 1000)
                .toFixed(0) +
            " mil"
        );

    }


    return (
        "$" +
        Number(valor || 0)
            .toLocaleString(
                "es-CO"
            )
    );

}


/* =========================================
   MENSAJES
   ========================================= */

function mostrarMensaje(
    texto,
    esError
) {

    message.textContent =
        texto;


    message.className =
        esError
            ? "message error"
            : "message success";

}


/* =========================================
   ESCAPAR HTML
   ========================================= */

function escaparHtml(valor) {

    if (
        valor === null ||
        valor === undefined
    ) {

        return "";

    }


    const div =
        document.createElement(
            "div"
        );


    div.textContent =
        String(valor);


    return div.innerHTML;

}